using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Diagnostics;
using Poker.Interface.Communication;
using Poker.Interface.Metier;
using Poker.Serveur.Metier;
using Poker.Serveur.Technique;
using Poker.Interface.Bot;
using System.Reflection;
using System.Threading;
using Poker.Interface.CalculBlind;
using System.IO;
using Poker.Interface.Stats;
using System.Linq;

namespace Poker.Serveur
{
    /// <summary>
    /// Classe d'h�bergement du serveur
   ///  
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Serveur : IServeur
    {
        #region Membres priv�s
        /// <summary>
        /// On stocke � la fois les CallBacks index�es par les joueurs & la liste des joueurs (on peut ainsi y acc�der par leur num�ro)
        /// <remarks>En cas de probl�me d'un appel � un joueur (detect� par l'exception CommunicationObjectAbortedException 
        /// on supprime ce joueur des callbacks et la partie continue sans lui : il tente de faire parole � chaque tour
        /// </remarks>
        /// </summary>
        private static Dictionary<Joueur, ICallBack> _listeCallBacks = new Dictionary<Joueur, ICallBack>();
        private static Dictionary<Guid, Joueur> _listeConnectes = new Dictionary<Guid, Joueur>();        
        private const int NOMBRE_MAXI_JOUEURS = 10;
        private static Guid _idAdmin = new Guid();        
        private const int POURCENTAGE_ATTENTE_ACTIONJOUEUR = 105;
        /// <summary>
        /// Temps d'attente (en ms) entre 2 coups
        /// </summary>
        private const int TEMPS_ATTENTE_ENTRE_COUPS = 2000;
        private static object lockActionJoueur = new object();
        private static Timer _timeOutActionJoueur = null;                
        private static InfosAdmin _infosAdmin = null;
        private static IAugmentationBlinds _methodeAugmentationBlinds = null;
        private static bool _augmentationBlinds = false;
        private static EnregistrementStats _statistiques = null;
        /// <summary>
        /// La liste des assembly pour chacun des bots
        /// </summary>
        private static Dictionary<string, Assembly> _assemblyBots = new Dictionary<string, Assembly>();
        #endregion

        #region IServeur Membres : m�thodes "ouvertes" � tous les joueurs
        /// <summary>
        /// Envoi d'un message d'information : s'affiche dans la deuxieme fenetre
        /// </summary>
        /// <param name="msg"></param>
        public void EnvoyerMessageInformation(MessageInfo msg)
        {
            foreach (Joueur j in _listeConnectes.Values)
            {
                try
                {
                    if (j.EstConnecte)
                    {
                        _listeCallBacks[j].RecevoirMessageInformation(msg);
                    }
                }
                catch (CommunicationObjectAbortedException ex)
                {
                    logServeur.Debug("Erreur dans EnvoiMessageInformation : " + ex.Message);
                    DeconnecterJoueur(j);
                }
            }
        }

        /// <summary>
        /// Utilisation du chat
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="expediteur"></param>
        public void EnvoyerMessagePublic(ChatMessage msg, Guid idExpediteur)
        {
            Joueur expediteur = RechercherJoueurSelonIdConnexion(idExpediteur);
            if (expediteur != null && !expediteur.EstSpectateur && msg.Contenu.Trim().Length > 0)
            {
                // Les stats "chat" ne sont enregistr�es qu'une fois la partie d�marr�e
                if (InfosTable.Singleton.PartieEnCours)
                    _statistiques.EnregistrerChat(msg.Contenu, expediteur);
                foreach (Joueur j in _listeConnectes.Values)
                    try
                    {
                        if (j.EstConnecte)
                        {
                            _listeCallBacks[j].RecevoirMessagePublic(msg, expediteur.Nom);
                        }
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {                        
                        logServeur.Debug("Erreur dans EnvoyerMessagePublic : " + ex.Message);
                        DeconnecterJoueur(j);
                    }
            }
        }
        
        /// <summary>
        /// Une demande de connection au serveur
        /// </summary>
        /// <param name="expediteur">Le demandeur</param>
        public ResultatConnection Connecter(Joueur expediteur)
        {
            return VerificationConditionsConnections(expediteur, OperationContext.Current.GetCallbackChannel<ICallBack>());
        }

        /// <summary>
        /// D�connection d'un joueur
        /// </summary>
        /// <param name="expediteur"></param>
        public void Deconnecter(Guid idExpediteur)
        {
            Joueur j = RechercherJoueurSelonIdConnexion(idExpediteur);
            if (j != null)
            {
                DeconnecterJoueur(j);            
            }
            else
            {
                logServeur.Debug("Tentative de deconnexion avec le guid {0} ne correspondant � aucun joueur", idExpediteur);
            }   
            
        }

        /// <summary>
        /// Gestion du regard des cartes
        /// </summary>
        /// <param name="expediteur"></param>
        /// <param name="regard"></param>
        public void EnvoyerRegard(Guid idExpediteur, bool regard)
        {
            Joueur expediteur = RechercherJoueurSelonIdConnexion(idExpediteur);
            if (expediteur != null && !expediteur.EstSpectateur)
            {
                foreach (Joueur j in _listeConnectes.Values)
                {
                    if (j.EstConnecte)
                    {
                        if (j.Identifiant != expediteur.Identifiant && j.EstConnecte)
                        {
                            try
                            {
                                if (regard)
                                    _listeCallBacks[j].RecevoirRegard(expediteur, EtatMain.Regardee);
                                else
                                    _listeCallBacks[j].RecevoirRegard(expediteur, EtatMain.Concurrente);
                            }
                            catch (CommunicationObjectAbortedException ex)
                            {
                                logServeur.Debug("Erreur dans EnvoyerRegard : " + ex.Message);
                                DeconnecterJoueur(j);
                            }
                        }
                    }
                }
            }
            else
            {
                logServeur.Debug("Envoi d'un regard avec le guid {0} ne correspondant � aucun joueur", idExpediteur);
            }
        }

        /// <summary>
        /// Envoi d'une action par un joueur
        /// </summary>
        /// <param name="expediteur"></param>
        /// <param name="action"></param>
        public void EnvoyerAction(Guid idExpediteur, ActionJoueur action)
        {
            Joueur expediteur = RechercherJoueurSelonIdConnexion(idExpediteur);
            if (expediteur != null && !expediteur.EstSpectateur)
            {
                logServeur.Debug("Reception de l'action {0} par le joueur {1}, montant : {2}", action.TypeAction, expediteur.Nom, action.Montant);
                if (InfosTable.Singleton.JoueurCourant.Equals(expediteur))
                {
                    // On lock pour qu'une seule personne � la fois puisse joueur
                    lock (lockActionJoueur)
                    {
                        logServeur.Debug("  C'est bien son tour");
                        StopperTimerActionJoueur();
                        bool tourTermine = false;       // Dans certains cas, on saura que le tour est termin�
                        bool actionValidee = false;     // V�rification que l'action demand�e est bien valide
                        ActionJoueur actionReelle = new ActionJoueur();

                        InfosTable.Singleton.JoueurCourant.DerniereAction = action.TypeAction;

                        do
                        {
                            logServeur.Debug("  Validation de l'action {0} du joueur", InfosTable.Singleton.JoueurCourant.DerniereAction);
                            // On v�rifie que l'action demand�e est autoris�e
                            switch (InfosTable.Singleton.JoueurCourant.DerniereAction)
                            {
                                case TypeActionJoueur.Passe:
                                    if (!InfosTable.Singleton.JoueurCourant.JeterCartes)
                                    {
                                        InfosTable.Singleton.JoueurCourant.JeterCartes = true;
                                        EnvoyerJeterCartes(InfosTable.Singleton.JoueurCourant);
                                        // On determine si le tour d'ench�re est termin� :  reste t il un seul joueur
                                        List<Joueur> derniersJoueursEnCourse = InfosTable.Singleton.JoueursEncoreEnCourse();

                                        if (derniersJoueursEnCourse.Count == 1)
                                        {
                                            InfosTable.Singleton.RAZMiseJoueurs(true);                                            
                                            derniersJoueursEnCourse[0].TapisJoueur += InfosTable.Singleton.Pot;
                                            actionReelle.TypeAction = TypeActionJoueur.Passe;
                                            EnvoyerMessageInformation(new MessageInfo(InfosTable.Singleton.JoueurCourant, actionReelle));
                                            _statistiques.EnregistrerActionJoueur(InfosTable.Singleton.JoueurCourant, actionReelle.TypeAction);
                                            EnvoyerMessageInformation(new MessageInfo(derniersJoueursEnCourse[0], null, InfosTable.Singleton.Pot));
                                            _statistiques.EnregistrerFinDonne(InfosTable.Singleton.ListeJoueurs, null, derniersJoueursEnCourse, InfosTable.Singleton.Pot);
                                            Thread.Sleep(InfosTable.Singleton.OptionsJeu.TimeOutFinDonneCartesCachees * 1000);

                                            NouvelleDonne();
                                            MAJJoueurs(false);
                                            return;
                                        }
                                    }
                                    actionValidee = true;
                                    break;

                                case TypeActionJoueur.Parole:
                                    if (InfosTable.Singleton.Etape == EtapeDonne.PreFlop)
                                    {
                                        // A l'�tape du pr�flop : Le joueur a pay� la grosse blind et tout le monde a suivi ou jet�
                                        if (InfosTable.Singleton.JoueurCourant.Mise == InfosTable.Singleton.MontantGrosseBlind)
                                        {
                                            actionValidee = true;
                                            foreach (Joueur j in InfosTable.Singleton.ListeJoueurs)
                                            {
                                                if (j.Mise > InfosTable.Singleton.JoueurCourant.Mise)
                                                {
                                                    logServeur.Debug("  Le joueur ne pouvait pas faire parole : il passe");
                                                    InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Passe;
                                                    actionValidee = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logServeur.Debug("  Le joueur ne pouvait pas faire parole : il passe");
                                            InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Passe;
                                            actionValidee = false;
                                        }
                                    }
                                    else
                                    {
                                        actionValidee = true;
                                        // Tous les joueurs doivent avoir fait parole ou jeter avant
                                        foreach (Joueur j in InfosTable.Singleton.ListeJoueurs)
                                        {
                                            if (j != InfosTable.Singleton.JoueurCourant && j.DerniereAction != TypeActionJoueur.Passe && j.DerniereAction != TypeActionJoueur.Aucune && j.DerniereAction != TypeActionJoueur.Parole)
                                            {
                                                logServeur.Debug("  Le joueur ne pouvait pas faire parole : il passe");
                                                InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Passe;
                                                actionValidee = false;
                                                break;
                                            }
                                        }
                                    }

                                    break;

                                case TypeActionJoueur.Suit:
                                    // Le joueur doit suivre de la derniere mise : exception, son tapis est inf�rieur � la mise : il fait Tapis
                                    if (action.Montant >= InfosTable.Singleton.JoueurCourant.TapisJoueur)
                                    {
                                        logServeur.Debug("  Le joueur n'a pas relanc� : il a fait tapis");
                                        InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Tapis;
                                        actionReelle.Montant = InfosTable.Singleton.JoueurCourant.TapisJoueur;
                                        InfosTable.Singleton.Miser(InfosTable.Singleton.JoueurCourant, actionReelle.Montant);
                                        if (InfosTable.Singleton.JoueurCourant.Mise > InfosTable.Singleton.MontantDerniereRelance)
                                        {
                                            InfosTable.Singleton.DerniereRelance = InfosTable.Singleton.JoueurCourant;
                                            InfosTable.Singleton.MontantDerniereRelance = InfosTable.Singleton.JoueurCourant.Mise;
                                        }
                                        actionValidee = true;
                                    }
                                    else if (action.Montant >= InfosTable.Singleton.MontantDerniereRelance)
                                    {
                                        // Si un joueur suit � 0 : il fait parole si c'est possible                            
                                        if (action.Montant == 0)
                                        {
                                            InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Parole;
                                            actionValidee = false;
                                        }
                                        else
                                        {
                                            InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Suit;
                                            actionReelle.Montant = action.Montant;
                                            InfosTable.Singleton.Miser(InfosTable.Singleton.JoueurCourant, actionReelle.Montant);
                                            actionValidee = true;
                                        }
                                    }
                                    else
                                    {
                                        // Tapis ?
                                        if (action.Montant < InfosTable.Singleton.MontantDerniereRelance && action.Montant >= InfosTable.Singleton.JoueurCourant.TapisJoueur)
                                        {
                                            logServeur.Debug("  Le joueur a fait tapis (donc pas suivi) : son tapis n'etait pas suffisant");

                                            InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Tapis;
                                            actionReelle.Montant = InfosTable.Singleton.JoueurCourant.TapisJoueur;
                                            InfosTable.Singleton.Miser(InfosTable.Singleton.JoueurCourant, actionReelle.Montant);
                                        }
                                        else
                                        {
                                            logServeur.Debug("  Le joueur a suivi avec moins que ce qui �tait demand� : il passe");
                                            InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Passe;
                                            actionValidee = false;
                                        }

                                    }
                                    break;

                                case TypeActionJoueur.Mise:
                                case TypeActionJoueur.Relance:
                                    if (action.Montant < InfosTable.Singleton.RelanceMinimum())
                                    {
                                        logServeur.Debug("  Le joueur n'a pas relanc� assez : il passe");
                                        actionValidee = false;
                                        InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Passe;
                                    }
                                    else if (action.Montant >= InfosTable.Singleton.JoueurCourant.TapisJoueur)
                                    {
                                        logServeur.Debug("  Le joueur n'a pas relanc� : il a fait tapis");
                                        InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Tapis;
                                        actionReelle.Montant = InfosTable.Singleton.JoueurCourant.TapisJoueur;
                                        InfosTable.Singleton.Miser(InfosTable.Singleton.JoueurCourant, actionReelle.Montant);
                                        if (InfosTable.Singleton.JoueurCourant.Mise > InfosTable.Singleton.MontantDerniereRelance)
                                        {
                                            InfosTable.Singleton.DerniereRelance = InfosTable.Singleton.JoueurCourant;
                                            InfosTable.Singleton.MontantDerniereRelance = InfosTable.Singleton.JoueurCourant.Mise;
                                        }
                                        actionValidee = true;
                                    }
                                    else
                                    {
                                        actionReelle.TypeAction = action.TypeAction;
                                        actionReelle.Montant = action.Montant;
                                        InfosTable.Singleton.Miser(InfosTable.Singleton.JoueurCourant, actionReelle.Montant);
                                        InfosTable.Singleton.JoueurCourant.DerniereAction = action.TypeAction;
                                        InfosTable.Singleton.DerniereRelance = InfosTable.Singleton.JoueurCourant;
                                        InfosTable.Singleton.MontantDerniereRelance = InfosTable.Singleton.JoueurCourant.Mise;
                                        actionValidee = true;
                                    }
                                    break;

                                case TypeActionJoueur.Tapis:
                                    if (action.Montant == 0 || action.Montant < InfosTable.Singleton.JoueurCourant.TapisJoueur)
                                    {
                                        logServeur.Debug("  Le joueur ne peut pas faire tapis : il n'a plus d'argent ou n'a pas tout mis");
                                        actionValidee = false;
                                        InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Passe;
                                    }
                                    else
                                    {

                                        InfosTable.Singleton.JoueurCourant.DerniereAction = TypeActionJoueur.Tapis;
                                        actionReelle.Montant = InfosTable.Singleton.JoueurCourant.TapisJoueur;
                                        InfosTable.Singleton.Miser(InfosTable.Singleton.JoueurCourant, actionReelle.Montant);

                                        if (InfosTable.Singleton.JoueurCourant.Mise > InfosTable.Singleton.MontantDerniereRelance)
                                        {
                                            InfosTable.Singleton.DerniereRelance = InfosTable.Singleton.JoueurCourant;
                                            InfosTable.Singleton.MontantDerniereRelance = InfosTable.Singleton.JoueurCourant.Mise;
                                        }
                                        actionValidee = true;
                                    }
                                    break;
                            }
                        } while (!actionValidee);

                        actionReelle.TypeAction = InfosTable.Singleton.JoueurCourant.DerniereAction;
                        EnvoyerMessageInformation(new MessageInfo(InfosTable.Singleton.JoueurCourant, actionReelle));
                        _statistiques.EnregistrerActionJoueur(InfosTable.Singleton.JoueurCourant, actionReelle.TypeAction);

                        int nbJoueursPouvantParler = InfosTable.Singleton.NombreDeJoueursPouvantParler(InfosTable.Singleton.MontantDerniereRelance);
                        if (!tourTermine)
                        {
                            tourTermine = (nbJoueursPouvantParler == 0);
                        }

                        // Tout le monde a fait parole ou pass� (On est forc�ment pas dans le preflop : trait� par le cas ci-dessus
                        if (!tourTermine)
                        {
                            tourTermine = true;
                            foreach (Joueur j in InfosTable.Singleton.ListeJoueurs)
                            {
                                if (!j.JeterCartes && j.TapisJoueur > 0 && j.DerniereAction != TypeActionJoueur.Suit && j.DerniereAction != TypeActionJoueur.Passe && j.DerniereAction != TypeActionJoueur.Parole)
                                {
                                    tourTermine = false;
                                    break;
                                }
                            }
                        }

                        InfosTable.Singleton.JoueurCourant.TourDeJeu = false;
                        MAJJoueurs(false);

                        // Passage au joueur suivant ou � l'�tape suivante 
                        if (tourTermine)
                        {
                            if (TerminerTourEnchere() == EtapeDonne.FinDonne)
                                return;
                            else
                            {
                                // Reste le cas particulier ou un seul joueur peut encore parler : on d�roule mais avant on montre les cartes de tout le monde
                                nbJoueursPouvantParler = InfosTable.Singleton.NombreDeJoueursPouvantParler(InfosTable.Singleton.MontantDerniereRelance);
                                if (nbJoueursPouvantParler == 1)
                                {
                                    MAJJoueurs(true);
                                    do
                                    {
                                        Thread.Sleep(2 * TEMPS_ATTENTE_ENTRE_COUPS);
                                    }
                                    while (TerminerTourEnchere() != EtapeDonne.FinDonne);
                                    return;
                                }
                            }
                        }


                        Dictionary<TypeActionJoueur, ActionPossible> actions = null;
                        int nbCalculsActions = 1;
                        while (actions == null)
                        {
                            // Si le tour est termin� : on est d�j� pass� au joueur suivant par TerminerTourEnchere : pas besoin de l'incr�menter
                            if (tourTermine)
                            {
                                tourTermine = false;
                            }
                            else
                            {
                                InfosTable.Singleton.JoueurSuivant();
                            }
                            actions = InfosTable.Singleton.CalculActionsPossibles(InfosTable.Singleton.JoueurCourant);
                            MAJJoueurs(false);
                            if (nbCalculsActions == InfosTable.Singleton.ListeJoueurs.Count)
                            {
                                // On boucle : plus personne ne peut jouer : on va jusqu'au bout
                                MAJJoueurs(true);
                                do
                                {
                                    Thread.Sleep(2 * TEMPS_ATTENTE_ENTRE_COUPS);
                                } while (TerminerTourEnchere() != EtapeDonne.FinDonne);
                                return;
                            }
                            nbCalculsActions++;
                        }

                        DemarrerTimerActionJoueur(InfosTable.Singleton.OptionsJeu.TimeOutAction);
                        EnvoyerActionsPossiblesAsynchrone(actions);
                    }
                }
                else if (action.TypeAction == TypeActionJoueur.Passe && !InfosTable.Singleton.JoueurCourant.Bot)
                {
                    logServeur.Debug("  Ce n'est pas son tour mais il jette : on en tient compte");
                    // TODO : un joueur peut jeter alors que ce n'est pas son tour : sauf les bots, ils peuvent attendre un peu
                }
            }
            else {
                logServeur.Debug(" R�ception d'un mauvais Guid dans EnvoyerAction : " + idExpediteur.ToString());
            }
        }
        #endregion

        #region IServeur Membres : m�thodes "r�serv�es" � l'administrateur
        /// <summary>
        /// Changement des options de jeu
        /// </summary>
        /// <param name="expediteur">Le demandeur du changement</param>
        /// <param name="opt"></param>
        public void ChangerOptions(Guid idExpediteur, Options opt)
        {            
            if (idExpediteur == _idAdmin)
            {
                // TODO : toutes les options ne sont pas chang�es
                InfosTable.Singleton.OptionsJeu = opt;
            }
        }
        /// <summary>
        /// D�marrage de la partie
        /// </summary>
        /// <param name="admin"></param>
        public void DemarrerPartie(Guid idAdmin, Options optionsJeu)
        {            
            if (idAdmin == _idAdmin)
            {
                logServeur.Debug("================= D�marrage d'une nouvelle partie =================");
                
                InfosTable.Singleton.OptionsJeu = optionsJeu;

                // Calcul de la m�thode d'augmentation des blinds
                InfosTable.Singleton.OptionsJeu.MethodeAugmentationBlinds = _methodeAugmentationBlinds;
                _methodeAugmentationBlinds.ParametreCalcul = optionsJeu.ParametreAugmentationBlinds.ToString();
                _methodeAugmentationBlinds.AugmentationBlinds += new AugmentationBlindsHandler(_augmentationBlinds_AugmentationBlinds);                
                _methodeAugmentationBlinds.DemarrerCalculBlinds(optionsJeu.MontantPetiteBlindInitial, InfosTable.Singleton.ListeJoueurs.Count, optionsJeu.TapisInitial);
                InfosTable.Singleton.MontantPetiteBlind = _methodeAugmentationBlinds.InfosBlind.MontantPetiteBlind;
                InfosTable.Singleton.MontantGrosseBlind = _methodeAugmentationBlinds.InfosBlind.MontantGrosseBlind;
                logServeur.Debug("Augmentation des blinds : ");
                logServeur.Debug(" Classe       : " + _methodeAugmentationBlinds.GetType().ToString());
                logServeur.Debug(" Param calcul : " + _methodeAugmentationBlinds.ParametreCalcul);
                logServeur.Debug(" Petite blind : " + _methodeAugmentationBlinds.InfosBlind.MontantPetiteBlind.ToString());
                logServeur.Debug(" Grosse blind : " + _methodeAugmentationBlinds.InfosBlind.MontantGrosseBlind.ToString());

                InfosTable.Singleton.NouvellePartie();
                _statistiques.EnregistrerNouvellePartie(InfosTable.Singleton.ListeJoueurs, InfosTable.Singleton.OptionsJeu);
                EnvoyerMessageInformation(new MessageInfo(optionsJeu));
                EnvoyerMessageInformation(new MessageInfo(_methodeAugmentationBlinds.InfosBlind));
                _statistiques.EnregistrerAugmentationBlinds(_methodeAugmentationBlinds.InfosBlind);
                NouvelleDonne();
            }
        }        

        /// <summary>
        /// Ajout d'un bot
        /// </summary>
        /// <param name="idAdmin">L'id de l'utilisateur qui demande l'ajout d'un bot</param>
        /// <param name="nom">Le nom du bot</param>
        /// <param name="typeBot">Le type du bot</param>        
        public void AjouterBot(Guid idAdmin, string typeBot, string nom)
        {
            if (idAdmin == _idAdmin)
            {
                try
                {
                    // Init
                    Joueur botJoueur = new Joueur(nom + " (bot)");
                    botJoueur.Bot = true;

                    IBotPoker botInstance = (IBotPoker)Activator.CreateInstance(_assemblyBots[typeBot].GetType(typeBot));

                    ResultatConnection res = VerificationConditionsConnections(botJoueur, botInstance);
                    if (res.Connection == RetourConnection.Ok)
                    {
                        List<Joueur> listeJoueurs = new List<Joueur>(res.ListeJoueurs);
                        botInstance.Initialiser(this, res.IdentifiantConnexion, botJoueur, listeJoueurs, res.PositionJoueur);
                    }
                }
                catch (Exception ex)
                {
                    logServeur.Debug("Erreur lors de l'ajout du bot. Erreur : {0}. type = {1}", ex.Message, typeBot);
                }
            }
            else
            {
                logServeur.Debug(" R�ception d'un mauvais Guid dans AjouterBot : " + idAdmin.ToString());
            }
        }

        /// <summary>
        /// R�cup�ration des infos d'administration
        /// </summary>
        /// <returns></returns>
        public InfosAdmin RecupererInformationsAdministrations()
        {                       
            return _infosAdmin;
        }
        #endregion

        #region M�thodes priv�es      
        /// <summary>
        /// R�cup�ration de toutes les m�thodes de calcul de statistiques
        /// </summary>
        /// <returns></returns>
        private static EnregistrementStats RecupererStatistiques()
        {
            List<IStatistiques> res = new List<IStatistiques>();

            foreach (string cheminAssembly in Directory.GetFiles("stats", "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, cheminAssembly));

                foreach (Type t in asm.GetTypes())
                {
                    Type typeAugmentationBlind = t.GetInterface("IStatistiques");
                    if (typeAugmentationBlind != null && !t.IsAbstract)
                    {
                        res.Add(Activator.CreateInstance(t) as IStatistiques);
                    }
                }
            }

            return new EnregistrementStats(res);            
        }

        /// <summary>
        /// R�cup�ration de la liste des bots
        /// </summary>
        /// <returns></returns>
        private static List<DescriptionBot> RecupererListeBots()
        {
            List<DescriptionBot> res = new List<DescriptionBot>();

            foreach (string cheminAssembly in Directory.GetFiles("bots", "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, cheminAssembly));

                // Remplissage de la lstView par Recup�ration de toutes les classes impl�mentant ControleBase et disposant de l'attribut Controle
                foreach (Type t in asm.GetTypes())
                {                    
                    if (t.GetInterface("IBotPoker") != null && !t.IsAbstract)
                    {
                        _assemblyBots[t.FullName] = asm;
                        string description;
                        object[] attributs = t.GetCustomAttributes(typeof(BotAttribute), false);
                        if (attributs == null || attributs.Length == 0)
                        {
                            description = t.Name;
                        }
                        else
                        {
                            BotAttribute attribut = (BotAttribute)attributs[0];
                            description = string.Format("{0} ({1}, {2})", t.Name, attribut.Niveau, attribut.Description);
                        }
                        
                        res.Add(new DescriptionBot() { Description = description, TypeBot = t.FullName});
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// R�cup�ration de toutes les m�thodes d'augmentation des blinds : normalement une seule (on renvoie direct la premiere)
        /// </summary>
        /// <returns></returns>
        private static IAugmentationBlinds RecupererMethodeAugmentationBlinds()
        {
            foreach (string cheminAssembly in Directory.GetFiles("blind", "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, cheminAssembly));

                foreach (Type t in asm.GetTypes())
                {
                    Type typeAugmentationBlind = t.GetInterface("IAugmentationBlinds");
                    if (typeAugmentationBlind != null && !t.IsAbstract)
                    {                        
                        return Activator.CreateInstance(t) as IAugmentationBlinds;
                    }
                }                
            }

            return null;
        }
       
        /// <summary>
        /// On met � jour les clients en leur disant qu'une personne a jet� ses cartes
        /// </summary>
        /// <param name="lacheur">Le joueur qui jette ses cartes</param>
        private void EnvoyerJeterCartes(Joueur lacheur)
        {
            foreach (Joueur j in _listeConnectes.Values)
            {
                if (j.EstConnecte)
                {
                    // Si le joueur est un bot : on envoie un clone de lacheur. SInon, serialization + deserialization et pas de probl�me
                    if (j.Bot)
                        _listeCallBacks[j].ChangerJoueur((Joueur)lacheur.Clone(), EtatMain.JeteeConcurrente, null, null, InfosTable.Singleton.Pot);
                    else
                    {
                        try
                        {
                            if (lacheur == j)
                                _listeCallBacks[j].ChangerJoueur(lacheur, EtatMain.JeteePersonnelle, lacheur.Carte1, lacheur.Carte2, InfosTable.Singleton.Pot);
                            else
                                _listeCallBacks[j].ChangerJoueur(lacheur, EtatMain.JeteeConcurrente, null, null, InfosTable.Singleton.Pot);
                        }
                        catch (CommunicationObjectAbortedException ex)
                        {
                            logServeur.Debug("Erreur dans EnvoyerJeterCartes : " + ex.Message);
                            DeconnecterJoueur(j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mise � jour de toutes les informations des joueurs � l'exception de leurs cartes
        /// </summary>
        /// <param name="montrerCartes">Si vrai : on montre les cartes de tout le monde sauf ceux qui les ont jet�s</param>
        private void MAJJoueurs(bool montrerCartes)
        {
            foreach (Joueur j in _listeConnectes.Values)
            {
                if (j.EstConnecte)
                {
                    foreach (Joueur infoJoueur in InfosTable.Singleton.ListeJoueurs)
                    {
                        try
                        {
                            if (montrerCartes && !infoJoueur.JeterCartes)
                                _listeCallBacks[j].ChangerJoueur(infoJoueur, EtatMain.Montree, infoJoueur.Carte1, infoJoueur.Carte2, InfosTable.Singleton.Pot);
                            else if (!infoJoueur.Elimine)
                                _listeCallBacks[j].ChangerJoueurSansCartes(infoJoueur, InfosTable.Singleton.Pot);
                            else
                                _listeCallBacks[j].ChangerJoueur(infoJoueur, EtatMain.PasDeCartes, null, null, InfosTable.Singleton.Pot);
                        }
                        catch (CommunicationObjectAbortedException ex)
                        {
                            logServeur.Debug("Erreur dans MAJJoueurs : " + ex.Message);
                            DeconnecterJoueur(j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// D�marrage d'une nouvelle donne
        /// </summary>
        private void NouvelleDonne()
        {
            // On fait le m�nage parmi les joueurs
            if (InfosTable.Singleton.EliminerJoueurs() == 1)
            {
                _statistiques.EnregistrerFinPartie(InfosTable.Singleton.ListeJoueurs);
                EnvoyerMessageInformation(new MessageInfo(InfosTable.Singleton.ListeJoueurs));
                _methodeAugmentationBlinds.ArreterCalculBlinds();
                _methodeAugmentationBlinds.AugmentationBlinds -= new AugmentationBlindsHandler(_augmentationBlinds_AugmentationBlinds);                
            }
            else
            {
                // Reinitialisation diverses et vari�es                 
                InfosTable.Singleton.NouvelleDonne();                

                EnvoyerMessageInformation(new MessageInfo(MessageJeu.NouvelleDonne) { InfosBlind = _methodeAugmentationBlinds.InfosBlind });
                Joueur petiteBlind, grosseBlind;
                GestionBlinds(out petiteBlind, out grosseBlind);
                _statistiques.EnregistrerNouvelleDonne(InfosTable.Singleton.NumeroDonne, InfosTable.Singleton.ListeJoueurs, _methodeAugmentationBlinds.InfosBlind);
                _statistiques.EnregistrerActionJoueur(petiteBlind, TypeActionJoueur.PetiteBlind);
                _statistiques.EnregistrerActionJoueur(grosseBlind, TypeActionJoueur.GrosseBlind);


                MAJJoueurs(false);

                Thread.Sleep(TEMPS_ATTENTE_ENTRE_COUPS);

                foreach (Joueur dest in _listeConnectes.Values)
                {
                    if (dest.EstConnecte)
                    {
                        foreach (Joueur j in InfosTable.Singleton.ListeJoueurs)
                        {
                            try
                            {
                                if (j.Elimine)
                                {
                                    _listeCallBacks[dest].ChangerJoueur(j, EtatMain.PasDeCartes, null, null, InfosTable.Singleton.Pot);
                                }
                                else
                                {
                                    // On envoie ses cartes au joueur : tout les autres recoivent une main concurrent
                                    if (j.Identifiant == dest.Identifiant)
                                    {
                                        _listeCallBacks[dest].ChangerJoueur(j, EtatMain.Personnelle, j.Carte1, j.Carte2, InfosTable.Singleton.Pot);
                                    }
                                    else
                                    {
                                        _listeCallBacks[dest].ChangerJoueur(j, EtatMain.Concurrente, null, null, InfosTable.Singleton.Pot);
                                    }
                                }
                            }
                            catch (CommunicationObjectAbortedException ex)
                            {
                                logServeur.Debug("Erreur dans NouvelleDonne : " + ex.Message);
                                DeconnecterJoueur(dest);
                            }
                        }
                    }
                }                

                DemarrerTimerActionJoueur(InfosTable.Singleton.OptionsJeu.TimeOutAction);                
                EnvoyerActionsPossiblesAsynchrone(InfosTable.Singleton.CalculActionsPossibles(InfosTable.Singleton.JoueurCourant));
            }
        }

        /// <summary>
        /// GEstion des blinds : On les augment si n�cessaire
        ///  et surtout on les distribue
        /// </summary>
        /// <param name="grosseBlind">Joueur de grosse blind</param>
        /// <param name="petiteBlind">Joueur de petite blind</param>
        private void GestionBlinds(out Joueur petiteBlind, out Joueur grosseBlind)
        {
            if (_augmentationBlinds)
            {
                InfosTable.Singleton.MontantPetiteBlind = _methodeAugmentationBlinds.InfosBlind.MontantPetiteBlind;
                InfosTable.Singleton.MontantGrosseBlind = _methodeAugmentationBlinds.InfosBlind.MontantGrosseBlind;
            }

            petiteBlind = InfosTable.Singleton.JoueurCourant;

            // Regle sp�cifique (extraite de WIkipedia) : 
            // When only two players remain, special 'head-to-head' or 'heads up' rules are enforced and the blinds are posted differently. 
            //In this case, the person with the dealer button posts the small blind, while his/her opponent places the big blind. 
            //The dealer acts first before the flop. After the flop, the dealer acts last for the remainder of the hand.
            if (InfosTable.Singleton.NombreDeJoueursNonElimines != 2)
            {
                // Petite blind
                petiteBlind = InfosTable.Singleton.JoueurSuivant();                
            }

            petiteBlind.DerniereAction = TypeActionJoueur.PetiteBlind;
            petiteBlind.Bouton = petiteBlind.Bouton | TypeBouton.PetiteBlind;
            InfosTable.Singleton.Miser(petiteBlind, InfosTable.Singleton.MontantPetiteBlind);
            EnvoyerMessageInformation(new MessageInfo(petiteBlind, new ActionJoueur(TypeActionJoueur.PetiteBlind, petiteBlind.Mise)));
            
            // Grosse blind
            grosseBlind = InfosTable.Singleton.JoueurSuivant();
            grosseBlind.DerniereAction = TypeActionJoueur.GrosseBlind;
            grosseBlind.Bouton = grosseBlind.Bouton | TypeBouton.GrosseBlind;
            InfosTable.Singleton.Miser(grosseBlind, InfosTable.Singleton.MontantGrosseBlind);
            EnvoyerMessageInformation(new MessageInfo(grosseBlind, new ActionJoueur(TypeActionJoueur.GrosseBlind, grosseBlind.Mise)));
            InfosTable.Singleton.DerniereRelance = grosseBlind;
            InfosTable.Singleton.MontantDerniereRelance = InfosTable.Singleton.MontantGrosseBlind;
            

            InfosTable.Singleton.JoueurSuivant();
        }

          

        /// <summary>
        /// Envoi l'ensemble des actions possibles du joueur en asynchrone :
        ///  Sert � eviter que les bots d�clenche des piles d'appel trop grosses dans le meme thread        
        /// </summary>
        private void EnvoyerActionsPossiblesAsynchrone(Dictionary<TypeActionJoueur, ActionPossible> actions)
        {
            new Thread(new ThreadStart(
                delegate()
                {
                    if (InfosTable.Singleton.JoueurCourant.EstConnecte)
                    {
                        try
                        {
                            _listeCallBacks[InfosTable.Singleton.JoueurCourant].RecevoirListeActionsPossibles(actions, InfosTable.Singleton.OptionsJeu.TimeOutAction);
                        }
                        catch (CommunicationObjectAbortedException ex)
                        {
                            logServeur.Debug("Erreur dans EnvoyerActionsPossiblesAsynchrone : " + ex.Message);
                            DeconnecterJoueur(InfosTable.Singleton.JoueurCourant);
                        }
                    }
                    else
                    {
                        InfosTable.Singleton.JoueurCourant.EstConnecte = false;
                        this.EnvoyerAction(RechercherIdConnexionSelonJoueur(InfosTable.Singleton.JoueurCourant), ActionJoueur.Parole);
                    }
                })).Start();
        }
        
        /// <summary>
        /// Deconnection d'un joueur
        /// Suppression d'un joueur dans la liste des callbacks : arrive quand le joueur se d�connecte volontairement ou involontairement
        /// Cette d�connection intervenient sur l'interception de l'exception CommunicationObjectAbortedException
        /// </summary>
        /// <param name="j">Le joueur en erreur</param>
        private void DeconnecterJoueur(Joueur j)
        {
            logServeur.Debug("Deconnection du joueur {0}", j.Nom);
            j.EstConnecte = false;
            _listeCallBacks.Remove(j);            
            _listeConnectes.Remove(RecupererIdentifiantConnexionSelonJoueur(j));       
        }
        
        /// <summary>
        /// r�cup�ration de l'identifiant de connexion d'un joueur
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private Guid RecupererIdentifiantConnexionSelonJoueur(Joueur j)
        { 
            return (from cleValeur in _listeConnectes where cleValeur.Value.Identifiant == j.Identifiant select cleValeur.Key ).First();
        }

        /// <summary>
        /// Le nom du joueur est il correct ?
        /// </summary>
        /// <param name="nom">le nom</param>
        /// <returns>vrai si le nom est correct</returns>
        private bool NomCorrect(string nom)
        {
            return (nom.Length == 0 || nom.IndexOfAny(new char[] { ',', ';', '|' }) > -1);
        }

        private ResultatConnection VerificationConditionsConnections(Joueur expediteur, ICallBack callBack)
        {
            ResultatConnection res;
            if (expediteur.EstSpectateur)
            {
                // Connexion en mode spectateur
                res = Connection(expediteur, callBack);
                logServeur.Debug("Connection du spectateur {0} = {1}", expediteur.Nom, res.Connection);
            }
            else
            {
                // Connexion en mode joueur
                if (InfosTable.Singleton.PartieEnCours)
                {
                    res = new ResultatConnection(RetourConnection.PartieEnCours);
                    logServeur.Debug("Connection du joueur {0} impossible : Partie en cours", expediteur.Nom);
                }
                else if (InfosTable.Singleton.ListeJoueurs.Count >= NOMBRE_MAXI_JOUEURS)
                {
                    res = new ResultatConnection(RetourConnection.TropDeJoueurs);
                    logServeur.Debug("Connection du joueur {0} impossible : Trop de joueurs ({1}) sur la table", expediteur.Nom, NOMBRE_MAXI_JOUEURS);
                }
                else if (NomCorrect(expediteur.Nom))
                {
                    logServeur.Debug("Connection du joueur impossible : Nom incorrect");
                    res = new ResultatConnection(RetourConnection.NomIncorrect);
                }
                else if (InfosTable.Singleton.RechercherJoueur(expediteur.Nom) != null)
                {
                    res = new ResultatConnection(RetourConnection.NomDejaUtilise);
                    logServeur.Debug("Connection du joueur {0} impossible : Nom d�j� utilis�", expediteur.Nom);
                }
                else
                {
                    res = Connection(expediteur, callBack);
                    
                    logServeur.Debug("Connection du joueur {0} = {1}", expediteur.Nom, res.Connection);
                }
            }

            return res;
        }

        /// <summary>
        /// Recherche d'un joueur en fonction de son id de connexion
        /// </summary>
        /// <param name="idConnection">L'id </param>
        /// <returns>renvoie null si le joueur n'a pas �t� trouv�</returns>
        private Joueur RechercherJoueurSelonIdConnexion(Guid idConnexion)
        {
            Joueur res = null;
            _listeConnectes.TryGetValue(idConnexion, out res);

            return res;
        }


        /// <summary>
        /// Recherche de l'id de connexion d'un joueur
        /// </summary>
        /// <param name="joueur"></param>
        /// <returns></returns>
        private Guid RechercherIdConnexionSelonJoueur(Joueur joueur)
        {
            return (from idConn in _listeConnectes where idConn.Value.Equals(joueur) select idConn.Key).FirstOrDefault();
        }


        private ResultatConnection Connection(Joueur expediteur, ICallBack callBack)
        {            
            ResultatConnection res = new ResultatConnection(RetourConnection.Ok) { IdentifiantConnexion = Guid.NewGuid()};
            expediteur.Identifiant = Guid.NewGuid();
            expediteur.EstConnecte = true;

            if (!expediteur.EstSpectateur)
            {
                // Le 1er connect� est administrateur
                if (InfosTable.Singleton.ListeJoueurs.Count == 0)
                {
                    _idAdmin = res.IdentifiantConnexion;
                    res.AdminServeur = true;
                    expediteur.AdministrateurServeur = true;
                }
                    
                
                res.PositionJoueur = InfosTable.Singleton.AjouterJoueur(expediteur);

                // On envoie le nouveau joueur � tous les anciens clients sauf le demandeur (OperationContract IsOneWay oblige)
                foreach (Joueur j in _listeConnectes.Values)
                {
                    if (_listeCallBacks.ContainsKey(j))
                    {
                        if (j != expediteur)
                        {
                            // TODO : que fait on si un client s'est d�j� d�sist� ?
                            // Pour l'instant : on ne fait rien et il sera automatiquement �limin�
                            _listeCallBacks[j].RecevoirNouveauJoueur(expediteur, res.PositionJoueur);
                        }
                    }
                }
            }
            else
            {                
                res.PositionJoueur = 0;
            }

            _listeCallBacks.Add(expediteur, callBack);
            _listeConnectes[res.IdentifiantConnexion] = expediteur;
            res.ListeJoueurs = InfosTable.Singleton.ListeJoueurs;            

            return res;
        }

        /// <summary>
        /// D�marrage du timer d'attente d'un joueur
        /// </summary>
        /// <param name="timeout"></param>
        private void DemarrerTimerActionJoueur(int timeout)
        {
            _timeOutActionJoueur = new Timer(FinTimerActionJoueur, null, timeout * 10 * POURCENTAGE_ATTENTE_ACTIONJOUEUR, System.Threading.Timeout.Infinite);                        
        }

        /// <summary>
        /// Le timer de fin se d�clenche : le joueur fait parole (converti en passe si parole n'est pas possible)
        /// </summary>
        /// <param name="o"></param>
        private void FinTimerActionJoueur(object o)
        {
            logServeur.Debug("Le joueur {0} a mis trop longtemps a jou� : on fait une parole � sa place", InfosTable.Singleton.JoueurCourant.Nom);
            this.EnvoyerAction(RechercherIdConnexionSelonJoueur(InfosTable.Singleton.JoueurCourant), new ActionJoueur(TypeActionJoueur.Parole));
        }

        /// <summary>
        /// Arret du timer
        /// </summary>
        private void StopperTimerActionJoueur()
        {
            if (_timeOutActionJoueur != null) _timeOutActionJoueur.Dispose();
        }

        /// <summary>
        /// On termine le tour d'enchere : correspond � une �tape
        /// </summary>
        private EtapeDonne TerminerTourEnchere()
        {
            EtapeDonne etapeSuivante = InfosTable.Singleton.EtapeSuivante();
            if (etapeSuivante == EtapeDonne.FinDonne)
            {
                InfosTable.Singleton.JoueurCourant.TourDeJeu = false;
                MAJJoueurs(true);
                MessageInfo msg = InfosTable.Singleton.DistribuerGains(InfosTable.Singleton.CalculerMainsGagnantes(), InfosTable.Singleton.MiseMaximum(), true);

                _statistiques.EnregistrerFinDonne(InfosTable.Singleton.ListeJoueurs, msg.CombinaisonGagnante, msg.JoueursEmpochantPot, msg.MontantPotEmpoche);
                // On log
                logServeur.Debug("Les joueurs : ");
                foreach (Joueur j in msg.JoueursEmpochantPot)
                {
                    logServeur.Debug("  * {0} ", j.Nom);
                }
                logServeur.Debug("empochent le pot de {0}", msg.MontantPotEmpoche);
                EnvoyerMessageInformation(msg);
                Thread.Sleep(InfosTable.Singleton.OptionsJeu.TimeOutFinDonneCartesMontrees * 1000);
                InfosTable.Singleton.RAZMiseJoueurs(true);
                NouvelleDonne();
            }
            else
            {
                if (etapeSuivante == EtapeDonne.Flop)
                {
                    _statistiques.EnregistrerFlop(InfosTable.Singleton.Board[0], InfosTable.Singleton.Board[1], InfosTable.Singleton.Board[2], InfosTable.Singleton.Pot);
                }
                else if (etapeSuivante == EtapeDonne.Turn)
                {
                    _statistiques.EnregistrerTurn(InfosTable.Singleton.Board[3], InfosTable.Singleton.Pot);
                }
                else if (etapeSuivante == EtapeDonne.River)
                {
                    _statistiques.EnregistrerRiver(InfosTable.Singleton.Board[4], InfosTable.Singleton.Pot);
                }
                
                Thread.Sleep(TEMPS_ATTENTE_ENTRE_COUPS);
                EnvoyerMessageInformation(new MessageInfo(etapeSuivante, InfosTable.Singleton.Board));
                MAJJoueurs(false);
            }

            return etapeSuivante;
        }
        #endregion

        #region Evenements
        void _augmentationBlinds_AugmentationBlinds(object sender, Blind infosBlind)
        {
            _augmentationBlinds = true;
            _statistiques.EnregistrerAugmentationBlinds(infosBlind);
            EnvoyerMessageInformation(new MessageInfo(infosBlind));
        }
        #endregion

        #region Methodes static        

        public static bool DemarrerServeur(string adresse)
        {
            ServiceHost host = new ServiceHost(typeof(Serveur));
            host.AddServiceEndpoint(typeof(Poker.Interface.Communication.IServeur), new NetTcpBinding(SecurityMode.None), adresse);
            // D�marrage du serveur 
            bool serveurDemarre = false;
            try
            {   
                host.Open();
                InitialisationServeur();
                serveurDemarre = true;
            }
            catch (Exception ex)
            {
                serveurDemarre = false;
                logServeur.Debug("Erreur lors du d�marrage du serveur : " + ex.Message);
            }
            return serveurDemarre;
        }

        /// <summary>
        /// Diff�rentes initialisations du serveurs
        ///  InfosAdmin / Statistiques / Bots ...
        /// </summary>
        private static void InitialisationServeur()
        {
            // InfosAdmin
            _infosAdmin = new InfosAdmin();
            _methodeAugmentationBlinds = RecupererMethodeAugmentationBlinds();
            _infosAdmin.OptionsParDefaut = new Options();
            _infosAdmin.MethodeCalculAugmentationBlind = new InfosAdmin.CalculAugmentationBlinds();
            _infosAdmin.MethodeCalculAugmentationBlind.LibelleMethode = _methodeAugmentationBlinds.Description;
            _infosAdmin.MethodeCalculAugmentationBlind.LibelleParametre = _methodeAugmentationBlinds.DescriptionParametre;  

            // Liste des bots            
            _infosAdmin.ListeBots = RecupererListeBots();

            // Statistiques
            _statistiques = RecupererStatistiques();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using Poker.Interface.ExtensionsClient.Replay;
using Poker.Interface.Metier;
using Poker.Interface.Outils;
using Poker.Interface.Stats;

namespace Poker.Client
{
    class GestionReplay
    {
        #region Evenements
        public event MessagePublicHandler MessagePublic;
        public event MessageInformationHandler MessageInformation;
        public event ChangementInfosJoueurHandler ChangementInfosJoueur;
        public event ChangementInfosJoueurSansCartesHandler ChangementInfosJoueurSansCartes;
        public event DemarrageNouvellePartieHandler DemarrageNouvellePartie;        
        public event ChangementDonneHandler ChangementDonne;        
        #endregion

        #region Attributs
        private int _delaiCoup = 1000; // Delai entre chaque coup
        private Timer _timerEvt;
        private int _montantPetiteBlind = 0;
        private int _montantGrosseBlind = 0;
        private Dictionary<string, Joueur> _listeJoueurs = null;
        private int? _pot = 0;
        private CartePoker[] _board;
        private bool _ecouterChangementDonne = true;
        private bool _finPartie = false;
        #endregion

        #region Methodes publiques
        /// <summary>
        /// Demarrage de la lecture de la partie
        /// </summary>
        /// <param name="lecturePartie"></param>
        /// <param name="partie"></param>
        internal void DemarrerLecturePartie(ILecturePartie lecturePartie, Partie partie)
        {
            this.PartieChoisie = partie;
            this.LecturePartie = lecturePartie;
            this.LecturePartie.ChangementDonne += new ChangementDonneHandler(LecturePartie_ChangementDonne);
            this.LecturePartie.DemarrageLecturePartie(this.PartieChoisie);

            _timerEvt_Elapsed(this, null);
            if (_timerEvt != null)
            {
                _timerEvt.Stop();
            }
            _timerEvt = new Timer(_delaiCoup);
            _timerEvt.AutoReset = true;
            _timerEvt.Elapsed += new ElapsedEventHandler(_timerEvt_Elapsed);
        }

        internal void AllerDonne(int numDonne)
        {
            try
            {
                _ecouterChangementDonne = false;
                _finPartie = false;
                bool timerDemarre = _timerEvt.Enabled;
                if (timerDemarre) PauserLecturePartie();
                TraiterEvenement(this.LecturePartie.AllerDonne(numDonne));
                if (timerDemarre) ReprendreLecturePartie();
                _ecouterChangementDonne = true;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Erreur dans le changement de la donne n° {0}. Erreur = {1}", numDonne, ex);
                logClient.Debug(msg);
                throw new ApplicationException(msg, ex);
            }
        }

        internal void PauserLecturePartie()
        {
            if (_timerEvt != null) _timerEvt.Stop();
        }

        internal void ReprendreLecturePartie()
        {
            if (_timerEvt != null) _timerEvt.Start();
        }
        #endregion

        #region Proprietes
        /// <summary>
        /// Le mode de lecture de la partie
        /// </summary>
        public ILecturePartie LecturePartie { get; private set; }

        /// <summary>
        /// La partie en cours de lecture
        /// </summary>
        public Partie PartieChoisie { get; private set; }
        #endregion

        #region Evenements
        void _timerEvt_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_finPartie)
            {
                try
                {
                    logClient.Debug("Lecture d'un evenement de jeu");
                    TraiterEvenement(this.LecturePartie.AvancerEvenement());
                }
                catch (Exception ex)
                {
                    logClient.Debug("******* Erreur lors de la lecture d'un evenement : " + ex.Message);
                }
            }
        }

        #endregion

        #region Methodes privées
        /// <summary>
        /// Lecture 
        /// </summary>
        /// <param name="evt"></param>
        void TraiterEvenement(EvenementJeu evt)
        {
            if (evt != null)
            {
                Type typeEvt = evt.GetType();
                logClient.Debug(" Traitement evenement de type : " + typeEvt.Name);
                if (typeEvt == typeof(NouvellePartie))
                {
                    TraiterEvtNouvellePartie(evt as NouvellePartie);
                }
                else if (typeEvt == typeof(NouvelleDonne))
                {
                    TraiterEvtNouvelleDonne(evt as NouvelleDonne);
                }
                else if (typeEvt == typeof(AugmentationBlinds))
                {
                    TraiterEvtAugmentationBlinds(evt as AugmentationBlinds);
                }
                else if (typeEvt == typeof(Chat))
                {
                    TraiterEvtChat(evt as Chat);
                }
                else if (typeEvt == typeof(ActionJoueurStat))
                {
                    TraiterEvtActionJoueur(evt as ActionJoueurStat);
                }
                else if (typeEvt == typeof(Flop))
                {
                    TraiterEvtFlop(evt as Flop);
                }
                else if (typeEvt == typeof(Turn))
                {
                    TraiterEvtTurn(evt as Turn);
                }
                else if (typeEvt == typeof(River))
                {
                    TraiterEvtRiver(evt as River);
                }
                else if (typeEvt == typeof(FinDonne))
                {
                    TraiterEvtFinDonne(evt as FinDonne);
                }
                else if (typeEvt == typeof(FinPartie))
                {
                    TraiterEvtFinPartie(evt as FinPartie);
                }
                else
                {
                    logClient.Debug("======== Evenement non reconnu");
                }
            }
            else
            {
                logClient.Debug("******* Erreur lors de la lecture d'un evenement : L'evenement est null");
            }
        }

        void LecturePartie_ChangementDonne(object sender, ChangementDonneEventArgs e)
        {
            if (_ecouterChangementDonne && ChangementDonne != null)
            {
                ChangementDonne(this, e);
            }
        }

        private void LancerEvtChangementInfosJoueurSansCarte(string nom, int mise, int tapis)
        {
            if (ChangementInfosJoueurSansCartes != null)
            {
                _listeJoueurs[nom].Mise = mise;
                _listeJoueurs[nom].TapisJoueur = tapis;
                ChangementInfosJoueurSansCartes(_listeJoueurs[nom], _pot);
            }
        }

        private void LancerEvtChangementInfosJoueur(string nom, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot)
        {
            if (ChangementInfosJoueur != null)
            {
                ChangementInfosJoueur(_listeJoueurs[nom], etatDeLaMain, carte1, carte2, pot);
            }
        }

        private void LancerEvtMessageInfo(MessageInfo msg)
        {
            if (MessageInformation != null)
            {
                MessageInformation(msg);
            }
        }

        private void TraiterEvtFinPartie(FinPartie evt)
        {
            _finPartie = true;
            List<Joueur> listeGagnants = new List<Joueur>();
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                _listeJoueurs[j.Nom].PositionElimination = j.PositionElimination;
                listeGagnants.Add(_listeJoueurs[j.Nom]);
            }
            LancerEvtMessageInfo(new MessageInfo(listeGagnants));            
        }

        private void TraiterEvtFinDonne(FinDonne evt)
        {
            List<Joueur> listeGagnants = new List<Joueur>();
            foreach (JoueurStat j in evt.JoueursGagnants)
            {
                listeGagnants.Add(_listeJoueurs[j.Nom]);
            }
            LancerEvtMessageInfo(new MessageInfo(listeGagnants, evt.CombinaisonGagnante, evt.Pot));
        }

        private void TraiterEvtFlop(Flop evt)
        {
            _board[0] = evt.Carte1;
            _board[1] = evt.Carte2;
            _board[2] = evt.Carte3;
            _pot = evt.Pot;
            LancerEvtMessageInfo(new MessageInfo(EtapeDonne.Flop, _board));
        }

        private void TraiterEvtTurn(Turn evt)
        {
            _board[3] = evt.Carte;
            _pot = evt.Pot;
            LancerEvtMessageInfo(new MessageInfo(EtapeDonne.Turn, _board));
        }

        private void TraiterEvtRiver(River evt)
        {
            _board[4] = evt.Carte;
            _pot = evt.Pot;
            LancerEvtMessageInfo(new MessageInfo(EtapeDonne.River, _board));
        }


        private void TraiterEvtActionJoueur(ActionJoueurStat evt)
        {
            _listeJoueurs[evt.Nom].DerniereAction = evt.TypeAction;
            AffecterTourDeJeu(evt.Nom);
            if (evt.TypeAction == TypeActionJoueur.Passe)
            {
                LancerEvtChangementInfosJoueur(evt.Nom, EtatMain.JeteePersonnelle, null, null, _pot);
            }
            else
            {
                LancerEvtChangementInfosJoueurSansCarte(evt.Nom, evt.Mise, evt.Tapis);
            }            
            LancerEvtMessageInfo(new MessageInfo(_listeJoueurs[evt.Nom], new ActionJoueur(evt.TypeAction, evt.Mise)));
        }

        private void TraiterEvtChat(Chat chat)
        {
            if (MessagePublic != null)
            {
                MessagePublic(new ChatMessage(chat.Message), chat.Nom);
            }
        }

        private void TraiterEvtNouvellePartie(NouvellePartie evt)
        {
            _listeJoueurs = new Dictionary<string, Joueur>();
            List<Joueur> resListe = new List<Joueur>();
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                Joueur jReplay = new Joueur(j.Nom);
                jReplay.Identifiant = Guid.NewGuid();
                jReplay.TapisJoueur = j.Tapis;
                _listeJoueurs[j.Nom] = jReplay;
                resListe.Add(jReplay);
            }

            if (DemarrageNouvellePartie != null)
            {
                DemarrageNouvellePartie(resListe);
            }
            if (MessageInformation != null)
            {
                MessageInformation(new MessageInfo(MessageJeu.NouvellePartie));
            }
        }

        private void TraiterEvtAugmentationBlinds(AugmentationBlinds evt)
        {
            logClient.Debug("* Augmentation Blinds");
            Blind infosBlind = new Blind();
            infosBlind.DelaiAugmentationBlinds = TimeSpan.Zero;
            infosBlind.MontantPetiteBlind = evt.MontantPetiteBlind;
            _montantPetiteBlind = infosBlind.MontantPetiteBlind;
            infosBlind.MontantGrosseBlind = evt.MontantGrosseBlind;
            _montantGrosseBlind = infosBlind.MontantGrosseBlind;
            infosBlind.MontantProchainePetiteBlind = infosBlind.MontantProchainePetiteBlind;
            infosBlind.MontantProchaineGrosseBlind = infosBlind.MontantProchaineGrosseBlind;
            infosBlind.DelaiAugmentationBlinds = infosBlind.DelaiAugmentationBlinds;

            LancerEvtMessageInfo(new MessageInfo(infosBlind));
        }

        private void TraiterEvtNouvelleDonne(NouvelleDonne evt)
        {
            logClient.Debug("* Nouvelle Donne");
            Blind infosBlind = evt.InfosBlind;
            _pot = null;
            _board = new CartePoker[5];

            LancerEvtMessageInfo(new MessageInfo(MessageJeu.NouvelleDonne) { InfosBlind = infosBlind });

            // Dealer, PetiteBlind, Grosse Blind
            ReinitialiserBouton(evt.Dealer, evt.PetiteBlind, evt.GrosseBlind);

            // Liste des cartes
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                Joueur joueurEnCours = _listeJoueurs[j.Nom];
                joueurEnCours.Carte1 = j.Carte1;
                joueurEnCours.Carte2 = j.Carte2;
                joueurEnCours.TapisJoueur = j.Tapis;
                joueurEnCours.TourDeJeu = false;
                joueurEnCours.Mise = 0;
                joueurEnCours.Elimine = (j.Tapis == 0);
                joueurEnCours.DerniereAction = TypeActionJoueur.Aucune;
                if (joueurEnCours.TapisJoueur == 0)
                {
                    LancerEvtChangementInfosJoueur(joueurEnCours.Nom, EtatMain.PasDeCartes, j.Carte1, j.Carte2, 0);
                }
                else
                {
                    LancerEvtChangementInfosJoueur(joueurEnCours.Nom, EtatMain.Montree, j.Carte1, j.Carte2, 0);
                }
            }
        }

        private void ReinitialiserBouton(string dealer, string petiteBlind, string grosseBlind)
        {
            // Dealer, PetiteBlind, Grosse Blind
            foreach (Joueur j in _listeJoueurs.Values)
            {
                j.Bouton = TypeBouton.Aucun;
            }
            _listeJoueurs[dealer].Bouton |= TypeBouton.Dealer;
            _listeJoueurs[petiteBlind].Bouton |= TypeBouton.PetiteBlind;
            _listeJoueurs[grosseBlind].Bouton |= TypeBouton.GrosseBlind;
        }

        private void AffecterTourDeJeu(string tourDuJoueur)
        {
            foreach (Joueur j in _listeJoueurs.Values)
            {
                j.TourDeJeu = (tourDuJoueur == j.Nom);
                LancerEvtChangementInfosJoueurSansCarte(j.Nom, j.Mise, j.TapisJoueur);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Communication;
using Poker.Interface.Metier;

namespace Poker.Client
{
    /// <summary>
    /// Classe permettant la gestion du callBack
    /// </summary>
    public class CommunicationServeur : ICallBack
    {
        #region Evenements
        public event MessagePublicHandler MessagePublic;
        public event MessageInformationHandler MessageInformation;
        public event NouveauJoueurHandler NouveauJoueur;
        public event ChangementInfosJoueurHandler ChangementInfosJoueur;
        public event ChangementInfosJoueurSansCartesHandler ChangementInfosJoueurSansCartes;
        public event RegardHandler Regard;
        public event ActionsPossiblesHandler ActionsPossibles;
        public event OptionsPartieHandler OptionsPartie;
        #endregion

        #region ICallBack Membres
        /// <summary>
        /// Réception d'un message public
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="expediteur"></param>
        public void RecevoirMessagePublic(ChatMessage msg, string expediteur)
        {
            logClient.Debug("Methode 'RecevoirMessagePublic'");
            logClient.Debug("   Message = " + msg.Contenu);
            logClient.Debug("   Expedit = " + expediteur);
            if (MessagePublic != null)            
                MessagePublic(msg, expediteur);
        }

        /// <summary>
        /// Réception du message d'information
        /// </summary>
        /// <param name="msg"></param>
        public void RecevoirMessageInformation(MessageInfo msg)
        {
            logClient.Debug("Methode 'RecevoirMessageInformation'");
            logClient.Debug("   TypeMessage = " + msg.TypeMessage);
            if (msg.Action != null) logClient.Debug("   TypeAction  = " + msg.Action.TypeAction);
            logClient.Debug("   EtapeDistri = " + msg.EtapeDistribution);
            if (MessageInformation != null)
                MessageInformation(msg);
        }

        /// <summary>
        /// Réception d'un nouveua joueur
        /// </summary>
        /// <param name="nouveauJoueur"></param>
        /// <param name="positionTable">La position du joueur</param>
        public void RecevoirNouveauJoueur(Joueur nouveauJoueur, int positionTable)
        {
            logClient.Debug("Methode 'RecevoirNouveauJoueur'");
            logClient.Debug("   Joueur = " + nouveauJoueur.Nom);
            logClient.Debug("   Position = " + positionTable.ToString());
            if (NouveauJoueur != null)
                NouveauJoueur(nouveauJoueur, positionTable);
        }

        /// <summary>
        /// Modification des infos d'un joueur + les cartes + le pot 
        /// </summary>
        /// <param name="joueurAChanger"></param>
        /// <param name="carte1"></param>
        /// <param name="carte2"></param>
        /// <param name="pot"></param>
        public void ChangerJoueur(Joueur joueurAChanger, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot)
        {
            logClient.Debug("Methode 'ChangerJoueur'");
            logClient.Debug("   Joueur   = " + joueurAChanger.Nom);
            logClient.Debug("   DerniereAction = " + joueurAChanger.DerniereAction.ToString());
            logClient.Debug("   Tapis = " + joueurAChanger.TapisJoueur.ToString());
            logClient.Debug("   JeterCartes = " + joueurAChanger.JeterCartes.ToString());
            logClient.Debug("   TourDeJeu = " + joueurAChanger.TourDeJeu.ToString());
            logClient.Debug("   EtatMain = " + etatDeLaMain);
            logClient.Debug("   Bouton = " + joueurAChanger.Bouton.ToString());
            logClient.Debug("   Carte1   = " + carte1);
            logClient.Debug("   Carte2   = " + carte2);
            if (pot.HasValue) logClient.Debug("   Pot      = " + pot.Value.ToString());
            if (ChangementInfosJoueur != null)
                ChangementInfosJoueur(joueurAChanger, etatDeLaMain, carte1, carte2, pot);
        }

        /// <summary>
        /// Modification des infos d'un joueur + le pot 
        /// </summary>
        /// <param name="joueurAChanger"></param>
        /// <param name="carte1"></param>
        /// <param name="carte2"></param>
        /// <param name="pot"></param>
        public void ChangerJoueurSansCartes(Joueur joueurAChanger, int? pot)
        {
            //logClient.Debug("Methode 'ChangerJoueurSansCartes'");
            //logClient.Debug("   Joueur   = " + joueurAChanger.Nom);
            //logClient.Debug("   DerniereAction = " + joueurAChanger.DerniereAction.ToString());
            //logClient.Debug("   Tapis = " + joueurAChanger.TapisJoueur.ToString());
            //logClient.Debug("   JeterCartes = " + joueurAChanger.JeterCartes.ToString());
            //logClient.Debug("   TourDeJeu = " + joueurAChanger.TourDeJeu.ToString());
            //logClient.Debug("   Bouton = " + joueurAChanger.Bouton.ToString());
            //if (pot.HasValue) logClient.Debug("   Pot      = " + pot.Value.ToString());
            if (ChangementInfosJoueurSansCartes != null)
                ChangementInfosJoueurSansCartes(joueurAChanger, pot);
        }

        /// <summary>
        /// Réception d'un regard de carte
        /// </summary>
        /// <param name="joueurRegard">Le joueur regarde sa carte</param>
        /// <param name="regard">Début ou fin du regard</param>
        public void RecevoirRegard(Joueur expediteur, EtatMain etatDeLaMain)
        {
            if (Regard != null)
                Regard(expediteur, etatDeLaMain);
        }

        /// <summary>
        /// Réception de la liste des actions possibles
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="timeout">Délai de réponse</param>
        public void RecevoirListeActionsPossibles(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout)
        {
            logClient.Debug("Methode 'RecevoirListeActionsPossibles'");            
            foreach (KeyValuePair<TypeActionJoueur, ActionPossible> act in actions)
                logClient.Debug("   action {0}   : min = {1}, max = {2}", act.Key, act.Value.MontantMin, act.Value.MontantMax);
            logClient.Debug("   TimeOut = " + timeout.ToString());
            if (ActionsPossibles != null)
                ActionsPossibles(actions, timeout);
        }

        /// <summary>
        /// Réception des options de la partie
        /// </summary>
        /// <param name="optionsPartie"></param>
        public void RecevoirOptionsPartie(Options optionsPartie)
        {
            logClient.Debug("Methode 'RecevoirOptionsPartie'");
            if (OptionsPartie != null)
                OptionsPartie(optionsPartie);
        }
        #endregion
        
    }
}

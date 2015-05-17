using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Poker.Interface.Communication;
using Poker.Interface.Metier;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Poker.Interface.Bot
{

    /// <summary>
    /// Classe abstraite de définition d'un bot
    /// </summary>    
    public abstract class BotPokerBase : IBotPoker
    {
        #region Membres privés
        protected IServeur _serveur;
        protected List<Joueur> _listeJoueurs;
        protected Joueur _bot;
        protected Options _optionsPartie;
        protected Guid _identifiantConnection = Guid.Empty;
        protected int _positionTable = 0;
        #endregion

        #region IBotPoker Membres
        /// <summary>
        /// Initialisation du bot : comprend la phase de connection
        /// </summary>
        /// <param name="serveur"></param>
        /// <param name="nom"></param>
        /// <returns></returns>
        public void Initialiser(IServeur serveur, Guid identifiantConnexion, Joueur bot, List<Joueur> listeJoueurs, int positionTable)
        {
            _bot = bot;
            _serveur = serveur;
            _listeJoueurs = listeJoueurs;
            _identifiantConnection = identifiantConnexion;
            _positionTable = positionTable;
        }
        #endregion

        #region ICallBack Membres
        /// <summary>
        /// Réception d'un message de chat
        /// </summary>
        /// <param name="msg">Le message en question</param>
        /// <param name="expediteur">L'expéditeur du message</param>
        public virtual void RecevoirMessagePublic(ChatMessage msg, string expediteur)
        { 
        }
        
        /// <summary>
        /// Réception d'un message d'information
        /// </summary>
        /// <param name="msg">Le message d'information</param>
        public virtual void RecevoirMessageInformation(MessageInfo msg)
        {
            
        }

        /// <summary>
        /// Réception d'un nouveau joueur : recu à la connection de ce nouveau joueur
        /// </summary>
        /// <param name="nouveauJoueur"></param>
        public virtual void RecevoirNouveauJoueur(Joueur nouveauJoueur, int positionTable)
        {
            _listeJoueurs.Add(nouveauJoueur);
        }

        /// <summary>
        /// Changement des infos d'un joueur
        /// </summary>
        /// <param name="joueurAChanger">Le joueur & ses infos</param>
        /// <param name="etatDeLaMain">L'etat de la main à afficher</param>
        /// <param name="carte1">Carte1 éventuellement</param>
        /// <param name="carte2">Carte2 éventuellement</param>
        /// <param name="pot">Le pot</param>
        public virtual void ChangerJoueur(Joueur joueurAChanger, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot)
        {
            if (joueurAChanger == _bot)
            {
                _bot.Carte1 = carte1;
                _bot.Carte2 = carte2;
            }

        }

        /// <summary>
        /// Idem ChangerJoueur mais sans changement de cartes
        /// </summary>
        /// <param name="joueurAChanger">Le joueur & ses infos</param>
        /// <param name="pot">le pot</param>
        public virtual void ChangerJoueurSansCartes(Joueur joueurAChanger, int? pot)
        {
            
        }

        /// <summary>
        /// Réception d'un regard : on sait quel est le joueur qui regarde ses cartes
        /// </summary>
        /// <param name="joueurRegard">Le joueur qui regarde</param>
        /// <param name="etatDeLaMain"></param>
        public virtual void RecevoirRegard(Joueur joueurRegard, EtatMain etatDeLaMain)
        {
        }

        /// <summary>
        /// Réception de la liste des actions possibles : déclenche une demande d'action
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="timeout"></param>
        public virtual void RecevoirListeActionsPossibles(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout)
        {
        }

        /// <summary>
        /// Réception des options de la partie
        /// </summary>
        /// <param name="optionsPartie"></param>
        public virtual void RecevoirOptionsPartie(Options optionsPartie)
        {
            _optionsPartie = optionsPartie;
        }
        #endregion       

        #region Methodes protégées
        /// <summary>
        /// Log d'un message par le bot
        /// </summary>
        /// <param name="message"></param>
        protected virtual void logBot(string message)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Bot {0} : {1}", _bot.Nom, message));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur lors du log d'un bot : " + ex.Message);
            }
        }

        /// <summary>
        /// Log d'un message par le bot
        /// </summary>
        /// <param name="message"></param>
        protected virtual void logBot(string message, params object[] prms)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Bot {0} : {1}", _bot.Nom, string.Format(message, prms)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur lors du log d'un bot : " + ex.Message);
            }
        }
#endregion
    }
}

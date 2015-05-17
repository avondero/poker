using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Poker.Interface.Metier;

namespace Poker.Interface.Communication
{
    /// <summary>
    /// Interface pour le callback : Ensemble des m�thodes que le serveur peut appeler sur le client
    /// </summary>
    public interface ICallBack
    {
        /// <summary>
        /// On retourne � tous les joueurs un message public
        /// </summary>
        /// <param name="msg">Le message</param>
        /// <param name="expediteur">L'exp�diteur du message</param>
        [OperationContract(IsOneWay = true)]
        void RecevoirMessagePublic(ChatMessage msg, string expediteur);

        /// <summary>
        /// Envoi d'un message d'information (Fenetre en haut)
        /// </summary>
        /// <param name="msg">Le message</param>
        [OperationContract(IsOneWay = true)]
        void RecevoirMessageInformation(MessageInfo msg);

        /// <summary>
        /// Envoie le joueur qui vient de se connecter
        /// </summary>
        /// <param name="nouveauJoueur">Le nouveau joueur</param>
        /// <param name="positionTable">Sa position autour de la table (varie de 0 � listeJoueurs.count</param>
        [OperationContract(IsOneWay = true)]
        void RecevoirNouveauJoueur(Joueur nouveauJoueur, int positionTable);

        /// <summary>
        /// Changement d'un joueur
        /// </summary>
        /// <param name="joueurAChanger">Le joueur concern�</param>
        /// <param name="carte1">Sa premi�re carte</param>
        /// <param name="carte2">Sa deuxi�me carte</param>
        /// <param name="pot">Le pot</param>
        [OperationContract(IsOneWay = true)]
        void ChangerJoueur(Joueur joueurAChanger, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot);

        /// <summary>
        /// Changement d'un joueur sans changer ses cartes
        /// </summary>
        /// <param name="joueurAChanger">Le joueur concern�</param>
        [OperationContract(IsOneWay = true)]
        void ChangerJoueurSansCartes(Joueur joueurAChanger, int? pot);

        /// <summary>
        /// Un joueur est en train de regarder ses cartes
        /// </summary>
        /// <param name="joueurRegard">Le joueur en question</param>
        /// <param name="etatDeLaMain">L'etat de la main</param>
        [OperationContract(IsOneWay = true)]
        void RecevoirRegard(Joueur joueurRegard, EtatMain etatDeLaMain);

        /// <summary>
        /// R�ception de la liste des actions possibles
        /// </summary>
        /// <param name="actions">Liste des actions possibles</param>
        /// <param name="timeout">Timeout : au bout de ce d�lai, le serveur consid�re que le client a jet�</param>
        [OperationContract(IsOneWay = true)]
        void RecevoirListeActionsPossibles(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout);

        /// <summary>
        /// R�ception des options de la partie
        /// </summary>
        /// <param name="optionsPartie"></param>
        [OperationContract(IsOneWay = true)]
        void RecevoirOptionsPartie(Options optionsPartie);

    }
}

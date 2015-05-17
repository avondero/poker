using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Poker.Interface.Metier;
using Poker.Interface.Bot;

namespace Poker.Interface.Communication
{    
    [ServiceContract(CallbackContract = typeof(ICallBack), SessionMode = SessionMode.Required)]
    public interface IServeur
    {
        #region Methodes autorisées pour tous les joueurs
        /// <summary>
        /// Le serveur recoit un message et va le redispatcher à tous les clients
        /// </summary>
        /// <param name="msg">Le message</param>
        /// <param name="expediteur">L'expéditeur</param>        
        [OperationContract(IsOneWay = true)]
        void EnvoyerMessagePublic(ChatMessage msg, Guid idExpediteur);

        /// <summary>
        /// Connection au serveur
        /// </summary>
        /// <param name="expediteur">Le demandeur de la connection</param>    
        /// <returns>Le résultat de la demande de connection + la liste des joueurs dans leur ordre d'arrivée</returns>
        [OperationContract()]
        ResultatConnection Connecter(Joueur expediteur);

        /// <summary>
        /// Déconnection du serveur
        /// </summary>
        /// <param name="expediteur">Le demandeur</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        void Deconnecter(Guid idExpediteur);

        /// <summary>
        /// Un joueur est en train de regarder ses cartes...
        /// </summary>
        /// <param name="expediteur"></param>
        /// <param name="debutRegard">Vrai : c'est le début, faux : c'est fini</param>
        [OperationContract(IsOneWay = true)]
        void EnvoyerRegard(Guid idExpediteur, bool debutRegard);

        /// <summary>
        /// Envoi d'une action par un joueur
        /// </summary>
        /// <param name="action">L'action demandée par le joueur</param>
        /// <remarks>L'expéditeur de l'action est dans l'action</remarks>
        [OperationContract(IsOneWay = true)]
        void EnvoyerAction(Guid idExpediteur, ActionJoueur action);
        #endregion

        #region Methodes autorisées uniquement à l'administrateur
        /// <summary>
        /// Changement des options
        /// </summary>
        /// <param name="expediteur">Le demandeur</param>
        /// <param name="opt">Les options de jeu qui changent</param>
        /// <remarks>Toutes les options de jeu ne seront pas forcément prises en compte</remarks>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        void ChangerOptions(Guid idAdmin, Options opt);

        /// <summary>
        /// Démarrage de la partie
        /// </summary>
        /// <param name="expediteur">Le demandeur</param>
        [OperationContract(IsOneWay = true)]
        void DemarrerPartie(Guid idExpediteur, Options optionsJeu);

        /// <summary>
        /// Ajout d'un bot
        /// </summary>
        /// <param name="cheminAssembly">Chemin vers l'assembly contenant le bot</param>
        /// <param name="typeBot">Le nom du type de la classe contenant le bot</param>
        /// <param name="nom">Le nom du bot, tel qu'il apparaitra aux autres joueurs</param>
        [OperationContract(IsOneWay=true)]
        void AjouterBot(Guid idAdmin, string typeBot, string nom);

        /// <summary>
        /// Récupération des informations d'administration
        ///  En vrac : liste des bots (à faire), méthode de calcul des blinds...
        /// </summary>
        /// <returns></returns>
        [OperationContract()]
        InfosAdmin RecupererInformationsAdministrations();
        #endregion
    }
}

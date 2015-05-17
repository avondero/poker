using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// classe de description des messages d'informations du jeu : message s'affichant dans la fenetre en haut
    /// </summary>
    [DataContract]
    public class MessageInfo
    {
        #region Constructeur
        /// <summary>
        /// Création d'un message d'information tout simple
        /// </summary>
        /// <param name="typeMessage"></param>
        public MessageInfo(MessageJeu typeMessage) 
        {
           this.TypeMessage = typeMessage;
        }

        /// <summary>
        /// Création d'un message d'information "Nouvelle Partie" avec les options de jeu
        /// </summary>
        /// <param name="optionsPartie"></param>
        public MessageInfo(Options optionsPartie)
        {
            this.TypeMessage = MessageJeu.NouvellePartie;
            this.OptionsPartie = optionsPartie;
        }

        /// <summary>
        /// Création d'un message d'information associé à une action du joueur
        /// </summary>
        /// <param name="typeMessage"></param>
        public MessageInfo(Joueur expediteur, ActionJoueur action)
        {
            this.TypeMessage = MessageJeu.ActionJoueur;
            this.Action = action;
            this.Expediteur = expediteur;
        }

        /// <summary>
        /// Création d'un message d'information associé à un empochement de pot
        /// </summary>
        /// <param name="listeJoueursEmpochantPot">La liste des joueurs</param>
        /// <param name="montantPotEmpoche">Le montant du pot</param>
        public MessageInfo(List<Joueur> vainqueurs, Combinaison comb, int montantPotEmpoche)
        {
            this.TypeMessage = MessageJeu.PotEmpoche;
            this.JoueursEmpochantPot = vainqueurs;
            this.MontantPotEmpoche = montantPotEmpoche;
            this.CombinaisonGagnante = comb;
        }

        /// <summary>
        /// Création d'un message d'information associé à un empochement de pot
        /// </summary>
        /// <param name="listeJoueursEmpochantPot">La liste des joueurs</param>
        /// <param name="montantPotEmpoche">Le montant du pot</param>
        public MessageInfo(Joueur vainqueur, Combinaison comb, int montantPotEmpoche)
        {
            this.TypeMessage = MessageJeu.PotEmpoche;
            this.CombinaisonGagnante = comb;
            this.MontantPotEmpoche = montantPotEmpoche;
            this.JoueursEmpochantPot = new List<Joueur>();
            this.JoueursEmpochantPot.Add(vainqueur);

        }

        /// <summary>
        /// Création d'un message d'information de fin de partie
        /// </summary>
        /// <param name="classement"></param>
        public MessageInfo(List<Joueur> classement)
        {
            this.TypeMessage = MessageJeu.FinPartie;
            this.Classement = classement;
        }

        /// <summary>
        /// Création d'un message d'information relatif à une distribution du board
        /// </summary>
        /// <param name="etape"></param>
        public MessageInfo(EtapeDonne etape, CartePoker[] board)
        {
            this.TypeMessage = MessageJeu.Distribution;
            this.EtapeDistribution = etape;
            if (board.Length > 5)
                throw new Exception("Erreur lors de l'envoi du board");
            this.Board = board;
        }

        /// <summary>
        /// Création d'un  message d'information relatif à l'augmentation des blinds
        /// </summary>
        /// <param name="montantPetiteBlind"></param>
        /// <param name="montantGrosseBlind"></param>
        public MessageInfo(Blind infosBlind)
        {
            this.TypeMessage = MessageJeu.AugmentationBlinds;
            this.InfosBlind = infosBlind;
        }        
        #endregion

        #region Methodes publiques
        /// <summary>
        /// Options de la partie
        /// </summary>
        [DataMember]
        public Options OptionsPartie { get; set; }
        
        /// <summary>
        /// Le board quand il change
        /// </summary>
        [DataMember]
        public CartePoker[] Board { get; set; }

        /// <summary>
        /// Etape de distribution
        /// </summary>
        [DataMember]
        public EtapeDonne EtapeDistribution { get; set; }

        /// <summary>
        /// Type de message envoyé
        /// </summary>
        [DataMember]
        public MessageJeu TypeMessage { get; set; }

        /// <summary>
        /// L'action du joueur
        /// </summary>
        [DataMember]
        public ActionJoueur Action { get; set; }

        /// <summary>
        /// Liste des joueurs qui ont empoché le pot
        /// </summary>
        [DataMember]
        public List<Joueur> JoueursEmpochantPot { get; set; }

        /// <summary>
        /// Renvoie le montant du pot empoche
        /// </summary>
        [DataMember]
        public int MontantPotEmpoche { get; set; }

        /// <summary>
        /// Renvoie la liste des joueurs : permet d'établir le classement grace à la propriété positionElimination
        /// </summary>
        [DataMember]
        public List<Joueur> Classement { get; set; }

        /// <summary>
        /// La combinaison gagnante
        /// </summary>
        [DataMember]
        public Combinaison CombinaisonGagnante { get; set; }

        /// <summary>
        /// L'expéditeur de l'action
        /// </summary>
        [DataMember]
        public Joueur Expediteur { get; set; }

        /// <summary>
        /// le détail de la blind
        /// </summary>
        [DataMember]
        public Blind InfosBlind { get; set; }
        #endregion
    }
}

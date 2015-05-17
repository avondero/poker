using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Classe décrivant l'ensemble des informations d'un joueur de Poker
    /// </summary>
    [DataContract()]
    public class Joueur :ICloneable
    {
        #region Constructeurs
        public Joueur()
        {
            this.EstConnecte = true;
        }

        public Joueur(string nomJoueur) : this()
        {
            this.Nom= nomJoueur;
            this.Identifiant = Guid.NewGuid();
        }

        #endregion

        #region Attributs
        private bool _jeterCartes = false;
        #endregion

        #region Methodes overridées
        /// <summary>
        /// 2 joueurs sont égaux si leur identifiants sont égaux
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ((Joueur) obj).Identifiant == this.Identifiant;
        }

        /// <summary>
        /// Le HashCode d'une joueur
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Identifiant.GetHashCode();
        }
        #endregion

        #region IJoueur Membres
        /// <summary>
        /// Derniere action du joueur
        /// </summary>
        [DataMember()]
        public TypeActionJoueur DerniereAction { get; set; }

        /// <summary>
        /// Renvoie le nom du joueur
        /// </summary>
        [DataMember()]
        public string Nom { get; set; }
        
        /// <summary>
        /// Renvoie le montant du pot du joueur
        /// </summary>
        [DataMember()]
        public int TapisJoueur { get; set; }

        /// <summary>
        /// Renvoie ou fixe la première carte du joueur
        /// </summary>
        public CartePoker Carte1 { get; set; }

        /// <summary>
        /// Renvoie ou fixe la deuxième carte du joueur
        /// </summary>
        public CartePoker Carte2 { get; set; }

        /// <summary>
        /// Le joueur est il connecté ?
        /// </summary>
        public bool EstConnecte { get; set; }

        /// <summary>
        /// Le joueur est il en train de distribuer
        /// </summary>
        [DataMember()]
        public TypeBouton Bouton{ get; set; }

        /// <summary>
        /// Le joueur est il administrateur du serveur
        /// </summary>
        [DataMember()]
        public bool AdministrateurServeur { get; set; }

        /// <summary>
        /// L'identifiant unique du joueur : sert à identifier le joueur et coté serveur et côté client
        /// </summary>    
        [DataMember()]
        public Guid Identifiant { get; set; }

        /// <summary>
        /// Le joueur est il éliminé
        /// </summary>
        [DataMember]
        public bool Elimine { get; set; }

        /// <summary>
        /// Au joueur de joué
        /// </summary>
        [DataMember()]
        public bool TourDeJeu { get; set; }

        /// <summary>
        /// Le joueur est il un bot ?
        /// </summary>
        public bool Bot { get; set; }

        /// <summary>
        /// Le "joueur" se connecte-t-il en spectateur ?
        /// </summary>
        [DataMember]
        public bool EstSpectateur { get; set; }

        /// <summary>
        /// Le joueur a t il jeté ses cartes
        /// </summary>
        [DataMember]
        public bool JeterCartes
        {
            get { return _jeterCartes || this.Elimine; }
            set { _jeterCartes = value || this.Elimine; }
        }

        /// <summary>
        /// La mise en cours du joueur : uniquement pour l'étape de jeu (1 mise pour le flop, 1 pour le river ...)
        /// </summary>
        [DataMember]
        public int Mise { get; set; }

        /// <summary>
        /// L'ensemble des mises faites par le joueur à l'exception de l'étape en cours
        /// </summary>
        [DataMember]
        public int MiseTotale { get; set; }

        /// <summary>
        /// Position d'élimination (le joueur qui a la plus grande position est le vainqueur
        /// </summary>
        [DataMember]
        public int PositionElimination { get; set; }
        
        #endregion
      
        #region ICloneable Membres
        /// <summary>
        /// Clone de l'objet en cours
        /// </summary>
        /// <returns>L'objet cloné</returns>
        public object Clone()
        {
            return this.MemberwiseClone(); ;
        }

        #endregion
    }
}

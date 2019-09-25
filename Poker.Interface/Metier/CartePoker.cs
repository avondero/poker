using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Poker.Interface.Outils;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Une carte du jeu de poker
    /// </summary>
    [DataContract()]
    [DebuggerDisplay("{Hauteur} de {Couleur}")]
   // [DebuggerVisualizer(typeof(CartePokerVisualizer))]
    [Serializable]
    public class CartePoker : IComparable<CartePoker>
    {
        #region Membres privés
        private HauteurCarte _hauteur;
        private CouleurCarte _couleur;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Unique constructeur d'une carte
        /// </summary>
        /// <param name="hauteur"></param>
        /// <param name="couleur"></param>
        public CartePoker(HauteurCarte hauteur, CouleurCarte couleur)
        {
            _hauteur = hauteur;
            _couleur = couleur;
            
        }
        #endregion

        #region Propriétés publiques
        /// <summary>
        /// Hauteur d'une carte
        /// </summary>
        [DataMember]
        public HauteurCarte Hauteur
        { 
            get { return _hauteur; }
            set { _hauteur = value; }
        }
        /// <summary>
        /// Couleur d'une carte
        /// </summary>
        [DataMember]
        public CouleurCarte Couleur
        {
            get { return _couleur; }
            set { _couleur = value; }
        }
        #endregion

        #region Methodes surchargées
        public override bool Equals(object obj)
        {
            CartePoker carteAComparee = (CartePoker)obj;
            return carteAComparee.Couleur == this.Couleur && carteAComparee.Hauteur == this.Hauteur;
        }

        public override int GetHashCode()
        {
            return string.Format("{0};{1}", _hauteur, _couleur).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} de {1}", _hauteur, _couleur);
        }
        #endregion

        #region Implémentation de IComparable
        /// <summary>
        /// Comparaison de 2 cartes de Poker
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(CartePoker carte1)
        {
            if (carte1 == null)
                return 1;
            else
                return ((int)this.Hauteur).CompareTo((int)carte1.Hauteur);
        }
        
        #endregion
    } 
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Une action faite par le joueur
    /// </summary>
    [DataContract]
    public class ActionJoueur
    {
        #region Constructeurs        
        /// <summary>
        /// Création d'une action avec un montant 
        /// </summary>
        /// <param name="expediteur">La source de l'action</param>
        /// <param name="type">Le type de l'action</param>
        /// <param name="montant">Le montant de l'action</param>
        public ActionJoueur(TypeActionJoueur type, int montant )
        {
            _typeAction = type;
            _montantAction = montant;
        }

        /// <summary>
        /// Création d'une action avec un montant égal à 0
        /// </summary>
        /// <param name="type">Le type de l'action</param>
        public ActionJoueur(TypeActionJoueur type)
            : this(type, 0)
        {
        }

        /// <summary>
        /// Création d'une action sans montant, ni type 
        /// </summary>
        public ActionJoueur()
            : this(TypeActionJoueur.Aucune, 0)
        { }
        #endregion

        #region Membres privés
        private TypeActionJoueur _typeAction;
        private int _montantAction = 0;
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Type d'action du joueur
        /// </summary>
        [DataMember]
        public TypeActionJoueur TypeAction
        {
            get { return _typeAction; }
            set { _typeAction = value; }
        }

        /// <summary>
        /// Montant de l'action faite par le joueur
        /// </summary>
        [DataMember]
        public int Montant
        {
            get { return _montantAction; }
            set { _montantAction = value; }
        }              
        #endregion

        #region Membres statiques
        /// <summary>
        /// Action Passe d'un joueur
        /// </summary>
        public static ActionJoueur Passe
        {
            get
            {
                return new ActionJoueur(TypeActionJoueur.Passe);
            }
    }

        /// <summary>
        /// Parole
        /// </summary>
        public static ActionJoueur Parole
        {
            get
            {
                return new ActionJoueur(TypeActionJoueur.Parole);
            }
        }
        #endregion
    }
}

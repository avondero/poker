using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Une action que peut faire par le joueur
    /// </summary>
    [DataContract]
    public class ActionPossible
    {
        #region Membres  privés
        private static ActionPossible _actionJeter = new ActionPossible(TypeActionJoueur.Passe);        
        #endregion

        #region Constructeurs
        /// <summary>
        /// Création d'une action possible
        /// </summary>
        /// <param name="type">Le type de l'action</param>
        /// <param name="active">Action active</param>
        public ActionPossible(TypeActionJoueur type)
        {
            _typeAction = type;
        }

        /// <summary>
        /// Création d'une action possible
        /// </summary>
        /// <param name="type">Le type de l'action</param>
        /// <param name="active">Action active</param>
        /// <pa
        public ActionPossible(TypeActionJoueur type, int montantMin, int montantMax)
            : this(type)
        {
            _montantMin = montantMin;
            _montantMax = montantMax;
        }
        #endregion

        #region Membres privés
        private TypeActionJoueur _typeAction;        
        private int _montantMin = int.MinValue, _montantMax = int.MaxValue;
        #endregion

        #region Proprietes publiques        
        /// <summary>
        /// Montant min de l'action : renseigné uniquement si TypeAction = Relance
        /// </summary>
        [DataMember]
        public int MontantMin
        {
            get { return _montantMin; }
            set { _montantMin = value; }
        }

        /// <summary>
        /// Montant min de l'action : renseigné uniquement si TypeAction = Relance
        /// </summary>
        [DataMember]
        public int MontantMax
        {
            get { return _montantMax; }
            set { _montantMax = value; }
        }
        #endregion

        #region Proprietes statiques

        /// <summary>
        /// L'action jeter /passer activée
        /// </summary>
        public static ActionPossible Jeter
        {
            get
            {
                return _actionJeter;
            }
        }
#endregion        
    }
}
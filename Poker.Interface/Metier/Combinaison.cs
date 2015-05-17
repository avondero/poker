using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Une combinaison de Poker
    /// </summary>
    [DataContract]
    public class Combinaison
    {
        #region Membres privés
        #endregion

        #region constructeur
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public Combinaison(TypeCombinaison type, List<CartePoker> mainGagnante)
        {
            this.TypeCombinaison = type;
            this.MainGagnante = mainGagnante;
        }
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Liste des cartes qui permettent d'identifier la combinaison
        ///  Ex: pour une couleur : la premiere carte est de la couleur souhaité
        ///      Pour une quinte : la premiere carte est la plus forte de la quinte
        /// </summary>
        [DataMember]
        public List<CartePoker> MainGagnante {get;set;}

        /// <summary>
        /// Type de la combinaison
        /// </summary>
        [DataMember]
        public TypeCombinaison TypeCombinaison {get;set;}
        #endregion
    }
}

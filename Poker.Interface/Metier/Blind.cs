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
    public class Blind
    {
        #region Constructeur
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Montant de la petite blind
        /// </summary>
        [DataMember]
        public int MontantPetiteBlind { get; set; }
        /// <summary>
        /// Montant de la grosse blind
        /// </summary>
        [DataMember]
        public int MontantGrosseBlind { get; set; }
        /// <summary>
        /// Montant de la prochaien petite blind
        /// </summary>
        [DataMember]
        public int MontantProchainePetiteBlind { get; set; }
        /// <summary>
        /// Montant de la prochaine grosse blind
        /// </summary>
        [DataMember]
        public int MontantProchaineGrosseBlind { get; set; }
        /// <summary>
        /// Délai d'augmentation de cette blind
        /// </summary>
        [DataMember]
        public TimeSpan DelaiAugmentationBlinds { get; set; }
        #endregion
    }

}

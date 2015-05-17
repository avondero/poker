using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Augmentation des blinds
    /// </summary>
    public class AugmentationBlinds : EvenementJeu
    {
        /// <summary>
        /// Montant de la petite blind
        /// </summary>
        public int MontantPetiteBlind { get; set; }
        /// <summary>
        /// Montant de la grosse blind
        /// </summary>
        public int MontantGrosseBlind { get; set; }
        /// <summary>
        /// Prochain montant de la petite blind
        /// </summary>
        public int MontantProchainePetiteBlind { get; set; }
        /// <summary>
        /// Prochain montant de la grosse blind
        /// </summary>
        public int MontantProchaineGrosseBlind { get; set; }
        /// <summary>
        /// Delai avant prochaine augmentation des blinds
        /// </summary>
        public TimeSpan DelaiAugmentationBlinds { get; set; }
    }
}

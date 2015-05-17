using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Options de jeu
    /// </summary>
    public class OptionsStat
    {
        /// <summary>
        /// Nom de la classe d'augmentation des blinds
        /// </summary>
        public string DescriptionAugmentationBlinds { get; set; }
        /// <summary>
        /// Montant initial de la petite blind
        /// </summary>
        public int MontantInitialPetiteBlind { get; set; }
        /// <summary>
        /// Tapis initial de chaque joueur
        /// </summary>
        public int TapisInitial { get; set; }
    }
}

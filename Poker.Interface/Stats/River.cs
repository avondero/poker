using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// River
    /// </summary>
    public class River : EvenementJeu
    {
        /// <summary>
        /// Carte de la river
        /// </summary>
        public CartePoker Carte { set; get; }

        /// <summary>
        /// Le pot
        /// </summary>
        public int Pot { set; get; }
    }
}

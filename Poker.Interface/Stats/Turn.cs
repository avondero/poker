using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Turn
    /// </summary>
    public class Turn : EvenementJeu
    {
        /// <summary>
        /// Carte
        /// </summary>
        public CartePoker Carte { set; get; }

        /// <summary>
        /// Le pot
        /// </summary>
        public int Pot { set; get; }
    }
}

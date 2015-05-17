using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Le flop
    /// </summary>
    public class Flop : EvenementJeu
    {
        /// <summary>
        /// Carte 1
        /// </summary>
        public CartePoker Carte1 { set; get; }
        /// <summary>
        /// Carte 2
        /// </summary>
        public CartePoker Carte2 { set; get; }
        /// <summary>
        /// Carte 3
        /// </summary>
        public CartePoker Carte3 { set; get; }

        /// <summary>
        /// Le pot
        /// </summary>
        public int Pot { set; get; }
    }
}

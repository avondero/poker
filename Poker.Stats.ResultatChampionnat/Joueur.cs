using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Stats.ResultatChampionnat
{
    /// <summary>
    /// Une classe joueur
    /// </summary>
    class Joueur : ICloneable
    {
        /// <summary>
        /// Le nom du joueur
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Position elimination
        /// </summary>
        public int PositionElimination { get; set; }

        /// <summary>
        /// Le résultat que l'on souhaite mettre
        /// </summary>
        public string Resultat { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}

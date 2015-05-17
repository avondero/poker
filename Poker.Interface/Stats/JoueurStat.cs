using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Une classe joueur mais pour les stats uniquement
    /// </summary>
    public class JoueurStat
    {
        /// <summary>
        /// Nom du joueur 
        /// </summary>
        public string Nom { set; get; }
        /// <summary>
        /// Premiere Carte
        /// </summary>
        public CartePoker Carte1 { set; get; }
        /// <summary>
        /// Deuxieme carte
        /// </summary>
        public CartePoker Carte2 { set; get; }
        /// <summary>
        /// Ordre d'élimination (1 puis 2 puis ...)
        /// </summary>
        public int PositionElimination { get; set; }
        /// <summary>
        /// Tapis du joueur
        /// </summary>
        public int Tapis { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Action d'un joueur
    /// </summary>
    public class ActionJoueurStat : EvenementJeu
    {
        /// <summary>
        /// Nom du joueur 
        /// </summary>
        public string Nom { set; get; }

        /// <summary>
        /// Le tapis du joueur après son action
        /// </summary>
        public int Tapis { set; get; }

        /// <summary>
        /// Sa Mise en cours
        /// </summary>
        public int Mise { set; get; }
             
        /// <summary>
        /// Type d'action du joueur
        /// </summary>
        public TypeActionJoueur TypeAction { set; get; }        
    }
}

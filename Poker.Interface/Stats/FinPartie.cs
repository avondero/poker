using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Fin d'une partie
    /// </summary>
    public class FinPartie : EvenementJeu
    {
        /// <summary>
        /// Liste des joueurs avec leur classement
        /// </summary>
        public List<JoueurStat> ListeJoueurs { get; set; }
    }
}

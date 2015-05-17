using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Un démarrage de partie
    /// </summary>
    public class NouvellePartie : EvenementJeu
    {
        /// <summary>
        /// Les options de jeu
        /// </summary>
        public OptionsStat OptionsJeu { get; set; }

        /// <summary>
        /// La liste des joueurs
        /// </summary>
        public List<JoueurStat> ListeJoueurs { get; set; }
    }
}

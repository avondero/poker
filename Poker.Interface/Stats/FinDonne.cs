using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Fin d'une donne : on redonne la liste des joueurs avec leur tapis
    /// </summary>
    public class FinDonne : EvenementJeu
    {
        /// <summary>
        /// Liste des joueurs
        /// </summary>
        public List<JoueurStat> ListeJoueurs { get; set; }

        /// <summary>
        /// La main gagnante
        /// </summary>
        public Combinaison CombinaisonGagnante{ get; set; }

        /// <summary>
        /// Le joueur gagnant 
        /// </summary>
        public List<JoueurStat> JoueursGagnants { get; set; }

        /// <summary>
        /// Montant du pot gagné
        /// </summary>
        public int Pot { get; set; }

        /// <summary>
        /// Numéro de la donne
        /// </summary>
        public int NumeroDonne { get; set; }
    }
}

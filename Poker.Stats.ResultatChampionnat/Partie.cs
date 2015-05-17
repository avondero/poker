using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Stats.ResultatChampionnat
{
    class Partie
    {
        public Partie()
        {
            this.ListeJoueurs = new List<Joueur>();
        }

        public Partie(int enjeu, DateTime date) : this()
        {
            this.Enjeu = enjeu;
            this.DatePartie = date;
        }

        /// <summary>
        /// La liste des joueurs de cette partie
        /// </summary>
        public List<Joueur> ListeJoueurs { get; private set;}

        public int Enjeu { get; private set; }

        public DateTime DatePartie { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Nouvelle donne
    /// </summary>
    public class NouvelleDonne : EvenementJeu
    {
        /// <summary>
        /// Le numéro de la donne
        /// </summary>
        public int NumeroDonne { get; set; }

        /// <summary>
        /// Informations relatives à la blind
        /// </summary>
        public Blind InfosBlind { get; set; }

        /// <summary>
        /// Le dealer de la partie
        /// </summary>
        public string Dealer { get; set; }

        /// <summary>
        /// Le joueur qui est petite blind
        /// </summary>
        public string PetiteBlind { get; set; }

        /// <summary>
        /// Le joueur qui est grosse blind
        /// </summary>
        public string GrosseBlind { get; set; }

        /// <summary>
        /// Renvoie la liste des joueurs (sans leurs cartes pour éviter de la triche ;) )
        /// </summary>
        public List<JoueurStat> ListeJoueurs { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Client
{
    /// <summary>
    /// Classe d'extension de la classe joueur
    /// </summary>
    public static class ExtensionJoueur
    {
#region Extension de méthodes joueurs
        /// <summary>
        /// Le joueur est-il dealer ?
        /// </summary>
        /// <param name="j">Le joueur</param>
        /// <returns>vria ou faux</returns>
        public static bool EstDealer(this Joueur j)
        {
            return EstTypeBouton(j, TypeBouton.Dealer);
        }

        /// <summary>
        /// Le joueur est-il petite blind ?
        /// </summary>
        /// <param name="j">Le joueur</param>
        /// <returns>vria ou faux</returns>
        public static bool EstPetiteBlind(this Joueur j)
        {
            return EstTypeBouton(j, TypeBouton.PetiteBlind);
        }

        /// <summary>
        /// Le joueur est-il grosse blind ?
        /// </summary>
        /// <param name="j">Le joueur</param>
        /// <returns>vria ou faux</returns>
        public static bool EstGrosseBlind(this Joueur j)
        {
            return EstTypeBouton(j, TypeBouton.GrosseBlind);
        }
#endregion

#region
        private static bool EstTypeBouton(this Joueur j, TypeBouton bouton)
        {
            return ((j.Bouton & bouton) == bouton);
        }
#endregion
        
    }
}

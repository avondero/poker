using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.ExtensionsServeur.Metier;

namespace Poker.Interface.ExtensionsServeur.ExtensionsServeur.Communication
{
    /// <summary>
    /// Interface de définition d'un bot au poker
    /// </summary>
    public interface IBotPoker : ICallBack
    {
        /// <summary>
        /// Initialisation d'un bot
        /// </summary>
        /// <param name="nom">Son nom</param>
        /// <returns></returns>
        void Initialiser(IServeur serveur, Guid identifiantConnexion, Joueur bot, List<Joueur> listeJoueurs);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;
using Poker.Interface.Communication;

namespace Poker.Interface.Bot
{
    /// <summary>
    /// Interface de définition d'un bot au poker
    /// </summary>
    public interface IBotPoker : ICallBack
    {
        /// <summary>
        /// Initialisation d'un bot
        /// </summary>                
        void Initialiser(IServeur serveur, Guid identifiantConnexion, Joueur bot, List<Joueur> listeJoueurs, int positionTable);
    }
}

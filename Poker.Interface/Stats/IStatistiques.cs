using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Interface de génération des stats
    /// </summary>
    public interface IStatistiques
    {
        /// <summary>
        /// enregistrement d'un evenement de jeu
        /// </summary>
        /// <param name="evt">L'evenement en question</param>
        void Enregistrer(EvenementJeu evt);
    }
}

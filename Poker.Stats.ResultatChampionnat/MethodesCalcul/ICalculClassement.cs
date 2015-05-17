using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Stats.ResultatChampionnat.MethodesCalcul
{
    interface ICalculClassement
    {
        /// <summary>
        /// Calcul du classement
        /// </summary>
        /// <param name="listeParties"></param>
        /// <returns>Renvoie la liste des joueurs avec leurs points</returns>
        List<Joueur> CalculerClassement(List<Partie> listeParties);

        /// <summary>
        /// Le libellé de la méthode de calcul (apparait dans la combo)
        /// </summary>
        string Libelle { get; }

        /// <summary>
        /// Libelle qui s'affiche dans la colonne de résultat
        /// </summary>
        string LibelleColonneResultat { get; }
    }
}

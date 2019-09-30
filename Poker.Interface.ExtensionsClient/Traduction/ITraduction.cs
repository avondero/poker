using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Poker.Interface.Metier;

namespace Poker.Interface.ExtensionsClient.Traduction
{
    /// <summary>
    /// Interface de traduction
    /// </summary>
    public interface ITraducteur
    {
        /// <summary>
        /// Renvoie le libell� correspondant � la cl� dans la langue de la classe
        /// </summary>
        /// <param name="cle">La cl�</param>
        /// <returns>Renvoie la chaine traduite</returns>        
        string Traduire(string cle);

        /// <summary>
        /// Renvoie le libell� de la combinaison choisie
        /// </summary>
        /// <param name="comb"></param>
        /// <returns></returns>
        string TraduireCombinaison(Combinaison comb);

        /// <summary>
        /// Traduit le type de combinaison (suite, paire, double-paires ....)
        /// </summary>
        /// <param name="typeComb"></param>
        /// <returns></returns>
        string TraduireTypeCombinaison(TypeCombinaison typeComb);

        /// <summary>
        /// Affiche la langue de traduction
        /// </summary>
        string LangueTraduction { get;}

        /// <summary>
        /// Traduction d'une carte de poker
        /// </summary>
        /// <param name="carte"></param>
        /// <remarks>Non impl�ment� pour l'instant</remarks>
        /// <returns></returns>
        string TraduireCarte(Poker.Interface.Metier.CartePoker carte);


    }
}

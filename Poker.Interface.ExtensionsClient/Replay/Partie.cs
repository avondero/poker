using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.ExtensionsClient.Replay
{
    /// <summary>
    /// Une partie à rejouer
    /// </summary>
    public class Partie
    {
        /// <summary>
        /// Le nom de la partie
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        /// La description de la partie
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// L'identifiant de la partie
        /// </summary>
        public string Identifiant { get; set; }
    }
}

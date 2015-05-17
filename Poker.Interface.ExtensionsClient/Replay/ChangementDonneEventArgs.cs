using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.ExtensionsClient.Replay
{
    /// <summary>
    /// Classe de gestion des changements d'evenements
    /// </summary>
    public class ChangementDonneEventArgs : EventArgs
    {
        /// <summary>
        /// La nouvelle donne
        /// </summary>
        public int NumeroDonne { get; set; }
    }
}

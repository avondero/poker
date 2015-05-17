using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Une action de Chat
    /// </summary>
    public class Chat : EvenementJeu
    {
        /// <summary>
        /// L'emissaire
        /// </summary>
        public string Nom { set; get; }

        /// <summary>
        /// Le message
        /// </summary>
        public string Message { set; get; }
    }
}

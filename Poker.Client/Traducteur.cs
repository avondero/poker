using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.ExtensionsClient.Traduction;

namespace Poker.Client
{
    public static class OutilsTraduction
    {
        private static ITraducteur _traducteur;

        /// <summary>
        /// Fixe/r�cup�re le traducteur utilis�
        /// </summary>
        public static ITraducteur Traducteur
        {
            set
            {
                _traducteur = value;
            }
            get
            {
                return _traducteur;
            }
        }
    }
}

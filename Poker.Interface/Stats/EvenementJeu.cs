using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Classe abstraite de définition d'un evenement de jeu
    /// </summary>
    public abstract class EvenementJeu
    {

        private DateTime _dateEvt = DateTime.Now;

        /// <summary>
        /// Date de l'evenement
        /// </summary>
        public DateTime DateEvenement
        {
            get
            {
                return _dateEvt;
            }
            set
            {
                _dateEvt = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Bot
{
    /// <summary>
    /// Attribut utilisé pour la description d'un bot
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BotAttribute : Attribute
    {
        private int _niveau;
        private const int NIVEAU_MIN = 0;
        private const int NIVEAU_MAX = 10;

        private BotAttribute()
        {
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        /// <param name="description"></param>
        /// <param name="niveau"></param>
        public BotAttribute(string description, int niveau)
        {
            this.Description = description;
            this.Niveau = niveau;
        }

        /// <summary>
        /// Niveau d'un bot : de 0 à 10
        /// </summary>
        public int Niveau
        {
            get { return _niveau; }
            set
            {
                if (value < NIVEAU_MIN)
                    _niveau = NIVEAU_MIN;
                else if (value > NIVEAU_MAX)
                    _niveau = NIVEAU_MAX;
                else
                    _niveau = value;
            }
        }

        /// <summary>
        /// Description d'un bot
        /// </summary>
        public string Description { get; set; }
    }
}

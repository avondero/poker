using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Une description de bot : utilisée pour l'administration
    /// </summary>
    [DataContract]
    public class DescriptionBot
    {
        /// <summary>
        /// La description du bot
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// L'information permettant de recréer un bot : son type
        /// </summary>
        [DataMember]        
        public string TypeBot { get; set; }

    }
}

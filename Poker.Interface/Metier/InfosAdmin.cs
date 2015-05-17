using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Informations d'administration
    ///  Utilisée par le client (administrateur) pour avoir l'ensemble des possibilités
    /// </summary>
    [DataContract]
    public class InfosAdmin
    {
        [DataContract]
        public class CalculAugmentationBlinds
        {
            #region Proprietes
            [DataMember]
            public string LibelleMethode { get; set; }

            [DataMember]
            public string LibelleParametre { get; set; }
            #endregion
        }

        [DataMember]
        public CalculAugmentationBlinds MethodeCalculAugmentationBlind { get; set; }

        /// <summary>
        /// Options par défaut
        /// </summary>
        [DataMember]
        public Options OptionsParDefaut { get; set; }

        [DataMember]
        public List<DescriptionBot> ListeBots { get; set; }
    }
}

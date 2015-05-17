using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Classe contenant les messages d'informations envoyés
    /// </summary>
    [DataContract()]
    public class ChatMessage
    {
        private string _contenu;

        /// <summary>
        /// Contenu du message
        /// </summary>
        [DataMember()]
        public string Contenu
        {
            get { return _contenu; }
            set { _contenu = value; }
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        /// <param name="contenu"></param>
        public ChatMessage(string contenu)
        {
            _contenu = contenu;            
        }
    }
}

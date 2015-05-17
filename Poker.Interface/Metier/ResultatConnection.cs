using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// R�sultat d'une demande de connection
    /// </summary>
    [DataContract]
    public class ResultatConnection
    {
        /// <summary>
        /// Constructeur par d�faut
        /// </summary>
        /// <param name="retour"></param>
        public ResultatConnection(RetourConnection retour)
        {
            this.ListeJoueurs = new List<Joueur>();
            this.Connection = retour;
        }
       
        /// <summary>
        /// R�sultat de la demande de connection
        /// </summary>
        [DataMember]
        public RetourConnection Connection { get; set; }

        /// <summary>
        /// Liste des joueurs connect�s (y compris le demandeur)
        /// </summary>
        [DataMember]
        public List<Joueur> ListeJoueurs { get; set; }

        /// <summary>
        /// Identifiant de la connexion : cet identifiant est unique et permet de savoir qui est connect�
        /// </summary>
        [DataMember]
        public Guid IdentifiantConnexion { get; set; }

        /// <summary>
        /// La position du joueur autour de la table
        /// </summary>
        [DataMember]
        public int PositionJoueur { get; set; }

        /// <summary>
        /// Le joueur connect� est il administrateur
        /// </summary>
        [DataMember]
        public bool AdminServeur { get; set; }
    }
}

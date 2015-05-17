using System;
using System.Collections.Generic;
using System.Text;
using Poker.Client;

namespace PokerEnReseau
{
    /// <summary>
    /// Classe de stockage des informations affichées sur l'écran de connexion
    /// </summary>
    public class InfosEcranConnexion : ICloneable
    {
        #region Proprietes
        /// <summary>
        /// Le nom du joueur
        /// </summary>
        public string NomJoueur { get; set; }

        /// <summary>
        /// Langue choisie
        /// </summary>
        public string Langue { get; set; }

        /// <summary>
        /// Connexion en tant que spectateur
        /// </summary>
        public bool EstSpectateur { get; set; }

        /// <summary>
        /// L'adresse du serveur hébergeant la partie
        /// </summary>
        public string AdresseServeur { get; set; }

        /// <summary>
        /// Liste des serveurs déjà sélectionné
        /// </summary>
        public List<String> ListeServeurs { get; set; }

        /// <summary>
        /// On se lance en tant que serveur ?
        /// </summary>
        public bool EstServeur { get; set; }

        /// <summary>
        /// Mode de lancement du client : replay ou jeu normal
        /// </summary>
        public ModeClient ModeLancementClient { get; set; }
        #endregion

        #region Constructeurs
        public InfosEcranConnexion(string nomJoueur, string adresseServeur, string langue, bool estServeur, bool estSpectateur, List<String> listeServeurs)
            : this()
        {
            this.NomJoueur = nomJoueur;
            this.Langue = langue;
            this.EstServeur = estServeur;
            this.ListeServeurs = listeServeurs;
            this.AdresseServeur = adresseServeur;
            this.EstSpectateur = EstSpectateur;
        }

        public InfosEcranConnexion()
        {            
            this.ModeLancementClient = ModeClient.Jeu;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clone de l'objet
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}

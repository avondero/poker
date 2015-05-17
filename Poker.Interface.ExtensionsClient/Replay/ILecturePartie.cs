using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Stats;
using Poker.Interface.Metier;

namespace Poker.Interface.ExtensionsClient.Replay
{
    /// <summary>
    /// Delegate de changement de donne (déclenché par Avancer ou reculerEvenement)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ChangementDonneHandler(object sender, ChangementDonneEventArgs e);
    
    /// <summary>
    /// Delegate de démarrage d'une nouvelle partie
    /// </summary>
    /// <param name="listeJoueurs"></param>
    public delegate void DemarrageNouvellePartieHandler(List<Joueur> listeJoueurs);

    public interface ILecturePartie
    {
        /// <summary>
        /// Description de la méthode de lecture de la partie
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Evenement déclenché par AvancerEvenement ou ReculerEvenement
        /// </summary>
        event ChangementDonneHandler ChangementDonne;
        /// <summary>
        /// Liste des parties disponibles
        /// </summary>
        List<Partie> PartiesDisponibles();
        /// <summary>
        /// Démarrage de la lecture de la partie
        /// </summary>
        /// <param name="partieSelectionne"></param>
        void DemarrageLecturePartie(Partie partieSelectionne);
        /// <summary>
        /// Fin de la lecture de la partie
        /// </summary>
        /// <param name="partieSelectionne"></param>
        void FinLecturePartie(Partie partieSelectionne);
        /// <summary>
        /// Permet de se déplace directement à une donne
        /// </summary>
        /// <param name="numeroDonne"></param>
        /// <returns></returns>
        EvenementJeu AllerDonne(int numeroDonne);
      
        /// <summary>
        /// Renvoie le nombre de donnes
        /// </summary>
        /// <returns></returns>
        int NombreDeDonnes();
        /// <summary>
        /// Avance d'un evenement de jeu
        /// </summary>
        /// <returns></returns>
        EvenementJeu AvancerEvenement();
      
        /// <summary>
        /// Renvoie l'evenement courant
        /// </summary>
        /// <returns></returns>
        EvenementJeu EvenementCourant { get; }
    }
}

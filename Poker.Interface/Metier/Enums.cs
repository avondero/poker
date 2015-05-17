using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Hauteur d'une carte
    /// </summary>
    public enum HauteurCarte
    {
        Deux = 2,
        Trois = 3,
        Quatre = 4,
        Cinq = 5,
        Six = 6,
        Sept = 7,
        Huit = 8,
        Neuf = 9,
        Dix = 10,
        Valet = 11,
        Dame = 12,
        Roi = 13,
        As = 14
    }

    /// <summary>
    /// Couleur d'une carte
    /// </summary>
    public enum CouleurCarte
    {
        Trefle = 1,
        Carreau = 2,
        Coeur = 3,
        Pique = 4,
    }

    /// <summary>
    /// Etat possible des cartes d'une main
    /// </summary>
    public enum EtatMain
    { 
        /// <summary>
        /// Carte montrée
        /// </summary>        
        Montree,
        /// <summary>
        /// Carte jetée personnelle
        /// </summary>
        JeteePersonnelle,
        /// <summary>
        /// Carte jetée de l'adversaire
        /// </summary>
        JeteeConcurrente,
        /// <summary>
        /// Ces propres cartes 
        /// </summary>
        Personnelle,    
        /// <summary>
        /// Les cartes des concurrents
        /// </summary>
        Concurrente,
        /// <summary>
        /// Pas de carte pour ce joueur
        /// </summary>
        PasDeCartes,
        /// <summary>
        /// Le joueur est en train de regarder ses cartes
        /// </summary>
        Regardee
    }

    /// <summary>
    /// Action d'un joueur
    /// </summary>
    public enum TypeActionJoueur
    { 
        /// <summary>
        /// Aucune action du joueur : permet de montrer qu'il n'a rien fait
        /// </summary>
        Aucune,
        Passe,
        Suit,
        Parole,
        /// <summary>
        /// Mise = aucun joueur n'a parlé
        /// </summary>
        Mise,
        /// <summary>
        /// Relance = 1 joueur à déjà parlé
        /// </summary>
        Relance,
        Tapis,
        PetiteBlind,
        GrosseBlind
    }

    /// <summary>
    /// Message de jeu
    /// </summary>
    public enum MessageJeu
    { 
        NouvellePartie,
        AugmentationBlinds,
        NouvelleDonne,
        PotEmpoche,
        Distribution,
        ActionJoueur,
        FinPartie
    }
    
    /// <summary>
    /// Retour d'une demande de connection
    /// </summary>
    public enum RetourConnection
    { 
        Ok,
        TropDeJoueurs,
        NomDejaUtilise,
        NomIncorrect,
        PartieEnCours
    }

    /// <summary>
    /// Combinaison
    /// </summary>
    public enum TypeCombinaison
    {
        Carte,
        Paire,
        DoublePaire,
        Brelan,
        Quinte,
        Couleur,
        Full,
        Carre,
        QuinteFlush
    }

    /// <summary>
    /// Etapes du jeu
    /// </summary>
    public enum EtapeDonne
    {
        PreFlop,
        Flop,
        Turn,
        River,
        FinDonne
    }
   
    [Flags]
    public enum TypeBouton
    {
        Aucun = 0,
        Dealer = 1,
        PetiteBlind = 2,
        GrosseBlind = 4
    }
}

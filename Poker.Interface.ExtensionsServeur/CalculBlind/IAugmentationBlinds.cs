using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.ExtensionsServeur.Metier;

namespace Poker.Interface.ExtensionsServeur.CalculBlind
{
    /// <summary>
    /// Delegué utiliser pour le traitemetn de augmentation des blinds
    /// </summary>
    /// <param name="?"></param>
    public delegate void AugmentationBlindsHandler(object sender, Blind InfosBlind);

    /// <summary>
    /// Interface définissant la méthode d'augmentation des blinds
    ///  Lorsque la blind doit augmenter, l'évenement AugmentationBlinds est déclenché. InfosBlind contient les nouvelles infos relatives
    /// </summary>
    public interface IAugmentationBlinds
    {
        #region Evenements
        /// <summary>
        /// Evenement déclenchés lorsque la blind augmente
        /// </summary>
        event AugmentationBlindsHandler AugmentationBlinds;        
        #endregion

        #region Proprietes
        /// <summary>
        /// Récupération des infos relatives à la blind (montant actuelle, futur montant et délai)
        /// </summary>
        Blind InfosBlind { get; }
        
        /// <summary>
        /// Description de la méthode de calcul
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Paramètre supplémentaire
        /// </summary>
        string ParametreCalcul { get; set; }

        /// <summary>
        /// Le paramètre est il obligatoire
        /// </summary>
        bool ParametreObligatoire { get; }

        /// <summary>
        /// Description du paramètre
        /// </summary>
        string DescriptionParametre { get; }

        #endregion

        #region Methodes publiques
        /// <summary>
        /// Démarrage du calcul des blinds
        /// </summary>
        /// <param name="montantInitialPetiteBlind">Montant initial de la petite blind</param>
        /// <param name="nombreDeJoueurs">Nombre de joueurs</param>
        /// <param name="tapisInitial">Tapis initial de chaque joueur</param>
        void DemarrerCalculBlinds(int montantInitialPetiteBlind, int nombreDeJoueurs, int tapisInitial);        
        #endregion
    }
}

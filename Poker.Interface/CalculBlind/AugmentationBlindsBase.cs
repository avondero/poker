using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Poker.Interface.Metier;

namespace Poker.Interface.CalculBlind
{
    /// <summary>
    /// Classe de calcul de l'augmentation des blinds
    /// </summary>
    public abstract class AugmentationBlindsBase : IAugmentationBlinds
    {
        #region Constructeurs
        protected AugmentationBlindsBase()
        {
            this.InfosBlind = new Blind();
            _timerAugmentationBlinds = new Timer();
            _timerAugmentationBlinds.Elapsed += new ElapsedEventHandler(timerAugmentationBlinds_Elapsed);
        }

        #endregion

        #region Attributs
        /// <summary>
        /// Le timer
        /// </summary>
        protected Timer _timerAugmentationBlinds = null;
        #endregion

#region Methodes protégées
        /// <summary>
        /// Calcul de la nouvelle blind
        /// </summary>
        protected abstract void CalculNouvelleBlind();        

        /// <summary>
        /// Evenement déclenché par le timer
        ///  Il ne reste plus qu'à paramètre le timer
        ///  Cette méthode appelle la méthode CalculerNouvelleBlind
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void timerAugmentationBlinds_Elapsed(object sender, ElapsedEventArgs e)
        {
            CalculNouvelleBlind();
            if (AugmentationBlinds != null)
            {
                AugmentationBlinds(this, this.InfosBlind);
            }
        }
#endregion


        #region Methodes publiques
        /// <summary>
        /// Démarrage du calcul des blinds. 
        ///  Cette méthode initialise la petite blind et la grosse blind et démarre le timer
        /// </summary>
        /// <param name="montantInitialPetiteBlind">Montant initial de la petite blind</param>
        /// <param name="nombreDeJoueurs">Nombre de joueurs</param>
        /// <param name="tapisInitial">Tapis initial de chaque joueur</param>
        public virtual void DemarrerCalculBlinds(int montantInitialPetiteBlind, int nombreDeJoueurs, int tapisInitial)
        {
            if (this.ParametreObligatoire && string.IsNullOrEmpty(this.ParametreCalcul))
                throw new ApplicationException("Parametre obligatoire non renseigné");

            this.InfosBlind.MontantPetiteBlind = montantInitialPetiteBlind;
            this.InfosBlind.MontantGrosseBlind = 2*montantInitialPetiteBlind;
            _timerAugmentationBlinds.Start();
        }

        /// <summary>
        /// Méthode appelée à la fin de la partie
        /// </summary>
        public virtual void ArreterCalculBlinds()
        {
            _timerAugmentationBlinds.Stop();
        }
        #endregion

        #region IAugmentationBlinds Members
        /// <summary>
        /// Evenement déclenchés lorsque la blind augmente
        /// </summary>
        public event AugmentationBlindsHandler AugmentationBlinds;

        /// <summary>
        /// Récupération des infos relatives à la blind (montant actuelle, futur montant et délai)
        /// </summary>
        public Blind InfosBlind {get; protected set;}      

        /// <summary>
        /// Description de la méthode de calcul
        /// </summary>
        public abstract string Description {get;}

        /// <summary>
        /// Description du paramètre
        /// </summary>
        public abstract string DescriptionParametre { get; }

        /// <summary>
        /// Paramètre supplémentaire
        /// </summary>
        public virtual string ParametreCalcul {get; set;}
        
        /// <summary>
        /// Le paramètre est il obligatoire
        /// </summary>
        public abstract bool ParametreObligatoire {get;}        
        #endregion
    }
}

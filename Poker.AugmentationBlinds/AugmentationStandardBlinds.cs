using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.CalculBlind;
using Poker.Interface.Metier;

namespace Poker.AugmentationBlinds
{
    /// <summary>
    /// Classe de calcul standard de l'augmentation des blinds
    /// </summary>
    public class AugmentationStandardBlinds : AugmentationBlindsBase
    {
        #region Attributs
        protected int _delaiMinutes = 0;
        #endregion

        #region Constructeurs
        public AugmentationStandardBlinds()
            : base()
        {

        }
        #endregion

        #region Methodes protegees
        /// <summary>
        /// Calcul de la nouvelle blind : mise *1, mise * 2, *3, *5, *8, *13
        /// </summary>
        protected override void CalculNouvelleBlind()
        {
            int montantPetiteBlind = this.InfosBlind.MontantPetiteBlind;
            this.InfosBlind.MontantPetiteBlind = this.InfosBlind.MontantProchainePetiteBlind;
            this.InfosBlind.MontantGrosseBlind = this.InfosBlind.MontantProchaineGrosseBlind;
            this.InfosBlind.MontantProchainePetiteBlind = this.InfosBlind.MontantPetiteBlind + montantPetiteBlind;
            this.InfosBlind.MontantProchaineGrosseBlind = 2 * this.InfosBlind.MontantProchainePetiteBlind;
        }
        #endregion

        #region Implémentations de AugmentationBlindsBase
        /// <summary>
        /// Démarrage du calcul des blinds
        /// </summary>
        /// <param name="montantInitialPetiteBlind"></param>
        /// <param name="nombreDeJoueurs"></param>
        /// <param name="tapisInitial"></param>
        public override void DemarrerCalculBlinds(int montantInitialPetiteBlind, int nombreDeJoueurs, int tapisInitial)
        {
            this.InfosBlind.MontantProchainePetiteBlind = 2 * montantInitialPetiteBlind;
            this.InfosBlind.MontantProchaineGrosseBlind = 4 * montantInitialPetiteBlind;
            base.DemarrerCalculBlinds(montantInitialPetiteBlind, nombreDeJoueurs, tapisInitial);
        }
        
        /// <summary>
        /// Description de la méthode de calcul
        /// </summary>
        public override string Description
        {
            get
            {
                return "Toutes les x minutes";
            }
        }

        /// <summary>
        /// Paramètre supplémentaire
        /// </summary>
        public override string ParametreCalcul
        {
            get
            {
                return _delaiMinutes.ToString();
            }
            set
            {

                if (!int.TryParse(value, out _delaiMinutes))
                {
                    throw new ApplicationException("Le paramètre de calcul doit être un entier !!");
                }
                this.InfosBlind.DelaiAugmentationBlinds = new TimeSpan(0, _delaiMinutes, 0);
                _timerAugmentationBlinds.Interval = _delaiMinutes * 1000 * 60;
            }
        }

        /// <summary>
        /// Description du paramètre
        /// </summary>
        public override string DescriptionParametre
        {
            get
            {
                return "Délai en minutes";
            }
        }

        /// <summary>
        /// Le paramètre est il obligatoire
        /// </summary>
        public override bool ParametreObligatoire
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}

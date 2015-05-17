using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Poker.Interface.CalculBlind;

namespace Poker.Interface.Metier
{
    /// <summary>
    /// Classe contenant la liste des options modifiables
    /// </summary>
    [DataContract]
    public class Options
    {       
        #region Membres privés                
        private int _prmAugmentationBlinds = 5;                
        private int _montantPetiteBlindInitial = 10;
        private int _tapisInitial = 1000;
        private int _timeOutAction = 30;
        private int _timeOutFinDonneCartesCachees = 6;
        private int _timeOutFinDonneCartesMontrees = 10;
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Méthode d'augmentation des blinds
        /// </summary>
        public IAugmentationBlinds MethodeAugmentationBlinds { get; set; }

        /// <summary>
        /// Delai d'augmentation des blinds (en minutes)
        ///  Est significatif que si AugmentationBlindSelonDuree = true
        /// </summary>
        [DataMember]
        public int ParametreAugmentationBlinds
        {
            get { return _prmAugmentationBlinds; }
            set { _prmAugmentationBlinds = value; }
        }
        
        /// <summary>
        /// Time out sur une fin de donne : tout le monde a jeté ses cartes
        /// </summary>
        [DataMember]
        public int TimeOutFinDonneCartesCachees
        {
            get { return _timeOutFinDonneCartesCachees; }
            set { _timeOutFinDonneCartesCachees = value; }
        }

        /// <summary>
        /// Time out sur une fin de donne : des cartes ont été montrées
        /// </summary>
        [DataMember]
        public int TimeOutFinDonneCartesMontrees
        {
            get { return _timeOutFinDonneCartesMontrees; }
            set { _timeOutFinDonneCartesMontrees = value; }
        }

        /// <summary>
        /// Time out sur une action
        /// </summary>
        [DataMember]
        public int TimeOutAction
        {
            get { return _timeOutAction; }
            set { _timeOutAction = value; }
        }
       
        /// <summary>
        /// Montant initial de la petite blind
        /// </summary>
        [DataMember]
        public int MontantPetiteBlindInitial
        {
            get { return _montantPetiteBlindInitial; }
            set { _montantPetiteBlindInitial = value; }
        }

        /// <summary>
        /// Tapis initial de chaque joueur
        /// </summary>
        [DataMember]
        public int TapisInitial
        {
            get { return _tapisInitial; }
            set { _tapisInitial = value; }
        }       
        #endregion
    }
}

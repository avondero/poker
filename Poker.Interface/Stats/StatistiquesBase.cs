using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Interface.Stats
{
    /// <summary>
    /// Classe base : fourni notamment le filtrage en fonction de la classe evenement de jeu
    /// </summary>
    public abstract class StatistiquesBase : IStatistiques
    {
        #region IStatistiques Members
        /// <summary>
        /// enregistrement d'un evenement de jeu
        /// </summary>
        /// <param name="evt">L'evenement en question</param>
        public virtual void Enregistrer(EvenementJeu evt)
        {
            Type typeEvt = evt.GetType();
            if (typeEvt == typeof(NouvellePartie))
            {
                EnregistrerNouvellePartie(evt as NouvellePartie);
            }
            else if (typeEvt == typeof(NouvelleDonne))
            {
                EnregistrerNouvelleDonne(evt as NouvelleDonne);
            }
            else if (typeEvt == typeof(AugmentationBlinds))
            {
                EnregistrerAugmentationBlinds(evt as AugmentationBlinds);
            }
            else if (typeEvt == typeof(Chat))
            {
                EnregistrerChat(evt as Chat);
            }
            else if (typeEvt == typeof(ActionJoueurStat))
            {
                EnregistrerActionJoueur(evt as ActionJoueurStat);
            }
            else if (typeEvt == typeof(Flop))
            { 
                EnregistrerFlop(evt as Flop);
            }
            else if (typeEvt == typeof(Turn))
            {
                EnregistrerTurn(evt as Turn);
            }
            else if (typeEvt == typeof(River))
            {
                EnregistrerRiver(evt as River);
            }
            else if (typeEvt == typeof(FinDonne))
            {
                EnregistrerFinDonne(evt as FinDonne);
            }
            else if (typeEvt == typeof(FinPartie))
            {
                EnregistrerFinPartie(evt as FinPartie);
            }
            else
            {
                throw new ApplicationException("Type " + typeEvt.ToString() + " n'existe pas");
            }
        }
        
        #endregion

        #region Methodes virtuelles protegees
        protected virtual void EnregistrerTurn(Turn evt) { }

        protected virtual void EnregistrerRiver(River evt) { }

        protected virtual void EnregistrerFinDonne(FinDonne evt) { }

        protected virtual void EnregistrerFinPartie(FinPartie evt) { }

        protected virtual void EnregistrerFlop(Flop evt) { }

        protected virtual void EnregistrerActionJoueur(ActionJoueurStat evt) { }

        protected virtual void EnregistrerChat(Chat evt) { }

        protected virtual void EnregistrerAugmentationBlinds(AugmentationBlinds evt) { }

        protected virtual void EnregistrerNouvellePartie(NouvellePartie evt) { }

        protected virtual void EnregistrerNouvelleDonne(NouvelleDonne evt) { }
        #endregion
    }
}

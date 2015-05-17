using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Poker.Interface.Metier;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Poker.Client.AffichageCartes;

namespace Poker.Client
{

    /// <summary>
    /// Un panel de jeu de poker : contient la liste des joueurs, le pot ainsi que 
    /// </summary>
    internal class PokerPanel : Canvas
    {

        #region Evenements
        public delegate void CarteRegardeeEventHandler(Joueur sender, bool debutRegard);
        public event CarteRegardeeEventHandler CarteRegardee;

        void cntr_CarteRegardee(Joueur sender, bool debutRegard)
        {
            if (CarteRegardee != null) CarteRegardee(sender, debutRegard);
        }

        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public PokerPanel()
        {
        }
        #endregion

        #region Membres privés
        private Dictionary<Joueur, JoueurControl> _listJoueurs = new Dictionary<Joueur, JoueurControl>();
        #endregion

        #region Methodes privées
        private string RecupererLibelleDerniereAction(Joueur j)
        {
            string derniereAction = string.Empty;
            switch (j.DerniereAction)
            {
                case TypeActionJoueur.Aucune:
                    derniereAction = string.Empty;
                    break;

                case TypeActionJoueur.PetiteBlind:
                    derniereAction = string.Format(OutilsTraduction.Traducteur.Traduire("PokerPanelPetiteBlind"), j.Mise);
                    break;

                case TypeActionJoueur.GrosseBlind:
                    derniereAction = string.Format(OutilsTraduction.Traducteur.Traduire("PokerPanelGrosseBlind"), j.Mise);
                    break;

                case TypeActionJoueur.Passe:
                    derniereAction = OutilsTraduction.Traducteur.Traduire("PokerPanelPasse");
                    break;

                case TypeActionJoueur.Parole:
                    derniereAction = OutilsTraduction.Traducteur.Traduire("PokerPanelParole");
                    break;

                case TypeActionJoueur.Suit:
                    derniereAction = string.Format(OutilsTraduction.Traducteur.Traduire("PokerPanelSuit"), j.Mise);
                    break;

                case TypeActionJoueur.Mise:
                    derniereAction = string.Format(OutilsTraduction.Traducteur.Traduire("PokerPanelMise"), j.Mise);
                    break;

                case TypeActionJoueur.Relance:
                    derniereAction = string.Format(OutilsTraduction.Traducteur.Traduire("PokerPanelRelance"), j.Mise);
                    break;

                case TypeActionJoueur.Tapis:
                    derniereAction = string.Format(OutilsTraduction.Traducteur.Traduire("PokerPanelTapis"), j.Mise);
                    break;
            }

            return derniereAction;
        }
        #endregion

        #region Methodes publiques
        /// <summary>
        /// Affichage de la main gagnant
        /// </summary>
        /// <param name="listeJoueurs">Liste des joueurs ayant gagné</param>
        /// <param name="mainGagnante">Main ayant permis de gagner</param>
        public void AfficherMainGagnante(List<Joueur> listeJoueurs, Combinaison mainGagnante)
        {
            if (mainGagnante != null)
            {
                // Si la main gagnante est définie on l'affiche sur tous les joueurs
                foreach (Joueur j in listeJoueurs)
                {
                    _listJoueurs[j].ChangerLibelleCombinaison(OutilsTraduction.Traducteur.TraduireTypeCombinaison(mainGagnante.TypeCombinaison).ToUpper());
                }

                // On "efface" les cartes de tous les joueurs
                foreach (JoueurControl ctrl in _listJoueurs.Values)
                {
                    ctrl.AfficherMainGagnante(mainGagnante.MainGagnante);
                }
            }
        }

        /// <summary>
        /// Suppression des informations relatives à la main gagnante
        /// </summary>
        public void SupprimerMainGagnante()
        {
            foreach (JoueurControl ctrl in _listJoueurs.Values)
            {
                ctrl.ChangerLibelleCombinaison(string.Empty);
                ctrl.SupprimerMainGagnante();
            }
            
        }

        /// <summary>
        /// Modification des propriétés d'un joueur
        /// </summary>
        /// <param name="joueurAChanger">Les nouvelles infos du joueur</param>
        /// <param name="etatDeLaMain">Le nouvel etat de sa main</param>
        public void ModifierCartesJoueur(Joueur joueurAChanger, EtatMain etatDeLaMain)
        {
            if (etatDeLaMain == EtatMain.PasDeCartes)
            {
                _listJoueurs[joueurAChanger].ChangerDerniereAction(string.Empty);
            }
            else
            {
                _listJoueurs[joueurAChanger].ChangerDerniereAction(RecupererLibelleDerniereAction(joueurAChanger));
            }
            _listJoueurs[joueurAChanger].ChangerCartes(joueurAChanger, etatDeLaMain);
        }

        /// <summary>
        /// Modification des propriétés d'un joueur
        /// </summary>
        /// <param name="joueurAChanger">Les nouvelles infos du joueur</param>
        public void ModifierJoueur(Joueur joueurAChanger)
        {
            _listJoueurs[joueurAChanger].ChangerDerniereAction(RecupererLibelleDerniereAction(joueurAChanger));
            _listJoueurs[joueurAChanger].ChangerJoueur(joueurAChanger);
        }

        /// <summary>
        /// Ajout d'un joueur (pas de changment
        /// </summary>
        /// <param name="joueurAAjouter">Le joueur à ajouter</param>
        public void AjouterJoueur(Joueur joueurAAjouter, EtatMain etatDeLaMain, int positionTable)
        {
            // Si nécessaire, on modifie toutes les positions des joueurs précédents
            foreach (JoueurControl jcJoueur in _listJoueurs.Values)
            {
                if (positionTable <= jcJoueur.PositionTable)
                {
                    jcJoueur.PositionTable++;
                }
            }

            JoueurControl cntr = new JoueurControl();
            cntr.PositionTable = positionTable;
            _listJoueurs[joueurAAjouter] = cntr;
            cntr.CarteRegardee += new JoueurControl.CarteRegardeeEventHandler(cntr_CarteRegardee);
            ModifierJoueur(joueurAAjouter);
            this.Children.Add(cntr);

            
        }

        /// <summary>
        /// Ajout d'un joueur (pas de changment
        /// </summary>
        /// <param name="joueurAAjouter">Le joueur à ajouter</param>
        public void AjouterJoueur(Joueur joueurAAjouter, EtatMain etatDeLaMain)
        {
            this.AjouterJoueur(joueurAAjouter, etatDeLaMain, this.Children.Count);
        }

        /// <summary>
        /// Suppression  d'un joueur
        /// </summary>
        /// <param name="joueurASupprimer"></param>
        public void EnleverJoueur(Joueur joueurASupprimer)
        {
            //TODO ?
            throw new Exception("Methode non implémentée");
        }

        /// <summary>
        /// Réinitialise le panel : enleve tous les joueurs
        /// </summary>
        public void ReinitialiserPanel()
        {
            _listJoueurs = new Dictionary<Joueur, JoueurControl>();
            this.Children.Clear();
        }
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Permet de cacher ou de montrer ses propres cartes
        /// </summary>
        public bool CacherLesCartes
        {
            get
            {
                return true;
            }
            set
            {
                foreach (JoueurControl j in _listJoueurs.Values)
                {
                    j.CacherLesCartes = value;
                }
            }
        }

        /// <summary>
        /// Renvoie / fixe la position du joueur : de 0 à count - 1
        /// </summary>
        public int PositionJoueur { get; set; }
        #endregion

        #region Methodes surchargées
        /// <summary>
        /// Arrangement du Panel tel qu'on le souhaite
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Affichage des mains des joueurs
            foreach (JoueurControl uie in _listJoueurs.Values)
            {
                double angle = Math.PI / 2 - 2 * Math.PI * (this.PositionJoueur - uie.PositionTable) / _listJoueurs.Count;
                double currentOriginX = Math.Cos(angle) * (finalSize.Width / 2 - uie.DesiredSize.Width / 2 - 5) + finalSize.Width / 2 - uie.DesiredSize.Width / 2;
                double currentOriginY = Math.Sin(angle) * (finalSize.Height / 2 - uie.DesiredSize.Height / 2 - 5) + finalSize.Height / 2 - uie.DesiredSize.Height / 2;

                uie.RenderTransform = new TranslateTransform(currentOriginX, currentOriginY);
                uie.Arrange(new Rect(0, 0, uie.DesiredSize.Width, uie.DesiredSize.Height));
            }

            return finalSize;
        }
        #endregion
    }    
}   

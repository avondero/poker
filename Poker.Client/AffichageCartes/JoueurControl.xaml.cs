using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Poker.Interface.Metier;

namespace Poker.Client.AffichageCartes
{
    /// <summary>
    /// Controle personnalisé d'affichage d'une main (Contient les cartes, le tapis, le nom de chaque joueur)
    /// </summary>
    public partial class JoueurControl : System.Windows.Controls.UserControl
    {
        public JoueurControl()
        {
            InitializeComponent();
            imgCarte1.Clip = GestionCartes.FormeCarte;
            imgCarte2.Clip = GestionCartes.FormeCarte;
            lblCombinaison.Content = string.Empty;
        }

        #region Proprietes publiques

        /// <summary>
        /// Position autour de la table : permet le positionnement des joueurs
        /// </summary>
        public int PositionTable {get;set;}
        
        /// <summary>
        /// Propriete à vrai s'il faut cliquer pour voir les cartes
        ///  Ne marche que pour les joueurs dont on connait les cartes
        /// </summary>
        public bool CacherLesCartes
        {
            get
            {
                return _cacherLesCartes;
            }
            set
            {
                _cacherLesCartes = value;
                MettreAJourImageCarteSelonEtat();
            }
        }
        #endregion

        #region Membres privés
        private EtatMain _etatDeLaMain = EtatMain.PasDeCartes;
        private bool _boutonPresse = false;
        private Joueur _joueur;
        private bool _cacherLesCartes = false;
        private bool _carteMontree = false;
        private const double GRIP_SIZE = 30;
        private const string STYLE_JOUEUR_ACTIF = "JoueurActif";
        private const string STYLE_JOUEUR_INACTIF = "JoueurInactif";
        #endregion

        #region Methodes privées
        /// <summary>
        /// Récupération d'une image de carte en fonction de son état et de la valeur de la carte
        /// </summary>
        /// <param name="carte">La carte</param>
        /// <param name="main">L'état de la main</param>
        /// <param name="carteMontree">Le carte doit elle être montrée</param>
        /// <param name="cacherLesCartes">Les cartes sont elles tout le temps cachées ?</param>
        /// <returns></returns>
        private BitmapSource RecupererImageSelonEtat(CartePoker carte)
        {
            BitmapSource bi = null;
            switch (_etatDeLaMain)
            {
                // Affichage de la main d'un autre joueur
                case EtatMain.Concurrente:
                    bi = GestionCartes.RecupererImageCarteCachee();
                    break;

                // Affichage de carte jetée
                case EtatMain.JeteePersonnelle:
                case EtatMain.JeteeConcurrente:
                    if (_carteMontree)
                        bi = GestionCartes.RecupererImageSelonCarte(carte);
                    else
                        bi = GestionCartes.RecupererImageCarteJetee();
                    break;

                // Affichage de carte montrées
                case EtatMain.Montree:
                    bi = GestionCartes.RecupererImageSelonCarte(carte);
                    break;

                // Affichage de son propre jeu
                case EtatMain.Personnelle:
                    if (_carteMontree || !_cacherLesCartes)
                        bi = GestionCartes.RecupererImageSelonCarte(carte);
                    else
                        bi = GestionCartes.RecupererImageCarteCachee();
                    break;

                // Affichage d'une carte regardée
                case EtatMain.Regardee:
                    bi = GestionCartes.RecupererImageCarteRegardee();
                    break;

                // On ne renvoie rien
                case EtatMain.PasDeCartes:
                    break;
            }

            return bi;
        }

        /// <summary>
        /// Mise à jour des images en fonction de tous les paramètres 
        /// </summary>        
        private void MettreAJourImageCarteSelonEtat()
        {
            imgCarte1.Source = RecupererImageSelonEtat(_joueur.Carte1);
            imgCarte2.Source = RecupererImageSelonEtat(_joueur.Carte2);
        }

        /// <summary>
        /// Changement du tour de jeu du joueur : change le style de la grille des cartes
        /// </summary>
        /// <param name="tourDeJeu"></param>
        private void ChangerTourDeJeu(bool tourDeJeu)
        {
            string styleJoueur = tourDeJeu ? STYLE_JOUEUR_ACTIF : STYLE_JOUEUR_INACTIF;
            grdCartes.Style = this.FindResource(styleJoueur) as Style;
            if (tourDeJeu)
                this.ChangerDerniereAction(string.Empty);
        }

        /// <summary>
        /// Modifie le style de la carte demandée
        /// </summary>
        /// <param name="style"></param>
        /// <param name="img"></param>
        private void ModifierStyleCarte(string style, Image img)
        {
            img.Style = this.FindResource(style) as Style;
        }
        #endregion

        #region Méthodes publiques
        /// <summary>
        /// Suppression de toutes les mains gagnantes
        /// </summary>
        public void SupprimerMainGagnante()
        {
            this.ModifierStyleCarte(ConstantesClient.STYLE_AUCUN, imgCarte1);
            this.ModifierStyleCarte(ConstantesClient.STYLE_AUCUN, imgCarte2);
            this.ChangerCartes(_joueur, EtatMain.PasDeCartes);
        }

        /// <summary>
        /// Affiche la liste des cartes gagnantes en les mettant en valeur
        /// </summary>
        /// <param name="listeCartes"></param>
        public void AfficherMainGagnante(List<CartePoker> listeCartes)
        {
            if (_etatDeLaMain != EtatMain.JeteeConcurrente && _etatDeLaMain != EtatMain.JeteePersonnelle)
            {
                this.ModifierStyleCarte(listeCartes.Contains(_joueur.Carte1) ? ConstantesClient.STYLE_CARTE_ACTIVEE : ConstantesClient.STYLE_CARTE_DESACTIVEE, imgCarte1);
                this.ModifierStyleCarte(listeCartes.Contains(_joueur.Carte2) ? ConstantesClient.STYLE_CARTE_ACTIVEE : ConstantesClient.STYLE_CARTE_DESACTIVEE, imgCarte2);
            }
        }

        /// <summary>
        /// Modifie le libellé affiché en haut des cartes
        /// </summary>
        /// <param name="libelle">Le libellé à afficher</param>
        public void ChangerDerniereAction(string libelle)
        {
            lblDerniereAction.Content = libelle;            
        }

        /// <summary>
        /// Etat de la main à afficher
        /// </summary>
        public void ChangerCartes(Joueur expediteur, EtatMain etatDeLaMain)
        {
            _etatDeLaMain = etatDeLaMain;
            _joueur = expediteur.Clone() as Joueur;
            ChangerTourDeJeu(_joueur.TourDeJeu);
            if (_etatDeLaMain != EtatMain.PasDeCartes)
            {
                _joueur.Carte1 = expediteur.Carte1;
                _joueur.Carte2 = expediteur.Carte2;
            }
            else
            {
                // Il faut impérativement enlever le style mis sur la grille sinon les cartes ne veulent pas s'effacer !!!!!
                grdCartes.Style = null;
                lblDerniereAction.Content = string.Empty;
            }
            this.lblJoueur.Content = _joueur.Nom;
            MettreAJourImageCarteSelonEtat();            
            try
            {
                this.lblTapis.Content = string.Format(OutilsTraduction.Traducteur.Traduire("TapisJoueur"), _joueur.TapisJoueur - _joueur.Mise);
                this.lblMise.Content = string.Format(OutilsTraduction.Traducteur.Traduire("MiseJoueur"), _joueur.Mise);
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors de la traduction des informations du joueur : " + ex.Message);
                throw new Exception("Erreur de traduction : ChangerCartes", ex);
            }
            this.imgDonneur.Visibility = _joueur.EstDealer() ? Visibility.Visible : Visibility.Hidden;
            this.imgPetiteBlind.Visibility = _joueur.EstPetiteBlind() ? Visibility.Visible : Visibility.Hidden;
            this.imgGrosseBlind.Visibility = _joueur.EstGrosseBlind() ? Visibility.Visible : Visibility.Hidden;
            
        }

        /// <summary>
        /// Renvoie ou fixe le propriétaire de la main
        /// </summary>
        public void ChangerJoueur(Joueur j)
        {            
            // On oublie pas les cartes qu'il possèdait !!!
            if (_joueur != null)
            {
                CartePoker carte1, carte2;
                carte1 = _joueur.Carte1;
                carte2 = _joueur.Carte2;
                _joueur = j;
                _joueur.Carte1 = carte1;
                _joueur.Carte2 = carte2;
            }
            else
            {
                _joueur = j.Clone() as Joueur;
            }
            ChangerTourDeJeu(_joueur.TourDeJeu);
            this.lblJoueur.Content = _joueur.Nom;
            try
            {
                this.lblTapis.Content = string.Format(OutilsTraduction.Traducteur.Traduire("TapisJoueur"), _joueur.TapisJoueur - _joueur.Mise);
                this.lblMise.Content = string.Format(OutilsTraduction.Traducteur.Traduire("MiseJoueur"), _joueur.Mise);
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors de la traduction des informations du joueur : " + ex.Message);
                throw new Exception("Erreur de traduction : ChangerJoueur", ex);
            }
            this.imgDonneur.Visibility = _joueur.EstDealer() ? Visibility.Visible : Visibility.Hidden;
            this.imgPetiteBlind.Visibility = _joueur.EstPetiteBlind() ? Visibility.Visible : Visibility.Hidden;
            this.imgGrosseBlind.Visibility = _joueur.EstGrosseBlind() ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Changement du libellé de la combinaison
        /// </summary>
        /// <param name="libelleCombinaison"></param>
        public void ChangerLibelleCombinaison(string libelleCombinaison)
        {
            lblCombinaison.Content = libelleCombinaison;
        }


        #endregion

        #region Evenements
        public delegate void CarteRegardeeEventHandler(Joueur sender, bool debutRegard);
        public event CarteRegardeeEventHandler CarteRegardee;

        /// <summary>
        /// /// Mouse down sur une carte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void carte_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_etatDeLaMain == EtatMain.Personnelle || _etatDeLaMain == EtatMain.JeteePersonnelle)
            {
                _boutonPresse = true;

                // Dans tous les cas : on voit les cartes
                _carteMontree = true;
                MettreAJourImageCarteSelonEtat();

                // On remonte un clic si carte perso (spectateur n'a pas d'influence)
                if (_etatDeLaMain == EtatMain.Personnelle && _cacherLesCartes)
                    if (CarteRegardee != null) CarteRegardee(_joueur, true);
            }
        }

        /// <summary>
        /// Mouse Up sur une carte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void carte_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((_etatDeLaMain == EtatMain.Personnelle || _etatDeLaMain == EtatMain.JeteePersonnelle) && _boutonPresse)
            {
                _boutonPresse = false;

                // Dans tous les cas : on ne voit plus les cartes
                _carteMontree = false;
                MettreAJourImageCarteSelonEtat();

                // On remonte un clic si carte perso 
                if (_etatDeLaMain == EtatMain.Personnelle && _cacherLesCartes)
                    if (CarteRegardee != null) CarteRegardee(_joueur, false);

            }
        }

        /// <summary>
        /// Mouseleave avec bouton enfoncé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void carte_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!imgCarte1.IsMouseOver && !imgCarte2.IsMouseOver)
                carte_MouseUp(sender, null);
        }

        /// <summary>
        /// Mouse mouve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void carte_MouseMove(object sender, MouseEventArgs e)
        {
        }
        #endregion 
    }
}

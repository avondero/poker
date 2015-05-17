using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using Poker.Interface.Metier;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;

namespace Poker.Client.AffichageCartes
{
    /// <summary>
    /// Classe de gestion des cartes (affichage, nom des cartes & nom des combinaisons associées)
    /// </summary>
    static class GestionCartes
    {
        #region Membres privés
        private static BitmapImage _listeImagesCartes = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Images/cartes.jpg"));
        private static TypeJeuCarte _jeuChoisi = TypeJeuCarte.Sympathique;// 1 = normal, 2 gros, 3 = Rigolo, 4 = quatres saisons     
        private const int _HAUTEUR_CARTE = 98;
        private const int _LARGEUR_CARTE = 73;
        private const int _ARRONDI_CARTE = 6;
        private static readonly RectangleGeometry _FORME_CARTE = new RectangleGeometry(new Rect(0, 0, _LARGEUR_CARTE, _HAUTEUR_CARTE), _ARRONDI_CARTE, _ARRONDI_CARTE);
        #endregion

        #region methodes publiques
        /// <summary>
        /// Le type de jeu à afficher
        /// </summary>
        public static TypeJeuCarte JeuChoisi
        {
            get
            {
                return _jeuChoisi;
            }

            set
            {
                _jeuChoisi = value;
            }
        }

        /// <summary>
        /// Ajoute le libellé court de la carte dans "infos"
        /// </summary>
        /// <param name="c"></param>
        public static void AfficherLibelleCourtCarte(CartePoker c, TextPointer infos)
        {
            string ht = string.Empty;
            switch (c.Hauteur)
            {
                case HauteurCarte.As:
                    ht = "A";
                    break;

                case HauteurCarte.Deux:
                case HauteurCarte.Trois:
                case HauteurCarte.Quatre:
                case HauteurCarte.Cinq:
                case HauteurCarte.Six:
                case HauteurCarte.Sept:
                case HauteurCarte.Huit:
                case HauteurCarte.Neuf:
                case HauteurCarte.Dix:
                    ht = ((int)c.Hauteur).ToString();
                    break;

                case HauteurCarte.Valet:
                    ht = "J";
                    break;

                case HauteurCarte.Dame:
                    ht = "Q";
                    break;

                case HauteurCarte.Roi:
                    ht = "K";
                    break;
            }

            Run r = new Run(ht, infos);
            r.Foreground = Brushes.Black;

            char res = ' ';
            SolidColorBrush carteCouleur = Brushes.Black;
            switch (c.Couleur)
            {
                case CouleurCarte.Trefle:
                    res = (char)9827;
                    break;

                case CouleurCarte.Carreau:
                    res = (char)9830;
                    carteCouleur = Brushes.Red;
                    break;

                case CouleurCarte.Coeur:
                    res = (char)9829;
                    carteCouleur = Brushes.Red;
                    break;

                case CouleurCarte.Pique:
                    res = (char)9824;
                    break;
            }
            r = new Run(res.ToString(), infos);
            r.FontFamily = new FontFamily("Courier New");
            r.Foreground = carteCouleur;
            r.FontSize = 15;
        }

        /// <summary>
        /// Renvoie le format de la carte (un rectangle arrondi)
        /// </summary>
        public static RectangleGeometry FormeCarte
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, _LARGEUR_CARTE, _HAUTEUR_CARTE), _ARRONDI_CARTE, _ARRONDI_CARTE);// _FORME_CARTE;
            }
        }

        /// <summary>
        /// Arrondi de la carte
        /// </summary>
        public static int ARRONDI_CARTE
        {
            get { return _ARRONDI_CARTE; }

        }
        /// <summary>
        /// Largeur d'une carte
        /// </summary>
        public static int LARGEUR_CARTE
        {

            get
            {
                return _LARGEUR_CARTE;
            }
        }

        /// <summary>
        /// Hauteur d'une carte
        /// </summary>
        public static int HAUTEUR_CARTE
        {
            get
            {
                return _HAUTEUR_CARTE;
            }

        }
        /// <summary>
        /// Récupération de l'image cachée
        /// </summary>
        public static BitmapSource RecupererImageCarteCachee()
        {
            return RecupererDosCarte(0);
        }

        /// <summary>
        /// Récupération de l'image d'une carte jetée
        /// </summary>
        public static BitmapSource RecupererImageCarteJetee()
        {
            return RecupererDosCarte(1);
        }

        /// <summary>
        /// Récupération de l'image d'une carte jetée
        /// </summary>
        public static BitmapSource RecupererImageCarteRegardee()
        {
            return RecupererDosCarte(2);
        }

        /// <summary>
        /// Récupération d'un dos de carte 
        ///  0 = Rose / rouge
        ///  1 = Coeur
        ///  2 = Bleue
        /// </summary>
        public static BitmapSource RecupererDosCarte(int noFondCarte)
        {
            return RecupererImageSelonColonneLigne(noFondCarte * 2 + 3, 7);
        }
        
        /// <summary>
        /// Récupération d'une image de carte
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        public static BitmapSource RecupererImageSelonCarte(CartePoker carte)
        {
            BitmapSource res = null;
            if (carte != null)
            {
                int hauteur = (int)carte.Hauteur;
                int couleur = (int)carte.Couleur;
                int jeuChoisi = (int)_jeuChoisi;

                switch (carte.Hauteur)
                {
                    case HauteurCarte.As:

                        if (carte.Couleur == CouleurCarte.Pique)
                            res = RecupererImageSelonColonneLigne(1, 8);
                        else
                            res = RecupererImageSelonColonneLigne(7 + 2 * couleur, 6);
                        break;

                    case HauteurCarte.Deux:
                    case HauteurCarte.Trois:
                    case HauteurCarte.Quatre:
                    case HauteurCarte.Cinq:
                    case HauteurCarte.Six:
                    case HauteurCarte.Sept:
                    case HauteurCarte.Huit:
                    case HauteurCarte.Neuf:
                    case HauteurCarte.Dix:
                        if (JeuChoisi != TypeJeuCarte.GrossesCartes)
                        {
                            // On n'est pas en "gros jeu"
                            if (carte.Couleur == CouleurCarte.Pique)
                                res = RecupererImageSelonColonneLigne(5 + (hauteur - 1) % 4, 8 + (hauteur - 1) / 4);
                            else
                                res = RecupererImageSelonColonneLigne(10 - hauteur % 2 + 2 * (couleur - 1), (hauteur - 1) / 2 + 1);
                        }
                        else
                        { 
                            // On est en gros jeu
                            if (carte.Couleur == CouleurCarte.Pique)
                                res = RecupererImageSelonColonneLigne(5 + (hauteur - 1) % 4 - 4, 8 + (hauteur - 1) / 4);
                            else
                                res = RecupererImageSelonColonneLigne(10 - hauteur % 2 + 2 * (couleur - 1), (hauteur - 1) / 2 + 6);
                        }
                        break;

                    case HauteurCarte.Valet:
                    case HauteurCarte.Dame:
                    case HauteurCarte.Roi:
                        switch (carte.Couleur)
                        {
                            case CouleurCarte.Trefle:
                                res = RecupererImageSelonColonneLigne(2 * jeuChoisi - 1 + (hauteur - 11) % 2, (hauteur - 11) / 2 + 1);
                                break;

                            case CouleurCarte.Carreau:
                                res = RecupererImageSelonColonneLigne(2 * jeuChoisi - 1 + (hauteur - 10) % 2, (hauteur - 10) / 2 + 2);
                                break;

                            case CouleurCarte.Coeur:
                                res = RecupererImageSelonColonneLigne(2 * jeuChoisi - 1 + (hauteur - 10) % 2, (hauteur - 10) / 2 + 5);
                                break;

                            case CouleurCarte.Pique:
                                res = RecupererImageSelonColonneLigne(2 * jeuChoisi - 1 + (hauteur - 11) % 2, (hauteur - 11) / 2 + 4);
                                break;
                        }

                        break;

                }
            }
            else
                res = new BitmapImage();

            return res;
        }

        #endregion

        #region Methodes privées

        /// <summary>
        /// Récupération d'une image selon sa ligne (de 1 à 10) et sa colonne (de 1 à 14)
        ///  Fichier cartes.jpg
        /// </summary>
        /// <param name="ligne">Numéro de ligne (1 à 10)</param>
        /// <param name="colonne">Numéro de colonne (1 à 14)</param>
        /// <returns></returns>
        private static BitmapSource RecupererImageSelonColonneLigne(int colonne, int ligne)
        {

            if (colonne > 14 || colonne < 1 || ligne < 1 || ligne > 10)
                throw new Exception(string.Format("Erreur lors de la récupération de l'image d'une carte. ligne = {0}, colonne = {1}", ligne, colonne));

            return new CroppedBitmap(_listeImagesCartes, new Int32Rect((colonne - 1) * LARGEUR_CARTE, (ligne - 1) * HAUTEUR_CARTE, LARGEUR_CARTE, HAUTEUR_CARTE));
        }

                #endregion
    }
}

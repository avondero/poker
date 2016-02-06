using Microsoft.VisualStudio.DebuggerVisualizers;
using Poker.Interface.Metier;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Poker.Interface.Outils
{
    public class CartePokerVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            var data = objectProvider.GetObject() as CartePoker;
            var bmp = RecupererImageSelonCarte(data);

            using (Form displayForm = new Form() { ClientSize = bmp.Size, ShowInTaskbar = false, FormBorderStyle = FormBorderStyle.FixedDialog })
            {
                displayForm.Text = data.ToString();
                displayForm.Controls.Add(new PictureBox() { Image = bmp, Size = bmp.Size });
                windowService.ShowDialog(displayForm);
            }
        }




        #region Membres privés
        // EN dur car juste pour du débug
        private static Bitmap _listeImagesCartes = new Bitmap(
            Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Assembly.GetExecutingAssembly().Location)))),
                                @"Poker.Client\Images\Cartes.jpg"));
        private const int _HAUTEUR_CARTE = 98;
        private const int _LARGEUR_CARTE = 73;
        private const int _ARRONDI_CARTE = 6;
        #endregion

        #region methodes privées
        /// <summary>
        /// Récupération d'une image de carte
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        private Bitmap RecupererImageSelonCarte(CartePoker carte)
        {
            Bitmap res = null;
            if (carte != null)
            {
                int hauteur = (int)carte.Hauteur;
                int couleur = (int)carte.Couleur;
                int jeuChoisi = 3;

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
                        // On n'est pas en "gros jeu"
                        if (carte.Couleur == CouleurCarte.Pique)
                            res = RecupererImageSelonColonneLigne(5 + (hauteur - 1) % 4, 8 + (hauteur - 1) / 4);
                        else
                            res = RecupererImageSelonColonneLigne(10 - hauteur % 2 + 2 * (couleur - 1), (hauteur - 1) / 2 + 1);
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
                throw new NotImplementedException();

            return res;
        }

        /// <summary>
        /// Récupération d'une image selon sa ligne (de 1 à 10) et sa colonne (de 1 à 14)
        ///  Fichier cartes.jpg
        /// </summary>
        /// <param name="ligne">Numéro de ligne (1 à 10)</param>
        /// <param name="colonne">Numéro de colonne (1 à 14)</param>
        /// <returns></returns>
        private Bitmap RecupererImageSelonColonneLigne(int colonne, int ligne)
        {
            if (colonne > 14 || colonne < 1 || ligne < 1 || ligne > 10)
                throw new Exception(string.Format("Erreur lors de la récupération de l'image d'une carte. ligne = {0}, colonne = {1}", ligne, colonne));

            //var res = new Bitmap(_listeImagesCartes);
            return _listeImagesCartes.Clone(
                new Rectangle((colonne - 1) * _LARGEUR_CARTE, (ligne - 1) * _HAUTEUR_CARTE, _LARGEUR_CARTE, _HAUTEUR_CARTE),
               _listeImagesCartes.PixelFormat);

        }

        #endregion
    }
}

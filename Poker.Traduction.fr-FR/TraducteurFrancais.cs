using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;
using Poker.Interface.ExtensionsClient.Traduction;

namespace Poker.Traduction.Francais
{
    public class TraducteurFrancais : ITraducteur
    {
        #region ITraduction Membres

        /// <summary>
        /// Trauction d'un libellé
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public string Traduire(string cle)
        {
            string res = Poker.Traduction.Francais.Traductions.ResourceManager.GetString(cle);
            if (res == null || res.Length == 0)
                return "Cle = " + cle;
            else
            {
                res = res.Replace("{\\n}", "\n").Replace("{\\r}", "\r").Replace("{\\t}", "\t");
                return res;
            }
        }

        /// <summary>
        /// Traduction d'une combinaison
        /// </summary>
        /// <param name="comb"></param>
        /// <returns></returns>
        public string TraduireCombinaison(Poker.Interface.Metier.Combinaison comb)
        {
            string nom = string.Empty;
            switch (comb.TypeCombinaison)
            {
                case TypeCombinaison.Carte:
                    nom = NomCarte(comb.MainGagnante[0], true, false, false);
                    break;
                case TypeCombinaison.Paire:
                    nom = "une paire " + NomCarte(comb.MainGagnante[0], false, true, true);
                    break;
                case TypeCombinaison.DoublePaire:
                    nom = "une double paire "
                        + NomCarte(comb.MainGagnante[0], false, false, false) + "-"
                        + NomCarte(comb.MainGagnante[2], false, false, false);
                    break;
                case TypeCombinaison.Brelan:
                    nom = "un brelan " + NomCarte(comb.MainGagnante[0], false, true, true);
                    break;
                case TypeCombinaison.Quinte:
                    nom = "une quinte " + NomCarte(comb.MainGagnante[0], true, true, false);
                    break;
                case TypeCombinaison.Couleur:
                    nom = "une couleur à " + NomCouleur(comb.MainGagnante[0]);
                    break;
                case TypeCombinaison.Full:
                    nom = "un full "
                        + NomCarte(comb.MainGagnante[0], false, false, false) + "-"
                        + NomCarte(comb.MainGagnante[3], false, false, false);
                    break;
                case TypeCombinaison.Carre:
                    nom = "un carré " + NomCarte(comb.MainGagnante[0], false, true, true);
                    break;
                case TypeCombinaison.QuinteFlush:
                    nom = "une quinte Flush " + NomCarte(comb.MainGagnante[0], true, true, false) + " de " + NomCouleur(comb.MainGagnante[0]);
                    break;
            }
            return nom;
        }

        /// <summary>
        /// Traduit le type de combinaison (suite, paire, double-paires ....)
        /// </summary>
        /// <param name="typeComb"></param>
        /// <returns></returns>
        public string TraduireTypeCombinaison(TypeCombinaison typeComb)
        {
            string nom = string.Empty;
            switch (typeComb)
            {
                case TypeCombinaison.Carte:
                    nom = "Carte";
                    break;
                case TypeCombinaison.Paire:
                    nom = "Paire";
                    break;
                case TypeCombinaison.DoublePaire:
                    nom = "Double paire";
                    break;
                case TypeCombinaison.Brelan:
                    nom = "Brelan";
                    break;
                case TypeCombinaison.Quinte:
                    nom = "Quinte";
                    break;
                case TypeCombinaison.Couleur:
                    nom = "Couleur";
                    break;
                case TypeCombinaison.Full:
                    nom = "Full";
                    break;
                case TypeCombinaison.Carre:
                    nom = "Carré";
                    break;
                case TypeCombinaison.QuinteFlush:
                    nom = "Quinte Flush";
                    break;
            }
            return nom;
        }

        /// <summary>
        /// Traduction de la carte
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        public string TraduireCarte(Poker.Interface.Metier.CartePoker carte)
        {
            return NomCarte(carte, false, false, false) + " de " + NomCouleur(carte);
        }

        /// <summary>
        /// Langue de traduction
        /// </summary>
        public string LangueTraduction
        {
            get
            {
                return "Français";
            }
        }

        /// <summary>
        /// LEcture vocale d'une carte
        /// </summary>
        /// <param name="carte"></param>
        public void LireCarte(Poker.Interface.Metier.CartePoker carte)
        { 
            
        }

        /// <summary>
        /// Lecture d'un message
        /// </summary>
        /// <param name="message"></param>
        public void LireMessage(string message)
        { 
        }
        #endregion

        #region Methodes privées
        /// <summary>
        /// Nom d'une carte : utilisé pour les combinaisons
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="article"></param>
        /// <param name="possessif"></param>
        /// <param name="pluriel"></param>
        /// <returns></returns>
        private string NomCarte(CartePoker carte, bool article, bool possessif, bool pluriel)
        {
            string nom = string.Empty;
            string art = string.Empty;
            string pos = string.Empty;
            string posart = string.Empty;
            string term = string.Empty;
            if (pluriel)
            {
                term = "s";
            }

            switch (carte.Hauteur)
            {
                case HauteurCarte.As:
                    art = "un ";
                    pos = "d'";
                    posart = "à l'";
                    nom = "As";
                    break;
                case HauteurCarte.Roi:
                    art = "un ";
                    pos = "de ";
                    posart = "au ";
                    nom = "Roi" + term;
                    break;
                case HauteurCarte.Dame:
                    art = "une ";
                    pos = "de ";
                    posart = "à la ";
                    nom = "Dame" + term;
                    break;
                case HauteurCarte.Valet:
                    art = "un ";
                    pos = "de ";
                    posart = "au ";
                    nom = "Valet" + term;
                    break;
                case HauteurCarte.Dix:
                case HauteurCarte.Neuf:
                case HauteurCarte.Huit:
                case HauteurCarte.Sept:
                case HauteurCarte.Six:
                case HauteurCarte.Cinq:
                case HauteurCarte.Quatre:
                case HauteurCarte.Trois:
                case HauteurCarte.Deux:
                    art = "un ";
                    pos = "de ";
                    posart = "au ";
                    nom = ((int)carte.Hauteur).ToString();
                    break;
            }

            if (article && possessif)
                nom = posart + nom;
            else if (article)
                nom = art + nom;
            else if (possessif)
                nom = pos + nom;

            return nom;
        }

        /// <summary>
        /// Nom de la couleur
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        private string NomCouleur(CartePoker carte)
        {
            string nom = string.Empty;
            switch (carte.Couleur)
            {
                case CouleurCarte.Coeur:
                    nom = "coeur";
                    break;
                case CouleurCarte.Pique:
                    nom = "pique";
                    break;
                case CouleurCarte.Trefle:
                    nom = "trèfle";
                    break;
                case CouleurCarte.Carreau:
                    nom = "carreau";
                    break;
            }
            return nom;
        }

        #endregion
    }
}

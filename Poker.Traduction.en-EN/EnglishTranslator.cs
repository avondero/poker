using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.ExtensionsClient.Traduction;
using System.Resources;
using Poker.Interface.Metier;

namespace Poker.Traduction.English
{
    public class EnglishTranslator : ITraducteur
    {

        #region ITraducteur Membres

        public string Traduire(string cle)
        {
            string res = Translations.ResourceManager.GetString(cle);
            if (res == null || res.Length == 0)
                return "Key = " + cle;
            else
            {
                res = res.Replace("{\\n}", "\n").Replace("{\\r}", "\r").Replace("{\\t}", "\t");
                return res;
            }
        }

        public string TraduireCombinaison(Poker.Interface.Metier.Combinaison comb)
        {
            string name = string.Empty;
            switch (comb.TypeCombinaison)
            {
                case TypeCombinaison.Carte:
                    name = CardName(comb.MainGagnante[0], true, false);
                    break;
                case TypeCombinaison.Paire:
                    name = "a pair of " + CardName(comb.MainGagnante[0], false, true);
                    break;
                case TypeCombinaison.DoublePaire:
                    name = "a "
                        + CardName(comb.MainGagnante[0], false, true) + "-"
                        + CardName(comb.MainGagnante[2], false, true);
                    break;
                case TypeCombinaison.Brelan:
                    name = "a three of a kind : " + CardName(comb.MainGagnante[0], false, true);
                    break;
                case TypeCombinaison.Quinte:
                    name = "a straight to " + CardName(comb.MainGagnante[0], false, false);
                    break;
                case TypeCombinaison.Couleur:
                    name = "a flush to " + ColorName(comb.MainGagnante[0]);
                    break;
                case TypeCombinaison.Full:
                    name = "a full "
                        + CardName(comb.MainGagnante[0], false, true) + "-"
                        + CardName(comb.MainGagnante[3], false, true);
                    break;
                case TypeCombinaison.Carre:
                    name = "a four of a kind : " + CardName(comb.MainGagnante[0], false, true);
                    break;
                case TypeCombinaison.QuinteFlush:
                    name = "a straight Flush to " + CardName(comb.MainGagnante[0], false, false) + " of " + ColorName(comb.MainGagnante[0]);
                    break;
            }
            return name;
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
                    nom = "Card";
                    break;
                case TypeCombinaison.Paire:
                    nom = "Pair";
                    break;
                case TypeCombinaison.DoublePaire:
                    nom = "Double pair";
                    break;
                case TypeCombinaison.Brelan:
                    nom = "Three of a kind";
                    break;
                case TypeCombinaison.Quinte:
                    nom = "Straight";
                    break;
                case TypeCombinaison.Couleur:
                    nom = "Flush";
                    break;
                case TypeCombinaison.Full:
                    nom = "Full";
                    break;
                case TypeCombinaison.Carre:
                    nom = "Four of a kind";
                    break;
                case TypeCombinaison.QuinteFlush:
                    nom = "Straight sFlush";
                    break;
            }
            return nom;
        }

        public string LangueTraduction
        {
            get
            {
                return "English";
            }
        }

        public string TraduireCarte(Poker.Interface.Metier.CartePoker carte)
        {
            return CardName(carte, true, false) + " of " + ColorName(carte);
        }

        #endregion

        #region Methodes  privées
        /// <summary>
        /// Nom d'une carte : utilisé pour les combinaisons
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="article"></param>
        /// <param name="possessif"></param>
        /// <param name="pluriel"></param>
        /// <returns></returns>
        private string CardName(CartePoker card, bool article, bool plural)
        {
            string name = string.Empty;
            string art = "a ";
            string term = string.Empty;
            if (plural)
            {
                term = "s";
            }

            switch (card.Hauteur)
            {
                case HauteurCarte.As:
                    art = "an ";
                    name = "Ace" + term;
                    break;
                case HauteurCarte.Roi:
                    name = "King" + term;
                    break;
                case HauteurCarte.Dame:
                    name = "Queen" + term;
                    break;
                case HauteurCarte.Valet:
                    name = "Jack" + term;
                    break;
                case HauteurCarte.Dix:
                    name = "Ten" + term;
                    break;
                case HauteurCarte.Neuf:
                    name = "Nine" + term;
                    break;
                case HauteurCarte.Huit:
                    name = "Eight" + term;
                    break;
                case HauteurCarte.Sept:
                    name = "Seven" + term;
                    break;
                case HauteurCarte.Six:
                    name = "Six" + term;
                    break;
                case HauteurCarte.Cinq:
                    name = "Five" + term;
                    break;
                case HauteurCarte.Quatre:
                    name = "Four" + term;
                    break;
                case HauteurCarte.Trois:
                    name = "Three" + term;
                    break;
                case HauteurCarte.Deux:
                    name = "Two" + term;
                    break;
            }

            if (article)
                name = art + name;
            
            return name;
        }

        /// <summary>
        /// Nom de la couleur
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        private string ColorName(CartePoker card)
        {
            string nom = string.Empty;
            switch (card.Couleur)
            {
                case CouleurCarte.Coeur:
                    nom = "heart";
                    break;
                case CouleurCarte.Pique:
                    nom = "spade";
                    break;
                case CouleurCarte.Trefle:
                    nom = "club";
                    break;
                case CouleurCarte.Carreau:
                    nom = "diamond";
                    break;
            }
            return nom;
        }

        #endregion
    }
}

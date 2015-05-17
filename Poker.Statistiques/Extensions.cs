using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;
using Poker.Interface.Stats;

namespace Poker.Statistiques
{
    static class Extensions
    {
        /// <summary>
        /// Renvoie la liste des noms des joueurs séparés par une ","
        /// </summary>
        /// <param name="listeJoueurs"></param>
        /// <returns></returns>
        public static string ListeNoms(this List<JoueurStat> listeJoueurs)
        {
            string res = string.Empty;
            if (listeJoueurs != null && listeJoueurs.Count > 0)
            {
                foreach (JoueurStat j in listeJoueurs)
                {
                    res += "," + j.Nom;
                }

                res = res.Substring(1);
            }
            return res;

        }

        /// <summary>
        /// Renvoie le libellé de la carte (idem traduction francaise)
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        public static string LibelleCarte(this CartePoker carte)
        {
            if (carte == null)
            {
                return "-";
            }
            else
            {
                return NomHauteur(carte) + " de " + NomCouleur(carte);
            }
        }

        /// <summary>
        /// Nom de la couleur
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        private static string NomCouleur(CartePoker carte)
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

        private static string NomHauteur(CartePoker carte)
        {
            string nom = string.Empty;
            switch (carte.Hauteur)
            {
                case HauteurCarte.As:
                    nom = "As";
                    break;
                case HauteurCarte.Roi:
                    nom = "Roi";
                    break;
                case HauteurCarte.Dame:
                    nom = "Dame";
                    break;
                case HauteurCarte.Valet:
                    nom = "Valet";
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
                    nom = ((int)carte.Hauteur).ToString();
                    break;
            }

            return nom;
        }

    }
}

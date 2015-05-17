using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;
using Poker.Serveur.Technique;

namespace Poker.Serveur.Metier
{
    /// <summary>
    /// Un jeu de poker : 52 cartes
    /// </summary>
    static class JeuPoker 
    {
        #region Membres privés
        private static Dictionary<CartePoker, int> _listeCartes = new Dictionary<CartePoker, int>();
        private static Array _hauteursCartes = Enum.GetValues(typeof(HauteurCarte));
        private static Array _couleursCartes = Enum.GetValues(typeof(CouleurCarte));
#if DEBUG 
        private const string NOM_FICHIER_TRICHE = "PokerCartes.Txt";
#endif
        #endregion

        #region methodes publiques
        internal static void ReinitialiserJeu()
        {
            _listeCartes = new Dictionary<CartePoker, int>();
        }

        /// <summary>
        /// Renvoie une carte de poker
        /// </summary>
        /// <returns>La carte tirée. Si aucune carte n'est disponible, renvoie une exception</returns>
        internal static CartePoker TirerUneCarteAuHasard()
        {
            bool tirageOk = false;
            CartePoker carte = null;

#if DEBUG
            carte = TricherPourTirage();
            if (carte != null) tirageOk = true;
#endif

            if (!tirageOk)
            {
                if (_listeCartes.Count == 52)
                    throw new ApplicationException("Plus de cartes disponibles");

                Random rd = new Random();
                HauteurCarte hauteur;
                CouleurCarte couleur;

                do
                {
                    hauteur = (HauteurCarte)_hauteursCartes.GetValue(rd.Next(_hauteursCartes.GetLength(0)));
                    couleur = (CouleurCarte)_couleursCartes.GetValue(rd.Next(_couleursCartes.GetLength(0)));
                    carte = new CartePoker(hauteur, couleur);
                }
                while (_listeCartes.ContainsKey(carte) && _listeCartes[carte] == 1);

                _listeCartes[carte] = 1;
            }

            return carte;
        }
        #endregion

        #region Methodes privées
        #if DEBUG
        /// <summary>
        /// Porte dérobée pour tirer les cartes souhaitées : ne marche qu'en debug 
        /// Les cartes sont dans le fichier %temp%\Pokercartes.txt et chaque ligne représente un tirage
        /// </summary>
        /// <returns>La carte choisie null si problème</returns>
        private static CartePoker TricherPourTirage()
        {
            CartePoker res = null;
            
            try
            {
                string pathCheat = System.IO.Path.Combine(Environment.GetEnvironmentVariable("TEMP"), NOM_FICHIER_TRICHE);
                if (System.IO.File.Exists(pathCheat) && new System.IO.FileInfo(pathCheat).Length > 0)
                {
                    string[] lines = System.IO.File.ReadAllLines(pathCheat);
                    bool ligneLue = false;
                    System.IO.StreamWriter wrt = new System.IO.StreamWriter(pathCheat);
                    foreach (string line in lines)
                    {
                        if (ligneLue)
                        {
                            wrt.WriteLine(line);
                        }
                        if (!ligneLue && line.Length != 0)
                        {
                            string[] hauteurcouleur = line.Split(' ');
                            if (hauteurcouleur.Length >= 2)
                            {
                                ligneLue = true;
                                try
                                {
                                    res = new CartePoker((HauteurCarte)Enum.Parse(typeof(HauteurCarte), hauteurcouleur[0]), (CouleurCarte)Enum.Parse(typeof(CouleurCarte), hauteurcouleur[1]));
                                }
                                catch (Exception ex)
                                {
                                    logServeur.Debug("    Warning : Problème 1 lors de la lecture des cartes à choisir : " + ex.Message);
                                }
                                
                            }
                        }
                    }
                    wrt.Close();
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("    Warning : Problème 2 lors de la lecture des cartes à choisir : " + ex.Message);
            }
            
            return res;
        }
#endif
        #endregion

        #region Classe interne
        internal class Cartes
        {
            internal static readonly CartePoker AsTrefle = new CartePoker(HauteurCarte.As, CouleurCarte.Trefle);
            internal static readonly CartePoker DeuxTrefle = new CartePoker(HauteurCarte.Deux, CouleurCarte.Trefle);
            internal static readonly CartePoker TroisTrefle = new CartePoker(HauteurCarte.Trois, CouleurCarte.Trefle);
            internal static readonly CartePoker QuatreTrefle = new CartePoker(HauteurCarte.Quatre, CouleurCarte.Trefle);
            internal static readonly CartePoker CinqTrefle = new CartePoker(HauteurCarte.Cinq, CouleurCarte.Trefle);
            internal static readonly CartePoker SixTrefle = new CartePoker(HauteurCarte.Six, CouleurCarte.Trefle);
            internal static readonly CartePoker SeptTrefle = new CartePoker(HauteurCarte.Sept, CouleurCarte.Trefle);
            internal static readonly CartePoker HuitTrefle = new CartePoker(HauteurCarte.Huit, CouleurCarte.Trefle);
            internal static readonly CartePoker NeufTrefle = new CartePoker(HauteurCarte.Neuf, CouleurCarte.Trefle);
            internal static readonly CartePoker DixTrefle = new CartePoker(HauteurCarte.Dix, CouleurCarte.Trefle);
            internal static readonly CartePoker ValetTrefle = new CartePoker(HauteurCarte.Valet, CouleurCarte.Trefle);
            internal static readonly CartePoker DameTrefle = new CartePoker(HauteurCarte.Dame, CouleurCarte.Trefle);
            internal static readonly CartePoker RoiTrefle = new CartePoker(HauteurCarte.Roi, CouleurCarte.Trefle);
            internal static readonly CartePoker AsCarreau = new CartePoker(HauteurCarte.As, CouleurCarte.Carreau);
            internal static readonly CartePoker DeuxCarreau = new CartePoker(HauteurCarte.Deux, CouleurCarte.Carreau);
            internal static readonly CartePoker TroisCarreau = new CartePoker(HauteurCarte.Trois, CouleurCarte.Carreau);
            internal static readonly CartePoker QuatreCarreau = new CartePoker(HauteurCarte.Quatre, CouleurCarte.Carreau);
            internal static readonly CartePoker CinqCarreau = new CartePoker(HauteurCarte.Cinq, CouleurCarte.Carreau);
            internal static readonly CartePoker SixCarreau = new CartePoker(HauteurCarte.Six, CouleurCarte.Carreau);
            internal static readonly CartePoker SeptCarreau = new CartePoker(HauteurCarte.Sept, CouleurCarte.Carreau);
            internal static readonly CartePoker HuitCarreau = new CartePoker(HauteurCarte.Huit, CouleurCarte.Carreau);
            internal static readonly CartePoker NeufCarreau = new CartePoker(HauteurCarte.Neuf, CouleurCarte.Carreau);
            internal static readonly CartePoker DixCarreau = new CartePoker(HauteurCarte.Dix, CouleurCarte.Carreau);
            internal static readonly CartePoker ValetCarreau = new CartePoker(HauteurCarte.Valet, CouleurCarte.Carreau);
            internal static readonly CartePoker DameCarreau = new CartePoker(HauteurCarte.Dame, CouleurCarte.Carreau);
            internal static readonly CartePoker RoiCarreau = new CartePoker(HauteurCarte.Roi, CouleurCarte.Carreau);
            internal static readonly CartePoker AsCoeur = new CartePoker(HauteurCarte.As, CouleurCarte.Coeur);
            internal static readonly CartePoker DeuxCoeur = new CartePoker(HauteurCarte.Deux, CouleurCarte.Coeur);
            internal static readonly CartePoker TroisCoeur = new CartePoker(HauteurCarte.Trois, CouleurCarte.Coeur);
            internal static readonly CartePoker QuatreCoeur = new CartePoker(HauteurCarte.Quatre, CouleurCarte.Coeur);
            internal static readonly CartePoker CinqCoeur = new CartePoker(HauteurCarte.Cinq, CouleurCarte.Coeur);
            internal static readonly CartePoker SixCoeur = new CartePoker(HauteurCarte.Six, CouleurCarte.Coeur);
            internal static readonly CartePoker SeptCoeur = new CartePoker(HauteurCarte.Sept, CouleurCarte.Coeur);
            internal static readonly CartePoker HuitCoeur = new CartePoker(HauteurCarte.Huit, CouleurCarte.Coeur);
            internal static readonly CartePoker NeufCoeur = new CartePoker(HauteurCarte.Neuf, CouleurCarte.Coeur);
            internal static readonly CartePoker DixCoeur = new CartePoker(HauteurCarte.Dix, CouleurCarte.Coeur);
            internal static readonly CartePoker ValetCoeur = new CartePoker(HauteurCarte.Valet, CouleurCarte.Coeur);
            internal static readonly CartePoker DameCoeur = new CartePoker(HauteurCarte.Dame, CouleurCarte.Coeur);
            internal static readonly CartePoker RoiCoeur = new CartePoker(HauteurCarte.Roi, CouleurCarte.Coeur);
            internal static readonly CartePoker AsPique = new CartePoker(HauteurCarte.As, CouleurCarte.Pique);
            internal static readonly CartePoker DeuxPique = new CartePoker(HauteurCarte.Deux, CouleurCarte.Pique);
            internal static readonly CartePoker TroisPique = new CartePoker(HauteurCarte.Trois, CouleurCarte.Pique);
            internal static readonly CartePoker QuatrePique = new CartePoker(HauteurCarte.Quatre, CouleurCarte.Pique);
            internal static readonly CartePoker CinqPique = new CartePoker(HauteurCarte.Cinq, CouleurCarte.Pique);
            internal static readonly CartePoker SixPique = new CartePoker(HauteurCarte.Six, CouleurCarte.Pique);
            internal static readonly CartePoker SeptPique = new CartePoker(HauteurCarte.Sept, CouleurCarte.Pique);
            internal static readonly CartePoker HuitPique = new CartePoker(HauteurCarte.Huit, CouleurCarte.Pique);
            internal static readonly CartePoker NeufPique = new CartePoker(HauteurCarte.Neuf, CouleurCarte.Pique);
            internal static readonly CartePoker DixPique = new CartePoker(HauteurCarte.Dix, CouleurCarte.Pique);
            internal static readonly CartePoker ValetPique = new CartePoker(HauteurCarte.Valet, CouleurCarte.Pique);
            internal static readonly CartePoker DamePique = new CartePoker(HauteurCarte.Dame, CouleurCarte.Pique);
            internal static readonly CartePoker RoiPique = new CartePoker(HauteurCarte.Roi, CouleurCarte.Pique);
        }
        #endregion
    }
}

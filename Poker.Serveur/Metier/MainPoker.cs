using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Metier;

namespace Poker.Serveur.Metier
{
    /// <summary>
    /// Classe de gestion d'une main de 2 + 5 cartes
    /// </summary>
    class MainPoker : IComparable<MainPoker>
    {
        #region Variables privées
        /// <summary>
        /// L'ensemble des cartes de la main : 5 visibles + 2 du joueur
        /// </summary>
        private List<CartePoker> _cartes;
        #endregion

        #region Constructeur
        private MainPoker()
        { }

        /// <summary>
        /// Constructeur à partir du board et de 2 cartes
        /// </summary>
        /// <param name="proprietaire">Le proprietaire de cette main</param>
        /// <param name="board">Le board</param>
        internal MainPoker(Joueur prop, CartePoker[] board)
        {
            if (board == null || board.Length != 5)
                throw new ArgumentException("Le board doit contenir 5 cartes uniquement");

            _cartes = new List<CartePoker>(board);
            _cartes.Add(prop.Carte1);
            _cartes.Add(prop.Carte2);
            this.Proprietaire = prop;            
        }
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Le proprietaire de cette main
        /// </summary>
        internal Joueur Proprietaire { get; private set; }

        /// <summary>
        /// Resultat de la main
        /// </summary>
        internal Combinaison ResultatMain {get;private set;}
        #endregion

        #region Methodes publiques
        /// <summary>
        /// Détermine la combinaison de la main
        /// </summary>
        /// <returns></returns>
        internal Combinaison DeterminerCombinaison()
        {
            _cartes.Sort(delegate(CartePoker c1, CartePoker c2) { return c2.CompareTo(c1); });
            this.ResultatMain = DeterminerCombinaisonInterne();
            return this.ResultatMain;
        }
        #endregion

        #region Méthodes privées
        /// <summary>
        /// Détermine la combinaison de la main
        /// Prérequis : les cartes sont triées par ordre croissant
        /// </summary>
        /// <returns></returns>
        private Combinaison DeterminerCombinaisonInterne()
        {
            Combinaison resComb = null;
            Dictionary<HauteurCarte, List<CartePoker>> hauteurs = new Dictionary<HauteurCarte, List<CartePoker>>();
            Dictionary<CouleurCarte, List<CartePoker>> couleurs = new Dictionary<CouleurCarte, List<CartePoker>>();
            int nbMaxHauteurs = 0, nbMaxCouleurs = 0;

            // On classe les cartes selon leur couleur et leur hauteur
            foreach (CartePoker carte in this._cartes)
            {
                if (!couleurs.ContainsKey(carte.Couleur))
                {
                    couleurs[carte.Couleur] = new List<CartePoker>();
                }
                couleurs[carte.Couleur].Add(carte);
                nbMaxCouleurs = Math.Max(nbMaxCouleurs, couleurs[carte.Couleur].Count);

                if (!hauteurs.ContainsKey(carte.Hauteur))
                {
                    hauteurs[carte.Hauteur] = new List<CartePoker>();
                }
                hauteurs[carte.Hauteur].Add(carte);
                nbMaxHauteurs = Math.Max(nbMaxHauteurs, hauteurs[carte.Hauteur].Count);
            }

            // Quinte Flush
            resComb = ChercherQuinteFlush(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Carré
            resComb = ChercherCarre(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Full
            resComb = ChercherFull(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Couleur
            resComb = ChercherCouleur(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Suite
            resComb = ChercherQuinte(_cartes);
            if (resComb != null) return resComb;

            // Brelan
            resComb = ChercherBrelan(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Double paire
            resComb = ChercherDoublePaire(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Paire
            resComb = ChercherPaire(hauteurs, couleurs, nbMaxHauteurs, nbMaxCouleurs);
            if (resComb != null) return resComb;

            // Carte : Pas de combinaisons, on renvoie les 5 premieres cartes qui sont les plus fortes
            List<CartePoker> cartes15 = new List<CartePoker>();
            for (int i = 0; i < 5; i++)
                cartes15.Add(_cartes[i]);
            return new Combinaison(TypeCombinaison.Carte, cartes15);
        }

        /// <summary>
        /// Recherche d'une paire : on prend la premiere qui vient : la double paire a déjà été detectée
        /// </summary>
        /// <param name="hauteurs"></param>
        /// <param name="couleurs"></param>
        /// <param name="nbMaxHauteurs"></param>
        /// <param name="nbMaxCouleurs"></param>
        /// <returns></returns>
        private Combinaison ChercherPaire(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxHauteurs < 2) return null;

            List<CartePoker> mainGagnante = new List<CartePoker>();

            foreach (List<CartePoker> hauteurGagnante in hauteurs.Values)
            {
                if (hauteurGagnante.Count == 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        mainGagnante.Add(hauteurGagnante[i]);
                    }
                    break;
                }
            }

            mainGagnante = CompleterListeCartes(mainGagnante, 3, mainGagnante[0].Hauteur);

            return new Combinaison(TypeCombinaison.Paire, mainGagnante);
        }

        private Combinaison ChercherDoublePaire(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxHauteurs < 2) return null;

            List<CartePoker> tmpMainGagnante = new List<CartePoker>();

            foreach (List<CartePoker> hauteurGagnante in hauteurs.Values)
            {
                if (hauteurGagnante.Count == 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        tmpMainGagnante.Add(hauteurGagnante[i]);
                    }
                }
            }

            // On n'a pas du tout de double paire
            if (tmpMainGagnante.Count < 4) return null;

            int indice1 = 0, indice2 = 0;
            if (tmpMainGagnante[0].CompareTo(tmpMainGagnante[2]) > 0)
            {
                if (tmpMainGagnante.Count == 4 || tmpMainGagnante[2].CompareTo(tmpMainGagnante[4]) > 0)
                {
                    // Ordre : 0, 2 ,4
                    indice1 = 0;
                    indice2 = 2;
                }
                else
                {
                    if (tmpMainGagnante[4].CompareTo(tmpMainGagnante[0]) > 0)
                    {
                        // Ordre : 4, 0, 2
                        indice1 = 4;
                        indice2 = 0;
                    }
                    else
                    {
                        // Ordre : 0, 4, 2
                        indice1 = 0;
                        indice2 = 4;
                    }
                }
            }
            else
            {
                if (tmpMainGagnante.Count == 4 || tmpMainGagnante[0].CompareTo(tmpMainGagnante[4]) > 0)
                {
                    // Ordre : 2 ,0, 4
                    indice1 = 2;
                    indice2 = 0;
                }
                else
                {
                    if (tmpMainGagnante[4].CompareTo(tmpMainGagnante[2]) > 0)
                    {
                        // Ordre : 4, 2, 0
                        indice1 = 4;
                        indice2 = 2;
                    }
                    else
                    {
                        // Ordre : 2, 4, 0
                        indice1 = 2;
                        indice2 = 4;
                    }
                }
            }

            List<CartePoker> mainGagnante = new List<CartePoker>();
            mainGagnante.Add(tmpMainGagnante[indice1]);
            mainGagnante.Add(tmpMainGagnante[indice1 + 1]);
            mainGagnante.Add(tmpMainGagnante[indice2]);
            mainGagnante.Add(tmpMainGagnante[indice2 + 1]);

            mainGagnante = CompleterListeCartes(mainGagnante, 1, mainGagnante[0].Hauteur, mainGagnante[2].Hauteur);

            return new Combinaison(TypeCombinaison.DoublePaire, mainGagnante);
        }

        private Combinaison ChercherCouleur(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxCouleurs < 5) return null;
            List<CartePoker> mainGagnante = new List<CartePoker>();

            foreach (List<CartePoker> couleurGagnante in couleurs.Values)
            {
                if (couleurGagnante.Count >= 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        mainGagnante.Add(couleurGagnante[i]);
                    }
                    break;
                }
            }

            return new Combinaison(TypeCombinaison.Couleur, mainGagnante);
        }

        private Combinaison ChercherBrelan(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxHauteurs < 3) return null;

            List<CartePoker> mainGagnante = new List<CartePoker>();

            foreach (List<CartePoker> hauteurGagnante in hauteurs.Values)
            {
                if (hauteurGagnante.Count == 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        mainGagnante.Add(hauteurGagnante[i]);
                    }
                    break;
                }
            }

            mainGagnante = CompleterListeCartes(mainGagnante, 2, mainGagnante[0].Hauteur);

            return new Combinaison(TypeCombinaison.Brelan, mainGagnante);
        }

        /// <summary>
        /// Recherche d'une quinte parmi des cartes "triées"
        /// </summary>
        /// <param name="cartes"></param>
        /// <returns></returns>
        private Combinaison ChercherQuinte(List<CartePoker> cartes)
        {
            CartePoker cartePrec = null;
            int taille = 0;
            List<CartePoker> mainGagnante = new List<CartePoker>();

            foreach (CartePoker carte in cartes)
            {
                if (taille == 0)
                {
                    taille = 1;
                    mainGagnante.Add(carte);
                    cartePrec = carte;
                }
                else
                {
                    if (carte.Hauteur != cartePrec.Hauteur)
                    {
                        if (StrictementInferieur(carte, cartePrec))
                        {
                            taille++;
                            mainGagnante.Add(carte);
                            cartePrec = carte;
                            if (taille == 5)
                            {
                                break;
                            }
                        }
                        else
                        {
                            mainGagnante.Clear();
                            mainGagnante.Add(carte);
                            cartePrec = carte;
                            taille = 1;
                        }
                    }
                }
            }
            // Cas particulier de la quinte ou l'As vaut 1
            if (taille == 4 && cartePrec.Hauteur == HauteurCarte.Deux)
            {
                if (cartes[0].Hauteur == HauteurCarte.As)
                {
                    // On a une quinte qui finit en 1 !
                    mainGagnante.Add(cartes[0]);
                    taille++;
                }
            }

            if (taille == 5)
                return new Combinaison(TypeCombinaison.Quinte, mainGagnante);
            else
                return null;
        }

        private Combinaison ChercherFull(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxHauteurs < 3) return null;

            List<CartePoker> tmpMainGagnante = new List<CartePoker>();
            List<CartePoker> paires = new List<CartePoker>();
            int nbBrelans = 0;
            foreach (List<CartePoker> hauteurGagnante in hauteurs.Values)
            {
                switch (hauteurGagnante.Count)
                {
                    case 3:
                        for (int i = 0; i < 3; i++)
                        {
                            tmpMainGagnante.Add(hauteurGagnante[i]);
                        }
                        nbBrelans++;
                        break;

                    case 2:
                        if (paires.Count == 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                paires.Add(hauteurGagnante[i]);
                            }
                        }
                        else
                        {
                            // C'est pas la premiere paire mais est elle plus forte. oui : on ne fait rien
                            if (paires[0].CompareTo(hauteurGagnante[0]) < 0)
                            {
                                // Non : on inverse 
                                paires = new List<CartePoker>();
                                for (int i = 0; i < 2; i++)
                                {
                                    paires.Add(hauteurGagnante[i]);
                                }
                            }
                        }
                        break;

                }
            }

            Combinaison resComb = null;
            List<CartePoker> mainGagnante = null;
            // Si on a 2 brelans, on prend le plus grand brelan & les 2ères cartes du 2ème brelan
            if (nbBrelans == 2)
            {
                if (tmpMainGagnante[0].CompareTo(tmpMainGagnante[3]) > 0)
                {
                    mainGagnante = new List<CartePoker>(tmpMainGagnante);
                    mainGagnante.RemoveAt(5);
                }
                else
                {
                    mainGagnante = new List<CartePoker>();
                    mainGagnante.Add(tmpMainGagnante[3]);
                    mainGagnante.Add(tmpMainGagnante[4]);
                    mainGagnante.Add(tmpMainGagnante[5]);
                    mainGagnante.Add(tmpMainGagnante[0]);
                    mainGagnante.Add(tmpMainGagnante[1]);
                }

                resComb = new Combinaison(TypeCombinaison.Full, mainGagnante);
            }
            else if (paires.Count > 0)
            {
                mainGagnante = new List<CartePoker>(tmpMainGagnante);
                mainGagnante.Add(paires[0]);
                mainGagnante.Add(paires[1]);
                resComb = new Combinaison(TypeCombinaison.Full, mainGagnante);
            }
            else
            {
                resComb = null;
            }

            return resComb;
        }

        private Combinaison ChercherCarre(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxHauteurs != 4) return null;

            List<CartePoker> mainGagnante = new List<CartePoker>();
            foreach (List<CartePoker> hauteurGagnante in hauteurs.Values)
            {
                if (hauteurGagnante.Count == 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        mainGagnante.Add(hauteurGagnante[i]);
                    }
                    break;
                }
            }

            mainGagnante = CompleterListeCartes(mainGagnante, 1, mainGagnante[0].Hauteur);

            return new Combinaison(TypeCombinaison.Carre, mainGagnante);
        }

        private Combinaison ChercherQuinteFlush(Dictionary<HauteurCarte, List<CartePoker>> hauteurs, Dictionary<CouleurCarte, List<CartePoker>> couleurs, int nbMaxHauteurs, int nbMaxCouleurs)
        {
            if (nbMaxCouleurs < 5) return null;

            Combinaison res = null;

            List<CartePoker> mainGagnante = new List<CartePoker>();
            foreach (List<CartePoker> couleurGagnante in couleurs.Values)
            {
                if (couleurGagnante.Count >= 5)
                {
                    couleurGagnante.Sort(delegate(CartePoker c1, CartePoker c2) { return c2.CompareTo(c1); });
                    res = ChercherQuinte(couleurGagnante);
                    break;
                }
            }

            if (res == null)
            {
                return null;
            }
            else
            {
                res.TypeCombinaison = TypeCombinaison.QuinteFlush;
                return res;
            }
        }

        /// <summary>
        /// Complétion d'une liste initiale de cartes à partir de la liste des cartes (_cartes) avec certaines interdites)
        /// </summary>
        /// <param name="listeInitiale">Liste initiale des cartes</param>
        /// <param name="nbCartesACompleter">Nombre de cartes à compléter</param>
        /// <param name="listeCartesInterdites">Liste des cartes interdites, hauteu</param>
        /// <returns></returns>
        private List<CartePoker> CompleterListeCartes(List<CartePoker> listeInitiale, int nbCartesACompleter, params HauteurCarte[] listeHauteursCartesInterdites)
        {
            List<CartePoker> res = new List<CartePoker>(listeInitiale);

            int nbCartesCompletees = 0;
            foreach (CartePoker c in _cartes)
            {
                bool carteEstInterdite = false;
                foreach (HauteurCarte hauteurCarteInterdite in listeHauteursCartesInterdites)
                {
                    if (hauteurCarteInterdite == c.Hauteur)
                    {
                        carteEstInterdite = true;
                        break;
                    }
                }
                // La carte n'est pas interdite : on l'ajoute
                if (!carteEstInterdite)
                {
                    res.Add(c);
                    nbCartesCompletees++;
                    if (nbCartesCompletees == nbCartesACompleter)
                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Les 2 cartes se suivent elles ?
        /// </summary>
        /// <param name="carte1"></param>
        /// <param name="carte2"></param>
        /// <returns></returns>
        private bool StrictementInferieur(CartePoker carte1, CartePoker carte2)
        {
            return ((int)carte2.Hauteur) == ((int)carte1.Hauteur) + 1;
        }
        #endregion

        #region IComparable Members
        /// <summary>
        /// Comparaison de 2 mains
        /// </summary>
        /// <param name="autreMain">La main à comparer</param>        
        /// <returns></returns>
        public int CompareTo(MainPoker autreMain)
        {
            if (autreMain.ResultatMain == null) autreMain.DeterminerCombinaison();
            if (this.ResultatMain == null) this.DeterminerCombinaison();

            int retour = 0;
            if (autreMain.Proprietaire.JeterCartes && this.Proprietaire.JeterCartes)
            {
                // Tout le monde a jeté ses cartes
                retour = 0;
            }
            else if (autreMain.Proprietaire.JeterCartes && !this.Proprietaire.JeterCartes)
            {
                //L'autre a jeté ses cartes on est plus fort
                retour = 1;
            }
            else if (!autreMain.Proprietaire.JeterCartes && this.Proprietaire.JeterCartes)
            {
                //L'autre n'a pas jeté ses cartes mais nous si 
                retour = -1;
            }
            else
            {
                retour = this.ResultatMain.TypeCombinaison.CompareTo(autreMain.ResultatMain.TypeCombinaison);
                if (retour == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        retour = this.ResultatMain.MainGagnante[i].CompareTo(autreMain.ResultatMain.MainGagnante[i]);
                        if (retour != 0)
                            break;
                    }
                }
            }
            return retour;
        }

        #endregion

        #region Surcharges de l'opérateur égal
        /// <summary>
        /// 2 objets MainPoker sont égaux si leur main gagnante sont égales
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (!(obj is MainPoker))
                throw new ArgumentException("L'instance doit être de type 'MainPoker'");
            else
                return this.CompareTo((MainPoker)obj) == 0;
        }

        /// <summary>
        /// Retour du HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}

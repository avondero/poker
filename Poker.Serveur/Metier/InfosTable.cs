using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Poker.Interface.Metier;
using System.Timers;
using Poker.Serveur.Technique;

namespace Poker.Serveur.Metier
{
    /// <summary>
    /// Classe contenant l'ensemble des informations relatives à la table
    /// </summary>
    class InfosTable
    {
        #region Membres privés
        private static InfosTable _singleton = new InfosTable();        
        private int _numJoueur = 0;
        private int _numDealer = -1;
        private int _numJoueurPrecedent = 0;                
        private int _positionElimination;                        
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur statique
        /// </summary>
        private InfosTable()
        {
            ListeJoueurs = new List<Joueur>();
            this.OptionsJeu = new Options();
            this.NumeroDonne = 0;
        }
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Le singleton InfosTable
        /// </summary>
        internal static InfosTable Singleton
        {
            get
            {
                return _singleton;
            }
        }


        /// <summary>
        /// Une partie est elle en cours
        /// </summary>
        internal bool PartieEnCours { get; private set; }
        
        /// <summary>
        /// Board
        /// </summary>
        internal CartePoker[] Board { get; private set; }

        /// <summary>
        /// Joueur ayant fait la derniere relance
        /// </summary>
        internal Joueur DerniereRelance { get; set; }

        /// <summary>
        /// L'etape en cours
        /// </summary>
        internal EtapeDonne Etape { get; private set; }

        /// <summary>
        /// Tapis initial de chaque joueur
        /// </summary>
        internal int TapisInitial
        {
            get { return this.OptionsJeu.TapisInitial; }            
        }
       
        /// <summary>
        /// Liste des joueurs
        /// </summary>
        internal List<Joueur> ListeJoueurs { get; private set; }

        /// <summary>
        /// Pot courant
        /// </summary>
        internal int Pot { get; set; }

        /// <summary>
        /// Montant de la petite blind
        /// </summary>
        internal int MontantPetiteBlind { get; set; }

        /// <summary>
        /// Montant de la grosse blind
        /// </summary>
        internal int MontantGrosseBlind { get; set; }

        /// <summary>
        /// Nombre de données jouées
        /// </summary>
        internal int NombreDeDonnes { get; set; }

        /// <summary>
        /// Options de jeu
        /// </summary>
        internal Options OptionsJeu { get; set; }

        /// <summary>
        /// Renvoie le nombre de joueurs non éliminés
        /// </summary>
        internal int NombreDeJoueursNonElimines
        {
            get
            {             
                return (from j in ListeJoueurs where !j.Elimine
                        select j).Count();
            }
        }

        /// <summary>
        /// Numéro de la donne en cours
        /// </summary>
        internal int NumeroDonne {get; private set;}

        /// <summary>
        /// Montant de la derniere relance
        /// </summary>
        internal int MontantDerniereRelance { get; set; }
        #endregion

        #region Methodes publiques        
        /// <summary>
        /// Ajoute un joueur à la liste des joueurs et renvoie sa position autour de la table
        /// </summary>        
        /// <param name="j">Le joueur</param>        
        /// <returns>La position du joueur autour de la table</returns>
        internal int AjouterJoueur(Joueur j)
        {
            int positionTable = new Random().Next(this.ListeJoueurs.Count + 1);            
            ListeJoueurs.Insert(positionTable, j);

            return positionTable;
        }

        /// <summary>
        /// Renvoie la mise maximum jouée par les différents joueurs
        /// </summary>
        /// <returns></returns>
        internal int MiseMaximum()
        {            
            return (from j in ListeJoueurs where !j.Elimine select j.MiseTotale).Max();         
        }

        /// <summary>
        /// Calcule les actions possible pour le joueur concerné : si aucune action n'est possible renvoie null
        /// </summary>
        /// <param name="J">Le joueur concerné</param>
        /// <returns></returns>
        internal Dictionary<TypeActionJoueur, ActionPossible> CalculActionsPossibles(Joueur j)
        {
            Dictionary<TypeActionJoueur, ActionPossible> actions = null;

            if (!j.JeterCartes && j.TapisJoueur > 0)
            {
                actions = new Dictionary<TypeActionJoueur, ActionPossible>();
                // Passe
                actions[TypeActionJoueur.Passe] = ActionPossible.Jeter;

                // Parole ou suivre
                if (
                    (j.DerniereAction == TypeActionJoueur.GrosseBlind && j == this.DerniereRelance && this.Etape == EtapeDonne.PreFlop) ||
                    (this.MontantDerniereRelance == 0)
                    )
                {
                    actions[TypeActionJoueur.Parole] = new ActionPossible(TypeActionJoueur.Parole, 0, 0);
                }
                else if (j.TapisJoueur >= this.MontantDerniereRelance)
                {
                    // Pour suivre, montant min = la mise actuelle du joueur, montant max = la somme totale à mettre
                    actions[TypeActionJoueur.Suit] = new ActionPossible(TypeActionJoueur.Suit,  j.Mise, this.MontantDerniereRelance);
                }
                

                // Relance / mise
                if (j.TapisJoueur > this.RelanceMinimum())
                {
                    // Si le joueur est le premier à parler : il mise, sinon il relance
                    TypeActionJoueur actionJoueur = TypeActionJoueur.Relance;
                    if (this.MontantDerniereRelance == 0)
                        actionJoueur = TypeActionJoueur.Mise;
                    actions[actionJoueur] = new ActionPossible(actionJoueur, this.RelanceMinimum(), j.TapisJoueur);
                }

                // Tapis
                actions[TypeActionJoueur.Tapis] = new ActionPossible(TypeActionJoueur.Tapis, j.TapisJoueur, j.TapisJoueur);
            }

            return actions;

        }

        /// <summary>
        /// Distribution des gains
        /// </summary>
        /// <param name="listeMains">Liste des mains des différents joueurs encore en lice : ordonnée de la plus mauvaise à la meilleure</param>
        /// <param name="miseMax">La mise maximum</param>
        /// <param name="premierAppel">Si vrai : on envoie en plus des messages au client car c'est le premier appel de la récursivité</param>
        /// <returns>Le message d'information correspondant au résultat si premierAppel = true</returns>
        internal MessageInfo DistribuerGains(List<MainPoker> listeMains, int miseMax, bool premierAppel)
        {
            logServeur.Debug("Distribution des gains : {0} mains. MiseMax = {1}. Premier appel = {2}", listeMains.Count, miseMax, premierAppel);
            // Gestion des égalités
            int nbGagnants = 1;
            while (nbGagnants != listeMains.Count && listeMains[listeMains.Count - nbGagnants - 1].Equals(listeMains[listeMains.Count - 1]))
                nbGagnants++;            

            MainPoker mainGagnante = listeMains[listeMains.Count - 1];
            Joueur vainqueur = mainGagnante.Proprietaire;
            List<Joueur> listeVainqueurs = new List<Joueur>();
            int montantTotal = 0;
            logServeur.Debug(" Nombre de gagnants : {0}", nbGagnants);
            logServeur.Debug(" Mise totale du vainqueur : {0}", vainqueur.MiseTotale);
            if (nbGagnants == 1)
            {
                if (premierAppel) listeVainqueurs.Add(vainqueur);
                // On commence par empocher le pot                
                if (vainqueur.MiseTotale == miseMax)
                {
                    logServeur.Debug(" Pas de problème : celui qui a gagné c'est celui qui a misé le plus");
                    foreach (Joueur j in ListeJoueurs)
                    {
                        montantTotal += j.MiseTotale;                        
                    }
                    vainqueur.TapisJoueur += montantTotal;
                }
                else
                {
                    logServeur.Debug(" Le vainqueur a misé moins que les autres : il faut redistribuer");
                    int miseTotaleVainqueur = vainqueur.MiseTotale;
                    
                    bool continu = false;
                    foreach (MainPoker main in listeMains)
                    {
                        if (main.Proprietaire.MiseTotale > miseTotaleVainqueur)
                        {
                            continu = true;
                            montantTotal += miseTotaleVainqueur;
                            main.Proprietaire.MiseTotale -= miseTotaleVainqueur;
                        }
                        else
                        {
                            montantTotal += main.Proprietaire.MiseTotale;
                            main.Proprietaire.MiseTotale = 0;
                        }
                    }
                    vainqueur.TapisJoueur += montantTotal;

                    if (continu)
                    {
                        miseMax -= miseTotaleVainqueur;
                        // On retire les mains qui sont à zéro des gagnants, et on recommence
                        List<MainPoker> nouvelleListeMains = new List<MainPoker>();
                        foreach (MainPoker main in listeMains)
                        {
                            if (main.Proprietaire.MiseTotale != 0)
                                nouvelleListeMains.Add(main);
                        }
                        DistribuerGains(nouvelleListeMains, miseMax, false);
                    }
                }
            }
            else
            {
                // Plusieurs gagnants : on boucle jusqu'à ne plus en avoir qu'un                
                int miseMini = int.MaxValue;

                // On récupère la mise la plus petite effectuée par un gagnant                
                foreach (MainPoker main in listeMains)
                {
                    if (mainGagnante.Equals(main))
                    {
                        miseMini = Math.Min(main.Proprietaire.MiseTotale, miseMini);
                    }
                }
                logServeur.Debug(" Mise mini : {0}", miseMini);

                // On additionne les mises (avec cette mise comme max)
                bool resteMise = false;
                foreach (MainPoker main in listeMains)
                {
                    if (main.Proprietaire.MiseTotale <= miseMini)
                    {
                        montantTotal += main.Proprietaire.MiseTotale;
                        main.Proprietaire.MiseTotale = 0;
                    }
                    else
                    {
                        resteMise = true;
                        montantTotal += miseMini;
                        main.Proprietaire.MiseTotale -= miseMini;
                    }
                    if (premierAppel && !main.Proprietaire.JeterCartes && mainGagnante.Equals(main)) listeVainqueurs.Add(main.Proprietaire);
                }

                // On partage le tout entre les gagnants et seulement les gagnants
                foreach (MainPoker main in listeMains)
                {
                    //TODO : montant total est il forcément un multiple de nbgagnants ?
                    if (mainGagnante.Equals(main))
                    {
                        main.Proprietaire.TapisJoueur += montantTotal / nbGagnants;
                    }
                }

                // Si la mise la plus petite était égale à la mise max --> Fini
                // Sinon, on retire les mains qui sont à zéro des gagnants, et on recommence
                if (resteMise)
                {
                    List<MainPoker> nouvelleListeMains = new List<MainPoker>();
                    foreach (MainPoker main in listeMains)
                    {
                        if (main.Proprietaire.MiseTotale != 0)
                            nouvelleListeMains.Add(main);
                    }
                    DistribuerGains(nouvelleListeMains, miseMax, false);
                }
            }

            if (premierAppel)
            {
                if (listeVainqueurs.Count == 1)
                    return new MessageInfo(vainqueur, mainGagnante.ResultatMain, montantTotal);
                else
                    return new MessageInfo(listeVainqueurs, mainGagnante.ResultatMain, montantTotal);
            }
            else
                return null;

        }

        /// <summary>
        /// Gestion de la répartition des gains en fonction des différentes mains
        /// </summary>
        internal List<MainPoker> CalculerMainsGagnantes()
        {
            // Construction des mains
            List<MainPoker> listeMains = new List<MainPoker>();
            foreach (Joueur j in this.ListeJoueurs)
                if (!j.Elimine)
                {
                    var  main = new MainPoker(j, this.Board);
                    main.DeterminerCombinaison();
                    listeMains.Add(main);
                }

            // On trie les mains
            listeMains.Sort();

            // On les trace
            logServeur.Debug("Board : {0}, {1}, {2}, {3}, {4}", this.Board[0], this.Board[1], this.Board[2], this.Board[3], this.Board[4]);
            logServeur.Debug("Liste des mains (du plus petit au plus grand):");
            foreach (MainPoker main in listeMains)
            {
                logServeur.Debug("  Joueur : {0} ({2}, {3}), Combinaison : {1}", main.Proprietaire.Nom, main.ResultatMain.TypeCombinaison.ToString(), main.Proprietaire.Carte1, main.Proprietaire.Carte2);
                logServeur.Debug("    Main : {0}, {1}, {2}, {3}, {4}", main.ResultatMain.MainGagnante[0], main.ResultatMain.MainGagnante[1], main.ResultatMain.MainGagnante[2], main.ResultatMain.MainGagnante[3], main.ResultatMain.MainGagnante[4]);
            }

            return listeMains;
        }

        /// <summary>
        /// Recherche d'un joueur parmi la liste des joueurs en fonction de son nom
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        internal Joueur RechercherJoueur(string nom)
        {
            return (from j in ListeJoueurs where j.Nom == nom select j).FirstOrDefault<Joueur>();
        }
        /// <summary>
        /// Recherche d'un joueur en fonction de son identifiant
        /// </summary>
        /// <param name="identifiant"></param>
        /// <returns></returns>
        internal Joueur RechercherJoueur(Guid identifiant)
        {
            return (from j in ListeJoueurs where j.Identifiant == identifiant select j).FirstOrDefault<Joueur>();
        }


        /// <summary>
        /// Renvoie le nombre de qui peuvent encore parler
        /// </summary>
        /// <returns></returns>
        internal int NombreDeJoueursPouvantParler(int miseMax)
        {            
            return (from j in ListeJoueurs where PeutEncoreParler(j, miseMax) select j).Count();
        }

        /// <summary>
        /// Remise à 0 des mises des joueurs et augmentation du pot en fonction
        /// </summary>
        /// <param name="complete">Si complete : on réinitialise aussi les mises totales. Sinon on ajoute au mise totale les mises en cours</param>
        internal void RAZMiseJoueurs(bool complete)
        {
            foreach (Joueur j in ListeJoueurs)
            {
                Pot += j.Mise;
                if (complete)
                    j.MiseTotale = 0;
                else
                    j.MiseTotale += j.Mise;
                j.TapisJoueur -= j.Mise;
                j.Mise = 0;
                j.DerniereAction = TypeActionJoueur.Aucune;
            }
        }

        /// <summary>
        /// Renvoie le dernier joueur en course : null s'il en reste plus qu'un
        /// </summary>
        /// <returns></returns>
        internal List<Joueur> JoueursEncoreEnCourse()
        {
            return (from j in ListeJoueurs where !j.JeterCartes select j).ToList<Joueur>();
        }

        /// <summary>
        /// Elimine tous les joueurs dont la mise est à 0 et qui ne sont pas encore éliminés
        /// </summary>
        /// <returns>Le nombre de joueurs restants</returns>
        internal int EliminerJoueurs()
        { 
            bool elimination = false;
            int nbEliminations = 0;

            foreach (Joueur j in ListeJoueurs)
            {
                if (!j.Elimine && j.TapisJoueur == 0)
                {
                    j.Elimine = true;
                    j.PositionElimination = _positionElimination;                    
                    elimination = true;
                    nbEliminations++;
                }
            }

            if (elimination) _positionElimination += nbEliminations;

            // Si un seul n'est pas éliminé : la partie est terminée. On flagge le dernier joueur comme éliminé en dernier
            if (_positionElimination == ListeJoueurs.Count)
            {
                PartieEnCours = false;
                foreach (Joueur j in ListeJoueurs)
                    if (!j.Elimine) j.PositionElimination = _positionElimination;
            }

            return ListeJoueurs.Count - _positionElimination + 1;
        }

        /// <summary>
        /// Démarre une nouvelle partie
        /// </summary>
        internal void NouvellePartie()
        {
            PartieEnCours = true;
            Pot = 0;
                        
            _numDealer = new Random().Next(ListeJoueurs.Count);
            _numJoueur = _numDealer;
            _numJoueurPrecedent = _numDealer;
            foreach (Joueur j in ListeJoueurs)
            {
                j.TapisJoueur = this.OptionsJeu.TapisInitial;
                j.TourDeJeu = false;
                j.Bouton = TypeBouton.Aucun;
                j.Elimine = false;
                j.PositionElimination = 0;
                j.Mise = 0;
                j.MiseTotale = 0;
            }
            _positionElimination = 1;
            NombreDeDonnes = 0;
        }
      
        /// <summary>
        /// Redémarre une donne
        ///  Etape : preflop
        /// </summary>
        internal void NouvelleDonne()
        {
            this.Etape = EtapeDonne.PreFlop;
            Pot = 0;
            // Le dealer change : c'est le suivant du précédent qui était dealer
            do
            {
                _numDealer = (_numDealer + 1) % ListeJoueurs.Count;
            } while (ListeJoueurs[_numDealer].Elimine);
            ChangerDealer(_numDealer);

            JeuPoker.ReinitialiserJeu();

            // On "reset" les infos de chaque joueur
            foreach (Joueur j in ListeJoueurs)
            {               
                j.Mise = 0;
                j.MiseTotale = 0;
                j.TourDeJeu = false;
                j.DerniereAction = TypeActionJoueur.Aucune;
                j.JeterCartes = false;

                if (!j.Elimine)
                {
                    j.Carte1 = JeuPoker.TirerUneCarteAuHasard();
                    j.Carte2 = JeuPoker.TirerUneCarteAuHasard();
                }
                else
                {
                    j.Carte1 = null;
                    j.Carte2 = null;
                    j.Bouton = TypeBouton.Aucun;
                }            
            }

            this.Board = new CartePoker[5];

            this.NumeroDonne++;
        }
        
        /// <summary>
        /// Passe au joueur suivant non éliminé ou qui n'a pas jeté ses cartes (on en profite pour dire que le tour de jeu est sur celui la et plus sur le précédent)
        /// </summary>
        /// <remarks>La fonction ne peut pas boucler car il reste forcement au moins un  joueur qui n'a pas jeté ses cartes</remarks>
        /// <returns></returns>
        internal Joueur JoueurSuivant()
        {
            _numJoueurPrecedent = _numJoueur;
            if (_numJoueurPrecedent != -1) ListeJoueurs[_numJoueurPrecedent].TourDeJeu = false;

            do
            {
                _numJoueur = (_numJoueur + 1) % ListeJoueurs.Count;
            } while (ListeJoueurs[_numJoueur].JeterCartes);

            ListeJoueurs[_numJoueur].TourDeJeu = true;

            return ListeJoueurs[_numJoueur];
        }

        /// <summary>
        /// Renvoie le dernier joueur renvoyé par JoueurSuivant (éventuellement éliminé)
        /// </summary>
        /// <returns>Le dernier joueur</returns>
        internal Joueur JoueurPrecedent
        {
            get
            {
                return ListeJoueurs[_numJoueurPrecedent];
            }
        }

        /// <summary>
        /// Renvoie le joueur courant 
        /// </summary>
        /// <returns>Le dernier joueur</returns>
        internal Joueur JoueurCourant
        {
            get
            {
                return ListeJoueurs[_numJoueur];
            }
        }       

        /// <summary>        
        /// 2 * la derniere relance si relance        
        ///   et grosse blind sinon
        /// </summary>
        /// <returns></returns>
        internal int RelanceMinimum()
        {
            return Math.Max(2 * this.MontantDerniereRelance, this.MontantGrosseBlind);            
        }

        /// <summary>
        /// Passe à l'étape d'après
        /// </summary>
        /// <returns>Vrai si on a réussi à passer à l'étape suivante</returns>
        internal EtapeDonne EtapeSuivante()
        {
            if (this.Etape != EtapeDonne.FinDonne)
            {
                this.Etape++;
                
                RAZMiseJoueurs(false);

                // Le joueur suivant est le suivant du donneur
                _numJoueur = _numDealer;
                do
                {
                    _numJoueur = (_numJoueur + 1) % ListeJoueurs.Count;
                } while (ListeJoueurs[_numJoueur].JeterCartes);
                _numJoueurPrecedent = _numDealer;

                // On lui dit aussi qu'il a fini de jouer
                foreach (Joueur j in ListeJoueurs)
                {
                    j.TourDeJeu = false;
                }
                ListeJoueurs[_numJoueur].TourDeJeu = true;

                // C'est le dealer qui a fait la derniere relance : 0 et qui a joué
                this.DerniereRelance = ListeJoueurs[_numDealer];
                this.MontantDerniereRelance = 0;

                logServeur.Debug("*** Début de l'étape : {0}. Dealer = {1}", this.Etape, ListeJoueurs[_numDealer].Nom);

                // On tire les cartes du board
                switch (this.Etape)
                {
                    case EtapeDonne.Flop:
                        this.Board[0] = JeuPoker.TirerUneCarteAuHasard();
                        this.Board[1] = JeuPoker.TirerUneCarteAuHasard();
                        this.Board[2] = JeuPoker.TirerUneCarteAuHasard();                        
                        break;

                    case EtapeDonne.Turn:
                        this.Board[3] = JeuPoker.TirerUneCarteAuHasard();
                        break;

                    case EtapeDonne.River:
                        this.Board[4] = JeuPoker.TirerUneCarteAuHasard();
                        break;
                }
            }

            return this.Etape;
        }

        /// <summary>
        /// Augmente la mise faite par le joueur j : positionne sa mise au montant demandée
        /// </summary>
        /// <param name="j"></param>
        /// <param name="montant"></param>
        internal void Miser(Joueur j, int montant)
        {
            if (j.TapisJoueur < montant)
                montant = j.TapisJoueur;

            j.Mise = montant;                        
        }
        #endregion

        #region methodes privées
        /// <summary>
        /// Changement de dealer
        /// </summary>
        /// <param name="numJoueur">Le numéro de joueur concerné</param>
        private void ChangerDealer(int numJoueur)
        {
            foreach (Joueur j in ListeJoueurs)
                j.Bouton = TypeBouton.Aucun;

            ListeJoueurs[numJoueur].Bouton = TypeBouton.Dealer;
            _numJoueurPrecedent = numJoueur;
            _numJoueur = numJoueur;
        }

        /// <summary>
        /// Peut encore parler (ne traite pas le cas du joueur ou tout le monde a fait parole et ne traite pas le cas ou le joueur peut faire tapis)
        /// </summary>
        /// <param name="miseRelance">Mise du dernier relanceur</param>
        /// <returns></returns>
        private bool PeutEncoreParler(Joueur j, int miseRelance)
        {
            if (j.JeterCartes)
                return false;

            if ((j.DerniereAction == TypeActionJoueur.Aucune || j.DerniereAction == TypeActionJoueur.PetiteBlind || j.DerniereAction == TypeActionJoueur.GrosseBlind) 
                && (j.TapisJoueur - j.Mise) > 0)
                return true;

            return (j.Mise < miseRelance && (j.TapisJoueur - j.Mise) > 0);
        }
        #endregion
    }
}
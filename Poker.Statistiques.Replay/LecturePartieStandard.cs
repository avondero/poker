using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.ExtensionsClient.Replay;
using Poker.Interface.Stats;
using System.Reflection;
using System.IO;
using Poker.Interface.Metier;

namespace Poker.Statistiques.Replay
{
    /// <summary>
    /// Implémentation par défaut de ILecturePartie
    /// </summary>
    public class LecturePartieStandard : ILecturePartie
    {
        #region Attributs
        private const string PATTERN_FICHIER_DETAILSPARTIE = "_DetailsPartie.csv";
        private const string PATTERN_FICHIER_LISTE_DONNES = "_ListeDonnes.csv";
        private string _cheminFichiers = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");
        private Dictionary<int, long> _posEvenementJeu = new Dictionary<int, long>();   // Position dans le fichier des evts
        private Dictionary<int, int> _posDebutDonne = new Dictionary<int, int>();       // N° de l'evt pour la donne
        private Dictionary<int, long> _posDonne = new Dictionary<int, long>();          // Position dans le fichier des donnes
        private const byte CARRIAGE_RETURN = 13;
        private const byte LINE_FEED = 10;
        private FileStream _lectureDetailPartie = null;
        private FileStream _lectureDonnes = null;        
        private int _numEvtJeu = 0;
        #endregion

        #region ILecturePartie Members

        /// <summary>
        /// Description de la classe
        /// </summary>
        public string Description { get { return "Replay standard"; } }

        public event ChangementDonneHandler ChangementDonne;

        /// <summary>
        /// Liste des parties disponibles : Une partie = 2 fichiers CSV (DetailPartie + listeDonne)
        /// </summary>
        /// <returns></returns>
        public List<Partie> PartiesDisponibles()
        {
            List<Partie> listeParties = new List<Partie>();
            
            foreach (string detailPartie in Directory.GetFiles(_cheminFichiers, "*" + PATTERN_FICHIER_DETAILSPARTIE))
            { 
                string datePartie = detailPartie.Substring(0, detailPartie.IndexOf(PATTERN_FICHIER_DETAILSPARTIE)) ;
                string listeDonnes = datePartie + PATTERN_FICHIER_LISTE_DONNES;
                if (File.Exists(listeDonnes))
                {
                    string nomPartie = Path.GetFileNameWithoutExtension(datePartie);
                    string desc = new DateTime(int.Parse(nomPartie.Substring(0,4)), int.Parse(nomPartie.Substring(4,2)), int.Parse(nomPartie.Substring(6,2))).ToString("dd/MM/yyyy");
                    listeParties.Add(new Partie() { Nom = nomPartie, Description = "Partie du " + desc, Identifiant = datePartie });
                }
            }

            return listeParties;
        }

        /// <summary>
        /// Demarrage de la lecture de la partie : on va lire le fichier partie pour faire un index des différentes positions du fichier partie
        ///  Idem pour le fichier donne
        /// <remarks>La partie ne démarre bien que lorsque l'on lance démarrer</remarks>
        /// </summary>
        /// <param name="partieSelectionne"></param>
        /// <returns>La liste des joueurs concernés par cette partie</returns>
        public void DemarrageLecturePartie(Partie partieSelectionne)
        {
            InitialiserListeEvenements(partieSelectionne);
            InitialiserListeDonnes(partieSelectionne);
          
            _numEvtJeu = 0;
            this.EvenementCourant = null;            
        }

        public void FinLecturePartie(Partie partieSelectionne)
        {
            _lectureDetailPartie.Close();
            _lectureDonnes.Close();
        }

        public EvenementJeu AllerDonne(int numeroDonne)
        {            
            _numEvtJeu = _posDebutDonne[numeroDonne];
            return this.AvancerEvenement();
        }

        
        public int NombreDeDonnes()
        {
            return _posDebutDonne.Count - 1;
        }

        public EvenementJeu AvancerEvenement()
        {
            _numEvtJeu++;
            this.EvenementCourant = null;

            string[] infosEvt = LireExtraitFichierDetailPartie(_posEvenementJeu[_numEvtJeu - 1], _posEvenementJeu[_numEvtJeu]).Split(';');
            switch (infosEvt[2])
            {
                case "NouvellePartie":
                    TraiterEvtNouvellePartie(infosEvt);
                    break;

                case "NouvelleDonne":
                    TraiterEvtNouvelleDonne(infosEvt);
                    break;

                case "AugmentationBlinds":
                    TraiterEvtAugmentationBlinds(infosEvt);
                    break;

                case "Chat":
                    TraiterEvtChat(infosEvt);
                    break;

                case "PetiteBlind":
                case "GrosseBlind":
                case "Passe":
                case "Parole":
                case "Suit":
                case "Mise":
                case "Relance":
                case "Tapis":
                    TraiterEvtActionJoueur(infosEvt, (TypeActionJoueur) Enum.Parse(typeof(TypeActionJoueur), infosEvt[2]));                    
                    break;

                case "Flop":
                    TraiterEvtFlop(infosEvt);
                    break;
                
                case "Turn":
                    TraiterEvtTurn(infosEvt);
                    break;

                case "River":
                    TraiterEvtRiver(infosEvt);
                    break;

                case "FinDonne":
                    TraiterEvtFinDonne(infosEvt);
                    break;

                case "FinPartie":
                    TraiterEvtFinPartie(infosEvt);
                    break;

                default:
                    // On ne doit pas arriver dans ce cas la
                    throw new ApplicationException("Erreur lors de la lecture du type " + infosEvt[2]);
            }

            if (this.EvenementCourant != null)
            {
                ModifierDateEvt(infosEvt[0], this.EvenementCourant);
            }

            return this.EvenementCourant;

        }

       

       
        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Evenement courant
        /// </summary>
        public EvenementJeu EvenementCourant { get; private set; }
        #endregion

        #region Methodes privées
        #region Traitement des evenements de jeu
        private void TraiterEvtFinPartie(string[] infosEvt)
        {
            FinPartie evt = new FinPartie();
            evt.ListeJoueurs = new List<JoueurStat>();
            int pos = 1;
            foreach (string joueur in infosEvt[3].Split(','))
            {
                evt.ListeJoueurs.Add(new JoueurStat() { PositionElimination = pos++, Nom = joueur});
            }

            this.EvenementCourant = evt;
        }

        private void TraiterEvtFinDonne(string[] infosEvt)
        {
            FinDonne evt = new FinDonne();

            // Liste des gagnants
            string[] gagnants = infosEvt[3].Split(',');
            evt.JoueursGagnants = new List<JoueurStat>();
            foreach (string gagnant in gagnants)
            {
                evt.JoueursGagnants.Add(new JoueurStat() { Nom = gagnant});
            }
            
            // Combinaison gagnante            
            string[] infosComb = infosEvt[4].Split(':');
            if (infosComb.Length == 1)
            {
                evt.CombinaisonGagnante = null;
            }
            else
            {
                evt.CombinaisonGagnante = new Combinaison(
                    (TypeCombinaison)Enum.Parse(typeof(TypeCombinaison), infosComb[0]),
                    ParserChaineEnListeCartes(infosComb[1], ',')
                    );
            }

            evt.Pot = int.Parse(infosEvt[5]);
           
            this.EvenementCourant = evt;            
        }

        private void TraiterEvtFlop(string[] infosEvt)
        {
            string[] flop = infosEvt[3].Split(',');
            this.EvenementCourant = new Flop()
            {
                Carte1 = ParserChaineEnCarte(flop[0]),
                Carte2 = ParserChaineEnCarte(flop[1]),
                Carte3 = ParserChaineEnCarte(flop[2]),
                Pot = int.Parse(infosEvt[4])
            };
        }

        private void TraiterEvtTurn(string[] infosEvt)
        {
            this.EvenementCourant = new Turn()
            {
                Carte = ParserChaineEnCarte(infosEvt[3]),
                Pot = int.Parse(infosEvt[4])
            };
        }

        private void TraiterEvtRiver(string[] infosEvt)
        {
            this.EvenementCourant = new River()
            {
                Carte = ParserChaineEnCarte(infosEvt[3]),
                Pot = int.Parse(infosEvt[4])
            };
        }

        private void TraiterEvtActionJoueur(string[] infosEvt, TypeActionJoueur typeAction)
        {
            this.EvenementCourant = new ActionJoueurStat()
            {
                TypeAction = typeAction,
                Nom = infosEvt[3],
                Mise = int.Parse(infosEvt[4]),
                Tapis = int.Parse(infosEvt[5])
            };            
        }

        private void TraiterEvtChat(string[] infosEvt)
        {
            this.EvenementCourant = new Chat() { Nom = infosEvt[3], Message = infosEvt[4] };
        }

        private void TraiterEvtAugmentationBlinds(string[] infosEvt)
        {
            string[] blindsActuelles = infosEvt[3].Split(',');
            string[] prochainesBlinds= infosEvt[4].Split(',');
            int delaiAugmentationBlinds = int.Parse(infosEvt[5]);

            this.EvenementCourant = new AugmentationBlinds()
            {
                MontantPetiteBlind = int.Parse(blindsActuelles[0]),
                MontantGrosseBlind = int.Parse(blindsActuelles[1]),
                MontantProchainePetiteBlind = int.Parse(prochainesBlinds[0]),
                MontantProchaineGrosseBlind = int.Parse(prochainesBlinds[1]),
                DelaiAugmentationBlinds = new TimeSpan(0, delaiAugmentationBlinds, 0)
            };
        }

        private void TraiterEvtNouvelleDonne(string[] infosEvt)
        {            
            NouvelleDonne nelleDonne = new NouvelleDonne();
            int numDonne = int.Parse(infosEvt[1]);
            nelleDonne.NumeroDonne = numDonne;

            // Lecture des infos dans le fichier des donnes
            string[] infosJoueurs = LireExtraitFichierDonne(_posDonne[numDonne], _posDonne[numDonne + 1]).Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<JoueurStat> listeJoueurs = new List<JoueurStat>();
            for (int i = 0; i < infosJoueurs.Length; i++)
            {
                string[] detJoueurs = infosJoueurs[i].Split(';');
                listeJoueurs.Add(new JoueurStat()
                {
                    Nom = detJoueurs[2],
                    Carte1 = ParserChaineEnCarte(detJoueurs[3]),
                    Carte2 = ParserChaineEnCarte(detJoueurs[4]),
                    Tapis = int.Parse(detJoueurs[5])
                });
            }
            string[] infosDealer = infosEvt[3].Split(',');
            nelleDonne.Dealer = infosDealer[0];
            nelleDonne.PetiteBlind = infosDealer[1];
            nelleDonne.GrosseBlind = infosDealer[2];

            
            nelleDonne.InfosBlind = new Blind();
            string[] infosBlind = infosEvt[4].Split(',');
            nelleDonne.InfosBlind.MontantPetiteBlind = int.Parse(infosBlind[0]);
            nelleDonne.InfosBlind.MontantGrosseBlind = int.Parse(infosBlind[1]);
            infosBlind = infosEvt[5].Split(',');
            nelleDonne.InfosBlind.MontantProchainePetiteBlind = int.Parse(infosBlind[0]);
            nelleDonne.InfosBlind.MontantProchaineGrosseBlind  = int.Parse(infosBlind[1]);
            
            nelleDonne.ListeJoueurs = listeJoueurs;

            this.EvenementCourant = nelleDonne;

            if (ChangementDonne != null) ChangementDonne(this, new ChangementDonneEventArgs() { NumeroDonne = numDonne });
        }

        private void TraiterEvtNouvellePartie(string[] infosEvt)
        {
            List<JoueurStat> resListe = new List<JoueurStat>();
            string[] listeJoueurs = infosEvt[3].Split(',');
            string[] potJoueurs = infosEvt[4].Split(',');
            for (int i = 0; i < listeJoueurs.Length; i++)
            {
                resListe.Add(new JoueurStat() { Nom = listeJoueurs[i], Tapis = int.Parse(potJoueurs[i]), Carte1 = null, Carte2 = null, PositionElimination = 0 });
            }
            NouvellePartie nellePartie = new NouvellePartie();
            nellePartie.ListeJoueurs = resListe;
            this.EvenementCourant = nellePartie;
        }
        #endregion

        private CartePoker ParserChaineEnCarte(string carte)
        {
            if (carte == "-") return null;

            try
            {
                string[] infosCarte = carte.Split(' ');
                HauteurCarte hauteur ;
                switch (infosCarte[0])
                { 
                    case "As":
                        hauteur = HauteurCarte.As;
                        break;

                    case "Roi":
                        hauteur = HauteurCarte.Roi;
                        break;

                    case "Dame":
                        hauteur = HauteurCarte.Dame;
                        break;

                    case "Valet":
                        hauteur = HauteurCarte.Valet;
                        break;
                    
                    default:
                        hauteur = (HauteurCarte) Enum.Parse(typeof(HauteurCarte), infosCarte[0]);
                        break;
                }

                CouleurCarte couleur;
                switch (infosCarte[2])
                { 
                    case "trèfle":
                        couleur = CouleurCarte.Trefle;
                        break;

                    case "carreau":
                        couleur = CouleurCarte.Carreau;
                        break;

                    case "coeur":
                        couleur = CouleurCarte.Coeur;
                        break;

                    case "pique":
                        couleur = CouleurCarte.Pique;
                        break;

                    default:
                        throw new ApplicationException("Erreur lors du parsing de la carte : la couleur " + infosCarte[2] + " n'existe pas");                      
                }

                return new CartePoker(hauteur, couleur);

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors du parsing de la carte : " + carte, ex);
            }

        }

        private List<CartePoker> ParserChaineEnListeCartes(string cartes, char sep)
        {
            List<CartePoker> res = new List<CartePoker>();
            string[] listeCartes = cartes.Split(sep);
            foreach (string carte in listeCartes)
            { 
                res.Add(ParserChaineEnCarte(carte));
            }

            return res;
        }
        private void ModifierDateEvt(string date, EvenementJeu evt)
        {            
            evt.DateEvenement = DateTime.Parse(date);
        }
        /// <summary>
        /// Renvoie la chaine correspondante comprise entre posDepart et posArrivee (exclus)
        /// </summary>
        /// <param name="posDepart"></param>
        /// <param name="posArrivee"></param>
        /// <returns></returns>
        private string LireExtraitFichierDetailPartie(long posDepart, long posArrivee)
        {
            int taille = (int)(posArrivee - posDepart) - 2; // -2 pour enlever /r/n
            byte[] res = new byte[taille];
            _lectureDetailPartie.Position = posDepart;
            _lectureDetailPartie.Read(res, 0, taille);
            return Encoding.UTF8.GetString(res);
        }

        private string LireExtraitFichierDonne(long posDepart, long posArrivee)
        {
            int taille = (int)(posArrivee - posDepart) - 2; // -2 pour enlever /r/n
            byte[] res = new byte[taille];
            _lectureDonnes.Position = posDepart;
            _lectureDonnes.Read(res, 0, taille); 
            return Encoding.UTF8.GetString(res);
        }

        private void InitialiserListeEvenements(Partie partieSelectionnee)
        {
            _posEvenementJeu = new Dictionary<int, long>();
            _posDebutDonne = new Dictionary<int, int>();
            _lectureDetailPartie = new FileStream(Path.Combine(_cheminFichiers, partieSelectionnee.Identifiant + PATTERN_FICHIER_DETAILSPARTIE), FileMode.Open, FileAccess.Read);
            int octet = 0;
            int numEvt = 0;            
            string ligne = string.Empty;
            long posFinLignePrec = 0;
            bool lectureEntete = true; // est vrai si on est en train de lire l'entete (la premiere ligne)
            while ((octet = _lectureDetailPartie.ReadByte()) != -1)
            {
                switch (octet)
                {
                    case LINE_FEED:
                        if (!lectureEntete)
                        {
                            _posEvenementJeu[numEvt] = posFinLignePrec; 
                            string[] infosEvt = ligne.Split(';');
                            switch (infosEvt[2])
                            {
                                case "NouvelleDonne":
                                    _posDebutDonne[int.Parse(infosEvt[1])] = numEvt;
                                    break;

                                case "NouvellePartie":
                                    _posDebutDonne[0] = numEvt;
                                    break;

                                default: break;
                            }
                            numEvt++;
                        }
                        else
                        {
                            lectureEntete = false;
                        }
                        posFinLignePrec = _lectureDetailPartie.Position;
                        ligne = string.Empty;
                        break;

                    default:
                        // On est en train de lire le numéro de donne
                        if (!lectureEntete)
                        {
                            ligne += (char)octet;
                        }
                        break;

                }
            }

            // On rajoute un evt bidon pour pouvoir lire tranquillement le dernier enregistrement
            _posEvenementJeu[numEvt] = _lectureDetailPartie.Length;            
        }

        private void InitialiserListeDonnes(Partie partieSelectionnee)
        {
            _posDonne = new Dictionary<int, long>();
            _lectureDonnes = new FileStream(Path.Combine(_cheminFichiers, partieSelectionnee.Identifiant + PATTERN_FICHIER_LISTE_DONNES), FileMode.Open, FileAccess.Read);
            int octet = 0;
            int numDonne = -1;
            string libNumDonne = "1";
            bool premierPV = true; // est vrai si le prochain ; est le premier
            bool secondPV = false; // est vrai si le prochain ; est le seconde
            bool lectureEntete = true; // est vrai si on est en train de lire l'entete (la premiere ligne)
            long posDebutLigne = 0;
            while ((octet = _lectureDonnes.ReadByte()) != -1)
            {
                switch (octet)
                {
                    case LINE_FEED:
                        if (lectureEntete)
                        {
                            lectureEntete = false;
                        }
                        else
                        {
                            premierPV = true;
                            secondPV = false;
                            if (int.Parse(libNumDonne) != numDonne)
                            {
                                numDonne = int.Parse(libNumDonne);
                                _posDonne[numDonne] = posDebutLigne;
                            }
                            libNumDonne = string.Empty;                            
                        }
                        posDebutLigne = _lectureDonnes.Position;
                        break;

                    case ';':
                        if (premierPV)
                        {
                            secondPV = true;
                            premierPV = false;
                        }
                        else if (secondPV)
                        {
                            premierPV = false;
                            secondPV = false;
                        }
                        break;

                    default:
                        // On est en train de lire le numéro de donne
                        if (!lectureEntete && !premierPV && secondPV)
                        {
                            libNumDonne += (char)octet;
                        }
                        break;

                }
            }

            _posDonne[numDonne + 1] = _lectureDonnes.Position;
        }
        #endregion
    }
}

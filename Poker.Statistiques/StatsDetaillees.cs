using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Stats;
using System.IO;
using System.Reflection;
using Poker.Interface.Metier;

namespace Poker.Statistiques
{
    /// <summary>
    /// Classe de stats détaillés : génère 3 fichiers CSV 
    ///  BilanPartie.csv : Bilan de la partie
    ///  Donne.csv  : Liste des donness
    ///  DetailPartie.csv : Détail de tous les evenements de la partie
    /// </summary>
    public class StatsDetaillees : StatistiquesBase
    {

        #region Attributs
        private StreamWriter _writerResumePartie = null;
        private StreamWriter _writerDetailPartie = null;
        private StreamWriter _writerListeDonnes = null; 
        private string _cheminPartieCsv = string.Empty;        
        private const string NOM_FICHIER_BILAN_PARTIE = "{0:yyyyMMddHHmmss}_BilanPartie.csv";
        private const string NOM_FICHIER_RESUME_PARTIE = "{0:yyyyMMddHHmmss}_ResumePartie.csv";
        private const string NOM_FICHIER_DETAILSPARTIE = "{0:yyyyMMddHHmmss}_DetailsPartie.csv";
        private const string NOM_FICHIER_LISTE_DONNES = "{0:yyyyMMddHHmmss}_ListeDonnes.csv";               
        private string _dealer = string.Empty;
        private string _petiteBlind = string.Empty;
        private string _grosseBlind = string.Empty;
        private int _montantPetiteBlind = 0;
        private int _montantGrosseBlind = 0;
        private CartePoker[] _flop = new CartePoker[3];
        private CartePoker _turn = null;
        private CartePoker _river = null;
        private int _enjeu = 0;
        private DateTime _datePartie;
        private List<JoueurStat> _listeJoueursDebutDonne = null;
        private int _numDonne = 0;
        private const string PAS_D_INFOS = "-";
        #endregion


        #region Constructeurs

        #endregion

        #region Methodes protegees surchargees
        protected override void EnregistrerNouvellePartie(NouvellePartie evt)
        {
            // Initialisation des chemins des fichiers
            string cheminFichiers = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constantes.REPERTOIRE_DATA);
            Directory.CreateDirectory(cheminFichiers);
            _cheminPartieCsv = Path.Combine(cheminFichiers, string.Format(NOM_FICHIER_BILAN_PARTIE, evt.DateEvenement));
            _writerResumePartie = new StreamWriter(Path.Combine(cheminFichiers, string.Format(NOM_FICHIER_RESUME_PARTIE, evt.DateEvenement)));
            _writerResumePartie.AutoFlush = false;
            _writerDetailPartie = new StreamWriter(Path.Combine(cheminFichiers, string.Format(NOM_FICHIER_DETAILSPARTIE, evt.DateEvenement)));
            _writerDetailPartie.AutoFlush = false;
            _writerListeDonnes = new StreamWriter(Path.Combine(cheminFichiers, string.Format(NOM_FICHIER_LISTE_DONNES, evt.DateEvenement)));
            _writerListeDonnes.AutoFlush = false;

            // Ecriture de l'entete de la partie
            _datePartie = evt.DateEvenement;
            _enjeu = evt.OptionsJeu.TapisInitial * evt.ListeJoueurs.Count;

            // Ecriture de l'entete du résume de la partie
            string entete = "DateDonne;NumeroDonne;";
            string donne0 = string.Format("{0};0;", evt.DateEvenement);
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                entete += j.Nom + ";";
                donne0 += j.Tapis.ToString() + ";";
            }
            entete += "Gain;JoueurGagnant;CombinaisonGagnante;Carte1;Carte2;Flop1;Flop2;Flop3;Turn;River;Dealer;PetiteBlind;GrosseBlind;MontantPetiteBlind;MontantGrosseBlind";
            donne0 += "0;-;-;-;-;-;-;-;-;-;-;-;-;-;-";

            _writerResumePartie.WriteLine(entete);
            _writerResumePartie.WriteLine(donne0);

            // Ecriture de l'entete de la liste des donnes
            _writerListeDonnes.WriteLine("DateEvenement;NumeroDonne;Joueur;Carte1;Carte2;Tapis");

            // Ecriture de l'entete du détail de la partie
            // Rappel : Les champs Infos1, Infos2 et Infos3 correspondent aux informations suivantes
            //          AugmentationBlinds	Chat	ActionJoueur	NouvelleDonne	Flop	Turn	River   FinDonne	            FinPartie	    NouvellePartie
            //  Infos1 	PB,GB   	        Message	Mise	        Dealer,PB,GB    Carte1	Carte1	Carte1  ListeJoueursGagnants    ListeJoueurs	ListeJoueurs
            //  Infos2	PPB, PGB            -       Tapis	        PB,GB           Carte2	-	    -       Combinaison:ListeCartes	-           	ListeTapisJoueurs
            //  Infos3	Delai               -       -               PPB,PGB         Carte3	-       -       Pot                     -               -
            _writerDetailPartie.WriteLine("DateEvenement;NumeroDonne;TypeAction;Infos1;Infos2;Infos3");
            string listeJoueurs = string.Empty;
            string listeTapis = string.Empty;
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                listeJoueurs += "," + j.Nom;
                listeTapis += "," + j.Tapis;
            }
            EcrireDetailPartie(evt.DateEvenement, 0, "NouvellePartie", listeJoueurs.Substring(1), listeTapis.Substring(1), PAS_D_INFOS);
        }

        protected override void EnregistrerFinPartie(FinPartie evt)
        {
            StreamWriter wr = new StreamWriter(_cheminPartieCsv);
            string entete = "DatePartie;Enjeu";
            string detail = string.Format("{0};{1}", _datePartie.ToString("dd/MM/yyyy HH:mm:ss"), _enjeu);
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                entete += ";" + j.Nom;
                detail += ";" + j.PositionElimination;
            }
            wr.WriteLine(entete);
            wr.WriteLine(detail);
            wr.Close();

            // Detail de la partie
            List<JoueurStat> classementPartie = new List<JoueurStat>(evt.ListeJoueurs);
            classementPartie.Sort(delegate(JoueurStat j1, JoueurStat j2) { return j1.PositionElimination.CompareTo(j2.PositionElimination); });
            EcrireDetailPartie(evt.DateEvenement, _numDonne, "FinPartie", TranformerListeEnChaine(classementPartie), PAS_D_INFOS, PAS_D_INFOS);

            // Fermeture des autres fichiers
            _writerDetailPartie.Close();
            _writerResumePartie.Close();
            _writerListeDonnes.Close();
        }

        protected override void EnregistrerFinDonne(FinDonne evt)
        {
            // Ecriture de la liste des donnes
            for (int i = 0; i < evt.ListeJoueurs.Count; i++)
            {
                // DateEvenement	NumeroDonne	Joueur	Carte1	Carte2  Tapis
                _writerListeDonnes.WriteLine(
                    "{0};{1};{2};{3};{4};{5}",
                    evt.DateEvenement,
                    evt.NumeroDonne,
                    evt.ListeJoueurs[i].Nom,
                    evt.ListeJoueurs[i].Carte1.LibelleCarte(),
                    evt.ListeJoueurs[i].Carte2.LibelleCarte(),
                    _listeJoueursDebutDonne[i].Tapis
                    );
            }

            // Ecriture du detail de la partie
            string listeCartes = "-";
            if (evt.CombinaisonGagnante != null)
            {
                listeCartes = string.Empty;
                foreach (CartePoker carte in evt.CombinaisonGagnante.MainGagnante)
                {
                    listeCartes += "," + carte.LibelleCarte();
                }
                listeCartes = evt.CombinaisonGagnante.TypeCombinaison.ToString() + ":" + listeCartes.Substring(1);
            }
            EcrireDetailPartie(evt.DateEvenement, evt.NumeroDonne, "FinDonne", evt.JoueursGagnants.ListeNoms(), listeCartes, evt.Pot.ToString());

            // Format de sortie
            //DateDonne;NumDonn;Joueur 1;Joueur 2;Joueur 3;Gain;Joueur a gagné;CombinaisonGagnante;Carte1;Carte2;FLOP1;FLOP2;FLOP3;Turn;River;Dealer;PB;GB;MontantPB;MontantGB

            // Ecriture du résumé de la partie 
            string ligneDonne = string.Format("{0};{1};", evt.DateEvenement, evt.NumeroDonne);
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                ligneDonne += j.Tapis.ToString() + ";";
            }
            string libCombGagnante = (evt.CombinaisonGagnante != null) ? evt.CombinaisonGagnante.TypeCombinaison.ToString() : "-";
            ligneDonne += string.Format("{0};{1};{2};{3};{4};", evt.Pot, evt.JoueursGagnants.ListeNoms(), libCombGagnante, evt.JoueursGagnants[0].Carte1.LibelleCarte(), evt.JoueursGagnants[0].Carte2.LibelleCarte());
            ligneDonne += string.Format("{0};{1};{2};{3};{4}", _flop[0].LibelleCarte(), _flop[1].LibelleCarte(), _flop[2].LibelleCarte(), _turn, _river);
            ligneDonne += string.Format("{0};{1};{2};{3};{4}", _dealer, _petiteBlind, _grosseBlind, _montantPetiteBlind, _montantGrosseBlind);

            _writerResumePartie.WriteLine(ligneDonne);           
        }

        protected override void EnregistrerNouvelleDonne(NouvelleDonne evt)
        {
            _dealer = evt.Dealer;
            _petiteBlind = evt.PetiteBlind;
            _grosseBlind = evt.GrosseBlind;
            _montantGrosseBlind = evt.InfosBlind.MontantGrosseBlind;
            _montantPetiteBlind = evt.InfosBlind.MontantPetiteBlind;
            _listeJoueursDebutDonne = evt.ListeJoueurs;
            _numDonne = evt.NumeroDonne;

            EcrireDetailPartie(
                evt.DateEvenement,
                _numDonne,
                "NouvelleDonne",
                string.Format("{0},{1},{2}", evt.Dealer, evt.PetiteBlind, evt.GrosseBlind),
                string.Format("{0},{1}", evt.InfosBlind.MontantPetiteBlind, evt.InfosBlind.MontantGrosseBlind),
                string.Format("{0},{1}", evt.InfosBlind.MontantProchainePetiteBlind, evt.InfosBlind.MontantProchaineGrosseBlind)
                );

            // Résumé partie
            _flop = new CartePoker[3];
            _turn = null;
            _river = null;
        }

        protected override void EnregistrerFlop(Flop evt)
        {
            _flop[0] = evt.Carte1;
            _flop[1] = evt.Carte2;
            _flop[2] = evt.Carte3;

            EcrireDetailPartie(evt.DateEvenement, _numDonne, "Flop", evt.Carte1.LibelleCarte() + "," + evt.Carte2.LibelleCarte() + "," + evt.Carte3.LibelleCarte(), evt.Pot.ToString(), PAS_D_INFOS);                        
        }

        protected override void EnregistrerRiver(River evt)
        {
            _river = evt.Carte;

            EcrireDetailPartie(evt.DateEvenement, _numDonne, "River", evt.Carte.LibelleCarte(), evt.Pot.ToString(), PAS_D_INFOS);            
        }

        protected override void EnregistrerTurn(Turn evt)
        {
            _turn = evt.Carte;

            EcrireDetailPartie(evt.DateEvenement, _numDonne, "Turn", evt.Carte.LibelleCarte(), evt.Pot.ToString(), PAS_D_INFOS);           
        }

        protected override void EnregistrerAugmentationBlinds(AugmentationBlinds evt)
        {
            EcrireDetailPartie(evt.DateEvenement, _numDonne, "AugmentationBlinds",
                evt.MontantPetiteBlind.ToString() + "," + evt.MontantGrosseBlind.ToString(),
                evt.MontantProchainePetiteBlind.ToString() + "," + evt.MontantProchaineGrosseBlind.ToString(),
                evt.DelaiAugmentationBlinds.ToString());
        }

        protected override void EnregistrerChat(Chat evt)
        {
            EcrireDetailPartie(evt.DateEvenement, _numDonne, "Chat", evt.Nom, evt.Message, PAS_D_INFOS);            
        }

        protected override void EnregistrerActionJoueur(ActionJoueurStat evt)
        {
            EcrireDetailPartie(evt.DateEvenement, _numDonne, evt.TypeAction.ToString(), evt.Nom, evt.Mise.ToString(), evt.Tapis.ToString());            
        }
        #endregion

        #region Methodes privées
        private string TranformerListeEnChaine(List<JoueurStat> listeJoueurs)
        {
            string res = string.Empty;
            foreach (JoueurStat j in listeJoueurs)
            {
                res += "," + j.Nom;
            }

            return res.Substring(1);
        }

        private void EcrireDetailPartie(DateTime dateEvt, int numDonne, string typeAction, string infos1, string infos2, string infos3)
        {
            _writerDetailPartie.WriteLine(
              "{0:dd/MM/yyyy HH:mm:ss};{1};{2};{3};{4};{5}",
              dateEvt,
              numDonne,
              typeAction, infos1, infos2, infos3);
        }
        #endregion
    }
}

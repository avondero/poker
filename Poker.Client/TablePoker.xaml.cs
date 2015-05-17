using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.ServiceModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using Poker.Interface.Metier;
using Poker.Interface.Communication;
using Poker.Interface.Bot;
using Poker.Client.Properties;
using Poker.Client.AffichageCartes;
using Poker.Interface.ExtensionsClient.Traduction;
using Poker.Interface.Outils;
using System.Net;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Poker.Interface.ExtensionsClient.Replay;
using System.Timers;
using Poker.Interface.Stats;

namespace Poker.Client
{
    /// <summary>
    /// Interaction logic for TablePoker.xaml
    /// </summary>

    public partial class TablePoker : System.Windows.Window
    {
        #region Membres privés
        private IServeur _serveur = null;
        private CommunicationServeur _comm = null;
        private Joueur _joueurConnecte = null;        
        private PokerPanel _pnlPoker;
        private bool _fermetureAuto = false;        
        private bool _partieEnCours = false;
        private Guid _identifiantConnection = Guid.Empty;
        private Dictionary<Joueur, Joueur> _listeJoueurs = new Dictionary<Joueur, Joueur>();
        private const string TIMER_ACTION_JOUEUR = "timerActionJoueur";
        private int _montantPetiteBlind = 0;
        private int _montantGrosseBlind = 0;
        private Storyboard _timerSB = null;
        private string _libelleRelancerMiser = null;
        private DispatcherTimer _timerBlinds = null;
        private TimeSpan _delaiAugmentationBlinds;
        private InfosAdmin _infosAdmin = null;
        private CartePoker[] _board = new CartePoker[5];
        private ModeClient _modeClient;
        private GestionReplay _replay = null;
        private bool _changementNumDonneParProgramme = true;
        private int _ancienNumeroDonne = -1;
        private ResultatConnection _resultatConnection = null;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Démarrage d'une table de poker pour le joueur en se connectant à un serveur
        /// </summary>
        /// <param name="joueur"></param>
        /// <param name="adresseServeur"></param>
        public TablePoker(ITraducteur traducteur)
            : this(null, null, null, null, traducteur, ModeClient.Jeu)
        { }

        /// <summary>
        /// Démarrage d'une table de poker pour le joueur en se connectant à un serveur
        /// </summary>
        /// <param name="joueur"></param>
        /// <param name="adresseServeur"></param>
        public TablePoker(Joueur joueurConnecte, CommunicationServeur comm, IServeur serveur, ResultatConnection resConnection, ITraducteur traducteur, ModeClient modeClient)
            : this()
        {
            _comm = comm;
            _serveur = serveur;
            _joueurConnecte = joueurConnecte;
            _joueurConnecte.AdministrateurServeur = resConnection.AdminServeur;
            _identifiantConnection = resConnection.IdentifiantConnexion;
            OutilsTraduction.Traducteur = traducteur;
            _modeClient = modeClient;
            _resultatConnection = resConnection;
            this.InitialisationsDiverses();
        }

        private TablePoker()
        {
            InitializeComponent();
            this.Pot = null;
            imgCarte1.Clip = GestionCartes.FormeCarte;
            imgCarte2.Clip = GestionCartes.FormeCarte;
            imgCarte3.Clip = GestionCartes.FormeCarte;
            imgCarte4.Clip = GestionCartes.FormeCarte;
            imgCarte5.Clip = GestionCartes.FormeCarte;
            _pnlPoker = new PokerPanel();

            _timerBlinds = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, TimerBlinds_Tick, Dispatcher.CurrentDispatcher);
            _timerBlinds.IsEnabled = false;
        }


        #endregion

        #region Methodes publiques
        /// <summary>
        /// Démarrage du client
        /// </summary>
        public void InitialisationsDiverses()
        {
            // Ajout graphique            
            grdTableJeu.Children.Add(_pnlPoker);
            _pnlPoker.CarteRegardee += new PokerPanel.CarteRegardeeEventHandler(_pnlPoker_CarteRegardee);

            if (_modeClient == ModeClient.Jeu)
            {                              
                // Gestion de l'onglet administration
                if (_joueurConnecte.AdministrateurServeur)
                {
                    // Récupération des informations d'administration
                    _infosAdmin = _serveur.RecupererInformationsAdministrations();
                    cboListeBots.ItemsSource = _infosAdmin.ListeBots;
                    cboListeBots.DisplayMemberPath = "Description";

                    tabAdmin.Visibility = Visibility.Visible;
                    tabAdmin.Focus();

                    // TODO : ne plus chercher les options par défaut mais celles en local
                    txtTapisInitial.Text = _infosAdmin.OptionsParDefaut.TapisInitial.ToString();
                    txtPetiteBlindInitiale.Text = _infosAdmin.OptionsParDefaut.MontantPetiteBlindInitial.ToString();
                    txtTimeOutAction.Text = _infosAdmin.OptionsParDefaut.TimeOutAction.ToString();
                    txtTimeOutFinDonneCartesCachees.Text = _infosAdmin.OptionsParDefaut.TimeOutFinDonneCartesCachees.ToString();
                    txtTimeOutFinDonneCartesMontrees.Text = _infosAdmin.OptionsParDefaut.TimeOutFinDonneCartesMontrees.ToString();
                    txtParametreAugmentationBlinds.Text = _infosAdmin.OptionsParDefaut.ParametreAugmentationBlinds.ToString();
                    lblLibelleParametre.Content = _infosAdmin.MethodeCalculAugmentationBlind.LibelleParametre;
                    lblMethodeCalcul.Content = _infosAdmin.MethodeCalculAugmentationBlind.LibelleMethode;

                    TraduireOngletAdministration();
                }
                else
                {
                    tabChatAdminOptions.Items.Remove(tabAdmin);
                    tabPartie.Focus();
                }
                
                if (_joueurConnecte.EstSpectateur)
                {
                   pnlAction.Visibility = Visibility.Collapsed;
                   pnlReplay.Visibility = Visibility.Collapsed;
                   tabChatAdminOptions.Items.Remove(tabOptions);
                   btnEnvoyer.IsEnabled = false;
                   txtChatMessage.IsEnabled = false;
                }
                else
                {
                    pnlReplay.Visibility = Visibility.Hidden;
                    pnlAction.Visibility = Visibility.Visible;                    
                }
                

                _pnlPoker.PositionJoueur = _resultatConnection.PositionJoueur;
                foreach (Joueur j in _resultatConnection.ListeJoueurs)
                {
                    _pnlPoker.AjouterJoueur(j, EtatMain.PasDeCartes);
                    _listeJoueurs[j] = j;
                }

                _comm.MessageInformation += new MessageInformationHandler(_comm_MessageInformation);
                _comm.MessagePublic += new MessagePublicHandler(_comm_MessagePublic);
                _comm.NouveauJoueur += new NouveauJoueurHandler(_comm_NouveauJoueur);
                _comm.ChangementInfosJoueur += new ChangementInfosJoueurHandler(_comm_ChangementInfosJoueur);
                _comm.Regard += new RegardHandler(_comm_Regard);
                _comm.ChangementInfosJoueurSansCartes += new ChangementInfosJoueurSansCartesHandler(_comm_ChangementInfosJoueurSansCartes);
                _comm.ActionsPossibles += new ActionsPossiblesHandler(_comm_ActionsPossibles);
                _comm.OptionsPartie += new OptionsPartieHandler(_comm_OptionsPartie);
            }
            else
            {
                // Mode replay
                pnlAction.Visibility = Visibility.Hidden;
                pnlReplay.Visibility = Visibility.Visible;
                cboLecturePartie.ItemsSource = RecupererListeLecteurs();
                cboLecturePartie.DisplayMemberPath = "Description";
                cboLecturePartie.SelectedIndex = 0;
                sldNumDonne.IsEnabled = false;
                btnPlay.IsEnabled = false;
                btnPause.IsEnabled = false;
                btnEnvoyer.IsEnabled = false;
                tabAdmin.Visibility = Visibility.Collapsed;

                TraduireReplay();

                _replay = new GestionReplay();
                _replay.DemarrageNouvellePartie += new DemarrageNouvellePartieHandler(_replay_DemarrageNouvellePartie);
                _replay.MessageInformation += new MessageInformationHandler(_comm_MessageInformation);
                _replay.MessagePublic += new MessagePublicHandler(_comm_MessagePublic);                
                _replay.ChangementInfosJoueur += new ChangementInfosJoueurHandler(_comm_ChangementInfosJoueur);                
                _replay.ChangementInfosJoueurSansCartes += new ChangementInfosJoueurSansCartesHandler(_comm_ChangementInfosJoueurSansCartes);
                _replay.ChangementDonne += new ChangementDonneHandler(_replay_ChangementDonne);

                
            }

            TraduireOngletPartie();
            TraduireOngletOptions();
            this.Title = OutilsTraduction.Traducteur.Traduire("TitreClient");
            InitialiserControlesDebutDePartie();            
        }               
        #endregion

        #region Methodes privées                
        private void InitialiserControlesDebutDePartie()
        {
            btnJeter.Content = OutilsTraduction.Traducteur.Traduire("Jeter");
            btnSuivre.Content = OutilsTraduction.Traducteur.Traduire("Parole");
            btnMiserOuRelancer.Content = OutilsTraduction.Traducteur.Traduire("Miser");
            btnMiserTapis.Content = OutilsTraduction.Traducteur.Traduire("Tapis");
            lblMontantBlind.Content = string.Empty;
            lblProchainesBlinds.Content = string.Empty;
            lblTempsBlinds.Content = string.Empty;
            lblMontantProchainesBlinds.Content = string.Empty;  
        }

        private List<ILecturePartie> RecupererListeLecteurs()
        {
            List<ILecturePartie> listeLecteurs = new List<ILecturePartie>();
            foreach (string cheminAssembly in Directory.GetFiles("stats", "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, cheminAssembly));

                foreach (Type t in asm.GetTypes())
                {
                    Type typeLecturePartie = t.GetInterface("ILecturePartie");
                    if (typeLecturePartie != null && !t.IsAbstract)
                    {
                        listeLecteurs.Add(Activator.CreateInstance(t) as ILecturePartie);
                    }
                }
            }

            return listeLecteurs;
        }

        private void AfficherMainGagnante(List<Joueur> listeJoueurs, Combinaison mainGagnante)
        {
            if (mainGagnante != null)
            {
                // Les joueurs 
                _pnlPoker.AfficherMainGagnante(listeJoueurs, mainGagnante);

                // Le board
                for (int i = 0; i < 5; i++)
                {
                    string style = null;
                    if (mainGagnante.MainGagnante.Contains(_board[i]))
                    {
                        style = ConstantesClient.STYLE_CARTE_ACTIVEE;
                    }
                    else
                    {
                        style = ConstantesClient.STYLE_CARTE_DESACTIVEE;
                    }
                    ((Image)grdBoard.Children[i]).Style = this.FindResource(style) as Style;
                }
            }
        }

        /// <summary>
        /// Active / désactive les options de jeu
        /// </summary>
        /// <param name="activation"></param>
        private void ActiverDesactiverOptions(bool activation)
        {
            txtTapisInitial.IsEnabled = activation;
            txtPetiteBlindInitiale.IsEnabled = activation;
            txtTimeOutAction.IsEnabled = activation;
            txtTimeOutFinDonneCartesMontrees.IsEnabled = activation;
            txtTimeOutFinDonneCartesCachees.IsEnabled = activation;
            grpAugmentationBlinds.IsEnabled = activation;
        }

        /// <summary>
        /// Recherche tous les joueurs éliminés selon leur position
        /// </summary>
        /// <param name="joueurs"></param>
        /// <param name="positionElimination"></param>
        /// <returns></returns>
        private List<Joueur> RechercheJoueursElimines(List<Joueur> joueurs, int positionElimination)
        {
            List<Joueur> res = new List<Joueur>();
            foreach (Joueur j in joueurs)
            {
                if (j.PositionElimination == positionElimination)
                    res.Add(j);
            }

            return res;
        }
    
        private void TraduireOngletOptions()
        {
            try
            {
                tabOptions.Header = OutilsTraduction.Traducteur.Traduire("Options");
                grpOptionsJeu.Header = OutilsTraduction.Traducteur.Traduire("Jeu");
                chkCacherCartes.Content = OutilsTraduction.Traducteur.Traduire("CacherLesCartes");
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors de la traduction de l'onglet options : " + ex.Message);
                throw new Exception("Erreur de traduction : TraduireOngletOptions", ex);
            }
        }

        private void TraduireOngletPartie()
        {
            try
            {
                tabPartie.Header = OutilsTraduction.Traducteur.Traduire("Partie");
                btnEnvoyer.Content = OutilsTraduction.Traducteur.Traduire("Envoyer");
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors de la traduction de l'onglet partie : " + ex.Message);
                throw new Exception("Erreur de traduction : TraduireOngletPartie", ex);
            }
        }

        private void TraduireOngletAdministration()
        {
            try
            {
                tabAdmin.Header = OutilsTraduction.Traducteur.Traduire("Administration");
                grpOptions.Header = OutilsTraduction.Traducteur.Traduire("OptionsPartie");
                lblTapisInitial.Content = OutilsTraduction.Traducteur.Traduire("TapisInitial");
                lblPetiteBlindInitiale.Content = OutilsTraduction.Traducteur.Traduire("PetiteBlindInitiale");
                grpAugmentationBlinds.Header = OutilsTraduction.Traducteur.Traduire("AugmentationBlinds");
                lblTimeOutAction.Content = OutilsTraduction.Traducteur.Traduire("TimeOutJoueur");
                lblTimeOutFinDonneCartesCachees.Content = OutilsTraduction.Traducteur.Traduire("TimeOutDonne");
                lblTimeOutFinDonneCartesMontrees.Content = OutilsTraduction.Traducteur.Traduire("TimeOutDonne");
                grpBots.Header = OutilsTraduction.Traducteur.Traduire("GestionBots");
                btnAJouterBot.Content = OutilsTraduction.Traducteur.Traduire("AjoutBot");
                grpListeJoueurs.Header = OutilsTraduction.Traducteur.Traduire("ListeConnectes");
                btnDemarrer.Content = OutilsTraduction.Traducteur.Traduire("DemarrerPartie");
                btnListeIp.Content = OutilsTraduction.Traducteur.Traduire("BoutonListeIPServeur");
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors de la traduction de l'onglet Administration : " + ex.Message);
                throw new Exception("Erreur de traduction : TraduireOngletAdministration", ex);
            }
        }

        private void TraduireReplay()
        {
            lblNumDonneReplay.Content = OutilsTraduction.Traducteur.Traduire("NumeroDonneReplay");
            lblMethodeLecture.Content = OutilsTraduction.Traducteur.Traduire("MethodeLecture");
            btnChoisirPartie.Content = OutilsTraduction.Traducteur.Traduire("ChoisirPartie");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enleverEvtCompleted">Si vrai, supprimer l'event Completed de l'animation</param>
        private void DesactiverActions(bool enleverEvtCompleted)
        {
            if (enleverEvtCompleted) _timerSB.Completed -= new EventHandler(_timerSB_Completed);
            _timerSB.Stop(timer);
            timer.Value = timer.Maximum;
            txtMise.Text = "0";
            btnJeter.Content = OutilsTraduction.Traducteur.Traduire("Jeter");
            btnSuivre.Content = OutilsTraduction.Traducteur.Traduire("Parole");
            btnMiserOuRelancer.Content = OutilsTraduction.Traducteur.Traduire("Miser");
            btnMiserTapis.Content = OutilsTraduction.Traducteur.Traduire("Tapis");
            pnlAction.IsEnabled = false;
        }

        /// <summary>
        /// Modifie / Affiche une des cartes du milieu
        /// </summary>
        /// <param name="indexCarte">l'index : doit etre entre 0 & 4</param>
        /// <param name="carte">La carte</param>
        private void ChangerCarteMilieu(int indexCarte, CartePoker carte)
        {
            if (indexCarte < 0 || indexCarte > 5)
                throw new ArgumentException("L'index de la carte doit etre compris entre 0 et 4");

            BitmapSource bi = GestionCartes.RecupererImageSelonCarte(carte);

            ((Image)grdBoard.Children[indexCarte]).Source = bi;

            _board[indexCarte] = carte;
        }

        /// <summary>
        /// Le pot
        /// </summary>
        private int? Pot
        {
            set
            {
                if (value == null)
                    lblPot.Content = string.Empty;
                else
                    lblPot.Content = string.Format(OutilsTraduction.Traducteur.Traduire("Pot"), value);
            }
        }
        #endregion

        #region Traitements des evenements de la fenetre
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            _replay.ReprendreLecturePartie();
            btnPause.Visibility = Visibility.Visible;
            btnPlay.Visibility = Visibility.Hidden;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            _replay.PauserLecturePartie();
            btnPause.Visibility = Visibility.Hidden;
            btnPlay.Visibility = Visibility.Visible;
        }
 

        private void btnChoisirPartie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ILecturePartie lecturePartie = cboLecturePartie.SelectedItem as ILecturePartie;
                SelectionParties selParties = new SelectionParties(lecturePartie);
                if (selParties.ShowDialog().Value)
                {
                    _replay.DemarrerLecturePartie(lecturePartie, selParties.PartieChoisie);
                    _changementNumDonneParProgramme = true;
                    sldNumDonne.Maximum = lecturePartie.NombreDeDonnes();
                    _changementNumDonneParProgramme = true;
                    sldNumDonne.Value = 0;
                    sldNumDonne.IsEnabled = true;
                    btnPlay.IsEnabled = true;
                    btnPause.IsEnabled = true;
                    btnPlay.Visibility = Visibility.Visible;
                    btnPause.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors du choix d'une partie : " + ex.Message);
                MessageBox.Show(OutilsTraduction.Traducteur.Traduire("ErreurChoixPartie") + ex.Message, Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimerBlinds_Tick(object sender, EventArgs e)
        {
            if (_delaiAugmentationBlinds.Minutes != 0 || _delaiAugmentationBlinds.Seconds != 0)
            {
                _delaiAugmentationBlinds = _delaiAugmentationBlinds.Subtract(new TimeSpan(0, 0, 1));
                lblTempsBlinds.Content = string.Format(OutilsTraduction.Traducteur.Traduire("DelaiProchaineBlind"), _delaiAugmentationBlinds.Minutes, _delaiAugmentationBlinds.Seconds);
            }
        }

        /// <summary>
        /// Le joueur courant doit cliquer ou non pour regarder ses cartes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCacherCartes_Click(object sender, RoutedEventArgs e)
        {
            if (_pnlPoker != null)
            {
                _pnlPoker.CacherLesCartes = chkCacherCartes.IsChecked.Value;
            }
        }

        private void btnMiserTapis_Click(object sender, RoutedEventArgs e)
        {
            sldMise.Value = sldMise.Maximum;
        }

        void _timerSB_Completed(object sender, EventArgs e)
        {
            DesactiverActions(false);
        }

        private void sldNumDonne_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int nouvelleValeur = (int) Math.Floor(sldNumDonne.Value);
            sldNumDonne.Value = nouvelleValeur;
            if (!_changementNumDonneParProgramme && nouvelleValeur != _ancienNumeroDonne)
            {
                try
                {
                    _replay.AllerDonne((int)sldNumDonne.Value);
                }
                catch (Exception ex)
                {
                    logClient.Debug("******** Erreur lors du changement de donne : " + ex.Message);
                    MessageBox.Show(OutilsTraduction.Traducteur.Traduire("ErreurChangementDonne") + ex.Message, Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            _changementNumDonneParProgramme = false;
            _ancienNumeroDonne = nouvelleValeur;
        }

        private void sldMise_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // On arrondit la valeur du slider à un nombre de fois le montant de la petite blind sauf si on est au tapis
            if (sldMise.Value < sldMise.Maximum)
            {
                if (_libelleRelancerMiser != null) btnMiserOuRelancer.Content = _libelleRelancerMiser;
                sldMise.Value = _montantPetiteBlind * Math.Floor(sldMise.Value / _montantPetiteBlind);
            }
            else
            {
                _libelleRelancerMiser = btnMiserOuRelancer.Content.ToString();
                btnMiserOuRelancer.Content = OutilsTraduction.Traducteur.Traduire("Tapis");
            }
        }

        private void btnJeter_Click(object sender, RoutedEventArgs e)
        {
            if ((btnSuivre.Tag as ActionJoueur).TypeAction == TypeActionJoueur.Parole && btnSuivre.IsEnabled)
            {
                MessageBoxResult reponseUtilisateur = MessageBox.Show(OutilsTraduction.Traducteur.Traduire("JeterOuParole"), Constantes.NOM_APPLICATION, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (reponseUtilisateur == MessageBoxResult.Yes)
                {
                    DesactiverActions(true);
                    _serveur.EnvoyerAction(_identifiantConnection, new ActionJoueur(TypeActionJoueur.Passe));
                }
                else if (reponseUtilisateur == MessageBoxResult.No)
                {
                    DesactiverActions(true);
                    _serveur.EnvoyerAction(_identifiantConnection, new ActionJoueur(TypeActionJoueur.Parole));                
                }
            }
            else
            {
                DesactiverActions(true);
                _serveur.EnvoyerAction(_identifiantConnection, new ActionJoueur(TypeActionJoueur.Parole));
            }
        }

        private void btnSuivre_Click(object sender, RoutedEventArgs e)
        {
            DesactiverActions(true);
            _serveur.EnvoyerAction(_identifiantConnection, btnSuivre.Tag as ActionJoueur);
        }

        private void btnMiserOuRelancer_Click(object sender, RoutedEventArgs e)
        {
            int montantMise = int.Parse(txtMise.Text);
            if (montantMise == sldMise.Maximum)
            {
                DesactiverActions(true);
                ActionJoueur action = new ActionJoueur(TypeActionJoueur.Tapis, montantMise);
                _serveur.EnvoyerAction(_identifiantConnection, action);
            }
            else if (montantMise > sldMise.Maximum || montantMise < sldMise.Minimum)
            {
                string msg = string.Format(OutilsTraduction.Traducteur.Traduire("MontantRelance"), sldMise.Minimum, sldMise.Maximum);
                MessageBox.Show(msg, Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                DesactiverActions(true);

                ActionJoueur action = btnMiserOuRelancer.Tag as ActionJoueur;
                if (action.TypeAction == TypeActionJoueur.Relance || action.TypeAction == TypeActionJoueur.Mise)
                {
                    action.Montant = montantMise;
                    _serveur.EnvoyerAction(_identifiantConnection, action);
                }
                else
                {
                    // Tapis
                    _serveur.EnvoyerAction(_identifiantConnection, action);
                }
            }
        }

        private void btnListeIp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string listeIPs = OutilsTraduction.Traducteur.Traduire("ListeIPServeur");
            foreach (IPAddress adr in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                // Pour l'instant on n'affiche pas les adresses en IPv6
                if (adr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    listeIPs += string.Format("\n\r * {0}", adr);
                }
            }
            MessageBox.Show(listeIPs, Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void VerificationSaisieChiffre(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;

            String newText = String.Empty;
            foreach (Char c in textBox.Text.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c)) newText += c;
            }

            textBox.Text = newText;

            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }


        private void AjouterBot(object sender, System.Windows.RoutedEventArgs e)
        {
            _serveur.AjouterBot(_identifiantConnection, (cboListeBots.SelectedItem as DescriptionBot).TypeBot, txtNomBot.Text);
            txtNomBot.Text = string.Empty;
        }

        private void ChoixBot(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            txtNomBot.IsEnabled = true;
        }

        private void SaisieNomBot(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            btnAJouterBot.IsEnabled = (txtNomBot.Text.Length > 0);
        }

        private void TablePoker_Closing(object sender, CancelEventArgs e)
        {
            if (!_fermetureAuto && _partieEnCours)
            {
                // TODO : si on est serveur couper proprement les clients
                if (_joueurConnecte.AdministrateurServeur)
                    e.Cancel = MessageBox.Show(OutilsTraduction.Traducteur.Traduire("PartieEnCours"), Constantes.NOM_APPLICATION, MessageBoxButton.YesNo) == MessageBoxResult.No;
            }
        }
        
        private void TablePoker_Closed(object sender, EventArgs e)
        {
            try
            {
                if (_modeClient == ModeClient.Jeu)
                {
                    _serveur.Deconnecter(_identifiantConnection);
                }
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur lors de la deconnection : " + ex.Message);
            }
        }

        private void EnvoyerChatMessage(object sender, RoutedEventArgs e)
        {
            _serveur.EnvoyerMessagePublic(new ChatMessage(txtChatMessage.Text), _identifiantConnection);
            txtChatMessage.Text = string.Empty;
        }

        /// <summary>
        /// Regard d'une carte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="debutRegard"></param>
        void _pnlPoker_CarteRegardee(Joueur sender, bool debutRegard)
        {
            _serveur.EnvoyerRegard(_identifiantConnection, debutRegard);
        }

        void DemarrerPartie(object sender, RoutedEventArgs e)
        {
            Options optionsJeu = new Options();
            optionsJeu.TapisInitial = int.Parse(txtTapisInitial.Text);
            optionsJeu.ParametreAugmentationBlinds = int.Parse(txtParametreAugmentationBlinds.Text);
            optionsJeu.TimeOutAction = int.Parse(txtTimeOutAction.Text);
            optionsJeu.TimeOutFinDonneCartesCachees = int.Parse(txtTimeOutFinDonneCartesCachees.Text);
            optionsJeu.TimeOutFinDonneCartesMontrees = int.Parse(txtTimeOutFinDonneCartesMontrees.Text);
            optionsJeu.MontantPetiteBlindInitial = int.Parse(txtPetiteBlindInitiale.Text);
            ActiverDesactiverOptions(false);

            _serveur.DemarrerPartie(_identifiantConnection, optionsJeu);
            tabPartie.Focus();
            grpBots.IsEnabled = false;
        }
        #endregion

        #region Traitement des evenements serveur

        private void _comm_ActionsPossibles(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout)
        {
            this.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        new ActionsPossiblesHandler(_comm_ActionsPossibles_Dispatcher),
                        actions,
                        timeout
                        );
        }

        private void _comm_ActionsPossibles_Dispatcher(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout)
        {
            GestionFenetre.ClignoterFenetre(3);
            pnlAction.IsEnabled = true;
            timer.Value = 0;
            btnMiserTapis.IsEnabled = true;
            btnMiserOuRelancer.IsEnabled = true;
            btnSuivre.IsEnabled = true;

            // Jeter
            btnJeter.Content = OutilsTraduction.Traducteur.Traduire("Jeter");

            // Parole ou suivre ?         
            if (actions.ContainsKey(TypeActionJoueur.Suit))
            {
                btnSuivre.Content = string.Format(OutilsTraduction.Traducteur.Traduire("Suivre"), actions[TypeActionJoueur.Suit].MontantMax - actions[TypeActionJoueur.Suit].MontantMin);
                btnSuivre.Tag = new ActionJoueur(TypeActionJoueur.Suit, actions[TypeActionJoueur.Suit].MontantMax);
            }
            else if (actions.ContainsKey(TypeActionJoueur.Parole))
            {
                btnSuivre.Content = OutilsTraduction.Traducteur.Traduire("Parole");
                btnSuivre.Tag = new ActionJoueur(TypeActionJoueur.Parole);
            }
            else
            {
                btnSuivre.Content = OutilsTraduction.Traducteur.Traduire("Parole");
                btnSuivre.IsEnabled = false;
            }

            // Relancer ou Tapis ?
            TypeActionJoueur actRelanceOuMise = TypeActionJoueur.Aucune;
            if (actions.ContainsKey(TypeActionJoueur.Mise)) actRelanceOuMise = TypeActionJoueur.Mise;
            if (actions.ContainsKey(TypeActionJoueur.Relance)) actRelanceOuMise = TypeActionJoueur.Relance;
            if (actRelanceOuMise != TypeActionJoueur.Aucune)
            {
                sldMise.Minimum = actions[actRelanceOuMise].MontantMin;
                sldMise.Maximum = actions[actRelanceOuMise].MontantMax;
                sldMise.Value = sldMise.Minimum;
                sldMise.SmallChange = _montantPetiteBlind;
                sldMise.LargeChange = _montantGrosseBlind;
                sldMise.TickFrequency = (sldMise.Maximum - sldMise.Minimum) / 10;
                if (actRelanceOuMise == TypeActionJoueur.Relance)
                {
                    btnMiserOuRelancer.Content = OutilsTraduction.Traducteur.Traduire("Relancer");
                    btnMiserOuRelancer.Tag = new ActionJoueur(TypeActionJoueur.Relance);
                }
                else
                {
                    btnMiserOuRelancer.Content = OutilsTraduction.Traducteur.Traduire("Miser");
                    btnMiserOuRelancer.Tag = new ActionJoueur(TypeActionJoueur.Mise);
                }
                _libelleRelancerMiser = btnMiserOuRelancer.Content.ToString();
                txtMise.Text = sldMise.Minimum.ToString();
            }
            else
            {
                sldMise.Minimum = actions[TypeActionJoueur.Tapis].MontantMax;
                sldMise.Maximum = actions[TypeActionJoueur.Tapis].MontantMax;
                btnMiserOuRelancer.Content = OutilsTraduction.Traducteur.Traduire("Tapis");
                btnMiserOuRelancer.Tag = new ActionJoueur(TypeActionJoueur.Tapis, actions[TypeActionJoueur.Tapis].MontantMax);
                btnMiserTapis.IsEnabled = false;
                txtMise.Text = sldMise.Minimum.ToString();
            }

            if (timeout != 0)
            {
                _timerSB = this.FindResource(TIMER_ACTION_JOUEUR) as Storyboard;
                _timerSB.Completed += new EventHandler(_timerSB_Completed);
                _timerSB.Children[0].Duration = new Duration(new TimeSpan(0, 0, timeout));
                _timerSB.Begin(timer, true);

            }
        }

        private void _comm_ChangementInfosJoueurSansCartes(Joueur joueurAChanger, int? pot)
        {
            this.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        new ChangementInfosJoueurSansCartesHandler(_comm_ChangementInfosJoueurSansCartesDispatcher),
                        joueurAChanger,                        
                        pot
                        );
        }

        private void _comm_ChangementInfosJoueurSansCartesDispatcher(Joueur joueurAChanger, int? pot)
        {
            this.Pot = pot;
            _listeJoueurs[joueurAChanger] = joueurAChanger;
            _pnlPoker.ModifierJoueur(joueurAChanger);
        }

        void _comm_ChangementInfosJoueur(Joueur joueurAChanger, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot)
        {
            this.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new ChangementInfosJoueurHandler(_comm_ChangementInfosJoueurDispatcher),
                    joueurAChanger,
                    etatDeLaMain,
                    carte1,
                    carte2,
                    pot
                    );
        }

        void _comm_ChangementInfosJoueurDispatcher(Joueur joueurAChanger, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot)
        {
            this.Pot = pot;
            _listeJoueurs[joueurAChanger] = joueurAChanger;
            if (carte1 != null && carte2 != null)
            {
                _listeJoueurs[joueurAChanger].Carte1 = carte1;
                _listeJoueurs[joueurAChanger].Carte2 = carte2;
            }
            _pnlPoker.ModifierCartesJoueur(joueurAChanger, etatDeLaMain);
        }

        private void _comm_MessagePublic(ChatMessage msg, string expediteur)
        {
            this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new MessagePublicHandler(_comm_MessagePublicDispatcher), 
                msg, 
                expediteur
                );                
        }

        private void _comm_MessagePublicDispatcher(ChatMessage msg, string expediteur)
        {
            rtbInfos.SelectAll();
            Run r = null;
            try
            {
                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("JoueurDit"), expediteur) + msg.Contenu + "\r", rtbInfos.Selection.End);
                r.FontWeight = FontWeights.Bold;
                r.Foreground = Brushes.Blue;
            }
            catch (Exception ex)
            {
                logClient.Debug("Erreur lors de la traduction d'un message public : " + ex.Message);
                throw new Exception("Erreur de traduction : _comm_MessagePublic", ex);
            }
            rtbInfos.ScrollToEnd();
        }


        private void _comm_MessageInformation(MessageInfo message)
        {
            this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new MessageInformationHandler(_comm_MessageInformationDispatcher),
                message);
        }

        private void _comm_MessageInformationDispatcher(MessageInfo message)
        {         
            rtbInfos.SelectAll();
            Run r;
            switch (message.TypeMessage)
            {
                case MessageJeu.NouvellePartie:
                    rtbInfos.Document.Blocks.Clear();
                    r = new Run(OutilsTraduction.Traducteur.Traduire("DemarragePartie") + "\r", rtbInfos.Selection.End);
                    r.Foreground = Brushes.Green;
                    r.FontWeight = FontWeights.Bold;                    
                    _pnlPoker.CacherLesCartes = chkCacherCartes.IsChecked.Value;
                    _partieEnCours = true;
                    InitialiserControlesDebutDePartie();
                    break;

                case MessageJeu.NouvelleDonne:
                    _montantPetiteBlind = message.InfosBlind.MontantPetiteBlind;
                    _montantGrosseBlind = message.InfosBlind.MontantGrosseBlind;
                    lblMontantBlind.Content = string.Format(OutilsTraduction.Traducteur.Traduire("MontantBlind"), _montantPetiteBlind, _montantGrosseBlind);
                    lblProchainesBlinds.Content = OutilsTraduction.Traducteur.Traduire("ProchainesBlinds");
                    lblMontantProchainesBlinds.Content = string.Format(OutilsTraduction.Traducteur.Traduire("MontantProchaineBlind"), message.InfosBlind.MontantProchainePetiteBlind, message.InfosBlind.MontantProchaineGrosseBlind);
                    _pnlPoker.SupprimerMainGagnante();
                    for (int i = 0; i < 5; i++)
                    {
                        ((Image)grdBoard.Children[i]).Style = this.FindResource(ConstantesClient.STYLE_AUCUN) as Style;
                        this.ChangerCarteMilieu(i, null);
                    }

                    r = new Run(OutilsTraduction.Traducteur.Traduire("NouvelleDonne") + "\r", rtbInfos.Selection.End);
                    r.Foreground = Brushes.Green;
                    break;

                case MessageJeu.AugmentationBlinds:
                    r = new Run(OutilsTraduction.Traducteur.Traduire("InfosAugmentationBlinds") + "\r", rtbInfos.Selection.End);
                    r.Foreground = Brushes.Blue;
                    lblProchainesBlinds.Content = OutilsTraduction.Traducteur.Traduire("ProchainesBlinds");
                    lblMontantProchainesBlinds.Content = string.Format(OutilsTraduction.Traducteur.Traduire("MontantProchaineBlind"), message.InfosBlind.MontantProchainePetiteBlind, message.InfosBlind.MontantProchaineGrosseBlind);
                    _delaiAugmentationBlinds = message.InfosBlind.DelaiAugmentationBlinds;
                    _timerBlinds.Stop();
                    _timerBlinds.Start();
                    break;

                case MessageJeu.Distribution:
                    for (int i = 0; i < 5; i++)
                        this.ChangerCarteMilieu(i, message.Board[i]);

                    r = new Run(OutilsTraduction.Traducteur.Traduire(message.EtapeDistribution.ToString()) + " : ", rtbInfos.Selection.End);
                    r.Foreground = Brushes.Green;
                    // OutilsTraduction.Traducteur.LireMessage(message.EtapeDistribution.ToString());
                    switch (message.EtapeDistribution)
                    {
                        case EtapeDonne.Flop:
                            // OutilsTraduction.Traducteur.LireCarte(message.Board[0]);
                            GestionCartes.AfficherLibelleCourtCarte(message.Board[0], rtbInfos.Selection.End);
                            r = new Run(", ", rtbInfos.Selection.End);
                            // OutilsTraduction.Traducteur.LireCarte(message.Board[1]);
                            GestionCartes.AfficherLibelleCourtCarte(message.Board[1], rtbInfos.Selection.End);
                            r = new Run(", ", rtbInfos.Selection.End);
                            // OutilsTraduction.Traducteur.LireCarte(message.Board[2]);
                            GestionCartes.AfficherLibelleCourtCarte(message.Board[2], rtbInfos.Selection.End);
                            break;

                        case EtapeDonne.Turn:
                            //OutilsTraduction.Traducteur.LireCarte(message.Board[3]);
                            GestionCartes.AfficherLibelleCourtCarte(message.Board[3], rtbInfos.Selection.End);
                            break;

                        case EtapeDonne.River:
                            //OutilsTraduction.Traducteur.LireCarte(message.Board[4]);
                            GestionCartes.AfficherLibelleCourtCarte(message.Board[4], rtbInfos.Selection.End);
                            break;
                    }
                    r = new Run("\r", rtbInfos.Selection.End);
                    break;

                case MessageJeu.PotEmpoche:
                    try
                    {
                        // On n'affiche les cartes dans le tableau des infos uniquement si on a montré ses cartes
                        if (message.CombinaisonGagnante != null)
                        {
                            foreach (Joueur j in _listeJoueurs.Values)
                            {
                                if (j.Carte1 != null && j.Carte2 != null && !j.JeterCartes)
                                {
                                    r = new Run(string.Format("{0} : ", j.Nom), rtbInfos.Selection.End);
                                    r.Foreground = Brushes.Green;
                                    GestionCartes.AfficherLibelleCourtCarte(j.Carte1, rtbInfos.Selection.End);
                                    r = new Run(", ", rtbInfos.Selection.End);
                                    GestionCartes.AfficherLibelleCourtCarte(j.Carte2, rtbInfos.Selection.End);
                                    r = new Run("\n", rtbInfos.Selection.End);
                                    j.Carte1 = null;
                                    j.Carte2 = null;
                                }
                            }
                        }

                        if (message.JoueursEmpochantPot.Count == 1)
                        {
                            if (message.CombinaisonGagnante == null)
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("InfosJoueurEmpochePot") + "\r", message.JoueursEmpochantPot[0].Nom, message.MontantPotEmpoche), rtbInfos.Selection.End);
                            else
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("InfosJoueurEmpochePotAvecCombinaison") + "\r", message.JoueursEmpochantPot[0].Nom, message.MontantPotEmpoche, OutilsTraduction.Traducteur.TraduireCombinaison(message.CombinaisonGagnante)), rtbInfos.Selection.End);
                        }
                        else
                        {
                            string listeJoueurs = string.Empty;
                            foreach (Joueur j in message.JoueursEmpochantPot)
                                listeJoueurs += ", " + j.Nom;

                            r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("InfosJoueursEmpochentPotAvecCombinaison") + "\r", listeJoueurs.Substring(2), message.MontantPotEmpoche, OutilsTraduction.Traducteur.TraduireCombinaison(message.CombinaisonGagnante)), rtbInfos.Selection.End);
                        }
                        this.AfficherMainGagnante(message.JoueursEmpochantPot, message.CombinaisonGagnante);
                        r.Foreground = Brushes.Red;
                        r.FontWeight = FontWeights.Bold;
                    }
                    catch (Exception ex)
                    {
                        logClient.Debug("Erreur lors de la traduction d'un empochage de pot : " + ex.Message);
                        throw new Exception("Erreur de traduction : _comm_MessageInformation", ex);
                    }
                    break;

                case MessageJeu.FinPartie:
                    _partieEnCours = false;
                    int posElimination = 1;
                    int classement;
                    List<Joueur> joueursElimines = RechercheJoueursElimines(message.Classement, posElimination);
                    int nbJoueursElimines = joueursElimines.Count;
                    while (joueursElimines.Count > 0)
                    {
                        classement = message.Classement.Count - nbJoueursElimines + 1;
                        foreach (Joueur j in joueursElimines)
                        {
                            try
                            {
                                if (classement != 1)
                                {
                                    r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("Classement" + classement.ToString()) + "\r", j.Nom), rtbInfos.Selection.End);
                                    r.Foreground = Brushes.Green;
                                }
                                else
                                {
                                    r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("Classement" + classement.ToString()) + "\r", j.Nom), rtbInfos.Selection.End);
                                    r.FontWeight = FontWeights.Bold;
                                    r.Foreground = Brushes.Red;
                                }
                            }
                            catch (Exception ex)
                            {
                                logClient.Debug("Erreur lors de la traduction d'un message de fin de partie : " + ex.Message);
                                throw new Exception("Erreur de traduction : _comm_MessageInformation", ex);
                            }
                        }
                        posElimination += joueursElimines.Count;
                        joueursElimines = RechercheJoueursElimines(message.Classement, posElimination);
                        nbJoueursElimines += joueursElimines.Count;
                    }
                    ActiverDesactiverOptions(true);
                    _timerBlinds.IsEnabled = false;
                    break;

                case MessageJeu.ActionJoueur:
                    try
                    {
                        switch (message.Action.TypeAction)
                        {
                            case TypeActionJoueur.PetiteBlind:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("PayePetiteBlind") + "\r", message.Expediteur.Nom, message.Action.Montant), rtbInfos.Selection.End);
                                break;

                            case TypeActionJoueur.GrosseBlind:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("PayeGrosseBlind") + "\r", message.Expediteur.Nom, message.Action.Montant), rtbInfos.Selection.End);
                                break;

                            case TypeActionJoueur.Suit:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("ActionSuit") + "\r", message.Expediteur.Nom), rtbInfos.Selection.End);
                                break;

                            case TypeActionJoueur.Parole:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("ActionParole") + "\r", message.Expediteur.Nom), rtbInfos.Selection.End);
                                break;

                            case TypeActionJoueur.Mise:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("ActionMise") + "\r", message.Expediteur.Nom, message.Action.Montant), rtbInfos.Selection.End);
                                break;

                            case TypeActionJoueur.Relance:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("ActionRelance") + "\r", message.Expediteur.Nom, message.Action.Montant), rtbInfos.Selection.End);
                                break;

                            case TypeActionJoueur.Passe:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("ActionJette") + "\r", message.Expediteur.Nom), rtbInfos.Selection.End);
                                _listeJoueurs[message.Expediteur].JeterCartes = true;
                                break;

                            case TypeActionJoueur.Tapis:
                                r = new Run(string.Format(OutilsTraduction.Traducteur.Traduire("ActionTapis") + "\r", message.Expediteur.Nom, message.Expediteur.Mise), rtbInfos.Selection.End);
                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        logClient.Debug("Erreur lors de la traduction d'un message d'action : " + ex.Message);
                        throw new Exception("Erreur de traduction : _comm_MessageInformation", ex);
                    }
                    break;
            }
            rtbInfos.ScrollToEnd();

        }

        private delegate void testNJ(Joueur nouveauJoueur, int positionTable);

        private void _comm_NouveauJoueur(Joueur nouveauJoueur, int positionTable)
        {
             this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new testNJ(_comm_NouveauJoueur_Dispatcher), nouveauJoueur, positionTable);
            
            
        }

        private void _comm_NouveauJoueur_Dispatcher(Joueur nouveauJoueur, int positionTable)
        {
            _listeJoueurs[nouveauJoueur] = nouveauJoueur;

            // Il faut eventuellement decaler les anciennes positiosn
            if (positionTable <= _pnlPoker.PositionJoueur)
            {
                _pnlPoker.PositionJoueur++;
            }
            _pnlPoker.AjouterJoueur(nouveauJoueur, EtatMain.PasDeCartes, positionTable);
            if (_joueurConnecte.AdministrateurServeur)
                btnDemarrer.IsEnabled = true;
        }

        

        void _comm_Regard(Joueur expediteur, EtatMain etatDeLaMain)
        {
            _pnlPoker.ModifierCartesJoueur(expediteur, etatDeLaMain);
        }

        void _comm_OptionsPartie(Options optionsPartie)
        {

        }

        void _replay_DemarrageNouvellePartie(List<Joueur> listeJoueurs)
        {
            _pnlPoker.ReinitialiserPanel();
            foreach (Joueur j in listeJoueurs)
            {
                _pnlPoker.AjouterJoueur(j, EtatMain.PasDeCartes);
            }
        }

        void _replay_ChangementDonne(object sender, ChangementDonneEventArgs e)
        {
            this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(delegate()
            {
                _changementNumDonneParProgramme = true;
                sldNumDonne.Value = e.NumeroDonne;
            })
            );
        }
        #endregion                

    }
}
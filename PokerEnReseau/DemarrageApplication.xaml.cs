using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Poker.Interface.ExtensionsClient.Traduction;
using PokerEnReseau.Properties;
using System.Reflection;
using System.ComponentModel;

namespace PokerEnReseau
{
	public partial class DemarrageApplication
	{
        private bool _initialisationComboLangue = false;
        
        #region Evenements
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void rbRejoindre_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (cboAdresse != null)
            {
                cboAdresse.IsEnabled = rbRejoindre.IsChecked.Value;
                chkSpectateur.IsEnabled = rbRejoindre.IsChecked.Value;
            }
            ActiverDesactiverBoutons();
        }

        private void rbHeberger_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (cboAdresse != null)
            {
                cboAdresse.IsEnabled = !rbHeberger.IsChecked.Value;
                chkSpectateur.IsEnabled = !rbHeberger.IsChecked.Value;
            }
            ActiverDesactiverBoutons();
        }

        private void txtNomJoueur_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActiverDesactiverBoutons();            
        }

        private void cboLangue_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_initialisationComboLangue)
                TraduireEcran(cboLangue.SelectedItem as ITraducteur);
        }

        private void btnDemarrer_Click(object sender, RoutedEventArgs e)
        {            
            this.DialogResult = true;         
        }

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            this.EstClientEnModeLecture = true;
            this.DialogResult = true;               
        }

        private void cboAdresse_OnTextChanged(object sender, EventArgs args)
        {
            ActiverDesactiverBoutons();
        }
        #endregion

        #region Methodes privées
        private void ActiverDesactiverBoutons()
        {
            btnDemarrer.IsEnabled = txtNomJoueur.Text.Length > 0 && 
                (
                 (rbRejoindre != null && rbRejoindre.IsChecked.Value && cboAdresse != null && cboAdresse.Text.Length > 0) ||
                 (rbHeberger != null && rbHeberger.IsChecked.Value)
                );
        }

        private void TraduireEcran(ITraducteur trad)
        {
            this.Title = trad.Traduire("ConnexionTitre");
            lblNomJoueur.Content = trad.Traduire("NomJoueur");
            lblLangue.Content = trad.Traduire("Langue");
            rbHeberger.Content = trad.Traduire("Heberger");
            rbRejoindre.Content = trad.Traduire("Rejoindre");
            grpAcces.Header = trad.Traduire("Acces");
            btnDemarrer.Content = trad.Traduire("Demarrer");
            btnQuitter.Content = trad.Traduire("Quitter");
            btnReplay.Content = trad.Traduire("Replay");
            chkSpectateur.Content = trad.Traduire("EnSpectateur");
        }

        #endregion

        #region Proprietes publiques
        /// <summary>
        /// Renvoie le traducteur choisi
        /// </summary>
        public ITraducteur TraducteurChoisi
        {
            get
            {
                return cboLangue.SelectedItem as ITraducteur;
            }
        }
        /// <summary>
        /// Hébergement du serveur
        /// </summary>
        public bool EstServeur
        {
            get
            {
                if (rbHeberger.IsChecked == null)
                    return false;
                else
                    return (bool)rbHeberger.IsChecked;
            }
            set
            {
                rbHeberger.IsChecked = value;
                rbRejoindre.IsChecked = !value;
            }
        }

        /// <summary>
        /// Le client est-il en mode lecture de partie
        /// </summary>
        public bool EstClientEnModeLecture { get; set; }

        /// <summary>
        /// Nom du joueur
        /// </summary>
        public string NomJoueur
        {
            get
            {
                return txtNomJoueur.Text;
            }
            set
            {
                txtNomJoueur.Text = value;
            }
        }

        /// <summary>
        /// Renvoie l'adresse saisi du serveur
        /// </summary>
        public string AdresseServeur
        {
            get
            {
                if (EstServeur || cboAdresse.Text.Length == 0)
                    return "localhost";
                else
                    return cboAdresse.Text;
            }
            set
            {
                cboAdresse.Text = value;
            }
        }

        /// <summary>
        /// Langue choisie
        /// </summary>
        public string Langue
        {
            get
            {
                return cboLangue.Text;
            }
            set
            {
                cboLangue.Text = value;
            }
        }

        /// <summary>
        /// Le joueur se connecte t il en spectateur ?
        /// </summary>
        public bool EstSpectateur
        {
            get
            {
                return chkSpectateur.IsChecked.Value;
            }
            set
            {
                chkSpectateur.IsChecked = value;
            }
        }
        #endregion

		public DemarrageApplication(InfosEcranConnexion infos)
		{
			this.InitializeComponent();

            // Initialisation des différents champs de l'écran
            this.AdresseServeur = infos.AdresseServeur;
            this.EstServeur = infos.EstServeur;
            this.NomJoueur = infos.NomJoueur;
            cboAdresse.ItemsSource = infos.ListeServeurs;
            cboAdresse.Text = infos.AdresseServeur;
            
            // Ecoute spécifique du TextChanged de la combo cboAdresse
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
            dpd.AddValueChanged(cboAdresse, cboAdresse_OnTextChanged);
             
            // On récupère toutes les langues disponibles & on affiche la bonne
            ITraducteur tradChoisi = null;
            _initialisationComboLangue = true;            
            foreach (string cheminAssembly in Directory.GetFiles("lang", "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory,cheminAssembly));

                // Remplissage de la lstView par Recupération de toutes les classes implémentant ITraducteur
                foreach (Type t in asm.GetTypes())
                {
                    Type typeTrad = t.GetInterface("ITraducteur");
                    if (typeTrad != null)
                    {
                        ITraducteur traducteur = Activator.CreateInstance(t) as ITraducteur;
                        cboLangue.Items.Add(traducteur);
                        cboLangue.DisplayMemberPath = "LangueTraduction";
                        if (traducteur.LangueTraduction.Equals(infos.Langue)) tradChoisi = traducteur;
                    }
                }
            }
            if (!cboLangue.HasItems)
                throw new ApplicationException("Aucune langue trouvée pour l'application");

            if (tradChoisi == null)
            {
                cboLangue.SelectedIndex = 0;
            }
            else
            {
                cboLangue.SelectedItem = tradChoisi;
            }
            _initialisationComboLangue = false;
            TraduireEcran(cboLangue.SelectedItem as ITraducteur);

            this.EstClientEnModeLecture = false;

            txtNomJoueur.Focus();
            txtNomJoueur.SelectAll();            
        }

      
	}
}
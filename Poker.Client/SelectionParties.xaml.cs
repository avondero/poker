using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Poker.Interface.ExtensionsClient.Replay;

namespace Poker.Client
{
	public partial class SelectionParties
	{
        #region Constructeurs
        public SelectionParties(ILecturePartie lecteur)
		{
			this.InitializeComponent();

            // Traduction
            this.Title = OutilsTraduction.Traducteur.Traduire("SelectionParties");
            btnDemarrerLecture.Content = OutilsTraduction.Traducteur.Traduire("DemarrerLecture");

            (lstParties.View as GridView).Columns[0].Header = OutilsTraduction.Traducteur.Traduire("NomPartie");
            (lstParties.View as GridView).Columns[1].Header = OutilsTraduction.Traducteur.Traduire("DescriptionPartie");

            lstParties.Items.Clear();
            lstParties.ItemsSource = lecteur.PartiesDisponibles();
        }
        #endregion 

        #region Proprietes
        /// <summary>
        /// Partie selectionnée        
        /// </summary>
        public Partie PartieChoisie
        {
            get
            {
                return lstParties.SelectedItem as Partie;
            }
        }
        #endregion

        #region Evenements
        private void btnDemarrerLecture_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = new bool?(true);
            this.Close();
        }

        private void lstParties_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnDemarrerLecture_Click(null, null);
        }
        #endregion

        
    }
}
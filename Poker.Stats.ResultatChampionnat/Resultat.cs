using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Poker.Stats.ResultatChampionnat.MethodesCalcul;
using System.Xml;
using System.IO;

namespace Poker.Stats.ResultatChampionnat
{
    public partial class Resultat : Form
    {
        public Resultat()
        {
            InitializeComponent();
        }

        private void Resultat_Load(object sender, EventArgs e)
        {
            // Remplissage de la lstView par Recupération de toutes les classes implémentant ControleBase et disposant de l'attribut Controle
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                Type typeCalcul = t.GetInterface("ICalculClassement");
                if (typeCalcul != null)
                {
                    ICalculClassement calc = Activator.CreateInstance(t) as ICalculClassement;
                    cboMethodeCalcul.Items.Add(calc);
                    if (t.Name == "CalculStandard")
                    {
                        cboMethodeCalcul.SelectedItem = calc;
                    }
                }
            }            
        }

        private void btnCalculClassement_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                string ficChampionnat = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data\\championnat.xml");
                if (File.Exists(ficChampionnat))
                {
                    doc.Load(ficChampionnat);

                    ICalculClassement methodeCalcul = cboMethodeCalcul.SelectedItem as ICalculClassement;

                    List<Partie> listeParties = new List<Partie>();
                    foreach (XmlNode nodPartie in doc.SelectNodes("championnat/partie"))
                    {
                        Partie p = new Partie(int.Parse(nodPartie.Attributes["enjeu"].Value), DateTime.Parse(nodPartie.Attributes["date"].Value));

                        foreach (XmlNode nodJoueur in nodPartie.ChildNodes)
                        {
                            Joueur j = new Joueur();
                            j.Nom = nodJoueur.Attributes["nom"].Value;
                            j.PositionElimination = int.Parse(nodJoueur.Attributes["positionElimination"].Value);
                            p.ListeJoueurs.Add(j);
                        }

                        listeParties.Add(p);
                    }

                    List<Joueur> classement = methodeCalcul.CalculerClassement(listeParties);
                    classement.Sort(new Comparison<Joueur>((j, k) => j.PositionElimination.CompareTo(k.PositionElimination)));
                    lstClassement.Items.Clear();

                    int pos = 1;
                    foreach (Joueur j in classement)
                    {
                        ListViewItem item = new ListViewItem(pos.ToString());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, j.Nom));
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, j.Resultat));
                        lstClassement.Items.Add(item);
                        pos++;
                    }

                    lstClassement.Columns[2].Text = methodeCalcul.LibelleColonneResultat;
                }
                else
                {
                    MessageBox.Show("Pas de fichier championnat trouvé (" + ficChampionnat + ")");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la lecture des données du championnat : " + ex.Message);
            }
        }
   }
}

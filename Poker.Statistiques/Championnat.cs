using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Poker.Interface.Stats;
using System.IO;
using System.Reflection;

namespace Poker.Statistiques
{
    /// <summary>
    /// Classe de calcul du championnat
    /// <example> Fichier XML : 
    ///   <championnat>
    ///     <partie date="20080523124500" enjeu="6000">
    ///       <joueur nom="Pico" positionElimination="1"/>
    ///     </partie>
    ///   </championnat>
    /// </example>
    /// </summary>
    public class Championnat : StatistiquesBase
    {

        #region Constructeur
        public Championnat()
        {
            _cheminFichierChampionnat = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.Combine(Constantes.REPERTOIRE_DATA, FICHIER_CHAMPIONNAT));
        }
        #endregion

        #region Attributs
        private readonly string _cheminFichierChampionnat = string.Empty;
        private XmlDocument _stats = new XmlDocument();
        private XmlNode _partieEnCours = null;
        private const string FICHIER_CHAMPIONNAT = "Championnat.xml";
        private const string XML_PARTIE = "partie";
        private const string XML_CHAMPIONNAT = "championnat";
        private const string XML_DATE = "date";
        private const string XML_ENJEU = "enjeu";
        private const string XML_JOUEUR = "joueur";
        private const string XML_NOM = "nom";        
        private const string XML_POSELIMINATION = "positionElimination";
        private const string FORMAT_DATE = "yyyy/MM/dd HH:mm:ss";
        #endregion

        #region Methodes protegees surchargées
        protected override void EnregistrerNouvellePartie(NouvellePartie evt)
        {            
            Directory.CreateDirectory(Path.GetDirectoryName(_cheminFichierChampionnat));
            if (File.Exists(_cheminFichierChampionnat))
            {
                _stats.Load(_cheminFichierChampionnat);
            }
            else
            {
                _stats.LoadXml("<" + XML_CHAMPIONNAT + "/>");
            }

            _partieEnCours = _stats.CreateNode(XmlNodeType.Element, XML_PARTIE, null);
            // Date de la partie
            XmlAttribute att = _stats.CreateAttribute(XML_DATE);
            att.Value = evt.DateEvenement.ToString(FORMAT_DATE);
            _partieEnCours.Attributes.Append(att);

            // Enjeu de la partie
            att = _stats.CreateAttribute(XML_ENJEU);
            att.Value = (evt.ListeJoueurs.Count * evt.OptionsJeu.TapisInitial).ToString();
            _partieEnCours.Attributes.Append(att);

            _stats.DocumentElement.AppendChild(_partieEnCours);
        }

        protected override void EnregistrerFinPartie(FinPartie evt)
        {
            // Liste des joueurs
            foreach (JoueurStat j in evt.ListeJoueurs)
            {
                XmlNode nodJoueur = _stats.CreateElement(XML_JOUEUR);
                // Nom + pos élimination
                XmlAttribute att = _stats.CreateAttribute(XML_NOM);
                att.Value = j.Nom;
                nodJoueur.Attributes.Append(att);
                att = _stats.CreateAttribute(XML_POSELIMINATION);
                att.Value = j.PositionElimination.ToString();
                nodJoueur.Attributes.Append(att);

                _partieEnCours.AppendChild(nodJoueur);
            }

            _stats.Save(_cheminFichierChampionnat);
        }
        #endregion
    }
}

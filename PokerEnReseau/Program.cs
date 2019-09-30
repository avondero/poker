using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Poker.Client;
using Poker.Interface.Communication;
using Poker.Interface.Metier;
using Poker.Serveur;
using System.Globalization;
using Poker.Interface.Outils;
using System.ServiceModel;
using System.Diagnostics;
using PokerEnReseau.Properties;

namespace PokerEnReseau
{
    class Program : System.Windows.Application
    {
        private const string URL_METADATA = "http://localhost:4601/Poker";
        private const string ADRESSE_SERVEUR = "net.tcp://{0}:4600/Poker";

        /// <summary>
        /// Point d'entrée de l'application
        ///  Des paramètres peuvent être utilisés :
        ///   /P:"Nom du joueur" ou /P:NomJoueur
        ///   /J:Serveur    Nom du serveur à rejoindre
        ///   /L:Français   Langue à utiliser
        ///   /H Héberger l'application        
        ///   /R Mode Replay
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(params string[] args)
        {
            Program monApp = new Program();
            monApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                
            try
            {
                // On boucle tant que l'utilisateur n'a pas cliqué sur "Annuler" ou qu'il n'a pas réussi à se connecter
                bool clicAnnuler = false;
                InfosEcranConnexion infos = null;
                while (!clicAnnuler)
                {
                    // 1er chargement : on récupère la sauvegarde et on applique la ligne de commande
                    // 2ème chargement : Les infos ont déjà été récupérées par la fenetre de connexion
                    if (infos == null)
                    {                     
                        infos = RecupererInfosConnexion();
                        infos = TraiterLigneCommande(infos, args);                        
                    }                                        
                    
                    DemarrageApplication formDemarrage = new DemarrageApplication(infos);                    
                    clicAnnuler = !(bool)formDemarrage.ShowDialog();

                    if (!clicAnnuler)
                    {
                        infos = new InfosEcranConnexion(formDemarrage.NomJoueur, formDemarrage.AdresseServeur, formDemarrage.Langue, formDemarrage.EstServeur, formDemarrage.EstSpectateur, infos.ListeServeurs);                        
                        infos.ModeLancementClient = formDemarrage.EstClientEnModeLecture ? ModeClient.LecturePartie : ModeClient.Jeu;
                        infos.EstSpectateur = formDemarrage.EstSpectateur;
                        if (infos.ListeServeurs == null) infos.ListeServeurs = new List<string>();
                        if (infos.ListeServeurs.Contains(infos.AdresseServeur))
                        {
                            // Une seule instance de l'adresse déjà choisie
                            infos.ListeServeurs.Remove(infos.AdresseServeur);
                        }
                        infos.ListeServeurs.Insert(0, infos.AdresseServeur);                        

                        Joueur joueurConnecte = new Joueur(infos.NomJoueur);
                        joueurConnecte.EstSpectateur = infos.EstSpectateur;

                        // On considére que le serveur est démarré si on rejoint une partie
                        bool serveurDemarre = true;                        
                        
                        // Démarrage du serveur
                        if (infos.EstServeur && infos.ModeLancementClient == ModeClient.Jeu)
                        {                            
                            serveurDemarre = Serveur.DemarrerServeur(string.Format(ADRESSE_SERVEUR, "localhost"));
                            if (!serveurDemarre)
                            {
                                MessageBox.Show(OutilsTraduction.Traducteur.Traduire("ErreurDemarrageServeur"), Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                        if (serveurDemarre)
                        {                           
                            // Démarrage du client
                            IServeur serveur = null;
                            CommunicationServeur comm = null;
                            ResultatConnection res;
                            if (infos.ModeLancementClient == ModeClient.Jeu)
                            {
                                res = ConnecterServeur(joueurConnecte, string.Format(ADRESSE_SERVEUR, infos.AdresseServeur), out serveur, out comm);
                            }
                            else
                            {
                                res = new ResultatConnection(RetourConnection.Ok);
                            }

                            if (res != null)
                            {
                                if (res.Connection == RetourConnection.Ok)
                                {
                                    try
                                    {                                        
                                        SauvegarderInfosConnexion(infos);
                                        clicAnnuler = true;
                                        TablePoker table = new TablePoker(joueurConnecte, comm, serveur, res, formDemarrage.TraducteurChoisi, infos.ModeLancementClient);
                                        monApp.Run(table);                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(formDemarrage.TraducteurChoisi.Traduire("ErreurApplication") + ex.Message, Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(formDemarrage.TraducteurChoisi.Traduire("ErreurConnection" + res.Connection.ToString()), Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show(formDemarrage.TraducteurChoisi.Traduire("ErreurConnection"), Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de l'application : " + ex.Message, Constantes.NOM_APPLICATION, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Traite la ligne de commande et modifie les informations de connexion en fonction        
        /// </summary>
        /// <param name="infos">Les informations de connexion (initialisé par la récupération des paramètres)</param>
        /// <param name="args">la ligne de commande</param>
        /// <remarks>La ligne de commande étant prioritaire sur les informations sauvegardées, cette fonction sera toujours appelée après la récupération des paramètres</remarks>
        /// <returns>Toutes les informations nécessaires à l'affichage de l'écran de connexion</returns>
        private static InfosEcranConnexion TraiterLigneCommande(InfosEcranConnexion infos, params string[] args)
        {
            InfosEcranConnexion resInfos = infos.Clone() as InfosEcranConnexion;            
            resInfos.EstServeur = false;
            resInfos.ModeLancementClient = ModeClient.Jeu;
            
            foreach (string param in args)
            {
                switch (param.Substring(0, 2))
                { 
                    case "/P":
                        resInfos.NomJoueur = param.Substring(3).Trim('\"');
                        break;

                    case "/L":
                        resInfos.Langue = param.Substring(3).Trim('\"');
                        break;

                    case "/H":
                        resInfos.EstServeur = true;
                        break;

                    case "/J":
                        resInfos.EstServeur = false;
                        resInfos.AdresseServeur = param.Substring(3).Trim('\"');
                        break;                    

                    case "/R":
                        resInfos.ModeLancementClient = ModeClient.LecturePartie;
                        break;

                    default:
                        break;
                }
            }

            return resInfos;
        }

        /// <summary>
        /// Connection au serveur
        /// </summary>
        /// <param name="joueurConnecte"></param>
        /// <param name="adresseServeur"></param>
        /// <param name="serveur"></param>
        /// <param name="commServeur"></param>
        /// <returns></returns>
        private static ResultatConnection ConnecterServeur(Joueur joueurConnecte,string adresseServeur, out IServeur serveur, out CommunicationServeur commServeur)
        {
            // Connection            
            ResultatConnection res = null;
            commServeur = null;
            serveur = null;

            try
            {
                commServeur = new CommunicationServeur();            
                serveur = new DuplexChannelFactory<IServeur>(new InstanceContext(commServeur), adresseServeur).CreateChannel();
                res = serveur.Connecter(joueurConnecte);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur lors de la connection : " + ex.Message);                
            }
            
            return res;
        }

        /// <summary>
        /// Sauvegarde des informations de connexion
        /// </summary>
        /// <param name="infosConn"></param>
        private static void SauvegarderInfosConnexion(InfosEcranConnexion infos)
        {
            Settings prm = new Settings();
            prm.NomUtilisateur = infos.NomJoueur;
            prm.ListeServeurs = new System.Collections.Specialized.StringCollection();
            prm.ListeServeurs.AddRange(infos.ListeServeurs.ToArray()) ;
            prm.EstServeur = infos.EstServeur;
            prm.Langue = infos.Langue;
            prm.Save();

        }

        /// <summary>
        /// Récupération des informations de connexion
        /// </summary>
        /// <returns></returns>
        private static InfosEcranConnexion RecupererInfosConnexion()
        {
            Settings prm = new Settings();
            // Si c'est les paramètres par défaut, on essaie de récupérer les parametres de la version précédente
            if (prm.NomUtilisateur == Settings.Default.NomUtilisateur && prm.Langue == Settings.Default.Langue &&
                prm.EstServeur == Settings.Default.EstServeur && prm.ListeServeurs == Settings.Default.ListeServeurs &&
                prm.EstSpectateur == Settings.Default.EstSpectateur)                
            {
                prm.Upgrade();
            }
            
            InfosEcranConnexion resInfos;
            if (prm.ListeServeurs == null)
            {
                resInfos = new InfosEcranConnexion(prm.NomUtilisateur, string.Empty, prm.Langue, prm.EstServeur, prm.EstSpectateur, null);
            }
            else
            { 
                List<String> listeServeurs = new List<string>();
                foreach (string srv in prm.ListeServeurs)
                {
                    listeServeurs.Add(srv);
                }
                resInfos = new InfosEcranConnexion(prm.NomUtilisateur, prm.ListeServeurs[0], prm.Langue, prm.EstServeur, prm.EstSpectateur, listeServeurs);
            }

            return resInfos;
        }
    }
}

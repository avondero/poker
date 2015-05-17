using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poker.Interface.Stats;
using Poker.Interface.Metier;
using Poker.Serveur.Technique;

namespace Poker.Serveur.Metier
{
    class EnregistrementStats
    {

        #region Membres privés
        private List<IStatistiques> _statistiques = null;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur 
        /// </summary>
        internal EnregistrementStats(List<IStatistiques> listeStats)
        {
            _statistiques = listeStats;
        }
        #endregion

        #region Methodes publiques / internes
        /// <summary>
        /// Enregistrement d'une nouvelle partie
        /// </summary>
        internal void EnregistrerNouvellePartie(List<Joueur> listeJoueurs, Options optJeu)
        {
            try
            {
                NouvellePartie partie = new NouvellePartie();
                partie.OptionsJeu = CloneOptionsEnOptionsStats(optJeu);
                partie.ListeJoueurs = CloneListeJoueurs(listeJoueurs);

                foreach (IStatistiques stat in _statistiques)
                {                    
                    stat.Enregistrer(partie);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerNouvellePartie : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement d'une fin de partie
        /// </summary>
        internal void EnregistrerFinPartie(List<Joueur> listeJoueurs)
        {
            try
            {
                FinPartie partie = new FinPartie();
                partie.ListeJoueurs = CloneListeJoueurs(listeJoueurs);

                foreach (IStatistiques stat in _statistiques)
                {                    
                    stat.Enregistrer(partie);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerFinPartie : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement d'une nouvelle donne
        /// </summary>
        /// <param name="listeJoueurs"></param>
        /// <param name="numeroDonne"></param>
        internal void EnregistrerNouvelleDonne(int numeroDonne, List<Joueur> listeJoueurs, Blind infosBlind)
        {
            try
            {
                NouvelleDonne donne = new NouvelleDonne();
                donne.Dealer = RechercherDealer(listeJoueurs);
                donne.PetiteBlind = RechercherPetiteBlind(listeJoueurs);
                donne.GrosseBlind = RechercherGrosseBlind(listeJoueurs);
                donne.NumeroDonne = numeroDonne;
                donne.InfosBlind = infosBlind;
                donne.ListeJoueurs = CloneListeJoueursSansCartes(listeJoueurs);
                foreach (IStatistiques stat in _statistiques)
                {
                    stat.Enregistrer(donne);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerNouvelleDonne : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement d'une nouvelle donne
        /// </summary>
        /// <param name="listeJoueurs"></param>
        /// <param name="numeroDonne"></param>
        internal void EnregistrerFinDonne(List<Joueur> listeJoueurs, Combinaison combGagnante, List<Joueur> gagnants, int pot)
        {
            try
            {
                FinDonne donne = new FinDonne();
                donne.CombinaisonGagnante = combGagnante;
                donne.JoueursGagnants = CloneListeJoueurs(gagnants);
                donne.ListeJoueurs = CloneListeJoueurs(listeJoueurs);
                donne.Pot = pot;
                donne.NumeroDonne = InfosTable.Singleton.NumeroDonne;
                foreach (IStatistiques stat in _statistiques)
                {                    
                    stat.Enregistrer(donne);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerFinDonne : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement d'une discussion
        /// </summary>
        /// <param name="message"></param>
        /// <param name="j"></param>
        internal void EnregistrerChat(string message, Joueur j)
        {
            try
            {
                Chat chatObj = new Chat();
                chatObj.Message = message;
                chatObj.Nom = j.Nom;
                foreach (IStatistiques stat in _statistiques)
                {                   
                    stat.Enregistrer(chatObj);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerChat : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement du flop
        /// </summary>
        /// <param name="carte1"></param>
        /// <param name="carte2"></param>
        /// <param name="carte3"></param>
        /// <param name="pot"></param>
        internal void EnregistrerFlop(CartePoker carte1, CartePoker carte2, CartePoker carte3, int pot)
        {
            try
            {
                Flop enregFlop = new Flop();
                enregFlop.Carte1 = carte1;
                enregFlop.Carte2 = carte2;
                enregFlop.Carte3 = carte3;
                enregFlop.Pot = pot;
                foreach (IStatistiques stat in _statistiques)
                {
                    stat.Enregistrer(enregFlop);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerFlop : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrer Turn
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="pot"></param>
        internal void EnregistrerTurn(CartePoker carte, int pot)
        {
            try
            {
                Turn enregTurn = new Turn();
                enregTurn.Carte = carte;
                enregTurn.Pot = pot;
                foreach (IStatistiques stat in _statistiques)
                {                    
                    stat.Enregistrer(enregTurn);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerTurn : " + ex.Message);

            }
        }

        /// <summary>
        /// Enregistrer River
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="pot"></param>
        internal void EnregistrerRiver(CartePoker carte, int pot)
        {
            try
            {
                River enregRiver = new River();
                enregRiver.Carte = carte;
                enregRiver.Pot = pot;
                foreach (IStatistiques stat in _statistiques)
                {                   
                    stat.Enregistrer(enregRiver);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerRiver : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement de l'augmentation des blinds
        /// </summary>
        /// <param name="montantGrosseBlind"></param>
        /// <param name="montantPetiteBlind"></param>
        internal void EnregistrerAugmentationBlinds(Blind infosBlind)
        {
            try
            {
                AugmentationBlinds aug = new AugmentationBlinds();
                aug.MontantPetiteBlind = infosBlind.MontantPetiteBlind;
                aug.MontantGrosseBlind = infosBlind.MontantGrosseBlind;
                aug.MontantProchainePetiteBlind = infosBlind.MontantProchainePetiteBlind;
                aug.MontantProchaineGrosseBlind = infosBlind.MontantProchaineGrosseBlind;
                aug.DelaiAugmentationBlinds = infosBlind.DelaiAugmentationBlinds;

                foreach (IStatistiques stat in _statistiques)
                {
                    stat.Enregistrer(aug);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerAugmentationBlinds : " + ex.Message);
            }
        }

        /// <summary>
        /// Enregistrement de l'action d'un joueur
        /// </summary>
        /// <param name="expediteur"></param>
        /// <param name="action"></param>
        internal void EnregistrerActionJoueur(Joueur expediteur, TypeActionJoueur action)
        {
            try
            {
                ActionJoueurStat actionStat = new ActionJoueurStat();
                actionStat.Tapis = expediteur.TapisJoueur;
                actionStat.Mise = expediteur.Mise;
                actionStat.Nom = expediteur.Nom;
                actionStat.TypeAction = action;
                foreach (IStatistiques stat in _statistiques)
                {
                    stat.Enregistrer(actionStat);
                }
            }
            catch (Exception ex)
            {
                logServeur.Debug("Erreur lors d'EnregistrerActionJoueur : " + ex.Message);
            }
        }


        #endregion

        #region Methodes privées
        private string RechercherDealer(List<Joueur> listeJoueur)
        {
            return RechercherTypeBouton(listeJoueur, TypeBouton.Dealer);
        }

        private string RechercherPetiteBlind(List<Joueur> listeJoueur)
        {
            return RechercherTypeBouton(listeJoueur, TypeBouton.PetiteBlind);
        }

        private string RechercherGrosseBlind(List<Joueur> listeJoueur)
        {
            return RechercherTypeBouton(listeJoueur, TypeBouton.GrosseBlind);
        }

        private string RechercherTypeBouton(List<Joueur> listeJoueur, TypeBouton typeBouton)
        {
            return (from j in listeJoueur where ((j.Bouton & typeBouton) == typeBouton) select j.Nom).First<string>();
        }

        private OptionsStat CloneOptionsEnOptionsStats(Options opt)
        {
            return new OptionsStat()
            {
                DescriptionAugmentationBlinds = opt.MethodeAugmentationBlinds.Description,
                MontantInitialPetiteBlind = opt.MontantPetiteBlindInitial,
                TapisInitial = opt.TapisInitial
            };
        }

        /// <summary>
        /// Clone des joueurs en joueurs Stats
        /// </summary>
        /// <param name="listeJoueurs"></param>
        /// <returns></returns>
        private List<JoueurStat> CloneListeJoueurs(List<Joueur> listeJoueurs)
        {
            List<JoueurStat> res = new List<JoueurStat>();
            foreach (Joueur j in listeJoueurs)
            {
                res.Add(CloneJoueurEnJoueurStat(j));
            }

            return res;

        }

        private List<JoueurStat> CloneListeJoueursSansCartes(List<Joueur> listeJoueurs)
        {
            List<JoueurStat> res = new List<JoueurStat>();
            foreach (Joueur j in listeJoueurs)
            {
                res.Add(CloneJoueurEnJoueurStatSansCartes(j));
            }

            return res;

        }

        /// Clone des joueurs en joueurs Stats
        private JoueurStat CloneJoueurEnJoueurStat(Joueur j)
        {
            return new JoueurStat()
            {
                Carte1 = j.Carte1,
                Carte2 = j.Carte2,
                Nom = j.Nom,
                PositionElimination = j.PositionElimination,
                Tapis = j.TapisJoueur
            };
        }

        /// Clone des joueurs en joueurs Stats
        private JoueurStat CloneJoueurEnJoueurStatSansCartes(Joueur j)
        {
            return new JoueurStat()
            {
                Carte1 = null,
                Carte2 = null,
                Nom = j.Nom,
                PositionElimination = j.PositionElimination,
                Tapis = j.TapisJoueur
            };
        }
        #endregion
    }
}

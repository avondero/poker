using Poker.Serveur.Metier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker.Interface.Metier;
using System.Collections.Generic;
using System;

namespace Poker.TestServeur
{    
    /// <summary>
    ///This is a test class for MainPokerTest and is intended
    ///to contain all MainPokerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MainPokerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
       
        #region Constructeurs
        /// <summary>
        /// 1 test constructeur : le board doit contenir 5 cartes : sinon ArgumentException
        ///</summary>
        [TestMethod(), Description("Constructeur avec un board vide")]
        public void ConstructorTest1()
        {
            var j = new Joueur("joueur");
            var exceptionLevee = false;
            var message = "Une exception ArgumentException aurait du être levée";
            try
            {
                var main = new MainPoker(j, null);
            }
            catch (ArgumentException)
            {
                exceptionLevee = true;
            }
            catch (Exception ex)
            {
                exceptionLevee = false;
                message = "Une exception ArgumentException aurait du être levée au lieu d'une " + ex.ToString();
            }
            Assert.IsTrue(exceptionLevee, message);            
        }

        /// <summary>
        /// 1 test constructeur : le board doit contenir 5 cartes : sinon ArgumentException
        ///</summary>
        [TestMethod(), Description("Constructeur avec un board à 7 cartes")]
        public void ConstructorTest2()
        {
            var j = new Joueur("joueur");
            var exceptionLevee = false;
            var message = "Une exception ArgumentException aurait du être levée";
            try
            {
                var main = new MainPoker(j, new CartePoker[] { 
                    JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.AsCarreau,JeuPoker.Cartes.AsCarreau,JeuPoker.Cartes.AsCarreau,
                    JeuPoker.Cartes.AsCarreau,JeuPoker.Cartes.AsCarreau,JeuPoker.Cartes.AsCarreau
                });                        
            }
            catch (ArgumentException)
            {
                exceptionLevee = true;
            }
            catch (Exception ex)
            {
                exceptionLevee = false;
                message = "Une exception ArgumentException aurait du être levée au lieu d'une " + ex.ToString();
            }
            Assert.IsTrue(exceptionLevee, message); 
                              
        }

        /// <summary>
        /// 1 test constructeur ok
        ///</summary>
        [TestMethod(), Description("Constructeur ok")]
        public void ConstructorTest3()
        {
            var j = new Joueur("joueur");
            var main = new MainPoker(j, new CartePoker[] { 
                    JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.AsCoeur,JeuPoker.Cartes.AsTrefle,JeuPoker.Cartes.AsPique,
                    JeuPoker.Cartes.RoiCarreau
                });      
        }
        
#endregion

        #region DeterminerCombinaison
        #region Quinte flush
        /// <summary>
        /// Quinte flush 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Quinte flush (7 cartes ordonnées)")]
        public void DeterminerCombinaisonTestQuinteFlush1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.TroisCoeur,
                JeuPoker.Cartes.QuatreCoeur, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.SixCoeur, JeuPoker.Cartes.SeptCoeur);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.QuinteFlush, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.SeptCoeur, main, 0);
            VerifierCarte(JeuPoker.Cartes.SixCoeur, main, 1);
            VerifierCarte(JeuPoker.Cartes.CinqCoeur, main, 2);
            VerifierCarte(JeuPoker.Cartes.QuatreCoeur, main, 3);
            VerifierCarte(JeuPoker.Cartes.TroisCoeur, main, 4);
        }

        /// <summary>
        /// Quinte flush 2
        ///</summary>
        [TestMethod(), Description("Combinaison : Quinte flush (5 cartes désordonnées)")]
        public void DeterminerCombinaisonTestQuinteFlush2()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.QuatreCoeur,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.RoiTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.QuinteFlush, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.CinqCoeur, main, 0);
            VerifierCarte(JeuPoker.Cartes.QuatreCoeur, main, 1);
            VerifierCarte(JeuPoker.Cartes.TroisCoeur, main, 2);
            VerifierCarte(JeuPoker.Cartes.DeuxCoeur, main, 3);
            VerifierCarte(JeuPoker.Cartes.AsCoeur, main, 4);
        }       
        #endregion

        #region Carre
        /// <summary>
        /// Carre 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Carre (4 cartes désordonnées)")]
        public void DeterminerCombinaisonTestCarre1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxPique,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DeuxTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Carre, main.ResultatMain.TypeCombinaison);
            VerifierCarte(HauteurCarte.Deux, main, 0);
            VerifierCarte(HauteurCarte.Deux, main, 1);
            VerifierCarte(HauteurCarte.Deux, main, 2);
            VerifierCarte(HauteurCarte.Deux, main, 3);
            VerifierCarte(JeuPoker.Cartes.AsCoeur, main, 4);
        }
        #endregion

        #region Full
        /// <summary>
        /// Full 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Carré = 3 + 4")]
        public void DeterminerCombinaisonTestFull1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxPique,
                     JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.TroisCarreau, JeuPoker.Cartes.TroisPique, JeuPoker.Cartes.TroisTrefle);
            main.DeterminerCombinaison();

            Assert.AreNotEqual(TypeCombinaison.Full, main.ResultatMain.TypeCombinaison);
        }

        /// <summary>
        /// Full 2
        ///</summary>
        [TestMethod(), Description("Combinaison : Full 3 & 3")]
        public void DeterminerCombinaisonTestFull2()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxPique,
                     JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.TroisCarreau, JeuPoker.Cartes.TroisPique, JeuPoker.Cartes.CinqTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(main.ResultatMain.TypeCombinaison, TypeCombinaison.Full);
            VerifierCarte(HauteurCarte.Trois, main, 0);
            VerifierCarte(HauteurCarte.Trois, main, 1);
            VerifierCarte(HauteurCarte.Trois, main, 2);
            VerifierCarte(HauteurCarte.Deux, main, 3);
            VerifierCarte(HauteurCarte.Deux, main, 4);
        }

        /// <summary>
        /// Full 3
        ///</summary>
        [TestMethod(), Description("Combinaison : Full 3 & 3 dans l'autre sens")]
        public void DeterminerCombinaisonTestFull3()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.TroisCarreau, JeuPoker.Cartes.TroisPique,
                JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxPique, JeuPoker.Cartes.CinqTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(main.ResultatMain.TypeCombinaison, TypeCombinaison.Full);
            VerifierCarte(HauteurCarte.Trois, main, 0);
            VerifierCarte(HauteurCarte.Trois, main, 1);
            VerifierCarte(HauteurCarte.Trois, main, 2);
            VerifierCarte(HauteurCarte.Deux, main, 3);
            VerifierCarte(HauteurCarte.Deux, main, 4);
        }

        /// <summary>
        /// Full 4
        ///</summary>
        [TestMethod(), Description("Combinaison : Full 3, 2, 2")]
        public void DeterminerCombinaisonTestFull4()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.ValetCarreau, JeuPoker.Cartes.ValetPique,
                JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DamePique, JeuPoker.Cartes.ValetTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(main.ResultatMain.TypeCombinaison, TypeCombinaison.Full);
            VerifierCarte(HauteurCarte.Valet, main, 0);
            VerifierCarte(HauteurCarte.Valet, main, 1);
            VerifierCarte(HauteurCarte.Valet, main, 2);
            VerifierCarte(HauteurCarte.Dame, main, 3);
            VerifierCarte(HauteurCarte.Dame, main, 4);
        }

        /// <summary>
        /// Full 5
        ///</summary>
        [TestMethod(), Description("Combinaison : Full 3, 2")]
        public void DeterminerCombinaisonTestFull5()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.ValetCarreau, JeuPoker.Cartes.ValetPique,
                JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.DamePique, JeuPoker.Cartes.ValetTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(main.ResultatMain.TypeCombinaison, TypeCombinaison.Full);
            VerifierCarte(HauteurCarte.Valet, main, 0);
            VerifierCarte(HauteurCarte.Valet, main, 1);
            VerifierCarte(HauteurCarte.Valet, main, 2);
            VerifierCarte(HauteurCarte.Dame, main, 3);
            VerifierCarte(HauteurCarte.Dame, main, 4);
        }

        #endregion

        #region Couleur
        /// <summary>
        /// Couleur 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Couleur (7 cartes de la même couleur)")]
        public void DeterminerCombinaisonTestCouleur1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.HuitCoeur, JeuPoker.Cartes.RoiCoeur,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameCoeur);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Couleur, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.AsCoeur, main, 0);
            VerifierCarte(JeuPoker.Cartes.RoiCoeur, main, 1);
            VerifierCarte(JeuPoker.Cartes.DameCoeur, main, 2);
            VerifierCarte(JeuPoker.Cartes.HuitCoeur, main, 3);
            VerifierCarte(JeuPoker.Cartes.CinqCoeur, main, 4);
        }

        /// <summary>
        /// Couleur 2
        ///</summary>
        [TestMethod(), Description("Combinaison : Couleur (5 cartes de la même couleur)")]
        public void DeterminerCombinaisonTestCouleur2()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.HuitCoeur, JeuPoker.Cartes.RoiCoeur,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Couleur, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.RoiCoeur, main, 0);
            VerifierCarte(JeuPoker.Cartes.HuitCoeur, main, 1);
            VerifierCarte(JeuPoker.Cartes.CinqCoeur, main, 2);
            VerifierCarte(JeuPoker.Cartes.TroisCoeur, main, 3);
            VerifierCarte(JeuPoker.Cartes.DeuxCoeur, main, 4);
        }

        /// <summary>
        /// Couleur 3
        ///</summary>
        [TestMethod(), Description("Combinaison : Pas Couleur (5 cartes Rouge)")]
        public void DeterminerCombinaisonTestCouleur3()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.HuitCoeur, JeuPoker.Cartes.RoiCoeur,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameCarreau);
            main.DeterminerCombinaison();

            Assert.AreNotEqual(TypeCombinaison.Couleur, main.ResultatMain.TypeCombinaison);
        }  
        #endregion

        #region Quinte
        /// <summary>
        /// Quinte 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Quinte (d'As à 5)")]
        public void DeterminerCombinaisonTestQuinte1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.HuitCoeur, JeuPoker.Cartes.QuatreTrefle,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Quinte, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.CinqCoeur, main, 0);
            VerifierCarte(JeuPoker.Cartes.QuatreTrefle, main, 1);
            VerifierCarte(JeuPoker.Cartes.TroisCoeur, main, 2);
            VerifierCarte(JeuPoker.Cartes.DeuxCoeur, main, 3);
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 4);
        }

        /// <summary>
        /// Quinte 2
        ///</summary>
        [TestMethod(), Description("Combinaison : Quinte (de 10 à As)")]
        public void DeterminerCombinaisonTestQuinte2()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.ValetCarreau, JeuPoker.Cartes.DixPique,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Quinte, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 0);
            VerifierCarte(JeuPoker.Cartes.RoiTrefle, main, 1);
            VerifierCarte(JeuPoker.Cartes.DameTrefle, main, 2);
            VerifierCarte(JeuPoker.Cartes.ValetCarreau, main, 3);
            VerifierCarte(JeuPoker.Cartes.DixPique, main, 4);
        }
        #endregion

        #region Brelan
        /// <summary>
        /// Brelan 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Brelan")]
        public void DeterminerCombinaisonTestBrelan1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle, JeuPoker.Cartes.DeuxPique,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Brelan, main.ResultatMain.TypeCombinaison);
            VerifierCarte(HauteurCarte.Deux, main, 0);
            VerifierCarte(HauteurCarte.Deux, main, 1);
            VerifierCarte(HauteurCarte.Deux, main, 2);
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 3);
            VerifierCarte(JeuPoker.Cartes.DameTrefle, main, 4);
        }
        #endregion

        #region Double paire
        /// <summary>
        /// Triple Paire 
        ///</summary>
        [TestMethod(), Description("Combinaison : Double paire (2,2 et 2)")]
        public void DeterminerCombinaisonTestDoublePaire1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle, JeuPoker.Cartes.DameCoeur,
                 JeuPoker.Cartes.CinqCarreau, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.DoublePaire, main.ResultatMain.TypeCombinaison);
            VerifierCarte(HauteurCarte.Dame, main, 0);
            VerifierCarte(HauteurCarte.Dame, main, 1);
            VerifierCarte(HauteurCarte.Cinq, main, 2);
            VerifierCarte(HauteurCarte.Cinq, main, 3);
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 4);
        }

        /// <summary>
        /// Double Paire 
        ///</summary>
        [TestMethod(), Description("Combinaison : Double paire")]
        public void DeterminerCombinaisonTestDoublePaire2()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle, JeuPoker.Cartes.ValetCoeur,
                 JeuPoker.Cartes.CinqCarreau, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.RoiCarreau, JeuPoker.Cartes.ValetTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.DoublePaire, main.ResultatMain.TypeCombinaison);
            VerifierCarte(HauteurCarte.Valet, main, 0);
            VerifierCarte(HauteurCarte.Valet, main, 1);
            VerifierCarte(HauteurCarte.Deux, main, 2);
            VerifierCarte(HauteurCarte.Deux, main, 3);
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 4);
        }
        #endregion

        #region Paire
        /// <summary>
        /// Paire 1
        ///</summary>
        [TestMethod(), Description("Combinaison : Paire")]
        public void DeterminerCombinaisonTestPaire1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle, JeuPoker.Cartes.RoiPique,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Paire, main.ResultatMain.TypeCombinaison);
            VerifierCarte(HauteurCarte.Deux, main, 0);
            VerifierCarte(HauteurCarte.Deux, main, 1);            
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 2);
            VerifierCarte(JeuPoker.Cartes.RoiPique, main, 3);
            VerifierCarte(JeuPoker.Cartes.DameTrefle, main, 4);
        }
        #endregion

        #region Carte simple
        /// <summary>
        /// Carte simple
        ///</summary>
        [TestMethod(), Description("Combinaison : Rien :(")]
        public void DeterminerCombinaisonTestCarteSimple1()
        {
            var main = ConstructeurMainPoker(JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.HuitPique, JeuPoker.Cartes.RoiPique,
                 JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameTrefle);
            main.DeterminerCombinaison();

            Assert.AreEqual(TypeCombinaison.Carte, main.ResultatMain.TypeCombinaison);
            VerifierCarte(JeuPoker.Cartes.AsCarreau, main, 0);
            VerifierCarte(JeuPoker.Cartes.RoiPique, main, 1);
            VerifierCarte(JeuPoker.Cartes.DameTrefle, main, 2); 
            VerifierCarte(JeuPoker.Cartes.HuitPique, main, 3);
            VerifierCarte(JeuPoker.Cartes.CinqCoeur, main, 4);
        }
        #endregion
        #endregion

        #region Comparaison de mains
        #region Tests d'egalité
        /// <summary>
        /// Test d'égalité : Les cartes sont égales mais pas les joueurs 
        ///</summary>        
        [TestMethod(), Description("Cartes égales, joueurs différents")]
        public void EqualsTest1()
        {
            MainPoker target = ConstructeurMainPoker("Joueur1", JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.AsCarreau,
                new CartePoker[] {JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle});
            object obj = ConstructeurMainPoker("Joueur2", JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.AsCarreau,
                new CartePoker[] {JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle});
           
            Assert.IsTrue(target.Equals(obj));            
        }

        /// <summary>
        /// Test d'égalité : Les cartes sont égales et les joueurs aussi
        ///</summary>
        [TestMethod(), Description("Cartes égales, joueurs égaux")]
        public void EqualsTest2()
        {
            Joueur j = new Joueur("Joueur");
            j.Carte1 = JeuPoker.Cartes.AsCoeur;
            j.Carte2 = JeuPoker.Cartes.AsCarreau;
            var board = new CartePoker[] { JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle };
            MainPoker target = new MainPoker(j, board);
            object obj = new MainPoker(j, board);
            
            Assert.IsTrue(target.Equals(obj));     
        }

        /// <summary>
        /// Test d'égalité : Les cartes ne sont pas égales (1 différente) mais les joueurs si
        ///</summary>
        [TestMethod(), Description("Cartes différentes (1 seule), joueurs égaux")]
        public void EqualsTest3()
        {
            Joueur j = new Joueur("Joueur");
            j.Carte1 = JeuPoker.Cartes.AsCoeur;
            j.Carte2 = JeuPoker.Cartes.AsCarreau;
            var board1 = new CartePoker[] { JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle };
            var board2 = new CartePoker[] { JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle };
            MainPoker target = new MainPoker(j, board1);
            object obj = new MainPoker(j, board2);

            Assert.IsFalse(target.Equals(obj));
        }

        /// <summary>
        /// Test d'égalité : Les cartes ne sont pas égales (toutes différentes) mais les joueurs si
        ///</summary>
        [TestMethod(), Description("Cartes différentes (5), joueurs égaux")]
        public void EqualsTest4()
        {
            Joueur j = new Joueur("Joueur");
            j.Carte1 = JeuPoker.Cartes.AsCoeur;
            j.Carte2 = JeuPoker.Cartes.AsCarreau;
            var board1 = new CartePoker[] { JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle };
            var board2 = new CartePoker[] { JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.RoiPique, JeuPoker.Cartes.RoiCarreau, JeuPoker.Cartes.RoiCoeur, JeuPoker.Cartes.DameTrefle};
            MainPoker target = new MainPoker(j, board1);
            object obj = new MainPoker(j, board2);

            Assert.IsFalse(target.Equals(obj));
        }

        /// <summary>
        /// Test d'égalité : Les cartes ne sont pas égales et les joueurs non plus
        ///</summary>
        [TestMethod(), Description("Cartes différentes, joueurs différents")]
        public void EqualsTest5()
        {
            var target = ConstructeurMainPoker("Joueur1", JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.AsCarreau,
                new CartePoker[] { JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle });
            object obj = ConstructeurMainPoker("Joueur2", JeuPoker.Cartes.RoiCoeur, JeuPoker.Cartes.RoiPique,
                new CartePoker[] { JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.RoiCarreau, JeuPoker.Cartes.DameTrefle, JeuPoker.Cartes.DameCarreau, JeuPoker.Cartes.DamePique });
            
            Assert.IsFalse(target.Equals(obj));    
        }

        /// <summary>
        /// Test d'égalité : Comparaison de la main à null
        ///</summary>
        [TestMethod(), Description("Egalité par rapport à null")]
        public void EqualsTest6()
        {
            var target = ConstructeurMainPoker("Joueur1", JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.AsCarreau,
               new CartePoker[] { JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle });
            object obj = null;
            
            Assert.IsFalse(target.Equals(obj));  
        }

        /// <summary>
        /// Test d'égalité : Comparaison de la main à un autre objet : on doit avoir une exception "ArgumentException"
        ///</summary>
        [TestMethod(), Description("Egalité par rapport à un autre objet (List<CartePoker>) : déclenchement d'exception")]
        public void EqualsTest7()
        {
            var target = ConstructeurMainPoker("Joueur1", JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.AsCarreau,
               new CartePoker[] { JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.AsPique, JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.DeuxTrefle });
            object obj = new List<CartePoker>();

            bool exceptionLevee = false;
            string message = "Une exception ArgumentException aurait du être levée";
            try
            {
                var actual = target.Equals(obj);
            }
            catch (ArgumentException)
            {
                exceptionLevee = true;
            }
            catch (Exception ex)
            {
                exceptionLevee = false;
                message = "Une exception ArgumentException aurait du être levée au lieu d'une " + ex.ToString(); 
            }
            Assert.IsTrue(exceptionLevee, message);            
        }
        #endregion

        #region Methode CompareTo
         /// <summary>
        /// Test de comparaison de mains
        ///</summary>
        [TestMethod(), Description("Comparaison de main")]
        public void CompareToTest()
        {
            var j1 = new Joueur("Joueur1");
            j1.Elimine = false;
            var j2 = new Joueur("Joueur2");
            j2.Elimine = false;
            var jElim = new Joueur("Joueur Eliminé");
            jElim.Elimine = true;

            // Quinte flush 1 & 2
            j1.Carte1 = JeuPoker.Cartes.AsCoeur;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var quinteFlush1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.RoiCoeur, JeuPoker.Cartes.ValetCoeur, JeuPoker.Cartes.HuitTrefle, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            quinteFlush1.DeterminerCombinaison();
            j2.Carte1 = JeuPoker.Cartes.AsCoeur;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var quinteFlush2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisCoeur, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.HuitTrefle, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.QuatreCoeur});
            quinteFlush2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.QuinteFlush, quinteFlush1.ResultatMain.TypeCombinaison);
            Assert.AreEqual(TypeCombinaison.QuinteFlush, quinteFlush2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(quinteFlush1.CompareTo(quinteFlush2) > 0, "Quinte flush à l'AS > Quinte flush au 5");

            //  Quinte flush 2 & Carre 1
            j1.Carte1 = JeuPoker.Cartes.AsCoeur;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var carre1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.NeufCoeur, JeuPoker.Cartes.NeufTrefle, JeuPoker.Cartes.NeufPique, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            carre1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Carre, carre1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(quinteFlush2.CompareTo(carre1) > 0, "Quinte flush au 5 > Carre au 9");

            // Carre 1 & Carre 2
            j2.Carte1 = JeuPoker.Cartes.DeuxCoeur;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var carre2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.DeuxCarreau, JeuPoker.Cartes.DeuxPique, JeuPoker.Cartes.DeuxTrefle, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            carre2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Carre, carre2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(carre1.CompareTo(carre2) > 0, "Carre au 9 > Carre au 2");

            //  Carré 2 & Full 1
            j1.Carte1 = JeuPoker.Cartes.AsCoeur;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var full1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.NeufCoeur, JeuPoker.Cartes.NeufTrefle, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.AsTrefle, JeuPoker.Cartes.DixCoeur });
            full1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Full, full1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(carre2.CompareTo(full1) > 0, "Carre au 9 > Full des As par les 9 ");

            // Full 1 & Full 2
            j2.Carte1 = JeuPoker.Cartes.AsCoeur;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var full2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.NeufCoeur, JeuPoker.Cartes.NeufTrefle, JeuPoker.Cartes.AsCarreau, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            full2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Full, full2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(full1.CompareTo(full2) > 0, "Full des As par les 9  > Full des 9 par les As");

            //  Full 2 & Couleur 1
            j1.Carte1 = JeuPoker.Cartes.AsCoeur;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var couleur1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.NeufCoeur, JeuPoker.Cartes.NeufTrefle, JeuPoker.Cartes.CinqCoeur, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            couleur1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Couleur, couleur1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(full2.CompareTo(couleur1) > 0, "Full des 9 par les As > Couleur à l'As");

            // Couleur 1 & Couleur 2
            j2.Carte1 = JeuPoker.Cartes.DeuxCarreau;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var couleur2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisCarreau, JeuPoker.Cartes.DeuxPique, JeuPoker.Cartes.CinqCarreau, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.SeptCarreau });
            couleur2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Couleur, couleur2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(couleur1.CompareTo(couleur2) > 0, "Couleur à l'as > Couleur à la dame");

            //  Couleur 2 & quinte 1
            j1.Carte1 = JeuPoker.Cartes.DeuxCarreau;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var quinte1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.HuitPique, JeuPoker.Cartes.DeuxPique, JeuPoker.Cartes.ValetTrefle, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            quinte1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Quinte, quinte1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(couleur2.CompareTo(quinte1) > 0, "Couleur à la dame > Quinte à la Dame");

            // quinte 1 & quinte 2
            j2.Carte1 = JeuPoker.Cartes.DeuxCarreau;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var quinte2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisTrefle, JeuPoker.Cartes.AsCoeur, JeuPoker.Cartes.QuatrePique, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.CinqPique});
            quinte2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Quinte, quinte2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(quinte1.CompareTo(quinte2) > 0, "Quinte à la Dame > Quinte au 5");

            //  Quinte 2 & brelan 1
            j1.Carte1 = JeuPoker.Cartes.DeuxCarreau;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var brelan1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.HuitPique, JeuPoker.Cartes.DamePique, JeuPoker.Cartes.ValetTrefle, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DameCarreau});
            brelan1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Brelan, brelan1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(quinte2.CompareTo(brelan1) > 0, "Quinte au 5 > Brelan de dames");

            // brelan 1 & brelan 2
            j2.Carte1 = JeuPoker.Cartes.DeuxCarreau;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var brelan2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisTrefle, JeuPoker.Cartes.SeptCoeur, JeuPoker.Cartes.SeptPique, JeuPoker.Cartes.SeptCarreau, JeuPoker.Cartes.CinqPique });
            brelan2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Brelan, brelan1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(brelan1.CompareTo(brelan2) > 0, "Brelan de dames > Brelan de sept");

            //  brelan 2 & Double paire 1
            j1.Carte1 = JeuPoker.Cartes.DeuxCarreau;
            j1.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var doublepaire1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.RoiPique, JeuPoker.Cartes.HuitCarreau, JeuPoker.Cartes.HuitCoeur, JeuPoker.Cartes.DameCarreau });
            doublepaire1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.DoublePaire, doublepaire1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(brelan2.CompareTo(doublepaire1) > 0, "Brelan de sept > Double paire Roi-Huit");

            // Double paire 1 & Double paire 2
            j2.Carte1 = JeuPoker.Cartes.RoiTrefle;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var doublepaire2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisTrefle, JeuPoker.Cartes.SeptCoeur, JeuPoker.Cartes.SeptPique, JeuPoker.Cartes.RoiCoeur, JeuPoker.Cartes.CinqPique });
            doublepaire2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.DoublePaire, doublepaire2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(doublepaire1.CompareTo(doublepaire2) > 0, "Double paire Roi-Huit > Double paire Roi-Sept");

            //  Double paire 2 & paire 1
            j1.Carte1 = JeuPoker.Cartes.AsCarreau;
            j1.Carte2 = JeuPoker.Cartes.AsCoeur;
            var paire1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.CinqPique, JeuPoker.Cartes.TroisCarreau, JeuPoker.Cartes.HuitCoeur, JeuPoker.Cartes.DameCarreau });
            paire1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Paire, paire1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(doublepaire2.CompareTo(paire1) > 0, "Double paire Roi-Huit > Paire d'As");

            // paire 1 & paire 2
            j2.Carte1 = JeuPoker.Cartes.RoiTrefle;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var paire2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisTrefle, JeuPoker.Cartes.SeptCoeur, JeuPoker.Cartes.SeptPique, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.CinqPique });
            paire2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Paire, paire2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(paire1.CompareTo(paire2) > 0, "Paire d'As > Paire Sept");

            //  paire 2 & carte 1
            j1.Carte1 = JeuPoker.Cartes.AsCarreau;
            j1.Carte2 = JeuPoker.Cartes.HuitPique;
            var carte1 = new MainPoker(j1, new CartePoker[] { JeuPoker.Cartes.RoiTrefle, JeuPoker.Cartes.CinqPique, JeuPoker.Cartes.TroisCarreau, JeuPoker.Cartes.QuatreCoeur, JeuPoker.Cartes.DameCarreau });
            carte1.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Carte, carte1.ResultatMain.TypeCombinaison);
            Assert.IsTrue(paire2.CompareTo(carte1) > 0, "Paire Sept > As");

            // carte 1 & carte 2
            j2.Carte1 = JeuPoker.Cartes.DameCoeur;
            j2.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var carte2 = new MainPoker(j2, new CartePoker[] { JeuPoker.Cartes.TroisTrefle, JeuPoker.Cartes.SeptCoeur, JeuPoker.Cartes.SixTrefle, JeuPoker.Cartes.DeuxCoeur, JeuPoker.Cartes.HuitTrefle});
            carte2.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.Carte, carte2.ResultatMain.TypeCombinaison);
            Assert.IsTrue(carte1.CompareTo(carte2) > 0, "As > Dame");

            // Carte2 à Quinte flush éliminé
            jElim.Carte1 = JeuPoker.Cartes.AsCoeur;
            jElim.Carte2 = JeuPoker.Cartes.NeufCarreau;
            var quinteFlushElimine = new MainPoker(jElim, new CartePoker[] { JeuPoker.Cartes.RoiCoeur, JeuPoker.Cartes.ValetCoeur, JeuPoker.Cartes.HuitTrefle, JeuPoker.Cartes.DameCoeur, JeuPoker.Cartes.DixCoeur });
            quinteFlushElimine.DeterminerCombinaison();
            Assert.AreEqual(TypeCombinaison.QuinteFlush, quinteFlushElimine.ResultatMain.TypeCombinaison);
            Assert.IsTrue(carte2.CompareTo(quinteFlushElimine) > 0, "Dame > Quinte flush éliminée");
        }
        #endregion
        #endregion

        #region Methodes privées
        /// <summary>
        /// Déclenche une vérification de carte
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="carteAttendue"></param>
        private void VerifierCarte(CartePoker carteAttendue, MainPoker main, int indexCarte)
        {
            var carte = main.ResultatMain.MainGagnante[indexCarte];
            Assert.AreEqual(carte, carteAttendue, string.Format("{0} : carte n°{1}", main.ResultatMain.TypeCombinaison, indexCarte + 1));
        }

        /// <summary>
        /// Déclenche une vérification de hauteur carte
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="carteAttendue"></param>        
        private void VerifierCarte(HauteurCarte hauteurAttendue, MainPoker main, int indexCarte)
        {
            var carte = main.ResultatMain.MainGagnante[indexCarte];
            Assert.AreEqual(carte.Hauteur, hauteurAttendue, string.Format("{0} : carte n°{1}", main.ResultatMain.TypeCombinaison, indexCarte + 1));
        }

        /// <summary>
        /// Déclenche une vérification de couleur carte
        /// </summary>
        /// <param name="carte"></param>
        /// <param name="carteAttendue"></param>
        private void VerifierCarte(CouleurCarte couleurAttendue, MainPoker main, int indexCarte)
        {
            var carte = main.ResultatMain.MainGagnante[indexCarte];
            Assert.AreEqual(carte.Couleur, couleurAttendue, string.Format("{0} : carte n°{1}", main.ResultatMain.TypeCombinaison, indexCarte + 1));
        }

        private MainPoker ConstructeurMainPoker(string nomJoueur, CartePoker carte1, CartePoker carte2, CartePoker[] board)
        { 
            return new MainPoker(new Joueur(nomJoueur) {Carte1 = carte1, Carte2 = carte2}, board);
        }

        private MainPoker ConstructeurMainPoker(CartePoker carte1, CartePoker carte2, CartePoker carte3, CartePoker carte4, CartePoker carte5, CartePoker carte6, CartePoker carte7)
        {
            return new MainPoker(new Joueur("Joueur") { Carte1 = carte1, Carte2 = carte2 }, new CartePoker[] {carte3, carte4, carte5, carte6, carte7});
        }

        #endregion
    }
}

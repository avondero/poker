using Poker.Serveur.Metier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker.Interface.Metier;
using System;

namespace Poker.TestServeur
{
    
    
    /// <summary>
    /// Classe de test de la classe JeuPoker en charge de distribuer les cartes    
    ///</summary>
    [TestClass()]
    public class JeuPokerTest
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

        /// <summary>
        ///Tirage de cinquante deux cartes d'affilée : celles ci doivent toutes se trouver entre 2 et As et entre trèfle et pique
        /// Le tirage de la 53eme doit planter
        ///</summary>
        [TestMethod()]
        public void TirerUneCarteAuHasardTest()
        {
            JeuPoker.ReinitialiserJeu();
            for (int i = 0;i<52;i++)
            {
                var carte = JeuPoker.TirerUneCarteAuHasard();
                int ht = (int) carte.Hauteur ;
                int couleur = (int)carte.Couleur ;
                Assert.IsTrue(ht >= 2 && ht <= 14 && couleur >= 1 && couleur <= 4, carte.ToString());
            }

            var exceptionLevee = false;
            try
            {
                var intrus = JeuPoker.TirerUneCarteAuHasard();
            }
            catch (ApplicationException)
            {
                exceptionLevee = true;
            }
            catch (Exception)
            {
                exceptionLevee = false;
            }
            Assert.IsTrue(exceptionLevee, "Mauvaise exception levée : attendue ApplicationException");
        }
    }
}

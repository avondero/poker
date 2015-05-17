using Poker.Serveur;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Poker.Interface.Metier;
using Poker.Interface.Communication;
using System.Collections.Generic;

namespace Poker.TestServeur
{
    
    
    ///// <summary>
    /////This is a test class for ServeurTest and is intended
    /////to contain all ServeurTest Unit Tests
    /////</summary>
    //[TestClass()]
    //public class ServeurTest
    //{


    //    private TestContext testContextInstance;

    //    /// <summary>
    //    ///Gets or sets the test context which provides
    //    ///information about and functionality for the current test run.
    //    ///</summary>
    //    public TestContext TestContext
    //    {
    //        get
    //        {
    //            return testContextInstance;
    //        }
    //        set
    //        {
    //            testContextInstance = value;
    //        }
    //    }

    //    #region Additional test attributes
    //    // 
    //    //You can use the following additional attributes as you write your tests:
    //    //
    //    //Use ClassInitialize to run code before running the first test in the class
    //    //[ClassInitialize()]
    //    //public static void MyClassInitialize(TestContext testContext)
    //    //{
    //    //}
    //    //
    //    //Use ClassCleanup to run code after all tests in a class have run
    //    //[ClassCleanup()]
    //    //public static void MyClassCleanup()
    //    //{
    //    //}
    //    //
    //    //Use TestInitialize to run code before running each test
    //    //[TestInitialize()]
    //    //public void MyTestInitialize()
    //    //{
    //    //}
    //    //
    //    //Use TestCleanup to run code after each test has run
    //    //[TestCleanup()]
    //    //public void MyTestCleanup()
    //    //{
    //    //}
    //    //
    //    #endregion
        
    //    /// <summary>
    //    ///A test for EnvoyerRegard
    //    ///</summary>
    //    [TestMethod()]
    //    public void EnvoyerRegardTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Guid idExpediteur = new Guid(); // TODO: Initialize to an appropriate value
    //        bool regard = false; // TODO: Initialize to an appropriate value
    //        target.EnvoyerRegard(idExpediteur, regard);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

    //    /// <summary>
    //    ///A test for EnvoyerMessagePublic
    //    ///</summary>
    //    [TestMethod()]
    //    public void EnvoyerMessagePublicTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        ChatMessage msg = null; // TODO: Initialize to an appropriate value
    //        Guid idExpediteur = new Guid(); // TODO: Initialize to an appropriate value
    //        target.EnvoyerMessagePublic(msg, idExpediteur);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }
        
    //    /// <summary>
    //    ///A test for EnvoyerMessageInformation
    //    ///</summary>
    //    [TestMethod()]
    //    public void EnvoyerMessageInformationTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        MessageInfo msg = null; // TODO: Initialize to an appropriate value
    //        target.EnvoyerMessageInformation(msg);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }       
            
    //    /// <summary>
    //    ///A test for EnvoyerAction
    //    ///</summary>
    //    [TestMethod()]
    //    public void EnvoyerActionTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Guid idExpediteur = new Guid(); // TODO: Initialize to an appropriate value
    //        ActionJoueur action = null; // TODO: Initialize to an appropriate value
    //        target.EnvoyerAction(idExpediteur, action);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }
        
    //    /// <summary>
    //    ///A test for DemarrerServeur
    //    ///</summary>
    //    [TestMethod()]
    //    public void DemarrerServeurTest()
    //    {
    //        string adresse = string.Empty; // TODO: Initialize to an appropriate value
    //        //Joueur administrateur = null; // TODO: Initialize to an appropriate value
    //        //bool expected = false; // TODO: Initialize to an appropriate value
    //        //bool actual;
    //        // actual = new Poker.Serveur.Serveur().DemarrerServeur(adresse, administrateur);
    //        //Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for DemarrerPartie
    //    ///</summary>
    //    [TestMethod()]
    //    public void DemarrerPartieTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Guid idAdmin = new Guid(); // TODO: Initialize to an appropriate value
    //        Options optionsJeu = null; // TODO: Initialize to an appropriate value
    //        target.DemarrerPartie(idAdmin, optionsJeu);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

    //    /// <summary>
    //    ///A test for Deconnecter
    //    ///</summary>
    //    [TestMethod()]
    //    public void DeconnecterTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Guid idExpediteur = new Guid(); // TODO: Initialize to an appropriate value
    //        target.Deconnecter(idExpediteur);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }
        
    //    /// <summary>
    //    ///A test for Connecter
    //    ///</summary>
    //    [TestMethod()]
    //    public void ConnecterTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Joueur expediteur = null; // TODO: Initialize to an appropriate value
    //        ResultatConnection expected = null; // TODO: Initialize to an appropriate value
    //        ResultatConnection actual;
    //        actual = target.Connecter(expediteur);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for ChangerOptions
    //    ///</summary>
    //    [TestMethod()]
    //    public void ChangerOptionsTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Guid idExpediteur = new Guid(); // TODO: Initialize to an appropriate value
    //        Options opt = null; // TODO: Initialize to an appropriate value
    //        target.ChangerOptions(idExpediteur, opt);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

    //    /// <summary>
    //    ///A test for AjouterBot
    //    ///</summary>
    //    [TestMethod()]
    //    public void AjouterBotTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur(); // TODO: Initialize to an appropriate value
    //        Guid idAdmin = new Guid(); // TODO: Initialize to an appropriate value
    //        string cheminAssembly = string.Empty; // TODO: Initialize to an appropriate value
    //        string typeBot = string.Empty; // TODO: Initialize to an appropriate value
    //        string nom = string.Empty; // TODO: Initialize to an appropriate value
    //        target.AjouterBot(idAdmin, cheminAssembly, typeBot, nom);
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

    //    /// <summary>
    //    ///A test for Serveur Constructor
    //    ///</summary>
    //    [TestMethod()]
    //    public void ServeurConstructorTest()
    //    {
    //        Poker.Serveur.Serveur target = new Poker.Serveur.Serveur();
    //        Assert.Inconclusive("TODO: Implement code to verify target");
    //    }
    //}
}

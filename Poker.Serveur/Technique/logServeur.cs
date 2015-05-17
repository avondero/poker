using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Poker.Serveur.Technique
{
    /// <summary>
    /// Classe de log     
    /// </summary>
    static class logServeur
    {
        private const string LOG_SERVEUR = @"log\PokerServeur.log";        
        /// <summary>
        /// Log du serveur
        /// </summary>
        /// <param name="message"></param>
        internal static void Debug(string message)
        {
            try
            {                
                System.Diagnostics.Debug.WriteLine(message);                
                FileInfo log = new FileInfo(LOG_SERVEUR);
                log.Directory.Create();
                if (log.Exists && log.Length > 2000000) log.Delete();
                StreamWriter wr  = log.AppendText();
                wr.WriteLine(string.Format("{0} : {1}", DateTime.Now, message));
                wr.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur en loggant : " + message + ", " + ex.Message);
            }
        }

        /// <summary>
        /// Log du serveur aussi
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prms"></param>
        internal static void Debug(string message, params object[] prms)
        {
            try
            {
                logServeur.Debug(string.Format(message, prms));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur en loggant : " + message + ", " + ex.Message);
            }
        }        
    }
}

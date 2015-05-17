using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Poker.Client
{
    static class logClient
    {
        private const string LOG_CLIENT = @"log\PokerClient.log";
       
        /// <summary>
        /// Log du client
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            try
            {
                FileInfo log = new FileInfo(LOG_CLIENT);
                log.Directory.Create();
                if (log.Exists && log.Length > 2000000) log.Delete();                
                StreamWriter wr = log.AppendText();
                wr.WriteLine(string.Format("{0} : {1}", DateTime.Now, message));
                wr.Close();
                System.Diagnostics.Debug.WriteLine(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur en loggant : " + message + ", " + ex.Message);
            }
        }

        /// <summary>
        /// Log du client aussi
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prms"></param>
        public static void Debug(string message, params object[] prms)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format(message, prms));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur en loggant : " + message + ", " + ex.Message);
            }
        }        
    }
}



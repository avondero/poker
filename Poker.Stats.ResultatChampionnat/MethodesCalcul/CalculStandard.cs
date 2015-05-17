using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Stats.ResultatChampionnat.MethodesCalcul
{
    class CalculStandard : ICalculClassement
    {
        #region ICalculClassement Members        
        /// <summary>
        /// Calcul du classement
        /// </summary>
        /// <param name="listeParties"></param>
        /// <returns>Renvoie la liste des joueurs avec leurs points</returns>
        public  List<Joueur> CalculerClassement(List<Partie> listeParties)
        {
            Dictionary<string, Joueur> classement = new Dictionary<string, Joueur>();
            foreach (Partie p in listeParties)
            {
                foreach (Joueur j in p.ListeJoueurs)
                {
                    if (!classement.ContainsKey(j.Nom))
                    {
                        classement[j.Nom] = j.Clone() as Joueur;
                        classement[j.Nom].Resultat = "0";
                    }

                    classement[j.Nom].Resultat = (int.Parse(classement[j.Nom].Resultat) + j.PositionElimination - 1).ToString();
                }
            }
            
            return classement.Values.ToList<Joueur>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Libelle
        {
            get
            {
                return "0 point pour le dernier, +1 ensuite";
            }
        }

        public string LibelleColonneResultat
        {
            get
            {
                return "Points";
            }
        }
        #endregion
    }
}

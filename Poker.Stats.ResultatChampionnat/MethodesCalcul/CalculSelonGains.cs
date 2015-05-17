using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Stats.ResultatChampionnat.MethodesCalcul
{
    class CalculSelonGains : ICalculClassement
    {

        #region ICalculClassement Members

        public List<Joueur> CalculerClassement(List<Partie> listeParties)
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
                }
                classement[p.ListeJoueurs[0].Nom].Resultat = (int.Parse(classement[p.ListeJoueurs[0].Nom].Resultat) + p.Enjeu).ToString();
            }

            return classement.Values.ToList<Joueur>();
        }

        public string Libelle
        {
            get {return "Selons les gains"; }
        }

        public string LibelleColonneResultat
        {
            get { return "Gains"; }
        }

        #endregion
    }
}

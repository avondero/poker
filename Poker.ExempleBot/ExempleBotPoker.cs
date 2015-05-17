using System;
using System.Collections.Generic;
using System.Text;
using Poker.Interface.Bot;
using Poker.Interface.Metier;
using Poker.Interface.Communication;
using System.Runtime.Serialization;

namespace Poker.ExempleBot
{
    /// <summary>
    /// Exemple de classe de bot
    /// </summary>
    [Bot("Exemple de bot", 0)]
    public class ExempleBotPoker : BotPokerBase
    {
        private int _choixPasse;
        private Random _rand = new Random();

        public ExempleBotPoker()
        {
            _choixPasse = _rand.Next(10); 
            // 0-3 : fait toujours parole (ou passe)
            // 4-8 : suit toujours
            // 9 : suit toujours mais relance s'il a un As ou un Roi
        }

        /// <summary>
        /// Le bot passe une fois sur 2 donnes et suit une fois sur 2 donnes
        /// </summary>
        /// <param name="msg"></param>
        public override void RecevoirMessageInformation(MessageInfo msg)
        {
            if (msg.TypeMessage == MessageJeu.NouvelleDonne)
            {
                _choixPasse = _rand.Next(10);
                if (_rand.Next(3) == 0) _serveur.EnvoyerMessagePublic(new ChatMessage("Encore une donne pffft"), _identifiantConnection);                
            }
        }

        /// <summary>
        /// Une fois l'action recue : on rappelle
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="timeout"></param>
        public override void RecevoirListeActionsPossibles(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout)
        {
            System.Threading.Thread.Sleep(_rand.Next(1000));

            ActionJoueur act;
            if (_choixPasse <= 3)
                act = new ActionJoueur(TypeActionJoueur.Passe);
            else if (_choixPasse >=4 && _choixPasse <= 8 )
            {
                act = SuitOuParole(actions);
            }
            else
            {
                if (_bot.Carte1.Hauteur == HauteurCarte.As || _bot.Carte2.Hauteur == HauteurCarte.As)
                {
                    if (actions.ContainsKey(TypeActionJoueur.Relance))
                    {
                        act = new ActionJoueur(TypeActionJoueur.Relance, 2 * actions[TypeActionJoueur.Relance].MontantMax);
                    }
                    else if (actions.ContainsKey(TypeActionJoueur.Mise))
                    {
                        act = new ActionJoueur(TypeActionJoueur.Mise, 2 * actions[TypeActionJoueur.Mise].MontantMax);
                    }
                    else
                    {
                        act = new ActionJoueur(TypeActionJoueur.Tapis, _bot.TapisJoueur);
                    }
                }
                else
                {
                    act = SuitOuParole(actions);
                }
            }

            _serveur.EnvoyerAction(_identifiantConnection, act);
        }

        private ActionJoueur SuitOuParole(Dictionary<TypeActionJoueur, ActionPossible> actions)
        {
            ActionJoueur res = null;
            if (actions.ContainsKey(TypeActionJoueur.Parole))
            {
                res = new ActionJoueur(TypeActionJoueur.Parole);
            }
            else if (actions.ContainsKey(TypeActionJoueur.Suit))
            {
                res = new ActionJoueur(TypeActionJoueur.Suit, actions[TypeActionJoueur.Suit].MontantMax);
            }
            else
            { 
                res = new ActionJoueur(TypeActionJoueur.Tapis, actions[TypeActionJoueur.Tapis].MontantMax);
            }

            return res;
        }
    }
}

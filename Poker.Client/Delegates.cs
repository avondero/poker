using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poker.Interface.Metier;
using Poker.Interface.Stats;
using Poker.Interface.ExtensionsClient.Replay;

namespace Poker.Client
{
    public delegate void MessagePublicHandler(ChatMessage msg, string expediteur);
    public delegate void MessageInformationHandler(MessageInfo message);
    public delegate void NouveauJoueurHandler(Joueur nouveauJoueur, int positionTable);
    public delegate void ChangementInfosJoueurHandler(Joueur joueurAChanger, EtatMain etatDeLaMain, CartePoker carte1, CartePoker carte2, int? pot);
    public delegate void ChangementInfosJoueurSansCartesHandler(Joueur joueurAChanger, int? pot);
    public delegate void RegardHandler(Joueur expediteur, EtatMain etatDeLaMain);
    public delegate void ActionsPossiblesHandler(Dictionary<TypeActionJoueur, ActionPossible> actions, int timeout);
    public delegate void OptionsPartieHandler(Options optionsPartie);        
}

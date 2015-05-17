using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Client
{
    /// <summary>
    /// Mode de fonctionnement du client
    /// </summary>
    public enum ModeClient
    {
        Jeu,
        LecturePartie
    }

    public enum TypeJeuCarte
    { 
        Normal = 1,
        GrossesCartes = 2,
        Sympathique = 3,
        QuatreSaisons = 4
    }
}

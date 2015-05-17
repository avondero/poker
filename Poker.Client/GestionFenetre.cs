using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Poker.Client
{
    /// <summary>
    /// Classe de gestion des fenetres (encapsulation des API Windows)
    /// </summary>
    static class GestionFenetre
    {
        #region DllImports
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
        #endregion

        #region Constantes
        //Stop flashing. The system restores the window to its original state.
        private const UInt32 FLASHW_STOP = 0;
        //Flash the window caption.
        private const UInt32 FLASHW_CAPTION = 1;
        //Flash the taskbar button.
        private const UInt32 FLASHW_TRAY = 2;
        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        private const UInt32 FLASHW_ALL = 3;
        //Flash continuously, until the FLASHW_STOP flag is set.
        private const UInt32 FLASHW_TIMER = 4;
        //Flash continuously until the window comes to the foreground.
        private const UInt32 FLASHW_TIMERNOFG = 12;
        #endregion

        #region Methodes publiques statiques
        /// <summary>
        /// Permet de faire clignoter la fenetre nbFois et ce jusqu'à ce qu'elle ait le focus
        /// </summary>
        /// <param name="nbFois">Nombre de clignotements</param>
        public static void ClignoterFenetre(byte nbFois)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = Process.GetCurrentProcess().MainWindowHandle;
            fInfo.dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG;
            fInfo.uCount = nbFois;
            fInfo.dwTimeout = 0;
            FlashWindowEx(ref fInfo);

        }
        #endregion

    }
}

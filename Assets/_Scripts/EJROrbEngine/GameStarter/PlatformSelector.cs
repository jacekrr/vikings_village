﻿// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAndroid;
using ClientAbstract;
using UnityEngine;
using ClientWinPC;

namespace GameStarter
{
    public enum Platform { WinPC, WinVR, Android, iOS}

    public class PlatformSelector : MonoBehaviour
    {
        public Platform DefaultPlatform;

        public ISceneConfigurator CreateSceneConfigurator()
        {
            if (DefaultPlatform == Platform.WinPC && System.Environment.GetCommandLineArgs().Length > 1 && System.Environment.GetCommandLineArgs()[1] == "WinVR")
                DefaultPlatform = Platform.WinVR;
            ISceneConfigurator konfigurator = null;
            switch(DefaultPlatform)
            {              
                case Platform.Android:
                    konfigurator = new AndroidSceneConfigurator();
                    break;
                case Platform.WinPC:
                    konfigurator = new WinPCSceneConfigurator();
                    break;
                default:
                    konfigurator = new AndroidSceneConfigurator();
                    break;
            }
            return konfigurator;
        }
        public IGameState CreateGameStateManager()
        {
            switch (DefaultPlatform)
            {
                case Platform.WinPC:
                    return new WinPCGameStateManager();
                case Platform.Android:
                    return new AndroidGameStateManager();
                default:
                    return new AndroidGameStateManager();
            }
        }
        public IInputManager CreateInputManager(GameObject mainObject)
        {
            switch (DefaultPlatform)
            {
                case Platform.WinPC:
                    return mainObject.GetComponent<WinPCInputManager>();
                case Platform.Android:
                    return mainObject.GetComponent<AndroidInputManager>();
                default:
                    return mainObject.GetComponent<AndroidInputManager>();
            }
        }
       

    }
}
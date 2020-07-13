// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.Characters
{
   
    public sealed class CharactersModuleManager : MonoBehaviour, IEngineModule
    {
        private Dictionary<string, BaseDataAddon> _characterTemplates;      // wzorce danych postaci takich jak npc, animals itp

        public static CharactersModuleManager Instance { get; private set; }
        public PlayerCharacter ThePlayer { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy CharactersModuleManager");
            Instance = this;
            _characterTemplates = new Dictionary<string, BaseDataAddon>();
        }

        public void OnLoad(IGameState gameState)
        {
            ThePlayer.LoadGame(gameState);
        }
        public void OnSave(IGameState gameState)
        {
            ThePlayer.SaveGame(gameState);
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            if (_characterTemplates.Count == 0)
            {
                //postacie
                XmlDataInfo charInfo = Utils.LoadXmlAssetFile("data/characters", "characters");
                if (charInfo != null)
                {
                    foreach (XElement element in charInfo.MainNodeElements)
                    {
                        BaseDataAddon nowyCharData = new BaseDataAddon();
                        nowyCharData.LoadData(charInfo, element);
                        _characterTemplates.Add(nowyCharData.Type, nowyCharData);
                    }
                }

            }
            ThePlayer = new PlayerCharacter(FindCharacterData("Player"));
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            BaseDataAddon charData = FindCharacterData(ao.Type);
            if (charData != null)
            {
                ao.DataObjects.AddDataAddon("characters", charData);
                ao.gameObject.AddComponent<SceneCharacter>();
            }
        }

        //próbuje dopasować wzorzec postaci do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindCharacterData(string type)
        {
            if (_characterTemplates.ContainsKey(type))
                return _characterTemplates[type];
            return null;
        }

    }
}


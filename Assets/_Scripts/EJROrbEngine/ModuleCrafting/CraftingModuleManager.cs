// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.Crafting
{
   
    public sealed class CraftingModuleManager : MonoBehaviour, IEngineModule
    {
        private Dictionary<string, RecipeDataAddon> _recipes;                        //wszystkie receptury dostepne w grze, klucz: nazwa receptury

        public static CraftingModuleManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy CraftingModuleManager");
            Instance = this;

            _recipes = new Dictionary<string, RecipeDataAddon>();
        }

        public void OnLoad(IGameState gameState)
        {
            // do nothing
        }
        public void OnSave(IGameState gameState)
        {
            // do nothing
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            //receptury
            if (_recipes.Count == 0)
            {
                XmlDataInfo recipesInfo = Utils.LoadXmlAssetFile("data/recipes", "recipes");
                if (recipesInfo != null)
                {
                    foreach (XElement element in recipesInfo.MainNodeElements)
                    {
                        RecipeDataAddon nowaReceptura = new RecipeDataAddon();
                        nowaReceptura.LoadData(recipesInfo, element);
                        _recipes.Add(nowaReceptura.Type, nowaReceptura);
                    }
                }
            }
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            RecipeDataAddon recipe = FindRecipe(ao.Type);
            if (recipe != null)
                ao.DataObjects.AddDataAddon("recipes", recipe);
        }

        //próbuje dopasować wzorzec receptury do podanego typu i jeśli mu się uda - zwraca go
        private RecipeDataAddon FindRecipe(string type)
        {
            if (_recipes.ContainsKey(type))
                return _recipes[type];
            return null;
        }

    }
}


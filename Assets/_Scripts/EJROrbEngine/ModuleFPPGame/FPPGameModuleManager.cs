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
using EJROrbEngine.NPCSystem;
using EJROrbEngine.PlayerInventory;

namespace EJROrbEngine.FPPGame
{
   
    public sealed class FPPGameModuleManager : MonoBehaviour, IEngineModule
    {
        //items
        private Dictionary<string, ItemDataAddon> _basicItemTemplates;     // wzorce wszystkich  przedmiotow, klucz: typ przedmiotu, wartosc: struktura z informacjami
        private Dictionary<string, BaseDataAddon> _weaponTemplates;           // wzorce broni, klucz: typ przedmiotu, wartosc: struktura z informacjami
        private Dictionary<string, BaseDataAddon> _destructibles;      // wzorce danych obiektow niszczalnych, a wiec zarowno NPCs jak i np. drzew
        private Dictionary<string, BaseDataAddon> _enemyTemplates;      // wzorce danych postaci wrogow

        public Inventory TheInventory { get; private set; }

        public static FPPGameModuleManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy FPPGameModuleManager");
            Instance = this;
            _basicItemTemplates = new Dictionary<string, ItemDataAddon>();
            _weaponTemplates = new Dictionary<string, BaseDataAddon>();
            _destructibles = new Dictionary<string, BaseDataAddon>();
            _enemyTemplates = new Dictionary<string, BaseDataAddon>();
            TheInventory = new Inventory();

        }

        public void OnLoad(IGameState gameState)
        {
            TheInventory.LoadGame(gameState);
        }
        public void CleanupBeforeSave()
        {
            // do nothing
        }
        public void OnSave(IGameState gameState)
        {
            TheInventory.SaveGame(gameState);
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            if (_basicItemTemplates.Count == 0)
            {
                //przedmioty ogolnie
                XmlDataInfo itemsInfo = Utils.LoadXmlAssetFile("data/items", "items");
                if (itemsInfo != null)
                {
                    foreach (XElement element in itemsInfo.MainNodeElements)
                    {
                        ItemDataAddon newItem = new ItemDataAddon();
                        newItem.LoadData(itemsInfo, element);
                        _basicItemTemplates.Add(newItem.Type, newItem);
                    }
                }
            } 
            if (_weaponTemplates.Count == 0)
            {
                //bronie
                XmlDataInfo weaponsInfo = Utils.LoadXmlAssetFile("data/weapons", "weapons");
                if (weaponsInfo != null)
                {
                    foreach (XElement element in weaponsInfo.MainNodeElements)
                    {
                        WeaponDataAddon newItem = new WeaponDataAddon();
                        newItem.LoadData(weaponsInfo, element);
                        _weaponTemplates.Add(newItem.Type, newItem);
                    }
                }
                //desctructibles
                XmlDataInfo destrInfo = Utils.LoadXmlAssetFile("data/destructibles", "destructibles");
                if (destrInfo != null)
                {
                    foreach (XElement element in destrInfo.MainNodeElements)
                    {
                        BaseDataAddon newItem = new BaseDataAddon();
                        newItem.LoadData(destrInfo, element);
                        _destructibles.Add(newItem.Type, newItem);
                    }
                }
            }
            if (_enemyTemplates.Count == 0)
            {
                //wrogowie
                XmlDataInfo enemyInfo = Utils.LoadXmlAssetFile("data/enemies", "enemies");
                if (enemyInfo != null)
                {
                    foreach (XElement element in enemyInfo.MainNodeElements)
                    {
                        BaseDataAddon nowyEnemyInfo = new BaseDataAddon();
                        nowyEnemyInfo.LoadData(enemyInfo, element);
                        _enemyTemplates.Add(nowyEnemyInfo.Type, nowyEnemyInfo);
                    }
                }
            }
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            
            ItemDataAddon itemData = FindItem(ao.Type);
            if (itemData != null)
            {
                ao.DataObjects.AddDataAddon("items", itemData);
                if(ao.gameObject.GetComponent< SceneItem>() == null)
                    ao.gameObject.AddComponent<SceneItem>();
            }
            BaseDataAddon destrData = FindDestructible(ao.Type);
            if (destrData != null)
            {
                ao.DataObjects.AddDataAddon("destructibles", destrData);
                if (ao.gameObject.GetComponent<SceneDestructible>() == null)
                    ao.gameObject.AddComponent<SceneDestructible>();
            }
            BaseDataAddon weapData = FindWeapon(ao.Type);
            if (weapData != null)
            {
                ao.DataObjects.AddDataAddon("weapons", weapData);
                if (ao.gameObject.GetComponent<SceneWeapon>() == null)
                    ao.gameObject.AddComponent<SceneWeapon>();
                SceneWeaponHitter[] wphs = ao.gameObject.GetComponentsInChildren<SceneWeaponHitter>();
                foreach (SceneWeaponHitter swh in wphs)
                    swh.RealWeapon = ao.gameObject.GetComponent<SceneWeapon>();
                if(wphs.Length == 0)
                    ao.gameObject.AddComponent<SceneWeaponHitter>().RealWeapon = ao.gameObject.GetComponent<SceneWeapon>();
            }
            BaseDataAddon enemyData = FindEnemyData(ao.Type);
            if (enemyData != null)
            {
                ao.DataObjects.AddDataAddon("enemies", enemyData);
                //ao.gameObject.AddComponent<SceneNPC>();
            }
        }


        //szuka przedmiotow posiadajacych atrybut o nazwie attr i wartosci val i zwraca liste takich przedmiotow (wartosci na liscie to typy przedmiotow)
        public List<string> FindItemWithAttr(string attr, string val)
        {
            List<string> retList = new List<string>();
            foreach (BaseDataAddon bda in _basicItemTemplates.Values)
                if (bda[attr] != null && bda[attr].ToString() == val)
                    retList.Add(bda.Type);
            /*foreach (MedicalItem bda in _medicalItemTemplates.Values)
                if (bda[attr] != null && bda[attr].ToString() == val)
                    retList.Add(bda.Type); */
            foreach (BaseDataAddon bda in _weaponTemplates.Values)
                if (bda[attr] != null && bda[attr].ToString() == val)
                    retList.Add(bda.Type);
            /*     foreach (BaseDataAddon bda in _recipes.Values)
                     if (bda[attr] != null && bda[attr].ToString() == val)
                         retList.Add(bda.Type);*/
            return retList;
        }
        //zwraca Nazwe losowo wybranego przedmiotu
        public string GetRandomItem()
        {
            int losowyIndeks = UnityEngine.Random.Range(0, _basicItemTemplates.Keys.Count);
            int aktualnyIndeks = 0;
            foreach (string nazwa in _basicItemTemplates.Keys)
                if (++aktualnyIndeks > losowyIndeks)
                    return nazwa;
            return null;
        }

        //próbuje dopasować wzorzec przedmiotu do podanego typu i jeśli mu się uda - zwraca go
        public ItemDataAddon FindItem(string type)
        {
            if (_basicItemTemplates.ContainsKey(type))
                return _basicItemTemplates[type];
            return null;
        }

        //próbuje dopasować wzorzec destructible do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindDestructible(string type)
        {
            if (_destructibles.ContainsKey(type))
                return _destructibles[type];
            return null;
        }
        //próbuje dopasować wzorzec broni do podanego typu i jeśli mu się uda - zwraca go
        public WeaponDataAddon FindWeapon(string type)
        {
            if (_weaponTemplates.ContainsKey(type))
                return (WeaponDataAddon) _weaponTemplates[type];
            return null;
        }
        //próbuje dopasować wzorzec wroga do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindEnemyData(string type)
        {
            if (_enemyTemplates.ContainsKey(type))
                return _enemyTemplates[type];
            return null;
        }

    }
}


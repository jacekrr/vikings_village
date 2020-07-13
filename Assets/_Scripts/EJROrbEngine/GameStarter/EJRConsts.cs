// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EJROrbEngine
{
    public class ABInfo
    {
        public string Name;                     //name of AB and it's file
        public List<string> ObjectsToLoad;      //list of objects to be loaded
        public bool IsInitializingAB;           //is or not bundle that must be loaded at first
    }
    //utility class that loads particular game configuration from Resources/data/config.xml (or it's representation from a server) and share it to the rest of code. 
    //provides also some simplified properties for accessing common info like ABs list to load, platform or DEBUG information
    public sealed class EJRConsts
    {
        //singleton instance property
        public static EJRConsts Instance { get { if (_instance == null) _instance = new EJRConsts(); return _instance; }  }
        public string this[string name]
        { 
            get
            {
                if (_constValues.ContainsKey(name))
                    return _constValues[name];
                return "";
            }
            private set
            {
                if (_constValues.ContainsKey(name))
                    _constValues[name] = value;
                else _constValues.Add(name, value);
            }
        }
        public string ENGINE_VER { get {return this["engineVer"]; } }
        public bool DEBUG { get { return this["DEBUG"] == "true"; } }
        public string SAVE_DIR { get { return this["Save"]; } }
        public bool usingSteam() { return this["useSteam"] == "true"; }
        //list of asset bundles that need to be loaded (not including Initializing AB)
        public List<ABInfo> AssetBundlesToLoad { get { List<ABInfo> retList = new List<ABInfo>(); retList.AddRange(_assetBundles.Values); return retList ; } }
        //initializing AB (must be loaded at first)
        public ABInfo InitializingAssetBundle { get; private set; }
        //list of code modules to be used in a game
        public List<string> ModulesToLoad { get; private set; }

        private Dictionary<string, string> _constValues;        //pairs of settings key-value
        private Dictionary<string, ABInfo> _assetBundles;       //asset bundles to load (key is an AB name, value is info of AB)
        private static EJRConsts _instance;
        private StringBuilder _cachedLog;

        private EJRConsts()
        {
            _instance = this;
            _constValues = new Dictionary<string, string>();
            _assetBundles = new Dictionary<string, ABInfo>();
            ModulesToLoad = new List<string>();
            LoadFromResources();
        }

        public void DebugLog(string msg)
        {
            if (DEBUG)
                Debug.Log(msg);
        }
    
        public void AddCachedLog(string msg)
        {
            if (_cachedLog == null)
                _cachedLog = new StringBuilder();
            _cachedLog.Append(msg + "\n");
        }
        public void OutputCachedLog()
        {
            if (DEBUG && _cachedLog != null)
                Debug.Log(_cachedLog.ToString());
            _cachedLog = null;
        }

        public string GetSaveDir()
        {
#if UNITY_STANDALONE
            return SAVE_DIR;
#else  
            
            string AppPath = Application.dataPath;
            
            return AppPath +"\\EJR\\" + SAVE_DIR;
#endif
        }


        private void LoadFromResources()
        {
            XmlDataInfo entInfo = Utils.LoadXmlAssetFile("data/config", "config");
            foreach (XElement element in entInfo.MainNodeElements)
            {
                if (element.Name.ToString() == "setting")
                    this[element.Attribute("name").Value] = element.Value;
                if(element.Name.ToString() == "assetBundle")
                {
                    ABInfo abInfo = new ABInfo() { Name = element.Attribute("name").Value, IsInitializingAB = element.Attribute("initializing") != null, ObjectsToLoad = new List<string>() };
                    foreach (XElement subElement in element.Elements())
                        if (subElement.Name.ToString() == "load")
                            abInfo.ObjectsToLoad.Add(subElement.Value);
                    if (abInfo.IsInitializingAB)
                        InitializingAssetBundle = abInfo;
                    else 
                        _assetBundles.Add(abInfo.Name, abInfo);
                }
                if (element.Name.ToString() == "module")
                    ModulesToLoad.Add(element.Value);
            }
        }
    }
}




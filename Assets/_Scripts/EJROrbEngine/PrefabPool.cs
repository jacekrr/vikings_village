// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using System.Collections.Generic;
using EJROrbEngine.SceneObjects;

namespace EJROrbEngine
{
    //PrefabPool holds and manages objects that are used frequently on the scene and we want to create them as rarely as possible to avoid fps hiccups, the PrefabPool creates objects only if needed
    //and make them inactive and active rather than destroy and recreate, it will only destroy objects of there's unnormal amount of them on the free objects list
    
    //PrefabPool doesnt track prefabs that are currently used. Object who uses a prefab need to keep it and release when stop using. This refactoring was done to make prefab releasing faster
    //because usedPrefabs.Remove(prefab) showed as very slow (there could be 1000 prefabs of type in use and removing is slow).
    public sealed class PrefabPool: MonoBehaviour
    {
        public static PrefabPool Instance { get; private set; }
        
        //objects that are marked as unused currently, key is an object's name (type of objects, for example TreeConifier - there may be many objects of this type), value - a list of these objects
        private Dictionary<string, List<GameObject>> _freePrefabs;
   
        //objects that are marked as used currently, key is an object's name, value - a list of these objects
        //currently changed to not use this list - the object who is using a prefab need to keep pointer to int and release in right time!
        //private Dictionary<string, List<GameObject>> _usedPrefabs;

        //objects that are templates for objects on free and used lists, templates are treated as a number 0 prefab (they might be used also)
        private Dictionary<string, GameObject> _prefabTemplates;    

        private int _debugFreeCount, _debugUsedCount; //these are debug information vars

        void Start()
        {
            _freePrefabs = new Dictionary<string, List<GameObject>>();
            //_usedPrefabs = new Dictionary<string, List<GameObject>>();
            _debugFreeCount = 0;
            _debugUsedCount = 0;
            _prefabTemplates = new Dictionary<string, GameObject>();         
        }
       
        //get object from free objects, generate on if there's not enough free objects of this type
        public GameObject GetPrefab(string aname, bool configure = true)
        {
            GameObject retComp = null;
            InitListIfEmpty(aname);
            if (_freePrefabs[aname].Count == 0)    //there's no enough free prefabs ? bad luck, we need to generate more and then try to continue
                SpawnNewPrefabs(aname);
            if (_freePrefabs[aname].Count > 0)
            {                
                retComp = _freePrefabs[aname][0];
                _freePrefabs[aname].RemoveAt(0);
               // _usedPrefabs[aname].Add(retComp);
                _debugUsedCount++;
                _debugFreeCount--;
                // templates are stored inactive on the scene, so will be newly spawned prefabs, and their's children - we must activate them now
                retComp.SetActive(true);
                for (int i = 0; i < retComp.transform.childCount; ++i)
                {
                    retComp.transform.GetChild(i).gameObject.SetActive(true);
                }
                if (configure && retComp.GetComponent<PrefabTemplate>() != null)
                    retComp.GetComponent<PrefabTemplate>().Configure();
                return retComp;
            }
            //Oh no, still nothing. Something must be broken in scene design or Resources
            Debug.Log("PrefabPool was unable to spawn a  prefab " + aname);
            return null;
        }

        //the prefab is not needed anymore - move it to the free list and make inactive, destroy only if there's too many objects of this type
        public void ReleasePrefab(GameObject prefab)
        {
            if (prefab != null)
            {
                InitListIfEmpty(prefab.name);
                //put it on free list only if on the free list is short, in order to not make free list too large
                if (_freePrefabs[prefab.name].Count > 32 /*&& _freePrefabs[prefab.name].Count > _usedPrefabs[prefab.name].Count*/)
                {
                    //_usedPrefabs[prefab.name].Remove(prefab);
                    DestroyImmediate(prefab);
                }
                else
                {
                    _freePrefabs[prefab.name].Add(prefab);
                    prefab.SetActive(false);
                    //_usedPrefabs[prefab.name].Remove(prefab);
                    _debugFreeCount++;
                }
                _debugUsedCount--;
            }
        }

        // same like the ReleasePrefab(GameObject prefab), but if caller has more than one prefab to be released it will be a little faster to use this function
        public void ReleasePrefabs(string aname, List<GameObject> prefabs)
        {
            InitListIfEmpty(aname);
            _freePrefabs[aname].AddRange(prefabs);            
            _debugUsedCount += prefabs.Count;
            _debugFreeCount-= prefabs.Count;
            foreach (GameObject fc in prefabs)
            {
                fc.SetActive(false);
                //_usedPrefabs[aname].Remove(fc);
            }
        }
                
        //creates a clone from prefab objects list, but not add it to any list, caller will have to manage the new object on it's own
        //this function is located here in PrefabPool because it manages templates from which the prefabs can be spwaned, but it's NOT a pooled gameobject and it's ALWAYS spawned and created (this function is SLOW)
        public GameObject InstantiateRaw(string aname, Vector3 newPosition, Quaternion newRotation)
        {
            if (!_prefabTemplates.ContainsKey(aname))
                LoadTemplate(aname);
            if (_prefabTemplates.ContainsKey(aname))
            {
                GameObject newGO = (GameObject)GameObject.Instantiate(_prefabTemplates[aname], newPosition, newRotation);
                newGO.transform.position = newPosition;
                newGO.transform.rotation = newRotation;
                newGO.name = aname;
                newGO.transform.parent = this.transform;
                return newGO;
            }
            return null;
        }

        //get a template of given name (null if there's no such template), the template is not instantiated. This function is intended for checking properties of the template like it's components and to for instantiating prefabs
        //from the template. Use GetPrefab or InstantiateRaw instead.
        public GameObject ShowTemplate(string aName)
        {
            if (!_prefabTemplates.ContainsKey(aName))
                 LoadTemplate(aName);
            if (_prefabTemplates.ContainsKey(aName))
                return _prefabTemplates[aName];
            return null;
        }

        //force load a template of a given name, it will be used after loading a map file with a list of objects prefabs used in the map
        public void ForceLoadTemplate(string aname)
        {
            LoadTemplate(aname);
        }

        // *** PRIVATE functions ***
        // initialization of list of object's of given type
        private void InitListIfEmpty(string aname)
        {
            if (!_freePrefabs.ContainsKey(aname))
            {
                _freePrefabs.Add(aname, new List<GameObject>());
                //_usedPrefabs.Add(aname, new List<GameObject>());
            }
        }
        //load a template game object from Resources if there's no object of given type on the templates list. Template is NOT instantiated after loading.
        private void LoadTemplate(string aname)
        {
            if (!_prefabTemplates.ContainsKey(aname))
            {
                GameObject templ = Utils.LoadObjectFromAssets(typeof(GameObject), "prefabs/" + aname) as GameObject;
                if (templ == null)
                    Debug.Log("Can't load: " + aname);
                _prefabTemplates.Add(aname, templ);
            }
        }
        //spawn a new game object of a given type, it will be cloned from a template (and the template will be created as well if there's none)
        private void SpawnNewPrefabs(string aname)
        {            
            if (!_prefabTemplates.ContainsKey(aname))
                LoadTemplate(aname);
            if (_prefabTemplates.ContainsKey(aname))
            {
                GameObject newGO = GameObject.Instantiate(_prefabTemplates[aname], _prefabTemplates[aname].transform.position, _prefabTemplates[aname].transform.rotation);
                newGO.name = aname;
                newGO.transform.parent = this.transform;
                _freePrefabs[aname].Add(newGO);
                _debugFreeCount++;
            }
        }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy PrefabPool");
            Instance = this;

        }
    }
}
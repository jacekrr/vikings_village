// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.EndlessWorld;
using System.Collections.Generic;
using UnityEngine;


namespace EJROrbEngine.ActiveObjects
{
    public class ActiveObjectsSpawner : MonoBehaviour
    {
        public const float RECALC_TIME = 0.49f;          //jak często sprawdzać odległość do gracza [s.]
        [Tooltip("Obiekty dozwolone do utworzenia.")]
        public string[] ObjectsToGenerate;
        [Tooltip("Prawdopodobienstwo utworzenia obiektów, musi mieć tyle samo elementów co ObjectsToGenerate.")]
        public int[] Propabilities;                    
        [Tooltip("Jak wysoko tworzyć obiekt nad ziemią [m].")]
        public float OffsetY = 0.3f;                      
        [Tooltip("True wymusza zmianę ziarna losowości dla każdego spawnera (pierwsze ziarno będzie nadane na podstawie znacznika czasu). False oznacza ziarno zależne od pierwszego położenia na mapie.")]
        public bool AlwaysChangeSeed = false;            
        [Tooltip("True oznacza, że generacja obiektów zostanie automatycznie wykonana podczas pierwszego wywołania Update. False - generacja musi być wyzwolona zdarzeniem bądź z kodu.")]
        public bool AutoSpawn = true;                
        [Tooltip("Losowy rozrzut miejsca generacji na mapie. Wartość 0 oznacza generację w miejscu ustawienia spawnera na wysokości OffsetY. Inna wartość - losowe położenie z uwzględnieniem wysokości terenu (OffsetY ponad terenem) i kolizji. Jeśli kilka kolejnych losowań zawsze wskaże kolizję - zostanie podjęta próba generacji w miejscu ustawienia spawnera.")]
        public float RandomRadius = 0;
        [Tooltip("Wygeneruj więcej niż 1 obiekt. Generuje takie same obiekty. Jest uwzględniane TYLKO jeśli RandomRadius jest większy niż 0.")]
        public int MultipleSpawn = 0;
        [Tooltip("Dla RandomRadius różnego od 0, CollisionRadius określa promień sprawdzania kolizji. Przyjmuje się minimim 0.1m.")]
        public float CollisionRadius = 0.1f;

        private float _timer;
        public static int Seed = 0;
        public void Spawn()
        {
            InitSeed();
            string objectToBeGenerated = "";
            string[] generateTheseObjects = new string[ObjectsToGenerate.Length];
            for (int i = 0; i < ObjectsToGenerate.Length; i++)
                generateTheseObjects[i] = ObjectsToGenerate[i];
            int randomNumber = UnityEngine.Random.Range(0, 100);
            int randCumulative = 0;
            for (int i = 0; i < generateTheseObjects.Length && objectToBeGenerated == ""; i++)
            {
                randCumulative += Propabilities[i];
                if (randCumulative > randomNumber)
                    objectToBeGenerated = ObjectsToGenerate[i];
            }
            if (objectToBeGenerated != "")
            {
                GameManager.Instance.TheGameState.SetKey("genid_" + UniqueID(), 1);
                InternalSpawn(objectToBeGenerated);
            }
        }
        public void ResetSpawner()
        {
            this.enabled = true;
            GameManager.Instance.TheGameState.DeleteKey("genid_" + UniqueID());
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }

        private void InternalSpawn(string prefabName)
        {
            if (prefabName == null || prefabName == "" )
                return;
            if(prefabName.Contains("[attr]") && !GameManager.Instance.IsModuleLoaded("FPPGame"))
            {
                prefabName = prefabName.Replace("[attr]", "");
                string []tokens = prefabName.Split('=');
                if (tokens.Length < 2)
                    prefabName = null;
                else
                {
                    string attrName = tokens[0];
                    string attrValue = tokens[1];
                    List<string> itemsList = FPPGame.FPPGameModuleManager.Instance.FindItemWithAttr(attrName, attrValue);
                    if (itemsList.Count == 0)
                        prefabName = null;
                    else
                        prefabName = itemsList[Random.Range(0, itemsList.Count)];
                }
            }
            if (prefabName != null)
            {
                Vector3[] positions = FindSpawnPositions();
                for (int i = 0; i < positions.Length; i++)
                {
                    GameObject createdObject = ActiveObjectsManager.Instance.CreateAvtiveObject(prefabName, positions[i]);                    
                    createdObject.SetActive(true);
                    if (createdObject.GetComponent<ParticleSystem>() != null)
                    {
                        createdObject.GetComponent<ParticleSystem>().Stop();
                        createdObject.GetComponent<ParticleSystem>().Clear();
                        createdObject.GetComponent<ParticleSystem>().Simulate(0, true, true);
                        createdObject.GetComponent<ParticleSystem>().Play();
                    }
                }
            }
        }
        private Vector3[] FindSpawnPositions()
        {
            if (CollisionRadius < 0.1f)
                CollisionRadius = 0.1f;
            if(RandomRadius <= 0.01f)
            {
                Vector3[] positions = new Vector3[1];
                positions[0] = new Vector3(transform.position.x, transform.position.y + OffsetY, transform.position.z);
                return positions;
            }
            else
            {
                Vector3[] positions = new Vector3[MultipleSpawn >= 1 ? MultipleSpawn : 1];
                for (int i = 0; i < positions.Length; i++)
                {
                    bool positionValid = false;
                    for(int tries = 0; tries < 5 && !positionValid; tries++)
                    {
                        float X = transform.position.x + Random.Range(-RandomRadius, RandomRadius);
                        float Z = transform.position.z + Random.Range(-RandomRadius, RandomRadius);
                        float Y = EndlessWorldModuleManager.Instance.CurrentBiom.TheTerrain.GetComponent<Terrain>().SampleHeight(new Vector3(X, 0, Z)) + OffsetY;
                        positions[i] = new Vector3(X, Z, Y);
                        RaycastHit info;
                        positionValid = !Physics.SphereCast(new Vector3(X, Y + CollisionRadius, Z), CollisionRadius, Vector3.down, out info);
                    }
                    if(!positionValid)
                        positions[i] = new Vector3(transform.position.x, transform.position.y + OffsetY, transform.position.z);
                }
                return positions;
            }            
        }
        private void InitSeed()
        {
            if (AlwaysChangeSeed)
            {
                UnityEngine.Random.InitState(Seed);
                Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
            else
            {
                //inicjuj ziarno liczba okreslona na podstawie id generatora
                UnityEngine.Random.InitState(UniqueID().GetHashCode());
            }
        }
        private string UniqueID()
        {
            if (transform.GetComponent<ActiveObject>() != null)
                return name + "_" + transform.GetComponent<ActiveObject>().UniqueID.ToString();
            if (transform.parent.transform.GetComponent<ActiveObject>() != null)
                return name + "_" + transform.parent.transform.GetComponent<ActiveObject>().UniqueID.ToString();
            return transform.parent.name + "_" + transform.localPosition.x.ToString() + "_" + transform.localPosition.z.ToString();
        }
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _timer = RECALC_TIME;
                float playerWorldX = Camera.main.transform.position.x;
                float playerWorldZ = Camera.main.transform.position.z;
                float rangeMax = ActiveObjectsManager.ACTIVEOBJECTS_MAXIMUM_VISIBILITY;
                bool czyPoiwnienBycWidoczny = transform.position.x > playerWorldX - rangeMax && transform.position.x < playerWorldX + rangeMax && transform.position.z > playerWorldZ - rangeMax && transform.position.z < playerWorldZ + rangeMax;
                if (czyPoiwnienBycWidoczny)
                {
                    if (GameManager.Instance.TheGameState.KeyExists("genid_" + UniqueID()))
                    {
                        //                        Destroy(this);
                        this.enabled = false;
                        if (GetComponent<Renderer>() != null)
                            GetComponent<Renderer>().enabled = false;
                        for (int i = 0; i < transform.childCount; i++)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else if (AutoSpawn)
                        Spawn();
                }
            }
        }
        private void Start()
        {
            if (ObjectsToGenerate.Length != Propabilities.Length)
                Debug.LogError("Nieprawidłowa wielkość tablic generatora w " + transform.position.ToString());
            _timer = RECALC_TIME;
            if(GetComponent<Renderer>() != null)
                GetComponent<Renderer>().enabled = false;
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;           
        }

    }
}

// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System.Collections.Generic;
using UnityEngine;

namespace EJROrbEngine.EndlessWorld
{
    //representation of one biom during the game (used for bioms visbility calculations by WorldManager)
    public class Biom
    {
        public static int BIOM_SIZE = 256;                      //Biom size in meters

        public int BiomX { get; private set; }                  //biom coords in bioms area
        public int BiomZ { get; private set; }                  //biom coords in bioms area
        public int CurrentLODLevel { get; private set; }          //current LOD, 0 = full terrain, 1 = LOD terrain, 2 = invisible
        public GameObject TheTerrain { get; private set; }      //the terrain game object
        public GameObject TheTerrainLOD { get; private set; }   //the terrain LOD game object
        public GameObject TheMainGO { get { return _mainObject; } }   //bioms GO
        public int HashKey { get { return BiomX + BiomZ * 65536; } }        //the key that can be used to uniquely identify the biom

        private GameObject _mainObject;                 //the biom game object
        private PrefabPool _prefabPool;
        private MapConfig _mapConfig;
        private List<MapObjectInfo> _objectsTracked;

        public static int BiomHashKey(int bX, int bZ)
        {
            return bX + bZ * 65536;
        }

        public Biom(MapConfig md, PrefabPool pool, int bX, int bZ)
        {
            BiomX = bX;
            BiomZ = bZ;
            _prefabPool = pool;
            _mapConfig = md;
            CurrentLODLevel = 2;
            BiomX = NormalizeSBX(BiomX, md.BiomsCountX);
            BiomZ = NormalizeSBZ(BiomZ, md.BiomsCountZ);
            _objectsTracked = new List<MapObjectInfo>();
            //     EngineConsts.DebugLog("Biom (" + BiomX + "," + BiomY + ") generated ");
        }
        //init the biom            
        public void InitBiom(GameObject BiomsRoot, EndlessWorldModuleManager worldManager, int firstLOD)
        {
            string mainName = "Maps" + "/" + _mapConfig.MapName + "/Bioms/" + _mapConfig.MapName + "Biom_x" + BiomX + "_y" + BiomZ + ".prefab";
            _mainObject = _prefabPool.GetPrefab(mainName, false);
            if (_mainObject == null)
                EJRConsts.Instance.DebugLog("NO Terrain PREFAB in biom : " + BiomX + "," + BiomZ);
            else
            {
                _mainObject.transform.position = worldManager.GetBiomPosition(this);
                _mainObject.transform.rotation = Quaternion.identity;
                _mainObject.SetActive(true);   
                _mainObject.transform.parent = BiomsRoot.transform;
                _mainObject.name = _mapConfig.MapName + "Biom_x" + BiomX + "_y" + BiomZ;
            }
            SetLODLevel(firstLOD);
        }
        public void SetLODLevel(int newLOD)
        {
            CurrentLODLevel = newLOD;
            if (_mainObject != null && _mainObject.activeInHierarchy)
            {
                if (CurrentLODLevel > 0)
                {
                    if (TheTerrainLOD == null)
                    {
                        if (TheTerrain != null)
                        {
                            PrefabPool.Instance.ReleasePrefab(TheTerrain);
                            TheTerrain = null;
                        }
                        string terrainName = "Maps" + "/" + _mapConfig.MapName + "/Terrains/LOD/LOD" + _mapConfig.MapName + "Terrain_x" + BiomX + "_y" + BiomZ + ".prefab";
                        TheTerrainLOD = PrefabPool.Instance.GetPrefab(terrainName, false);
                        TheTerrainLOD.SetActive(true);
                        TheTerrainLOD.transform.parent = _mainObject.transform;
                        TheTerrainLOD.transform.localPosition = Vector3.zero;
                        
                    }
                }
                else
                {
                    if (TheTerrain == null)
                    {
                        if (TheTerrainLOD != null)
                        {
                            PrefabPool.Instance.ReleasePrefab(TheTerrainLOD);
                            TheTerrainLOD = null;
                        }
                        string terrainName = "Maps" + "/" + _mapConfig.MapName + "/Terrains/" + _mapConfig.MapName + "Terrain_x" + BiomX + "_y" + BiomZ + ".prefab";
                        TheTerrain = PrefabPool.Instance.GetPrefab(terrainName, false);
                        TheTerrain.SetActive(true);
                        TheTerrain.transform.parent = _mainObject.transform;
                        TheTerrain.transform.localPosition = Vector3.zero;
                    }

                }
               
            }
        }
        public void ReleaseObjects()
        {
            if (TheTerrain != null)
                _prefabPool.ReleasePrefab(TheTerrain);
            if (TheTerrainLOD != null)
                _prefabPool.ReleasePrefab(TheTerrainLOD);
            if (_mainObject != null)
                _prefabPool.ReleasePrefab(_mainObject);
        }
       
        public static int NormalizeSBX(int sBX, int superbiomsSizeX)
        {
            int sx = sBX;
            while (sx < 0)
                sx += superbiomsSizeX;
            while (sx >= superbiomsSizeX)
                sx -= superbiomsSizeX;
            return sx;
        }
        public static int NormalizeSBZ(int sBZ, int superbiomsSizeZ)
        {
            int sz = sBZ;
            while (sz < 0)
                sz += superbiomsSizeZ;
            while (sz >= superbiomsSizeZ)
                sz -= superbiomsSizeZ;
            return sz;
        }

        public void AddObjectsToTrack(List<MapObjectInfo> objects)
        {
            _objectsTracked.AddRange(objects);
        }



    }


}

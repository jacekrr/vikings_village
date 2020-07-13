// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EJROrbEngine.EndlessWorld
{

    public class MapObjectInfo
    {
        public string ObjectName;
        public Vector3 ObjectPosition;
        public Quaternion ObjectRotation;
        public Vector3 ObjectScale;
        public int MaxLOD;
        public GameObject SceneObject;
        public Biom TheBiom;
        public VisibilityGroupInfo MyGroup;
    }
    public class VisibilityGroupInfo
    {
        public List<MapObjectInfo> ObjectList;       
    }
    /*
    public class ActiveObjectInfo
    {
        public int UniqueID;
        public Vector3 ChangedPosition;
        public Quaternion ChangedRotation;
        public Dictionary<string, string> AddonValues;              //additional data for AO's components

        public ActiveObjectInfo()
        {
            AddonValues = new Dictionary<string, string>();
        }
    }*/

    public class MapManager 
    {
        private Dictionary<string, List<MapObjectInfo>> _mapInfo;               //key is a biom name, value - list data of MapObjectInfo objects

        private string _mapName;
        public MapManager(string mapName)
        {
            _mapInfo = new Dictionary<string, List<MapObjectInfo>>();
            _mapName = mapName;
        }

        public void LoadMap(bool editorUse)
        {
            byte[] data;
            if (editorUse)
            {
                string fileName = Application.dataPath + "/prefabs/maps/" + _mapName + "/config/map.bytes";
                data = Utils.LoadBinaryFile(fileName);
            }
            else
                data = Utils.LoadBinaryAssetFile("map.bytes");
            if (data.Length < 3 * sizeof(int))
                Debug.LogError("Invalid map file");
            else
            {
                int readIndex = 0;
                int formatNumber = ReadIntFromBytes(data, ref readIndex);
                int biomsXCount = ReadIntFromBytes(data, ref readIndex);
                int biomsZCount = ReadIntFromBytes(data, ref readIndex);
                if (formatNumber == 3)  //one single file format
                {
                    for (int biomX = 0; biomX < biomsXCount; biomX++)
                        for (int biomZ = 0; biomZ < biomsZCount; biomZ++)
                            ReadSingleBiomFromMap(data, ref readIndex, biomX, biomZ);                       
                }  else if(formatNumber == 4) //biom in separate files
                {
                    for (int biomX = 0; biomX < biomsXCount; biomX++)
                        for (int biomZ = 0; biomZ < biomsZCount; biomZ++)
                        {
                            int internalReadIndex = 0;
                            if (editorUse)
                            {
                                string fileName = Application.dataPath + "/prefabs/maps/" + _mapName + "/config/biom" + biomX + "_" + biomZ + ".bytes";
                                data = Utils.LoadBinaryFile(fileName);
                            }
                            else
                                data = Utils.LoadBinaryAssetFile("biom" + biomX + "_" + biomZ + ".bytes");
                            ReadSingleBiomFromMap(data, ref internalReadIndex, biomX, biomZ);
                        }
                }
            }
        }
        private void ReadSingleBiomFromMap(byte[] data, ref int readIndex, int biomX, int biomZ)
        {
            int objsCount = ReadIntFromBytes(data, ref readIndex);
            List<MapObjectInfo> objList = new List<MapObjectInfo>();
            for (int gIndex = 0; gIndex < objsCount; gIndex++)
            {
                MapObjectInfo moi = new MapObjectInfo();
                moi.ObjectName = ReadStringFromBytes(data, ref readIndex);
                moi.ObjectPosition = ReadVector3FromBytes(data, ref readIndex);
                moi.ObjectRotation = ReadQuaternionFromBytes(data, ref readIndex);
                moi.ObjectScale = ReadVector3FromBytes(data, ref readIndex);
                moi.MaxLOD = ReadIntFromBytes(data, ref readIndex);
                objList.Add(moi);
            }
            SetBiomMapObjectsData(biomX, biomZ, objList);
        }


#if UNITY_EDITOR
        public void CreateEmptyStructure(EJRSceneBuild.MapEditConfig editConfig)
        {
            int biomsXCount = editConfig.BiomsCountX;
            int biomsZCount = editConfig.BiomsCountZ;
            for (int biomX = 0; biomX < biomsXCount; biomX++)
                for (int biomZ = 0; biomZ < biomsZCount; biomZ++)
                {
                    List<MapObjectInfo> objList = new List<MapObjectInfo>();
                    SetBiomMapObjectsData(biomX, biomZ, objList);
                }

        }
        public void SaveMap(EJRSceneBuild.MapEditConfig editConfig)
        {
            string fileName = Application.dataPath + "/prefabs/maps/" + _mapName + "/config/map.bytes";
            using (BinaryWriter b = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                b.Write(4); //format number
                b.Write(editConfig.BiomsCountX);
                b.Write(editConfig.BiomsCountZ);
            }
            for (int biomX = 0; biomX < editConfig.BiomsCountX; biomX++)
                for (int biomZ = 0; biomZ < editConfig.BiomsCountZ; biomZ++)
                {
                    fileName = Application.dataPath + "/prefabs/maps/" + _mapName + "/config/biom" + biomX + "_" + biomZ + ".bytes";
                    using (BinaryWriter b2 = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        SaveSingleBiomToMapFile(biomX, biomZ, b2);
                }                                                        
        }
        private void SaveSingleBiomToMapFile(int biomX, int biomZ, BinaryWriter b)
        {
            List<MapObjectInfo> objsList = GetBiomMapObjectsData(biomX, biomZ);
            if (objsList == null)
                b.Write(0);
            else
            {
                b.Write(objsList.Count);
                for (int gIndex = 0; gIndex < objsList.Count; gIndex++)
                {
                    byte[] strBytes2 = System.Text.Encoding.UTF8.GetBytes(objsList[gIndex].ObjectName);
                    Array.Resize(ref strBytes2, 32);
                    b.Write(strBytes2);
                    b.Write(objsList[gIndex].ObjectPosition.x);
                    b.Write(objsList[gIndex].ObjectPosition.y);
                    b.Write(objsList[gIndex].ObjectPosition.z);
                    b.Write(objsList[gIndex].ObjectRotation.x);
                    b.Write(objsList[gIndex].ObjectRotation.y);
                    b.Write(objsList[gIndex].ObjectRotation.z);
                    b.Write(objsList[gIndex].ObjectRotation.w);
                    b.Write(objsList[gIndex].ObjectScale.x);
                    b.Write(objsList[gIndex].ObjectScale.y);
                    b.Write(objsList[gIndex].ObjectScale.z);
                    b.Write(objsList[gIndex].MaxLOD);
                }
            }
        }
#endif
        public List<MapObjectInfo> GetBiomMapObjectsData(int biomX, int biomZ)
        {
            if (_mapInfo.ContainsKey("x" + biomX + "z" + biomZ))
                return _mapInfo["x" + biomX + "z" + biomZ];
            else
                return null;
        }
        public void SetBiomMapObjectsData(int biomX, int biomZ, List<MapObjectInfo> infoList)
        {
            if (_mapInfo.ContainsKey("x" + biomX + "z" + biomZ))
                _mapInfo["x" + biomX + "z" + biomZ] = infoList;
            else
                _mapInfo.Add("x" + biomX + "z" + biomZ, infoList);
        }

        private bool ReadBoolFromBytes(byte[] bytes, ref int readIndex)
        {
            if (bytes.Length < readIndex + 1)
                throw new Exception("index out of range in MapManager.ReadBoolFromBytes");
            byte value = bytes[readIndex++];
            return value == 1;
        }
        private int ReadIntFromBytes(byte [] bytes, ref int readIndex)
        {
            if (bytes.Length < readIndex + sizeof(int))
                throw new Exception("index out of range in MapManager.ReadIntFromBytes");
            int value = BitConverter.ToInt32(bytes, readIndex);
            readIndex += sizeof(int);
            return value;
        }
        private string ReadStringFromBytes(byte[] bytes, ref int readIndex)
        {
            if (bytes.Length < readIndex + 32)
                throw new Exception("index out of range in MapManager.ReadStringFromBytes");
            //      string value = BitConverter.ToString(bytes, readIndex, 32);
            string value = System.Text.Encoding.UTF8.GetString(bytes, readIndex, 32);
            value = value.TrimEnd('\0');
            readIndex += 32;
            return value;
        }
        private Vector3 ReadVector3FromBytes(byte[] bytes, ref int readIndex)
        {
            if (bytes.Length < readIndex + 3 * sizeof(float))
                throw new Exception("index out of range in MapManager.ReadVector3FromBytes");
            float xval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            float yval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            float zval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            return new Vector3(xval, yval, zval);
        }
        private Quaternion ReadQuaternionFromBytes(byte[] bytes, ref int readIndex)
        {
            if (bytes.Length < readIndex + 4 * sizeof(float))
                throw new Exception("index out of range in MapManager.ReadQuaternionFromBytes");
            float xval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            float yval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            float zval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            float wval = BitConverter.ToSingle(bytes, readIndex);
            readIndex += sizeof(float);
            return new Quaternion(xval, yval, zval, wval);
        }
    }
}
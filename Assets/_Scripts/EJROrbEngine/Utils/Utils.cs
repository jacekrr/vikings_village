// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine
{
   
    public class Utils 
    {
        public static Object LoadObjectFromAssets (System.Type type, string assetName)
        {
            Object obj = null;
            string abName = Utils.MakeResourceNameForAB(assetName);
            AssetBundle ab =  ABsManager.Instance.FindBundleWithAsset(abName);
            if(ab != null)
                obj = ab.LoadAsset(abName, type);
            if (obj == null)
                obj = Resources.Load( Utils.MakeResourceNameForResources(assetName), type);
            return obj;
        }
        //util xml loading func, from Resources or loaded asset bundles
        public static XmlDataInfo LoadXmlAssetFile(string assetName, string mainNode)
        {
            TextAsset txt = LoadObjectFromAssets(typeof(TextAsset), assetName) as TextAsset;
            if (txt == null)
            {
                Debug.LogError("Cant load : " + assetName);
                return null;
            }
            else
            {
                StringBuilder content = new StringBuilder();
                StringReader reader = new StringReader(txt.text);
                content.Append(reader.ReadToEnd());
                reader.Close();
                string cnt = content.ToString();
                XDocument doc = XDocument.Parse(cnt);                
                XmlDataInfo info = new XmlDataInfo(doc, mainNode);
                return info;
            }
        }



        //full name like Prefabs/Maps/Siberia1/config/Config.xml is changed into single asset name that is used in ABs like Config.xml
        public static string MakeResourceNameForAB(string fullResName)
        {
            if (fullResName.IndexOf(".") == -1)
                fullResName = fullResName + ".prefab";
            return fullResName.Substring(fullResName.LastIndexOf("/") + 1);
        }

        //full name like Prefabs/Maps/Siberia1/config/Config.xml is changed into resource-style asset name like Prefabs/Maps/Siberia1/config/Config
        public static string MakeResourceNameForResources(string fullResName)
        {
            if (fullResName.LastIndexOf(".") != -1)
                return fullResName.Substring(0, fullResName.LastIndexOf("."));
            else
                return fullResName;
            
        }
        //utility function, reads asset text file and returns its content as binary
        public static byte[] LoadBinaryAssetFile(string fileName)
        {
            TextAsset theAsset = LoadObjectFromAssets(typeof(TextAsset), fileName) as TextAsset;
            if (theAsset  == null)
            {
                Debug.LogError("Cant load : " + fileName);
                return null;
            }
            else
            {
                return theAsset.bytes;
            }
            
        }

        public static byte[] LoadBinaryFile(string fileName)
        {
            
            using (BinaryReader b = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                byte[] bytes = new byte[b.BaseStream.Length];
                b.Read(bytes, 0, (int)b.BaseStream.Length);
                return bytes;
            }

        }
        /*
        public static void LoadAndCoonvert()
        {
            TextAsset txtPL = LoadObjectFromAssets(typeof(TextAsset), "PL") as TextAsset;
            StringBuilder content = new StringBuilder();
            StringReader reader = new StringReader(txtPL.text);
            content.Append(reader.ReadToEnd());
            reader.Close();
            string cntPL = content.ToString();

            TextAsset txtEN = LoadObjectFromAssets(typeof(TextAsset), "EN") as TextAsset;
            content = new StringBuilder();
            reader = new StringReader(txtEN.text);
            content.Append(reader.ReadToEnd());
            reader.Close();
            string cntEN = content.ToString();

            XDocument docPL = XDocument.Parse(cntPL);
            XDocument docEN = XDocument.Parse(cntEN);

            foreach (XElement xe in docPL.Root.Elements("string"))
            {
                string id = xe.Attribute("name").Value;
                foreach (XElement xeEN in docEN.Root.Elements("string"))
                {
                    string idEN = xeEN.Attribute("name").Value;
                    if (id == idEN)
                        xe.Value = xeEN.Value;
                }
            }

            StreamWriter strumien;
            strumien = File.CreateText("final.xml");
            docPL.Save(strumien);
            strumien.Close();
            


        }

    */
    }

}

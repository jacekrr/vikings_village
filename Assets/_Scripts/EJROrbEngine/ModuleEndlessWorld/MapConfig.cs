// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.EndlessWorld
{
    //Map configuration, game version
    public class MapConfig
    {
        public string MapName { get; private set; }             //name of the map
        public int BiomsCountX { get; private set; }            //number of bioms, x axis
        public int BiomsCountZ { get; private set; }            //number of bioms, z axis
        public Vector3 StartPosition{ get; private set; }       //default start position of the player (when there is no save)

        public MapConfig(IEnumerable<XElement> nodes)
        {
            StartPosition = Vector3.zero;
            ParseDataXML(nodes);
        }
        private void ParseDataXML(IEnumerable<XElement> nodes)
        {
            try
            {
                foreach (XElement node in nodes)
                {
                    if (node.Name == "BiomsCountX")
                        BiomsCountX = int.Parse(node.Value);
                    else if (node.Name == "BiomsCountZ")
                        BiomsCountZ = int.Parse(node.Value);
                    else if (node.Name == "MapName")
                        MapName = node.Value;
                    else if (node.Name == "StartX")
                        StartPosition = new Vector3(float.Parse(node.Value), StartPosition.y, StartPosition.z);
                    else if (node.Name == "StartZ")
                        StartPosition = new Vector3(StartPosition.x, StartPosition.y, float.Parse(node.Value));
                    else if (node.Name == "StartY")
                        StartPosition = new Vector3(StartPosition.x, float.Parse(node.Value), StartPosition.z);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Błąd parsowania pliku konfiguracyjnego mapy " + e.Message);
            }
        }
    }
}
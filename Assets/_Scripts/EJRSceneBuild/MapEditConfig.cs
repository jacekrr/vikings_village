// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJRSceneBuild
{
    //Map configuration, editor version
    public class MapEditConfig
    {
        public string MapName { get; private set; }                 // Map name
        public string TerrainSource { get; private set; }           // asset source of terrains (generated with heightmap only)
        public string DefaultTexture { get; private set; }          // default terrain texture (asset path)
        public string DefaultNormalTexture { get; private set; }    // default terrain normal texture (asset path)
        public int BiomsCountX { get; private set; }                //number of bioms, x axis
        public int BiomsCountZ { get; private set; }                //number of bioms, z axis
        public List<TerrainObjectConfig> Trees { get; private set; } //list of trees configuration objects

        public MapEditConfig(IEnumerable<XElement> nodes, IEnumerable<XElement> treesNodes)
        {
            ParseDataXML(nodes);
            Trees = new List<TerrainObjectConfig>();
            ParseTreesDataXML(treesNodes);
        }
        private void ParseDataXML(IEnumerable<XElement> nodes)
        {
            foreach (XElement node in nodes)
            {
                if (node.Name == "TerrainSource")
                    TerrainSource = node.Value;
                else if (node.Name == "DefaultTexture")
                    DefaultTexture = node.Value;
                else if (node.Name == "DefaultNormalTexture")
                    DefaultNormalTexture = node.Value;
                else if (node.Name == "MapName")
                    MapName = node.Value;
                else if(node.Name == "BiomsCountX")
                    BiomsCountX = int.Parse(node.Value);
                else if(node.Name == "BiomsCountZ")
                    BiomsCountZ = int.Parse(node.Value);
            }
        }
        private void ParseTreesDataXML(IEnumerable<XElement> treesNodes)
        {
            foreach (XElement node in treesNodes)
            {
                if (node.Name == "Tree")
                    Trees.Add(new TerrainObjectConfig(node));
            }
        }
    }
}
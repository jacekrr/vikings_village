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
    //terrain tree configuration (editor)
    public class TerrainObjectConfig
    {
        public int NumberMin { get; private set; }
        public int NumberMax { get; private set; }
        public float HeightVariance { get; private set; }
        public int MinTerrainHeight { get; private set; }
        public int MaxTerrainHeight { get; private set; }
        public int MinSlope { get; private set; }
        public int MaxSlope { get; private set; }
        public string PrefabPath { get; private set; }

        public TerrainObjectConfig(XElement node)
        {            
            ParseDataXML(node);
        }
        private void ParseDataXML(XElement node)
        {
            try
            {
                if (node.Attribute("numMin") != null)
                    NumberMin = int.Parse(node.Attribute("numMin").Value);
                if (node.Attribute("numMax") != null)
                    NumberMax = int.Parse(node.Attribute("numMax").Value);
                if (node.Attribute("heightVariance") != null)
                    HeightVariance = float.Parse(node.Attribute("heightVariance").Value, System.Globalization.CultureInfo.InvariantCulture);
                if (node.Attribute("minTerrainHeight") != null)
                    MinTerrainHeight = int.Parse(node.Attribute("minTerrainHeight").Value);
                if (node.Attribute("maxTerrainHeight") != null)
                    MaxTerrainHeight = int.Parse(node.Attribute("maxTerrainHeight").Value);
                if (node.Attribute("minSlope") != null)
                    MinSlope = int.Parse(node.Attribute("minSlope").Value);
                if (node.Attribute("maxSlope") != null)
                    MaxSlope = int.Parse(node.Attribute("maxSlope").Value);
                PrefabPath = node.Value;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Błąd parsowania pliku konfiguracyjnego drzew " + e.Message);
            }
}

    }
}
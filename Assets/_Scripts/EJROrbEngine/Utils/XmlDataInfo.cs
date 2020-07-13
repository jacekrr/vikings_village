// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine
{
    public class XmlDataInfo
    {
        public IEnumerable<XElement> MainNodeElements;
        public Dictionary<string, string> AttributeTypes;
        public Dictionary<string, object> AttributeDefault;

        public XmlDataInfo(XDocument doc, string mainNode)
        {
            AttributeTypes = new Dictionary<string, string>();
            AttributeDefault = new Dictionary<string, object>();
            if (doc.Root.Name == mainNode)
                MainNodeElements = doc.Root.Elements();
            else
            {
                XElement main = doc.Root.Element(mainNode);
                MainNodeElements = main.Elements();

                XElement scheme = doc.Root.Element("attrTypes");
                if (scheme != null)
                    foreach (XAttribute attrElem in scheme.Attributes())
                        AttributeTypes.Add(attrElem.Name.LocalName, attrElem.Value);

                XElement defaults = doc.Root.Element("attrDefaults");
                if (defaults != null)
                    foreach (XAttribute attrElem in defaults.Attributes())
                        AttributeDefault.Add(attrElem.Name.LocalName, ParseAttribute(attrElem));
            }
        }

        public object GetDefaultForAttribute(string attrName)
        {
            if (AttributeDefault.ContainsKey(attrName))
                return AttributeDefault[attrName];
            else return null;
        }

        public object ParseAttribute(XAttribute attr)
        {
            object data = attr.Value;
            try
            {
                System.Type t = typeof(string);
                if (AttributeTypes.ContainsKey(attr.Name.LocalName))
                {
                    if (AttributeTypes[attr.Name.LocalName] == "int")
                        data = int.Parse(attr.Value);
                    else if (AttributeTypes[attr.Name.LocalName] == "float")
                        data = float.Parse(attr.Value, CultureInfo.InvariantCulture);
                    else if (AttributeTypes[attr.Name.LocalName] == "bool")
                        data = bool.Parse(attr.Value);
                }
            } catch (Exception)
            {
                Debug.LogError("Błąd parsowania atrybutu: " + attr.Name.LocalName);
            }
            return data;

        }
    }


}

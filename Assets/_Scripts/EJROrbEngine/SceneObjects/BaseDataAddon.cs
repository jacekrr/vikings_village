// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.
// **** Plik jest czescia projektu UniversalClient 
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of UniversalClient project
// **** Copyrights: EJR Sp. z o.o.

using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.SceneObjects
{
    //structure with data from xml configuration
    //it contains a hashtable of attributes
    public class BaseDataAddon
    {
        public string Type { get; protected set; }
        
        public object this[string attribKey]
        {
            get
            {
                if (_attribs.ContainsKey(attribKey))
                    return _attribs[attribKey];
                return null;
            }
            protected set
            {
                if (_attribs.ContainsKey(attribKey))
                    _attribs[attribKey] = value;
                else
                    _attribs.Add(attribKey, value);
            }
        }
        private Dictionary<string, object> _attribs;

        public BaseDataAddon()
        {
            _attribs = new Dictionary<string, object>();
        }
        public BaseDataAddon(BaseDataAddon klonujZTego)
        {
            _attribs = new Dictionary<string, object>();
            foreach (string attrName in klonujZTego._attribs.Keys)
                _attribs.Add(attrName, klonujZTego[attrName]);
            Type = klonujZTego.Type;
        }
        //zaladuj dane przedmiotu z pojedynczego wezla XML 
        public virtual void LoadData(XmlDataInfo dataInfo, XElement elementXMLDanych)
        {
            _attribs = new Dictionary<string, object>();
            //read attribures from the data node
            foreach(XAttribute attr in elementXMLDanych.Attributes())
            {
                if (attr.Name.LocalName == "type")
                    Type = attr.Value;
                else
                    _attribs.Add(attr.Name.LocalName, dataInfo.ParseAttribute(attr));
            }
            //check if any attribute is ommited and spawn a default value for it
            foreach (string attrName in dataInfo.AttributeTypes.Keys)
            {
                if (!_attribs.ContainsKey(attrName) && dataInfo.AttributeDefault.ContainsKey(attrName))
                    _attribs.Add(attrName, dataInfo.GetDefaultForAttribute(attrName));
            }
          

        }
    }
}

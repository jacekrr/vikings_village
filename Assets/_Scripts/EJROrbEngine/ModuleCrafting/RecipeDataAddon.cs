// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.Crafting
{

    //receptura rzemieślnicza
    public class RecipeDataAddon : BaseDataAddon
    {
        public List<string> Ingredients;             // przedmioty niezbedne do realizacji receptury (zawarte w Resources/Prefaby)
        public string Result;                    // przedmiot bedacy efektem wykonania receptury (rezultat)
        public RecipeDataAddon()
        {
            Ingredients = new List<string>();
        }
               
        //zaladuj dane receptury z pojedynczego wezla XML 
        public override void LoadData(XmlDataInfo dataInfo, XElement elementXMLDanych)
        {
            base.LoadData(dataInfo, elementXMLDanych);
         
            foreach (XElement podElement in elementXMLDanych.Elements())
            {
                if (podElement.Name == "ingredient")
                    Ingredients.Add(podElement.Value);
                else if (podElement.Name == "result")
                    Result = podElement.Value;
            }
        }      
    }
}

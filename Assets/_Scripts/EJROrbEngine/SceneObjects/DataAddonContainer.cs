// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System.Collections.Generic;

namespace EJROrbEngine.SceneObjects
{
    public class DataAddonContainer
    {
        private Dictionary<string, BaseDataAddon> DataObjects;       // data of all object's classes, Key - name of class, Value - data of this class

        public BaseDataAddon GetDataAddon(string addonClass)
        {
            if (DataObjects.ContainsKey(addonClass))
                return DataObjects[addonClass];
            return null;
        }
        public DataAddonContainer()
        {
            DataObjects = new Dictionary<string, BaseDataAddon>();
        }

        public void AddDataAddon(string addonClass, BaseDataAddon addon)
        {
            if (!DataObjects.ContainsKey(addonClass))
                DataObjects.Add(addonClass, addon);
        }
    }
}
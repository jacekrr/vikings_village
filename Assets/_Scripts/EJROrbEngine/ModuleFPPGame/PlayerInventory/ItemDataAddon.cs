// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using EJROrbEngine.FightSystem;
using EJROrbEngine.SceneObjects;
using UnityEngine;

namespace EJROrbEngine.PlayerInventory
{

    public class ItemDataAddon : BaseDataAddon
    {       
        public int Count { get; set; }                                   //how many of these item is stacked here
        

        public ItemDataAddon()
        {
            Count = 1;
        
        }
        public ItemDataAddon(int aCnt)
        {
            Count = aCnt;
        
        }


    }

}
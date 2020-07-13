// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using System.Collections.Generic;

namespace EJROrbEngine.PlayerInventory
{
    public class Inventory
    {
        //przedmiot na pozycji pozycja, null oznacza brak przedmiotu w plecaku na tej pozycji
        public ItemDataAddon this[int position]
        {
            get
            {
                if (position >= 0 && position < _items.Count)
                    return _items[position];
                else
                    return null;
            }
        }
        public int Count {  get { return _items.Count; } }
        //przedmiot na aktywnej pozycji pozycja, null oznacza brak przedmiotu na aktywnej pozycji
        public ItemDataAddon ActiveItem
        {
            get
            {
                return this[_selectedItem];
            }
        }

        private List<ItemDataAddon> _items;
        private int _selectedItem;

        public Inventory()
        {
            _items = new List<ItemDataAddon>();
            _selectedItem = 0;
        }

        public void LoadGame(IGameState aGameState)
        {
            int inventoryCount = aGameState.GetIntKey("inv_cnt");
            for (int i = 0; i < inventoryCount; i++)
            {
                string itemName = aGameState.GetStringKey("itm_" + i);
                int itemCount = aGameState.GetIntKey("itmc_" + i);
                if (itemName != null && itemName != "")
                {
                    ItemDataAddon it = FPPGame.FPPGameModuleManager.Instance.FindItem(itemName);
                    Add(it);
                }
            }
        }
        public void SaveGame(IGameState aGameState)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                aGameState.SetKey("itm_" + i, _items[i].Type);
                aGameState.SetKey("itmc_" + i, _items[i].Count);
            }
            aGameState.SetKey("inv_cnt", _items.Count);
        }

        //Dodaje przedmiot do plecaka na pierwsze wolne miejsce 
        //Stackuje przedmioty jeśli już istnieja
        public void Add(ItemDataAddon  addedItem)
        {
            ItemDataAddon existingItem = null;
            foreach (ItemDataAddon it in _items)
                if (it.Type == addedItem.Type)
                    existingItem = it;
            if (existingItem != null)
                existingItem.Count++;
            else
                _items.Add(addedItem);
        }
        
        //przesuwa wybrany panel o przesuniecie (1 w prawo -1 w lewo)
        public void MoveActiveItem(int przesuniecie)
        {
            _selectedItem -= przesuniecie;
            if (_selectedItem < 0)
                _selectedItem = _items.Count - 1;
            if (_selectedItem >= _items.Count)
                _selectedItem = 0;
        }
        //usuwa przedmiot z plecaka 
        public void RemoveItem(ItemDataAddon whichItem)
        {
            ItemDataAddon existingItem = null;
            foreach (ItemDataAddon it in _items)
                if (it.Type == whichItem.Type)
                    existingItem = it;
            if(existingItem != null)
            {
                if (existingItem.Count > 1)
                    existingItem.Count--;
                else
                {
                    _items.Remove(whichItem);
                    if (_selectedItem >= _items.Count)
                        _selectedItem = 0;
                }
            }
        }
        //zwraca true jesli plecak posiada przedmiot o podanej nazwie, lub false jesli nie
        public bool ItemExists(string itemType)
        {
            foreach (ItemDataAddon p in _items)
                if (p != null && p.Type == itemType)
                    return true;
            return false;
        }
        //zwraca przedmiot z plecaka szukajac go po nazwie albo null jesli przedmiotu o podadnej nazwie nie ma w plecaku
        public ItemDataAddon FindItem(string itemType)
        {
            foreach (ItemDataAddon p in _items)
                if (p != null && p.Type == itemType)
                    return p;
            return null;
        }
        //zwraca n przedmiotow poczawszy od aktywnego (i przewija do zera jesli dojdzie do konca)
        public List<ItemDataAddon> GetItemsFromActive(int n)
        {
            List<ItemDataAddon> retList = new List<ItemDataAddon>();
            if (_items.Count > 0)
            {
                int index = _selectedItem;
                bool stop = false;
                for (int i = 0; i < n && !stop; i++)
                {
                    retList.Add(_items[index]);
                    if (++index >= Count)
                        index = 0;
                    if (index == _selectedItem)
                        stop = true;
                }
            }
            return retList;
        }

    }
}

// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.ActiveObjects;
using EJROrbEngine.Characters;
using EJROrbEngine.FPPGame;
using EJROrbEngine.Herbology;
using EJROrbEngine.PlayerInventory;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using UnityEngine;


namespace EJROrbEngine.FPPGame.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public const int PANELS_ON_SCREEN = 8;   //maks. liczba paneli przedmiotow na ekranie
        public const int SPRITE_SIZE = 512;
        public GameObject PanelPrefab;   //wzorzec do generowania panelów przedmiotów
        public GameObject ItemPrefab;   //wzorzec dla duszków przedmiotów
        public GameObject ActivePanelPrefab;   //obiekt ktorym bedziemy zaznaczac aktywna ramke        
        public Canvas KanwaOpisuAktywnegoPrzedmiotu;
        public SceneItem ActiveSceneItem { get; private set; }

        private bool _extended;

        //informuje i ustawia plecak rozszerzony do wszystkich przedmiotow
        public bool Extended
        {
            get {
                return  _extended; 
            }
            set
            {               
                _extended = value;
                RefreshAll();
            }
        }
      
        private GameObject[] _inventoryPanels;
        private GameObject[] _itemSprites;
        private TextMesh[] _panelsCountTexts;
        

        //Dodaje przedmiot do plecaka na pierwsze wolne miejsce albo na miejsce _wybranyPanel jesli brak wolnych miejsc
        public void Add(SceneItem addedItem)
        {
            ItemDataAddon  newItem = (ItemDataAddon) addedItem.GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("items");
            FPPGameModuleManager.Instance.TheInventory.Add(newItem);
            ActiveObjectsManager.Instance.RemoveActiveObject(addedItem.GetComponent<ActiveObject>());
            addedItem.StartPickingUp();
       
            RefreshAll();
        }

        //usuwa aktualnie wybrany przedmiot wyrzucajac go na ziemie
        public void RemoveActiveItem()
        {
            ItemDataAddon it = FPPGameModuleManager.Instance.TheInventory.ActiveItem;
            if (it != null)
            {
                FPPGameModuleManager.Instance.TheInventory.RemoveItem(it);
                GameObject sceneItem = PrefabPool.Instance.GetPrefab(it.Type);
                sceneItem.GetComponent<ActiveObject>().ClearUsedObject();
                ActiveObjectsManager.Instance.AddActiveObject(sceneItem.GetComponent<ActiveObject>());
                sceneItem.GetComponent<SceneItem>().PrepareToThrowOut();
                RefreshAll();
            }
        }

        //usuwa wybrany przedmiot w zwiazku z rzuceniem go jako broń rzucaną
        public void RemoveItemAfterThrow()
        {
            ItemDataAddon it = FPPGameModuleManager.Instance.TheInventory.ActiveItem;
            if (it != null)
            {
                FPPGameModuleManager.Instance.TheInventory.RemoveItem(it);
                RefreshAll();
            }
        }

        //uzywa przedmiot
        public void TryUseItem()
        {
            ItemDataAddon it = FPPGameModuleManager.Instance.TheInventory.ActiveItem;
            if (it != null && GameManager.Instance.IsModuleLoaded("Herbology"))
            {
                MedicalItemDataAddon medicalItem = HerbologyModuleManager.Instance.FindMedicalItem(it.Type);
                if (medicalItem != null)
                {
                    CharactersModuleManager.Instance.ThePlayer.InternalPatient.AddTreatmentItem(medicalItem);
                    SilentDestroyItem(it.Type);
                }
            }
        }

        //przesuwa wybrany panel o przesuniecie (1 w prawo -1 w lewo)
        public void MoveActiveItem(int shift)
        {
            FPPGameModuleManager.Instance.TheInventory.MoveActiveItem(shift);
            RefreshAll();
        }
        //usuwa przedmiot z plecaka nie wyrzucajac go na ziemie (zniszczony, przekazany komus itp.)
        public void SilentDestroyItem(string type)
        {
            if(FPPGameModuleManager.Instance.TheInventory.ItemExists(type))
            {
                FPPGameModuleManager.Instance.TheInventory.RemoveItem(FPPGameModuleManager.Instance.TheInventory.FindItem(type));
                RefreshAll();
            }
        }
 
              
        private void Awake()
        {
            if (ActivePanelPrefab == null)
                Debug.Log("Nieustaiono znacznika aktywnego panelu");
            if (PanelPrefab == null)
                Debug.Log("Nieustaiono wzorca panelu plecaka");
            else if (ItemPrefab == null)
                Debug.Log("Nieustaiono wzorca przedmiotu");
            else
                GenerujPaneleUI();
            _extended = false;
            ActiveSceneItem = null;
        }
        private void GenerujPaneleUI()
        {
            if (_inventoryPanels != null)
                foreach (GameObject panel in _inventoryPanels)
                    Destroy(panel);
            if (_itemSprites != null)
                foreach (GameObject panel in _itemSprites)
                    Destroy(panel);
            _inventoryPanels = new GameObject[PANELS_ON_SCREEN];
            _itemSprites = new GameObject[PANELS_ON_SCREEN];
            _panelsCountTexts = new TextMesh[PANELS_ON_SCREEN];
            for (int i = 0; i < PANELS_ON_SCREEN; i++)
            {
                _inventoryPanels[i] = Instantiate(PanelPrefab, transform);
                _inventoryPanels[i].transform.localPosition = new Vector3(0f, 0f, 0f);
                _inventoryPanels[i].transform.localRotation = Quaternion.identity;
                _inventoryPanels[i].SetActive(true);
                _itemSprites[i] = Instantiate(ItemPrefab, transform);
                _itemSprites[i].transform.localPosition = new Vector3(0f, 0f, 0f);
                _itemSprites[i].transform.localRotation = Quaternion.identity;
                _itemSprites[i].SetActive(true);
                _panelsCountTexts[i] = _inventoryPanels[i].GetComponentInChildren<TextMesh>();
            }
            PanelPrefab.SetActive(false);
            ItemPrefab.SetActive(false);
        }
        public void RefreshAll()
        {
            RefreshPanels();
            RefresItemSprites();
            RefreshActive();
        }
        private void RefreshPanels()
        {
            if (_inventoryPanels == null)
                GenerujPaneleUI();
            float rozmiarPanelu = (SPRITE_SIZE / 128f) * PanelPrefab.transform.localScale.x;
            for (int i = 0; i < PANELS_ON_SCREEN; i++)
            {
                _inventoryPanels[i].SetActive(Extended | i == 0);                
                if (Extended)
                    _inventoryPanels[i].transform.localPosition = PanelPrefab.transform.localPosition + new Vector3(rozmiarPanelu * 0.5f + i * (rozmiarPanelu + 0.01f), rozmiarPanelu * 0.5f, 0);
                else
                    _inventoryPanels[i].transform.localPosition = PanelPrefab.transform.localPosition + new Vector3(rozmiarPanelu * 0.5f, rozmiarPanelu * 0.5f, 0);
                _itemSprites[i].SetActive(_inventoryPanels[i].activeInHierarchy);
                _itemSprites[i].transform.localPosition = _inventoryPanels[i].transform.localPosition;
            }
            ActivePanelPrefab.SetActive(Extended);
            ActivePanelPrefab.transform.localPosition = _inventoryPanels[0].transform.localPosition;
        }

        private void RefresItemSprites()
        {
            List<ItemDataAddon> items = FPPGameModuleManager.Instance.TheInventory.GetItemsFromActive(PANELS_ON_SCREEN);
            for (int i = 0; i < PANELS_ON_SCREEN; i++)
            {
                if (i >= items.Count)
                {
                    _itemSprites[i].GetComponent<SpriteRenderer>().enabled = false;
                    _panelsCountTexts[i].gameObject.SetActive(false);
                }
                else
                {
                    _itemSprites[i].GetComponent<SpriteRenderer>().enabled = true;
                    if (_itemSprites[i].GetComponent<SpriteRenderer>().sprite != null)
                        Resources.UnloadAsset(_itemSprites[i].GetComponent<SpriteRenderer>().sprite);
                    Sprite spr = Utils.LoadObjectFromAssets(typeof(Sprite), "Sprites/Spr" + items[i].Type) as Sprite;
                    _itemSprites[i].GetComponent<SpriteRenderer>().sprite = spr;
                    _panelsCountTexts[i].text = items[i].Count > 0 ? items[i].Count.ToString() : "";
                    _panelsCountTexts[i].gameObject.SetActive(true);
                }
            }             
        }
        private void RemoveSprite(int i)
        {
            PrefabPool.Instance.ReleasePrefab(_itemSprites[i]);
            _itemSprites[i] = null;
        }
        private void RefreshActive()
        {
            ItemDataAddon active = FPPGameModuleManager.Instance.TheInventory.ActiveItem;
            if (active == null)
                ActiveSceneItem = null;
            else
            {
                if (ActiveSceneItem == null || ActiveSceneItem.Type != active.Type)
                {
                    ActiveSceneItem  = PrefabPool.Instance.GetPrefab(active.Type).GetComponent<SceneItem>();
                    ActiveSceneItem.GetComponent<ActiveObject>().ClearUsedObject();
                    ActiveSceneItem.PrepareForUseInHand(active);
                }
            }
        }
    }
}

// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.FightSystem;
using EJROrbEngine.PlayerInventory;
using UnityEngine;
namespace EJROrbEngine.SceneObjects
{

    public class SceneItem : BaseSceneObject
    {
     
        public string CzytelnaNazwa
        {
            get
            {
                if (_AOComponent != null)
                    return _AOComponent.RedableName;
                else
                    return "?";
            }
        }
        public string Opis
        {
            get
            {
                if (_AOComponent != null)
                    return _AOComponent.Description;
                else
                    return "?";
            }
        }
          
        public ItemDataAddon ItemData { get; private set; }
       
        private SpriteRenderer _sprite;
                  
        public void StartPickingUp()
        {
            if (GetComponent<Rigidbody>() != null)
                Destroy(GetComponent<Rigidbody>());
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;
            SceneWeaponHitter[] hitters = gameObject.GetComponentsInChildren<SceneWeaponHitter>();
            foreach (SceneWeaponHitter hitter in hitters)
                hitter.gameObject.SetActive(false);
            gameObject.AddComponent<PickupEffect>();
        }
        public void FinalizePickingUp()
        {
            Destroy(GetComponent<PickupEffect>());
            PrefabPool.Instance.ReleasePrefab(gameObject);
            
        }

        public void PrepareForUseInHand(ItemDataAddon anItem)
        {
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = true;
            if (GetComponent<Rigidbody>() != null)
                Destroy(GetComponent<Rigidbody>());
            SceneWeaponHitter[] hitters = gameObject.GetComponentsInChildren<SceneWeaponHitter>(true);
            foreach (SceneWeaponHitter hitter in hitters)
                hitter.gameObject.SetActive(true);
            ItemData = anItem;
        }
        public void PrepareToThrowOut()
        {
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = true;
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
                GetComponent<Rigidbody>().mass = 1;
            }
            SceneWeaponHitter[] hitters = gameObject.GetComponentsInChildren<SceneWeaponHitter>();
            foreach (SceneWeaponHitter hitter in hitters)
                hitter.gameObject.SetActive(false);
            transform.parent = GameManager.Instance.transform;
            transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward *150);
        }
       
      
        public void ClearUsedObject()
        {
            PrepareToThrowOut();
        }

      

        protected override void OnStart()
        {
           
            
        }
        protected override void OnUpdate()
        {
           
        }
       

    }

}
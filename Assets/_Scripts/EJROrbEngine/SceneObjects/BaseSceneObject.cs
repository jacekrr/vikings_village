// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using ClientAbstract;
using EJROrbEngine.ActiveObjects;
using UnityEngine;
namespace EJROrbEngine.SceneObjects
{
    //base class for all SceneXXXX objects, Type identifies what classes should be aplied. Every class will produce SceneXXX and DataXXX (it will be done via GameConfig or modules managers) and called from PrefabTemplate
    public abstract class BaseSceneObject : MonoBehaviour
    {
        public string Type
        {
            get
            {
                if (_AOComponent != null)
                    return _AOComponent.Type;
                else
                    return "?";
            }
        }
        protected ActiveObject _AOComponent;


        public virtual void SaveGame(IGameState gameState)
        {

        }
        public virtual void LoadGame(IGameState gameState)
        {

        }
        public abstract void OnConfigure();
        protected abstract void OnAwake();
        protected abstract void OnStart();
        protected abstract void OnUpdate();

        private void Awake()
        {
            OnAwake();
        }
        private void Start()
        {
            _AOComponent = GetComponent<ActiveObject>();
            OnStart();
            LoadGame(GameManager.Instance.TheGameState);
        }
        private void Update()
        {
            OnUpdate();
        }


    }

}
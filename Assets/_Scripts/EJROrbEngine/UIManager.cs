// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EJROrbEngine
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public const float TOUCH_DISTANCE = 3.5f;     // maksymalny zasieg do obiektu aby mozna bylo z nim przeprowadzic interakcje
        public GameObject CurrentCameraLookObject { get; private set; }
        private float _licznikCzasuMalegoInfo;
        private IInputManager _menedzerWejscia;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy UIManager");
            Instance = this;
        }


        protected virtual void OnStart()
        {
            _menedzerWejscia = GameStarter.StartManager.Instance.Platform.CreateInputManager(gameObject);
            _menedzerWejscia.ListenInput(OnLogicalAction);
            CurrentCameraLookObject = gameObject; //patrzy tu sam na siebie, tym trickiem wymuszamy zmiane w pierwszym Update
        }

        protected virtual void OnUpdate()
        {
          /*  Vector3 hitPoint;
            GameObject patrzymy = CameraLooksOn(TOUCH_DISTANCE, out hitPoint);
            if (patrzymy != null && patrzymy.GetComponent<Button>() != null)
                patrzymy.GetComponent<Button>().Select();
            else if (CurrentCameraLookObject != null && CurrentCameraLookObject.GetComponent<Button>() != null)
                EventSystem.current.SetSelectedGameObject(null);
            if (patrzymy != CurrentCameraLookObject)
            {
                CurrentCameraLookObject = patrzymy;
                LookTargetChanged();                
            }*/
        }
        protected virtual void LookTargetChanged()
        {

        }

        public virtual GameObject CameraLooksOn(float maxDistance, out Vector3 hitPoint)
        {
            return ScreenPointLooksOn(new Vector2(Screen.width / 2, Screen.height / 2), maxDistance, out hitPoint);
        }
        public virtual GameObject LastScreenInputLooksOn(float maxDistance, out Vector3 hitPoint)
        {
            return ScreenPointLooksOn(_menedzerWejscia.GetLastScreenInput(), maxDistance, out hitPoint);
        }

        private GameObject ScreenPointLooksOn(Vector2 point, float maxDistance, out Vector3 hitPoint)
        {
            if (maxDistance <= 0)
                maxDistance = 0.1f;
            hitPoint = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(point);          
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 60);
            if (Physics.Raycast(ray, out hit))
                if (hit.distance < maxDistance)
                {
                    hitPoint = hit.point;
                    return hit.collider.gameObject;
                }          
            return null;
        }
        public virtual void OnLogicalAction(LogicalAction action)
        {
            if (action == LogicalAction.Interaction)
            {
                if (CurrentCameraLookObject != null)
                {
                    if (CurrentCameraLookObject.GetComponent<Button>() != null)
                        CurrentCameraLookObject.GetComponent<Button>().onClick.Invoke();
                }
            }

        }

        void Start()
        {
            OnStart();
        }

        void Update()
        {
            OnUpdate();
        }

    }
}
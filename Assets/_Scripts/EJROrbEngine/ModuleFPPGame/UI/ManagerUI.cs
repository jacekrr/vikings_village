// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using EJROrbEngine.ActiveObjects;
using EJROrbEngine.Characters;
using EJROrbEngine.FightSystem;
using EJROrbEngine.SceneObjects;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EJROrbEngine.UI
{


    public class ManagerUI : MonoBehaviour
    {
        public static ManagerUI Instance { get; private set; }
        public const float TOUCH_DISTANCE = 3.5f;     // maksymalny zasieg do obiektu aby mozna bylo z nim przeprowadzic interakcje
        public const float CZASMALEGOINFO = 4f;       // czas wysświetlania się małego komunikatu tekstowego w UI
    //    public const float DYSTANSPOKAZYWANIAKANWY = 4.5f;  // dystans pokazywania dynamicznych kanw dialogow [m.]
    //    public const int ROZMIARKROPEK_X = 64;
    //    public const int ROZMIARKROPEK_Y = 68;
        public Sprite WskaznikWlaczony, WskaznikWylaczony;
        public SpriteRenderer WskaznikStrzalCenter, WskaznikStrzalN, WskaznikStrzalS, WskaznikStrzalE, WskaznikStrzalW;
        public TextMesh HealthText, FoodText, ThirstText, DiseasesText;
        public Canvas KanwaOpisu, KanwaMalegoInfo;
   //     public Canvas KanwaRzemiosla;
   //     public Canvas KanwaPomocy;
   //     public Text TekstZlota;
  //      public Image KropkiSzybkosciCzasu;
        public InventoryUI InventoryObject;
        public FPPHandsController HandsController;

        private Text[] _tekstyOpisuPrzedmiotu;
        private Text[] _tekstyMalegoInfo;
        private SpriteRenderer _wskaznikKamery;        
        private GameObject _poprzednioPatrzylismy;
        private float _licznikCzasuMalegoInfo;
        private IInputManager _menedzerWejscia;
        

        public void AfterGameLoad()
        {
            InventoryObject.RefreshAll();
        }
        public bool IsActionInProgress(bool mainAction)
        {
            return mainAction ? _menedzerWejscia.IsLogicalBooleanState(LogicalBooleanState.MainActionHold) : _menedzerWejscia.IsLogicalBooleanState(LogicalBooleanState.SecondaryActionHold);
        }
   
        public void RefreshActiveItem(SceneItem anItem)
        {

        }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy MenedzerUI");
            Instance = this;
         }

        void Start()
        {
            _menedzerWejscia = GameStarter.StartManager.Instance.Platform.CreateInputManager(gameObject);
            _menedzerWejscia.ListenInput(OnLogicalAction);
            _wskaznikKamery = Camera.main.GetComponentInChildren<SpriteRenderer>();             // GameManager.Instance.ThePlayerController.GetComponentInChildren<SpriteRenderer>();           
            _poprzednioPatrzylismy = gameObject; //patrzy tu sam na siebie, tym trickiem wymuszamy zmiane w pierwszym Update
            if(HandsController == null)
                Debug.LogError("Brak kontrolera rąk!");
            if (HealthText == null)
                Debug.LogError("Brak HealthText!");
            if (FoodText == null)
                Debug.LogError("Brak FoodText!");
            if (ThirstText == null)
                Debug.LogError("Brak ThirstText!");
            if (DiseasesText == null)
                Debug.LogError("Brak DiseasesText!");
            if(WskaznikStrzalCenter == null || WskaznikStrzalN == null || WskaznikStrzalS == null || WskaznikStrzalE == null || WskaznikStrzalW == null)
                Debug.LogError("Brak wskaznika strzalu!");
                  if (KanwaMalegoInfo == null)
                      Debug.LogError("Kawna małego info nie jest ustawiona!");
            if (KanwaOpisu == null)
                Debug.LogError("Kawna opisu przedmiotu nie jest ustawiona!");
            /*      
                  if (KanwaRzemiosla == null)
                      Debug.LogError("Kawna rzemiosła nie jest ustawiona!");
                  if (TekstZlota == null )
                      Debug.LogError("Tekst ilości złota nie jest ustawiony!");
                  if (KanwaPomocy == null)
                      Debug.LogError("KanwaPomocy nie jest ustawiona!");
                  if (KropkiSzybkosciCzasu == null)
                      Debug.LogError("KropkiSzybkosciCzasu nie jest ustawiona!");
                  KanwaOpisu.gameObject.SetActive(false);
                  KanwaRzemiosla.gameObject.SetActive(false);
             */
            _tekstyOpisuPrzedmiotu = KanwaOpisu.GetComponentsInChildren<Text>();
            _tekstyMalegoInfo = KanwaMalegoInfo.GetComponentsInChildren<Text>();
            KanwaMalegoInfo.gameObject.SetActive(false);
            if (InventoryObject != null)
            {
                InventoryObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(5, 5, 0.35f));
           //     TekstZlota.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(5, Screen.height - 5, 0.35f));
             //   KropkiSzybkosciCzasu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - ROZMIARKROPEK_X, Screen.height - ROZMIARKROPEK_Y / 2, 0.35f));
            }
            _licznikCzasuMalegoInfo = 0;
           // OdswiezKrkopkiSzybkosciCzasu();
        }

        void Update()
        {
            Vector3 hitPoint;
            GameObject patrzymy = CameraLooksOn(TOUCH_DISTANCE, out hitPoint);
            if (_poprzednioPatrzylismy != patrzymy)
            {
                ActiveObject patrzyActive = null;
                if (patrzymy != null)
                {
                    patrzyActive = patrzymy.GetComponent<ActiveObject>();
                    if (patrzyActive == null && patrzymy.GetComponent<HitPoint>() != null && patrzymy.GetComponent<HitPoint>().ParentObject != null)
                        patrzyActive = patrzymy.GetComponent<HitPoint>().ParentObject.GetComponent<ActiveObject>();
                }
                if (patrzyActive != null)
                {
                    _wskaznikKamery.sprite = WskaznikWlaczony;
                    foreach(Text t in _tekstyOpisuPrzedmiotu)
                        t.text = patrzyActive.RedableName;
                    KanwaOpisu.gameObject.SetActive(true);
                }
                else
                {
                    _wskaznikKamery.sprite = WskaznikWylaczony;
                    KanwaOpisu.gameObject.SetActive(false);
                }
            }           
            if (patrzymy != null && patrzymy.GetComponent<Button>() != null)
                patrzymy.GetComponent<Button>().Select();
            else if(_poprzednioPatrzylismy != null && _poprzednioPatrzylismy.GetComponent<Button>() != null)
                EventSystem.current.SetSelectedGameObject(null);
            _poprzednioPatrzylismy = patrzymy;
        //    TekstZlota.text = TlumaczCiagow.PodajCiag("Zloto") + ((int) MenedzerGry.InstancjaMenedzeraGry.ObiektGracza.PobierzAtrybut("Zloto")).ToString();
            ReactInputRaw();
              if (_licznikCzasuMalegoInfo >= 0)
              {
                  _licznikCzasuMalegoInfo -= Time.deltaTime;
                  if (_licznikCzasuMalegoInfo <= 0)
                  {
                      foreach (Text t in _tekstyMalegoInfo)
                          t.text = "";
                      KanwaMalegoInfo.gameObject.SetActive(false);
                  }
              }
            if (InventoryObject.ActiveSceneItem != HandsController.UsedItem)
                HandsController.StartUsingItem(InventoryObject.ActiveSceneItem);
            HealthText.text = StringsTranslator.GetString("res_Health") + ": " + CharactersModuleManager.Instance.ThePlayer.getSkillScreenValue("Health");
            FoodText.text = StringsTranslator.GetString("res_Food") + ": " + CharactersModuleManager.Instance.ThePlayer.getSkillScreenValue("Food");
            ThirstText.text = StringsTranslator.GetString("res_Thirst") + ": " + CharactersModuleManager.Instance.ThePlayer.getSkillScreenValue("Thirst");
            DiseasesText.text = CharactersModuleManager.Instance.ThePlayer.InternalPatient.EventsDump(false);

            RefreshAccuracyIcons();
        }
        
        public GameObject CameraLooksOn(float maxDistance, out Vector3 hitPoint)
        {
            if (maxDistance <= 0)
                maxDistance = 0.1f;
            hitPoint = Vector3.zero;
            RaycastHit hit;
            Vector3 centerPoint = _wskaznikKamery.transform.position;
            if (Physics.Raycast(centerPoint, Camera.main.transform.forward, out hit))
                if (hit.distance < maxDistance)
                {
                    hitPoint = hit.point;
                    return hit.collider.gameObject;
                }
            // jezeli punkt centralny nie patrzy na obiekt, to sprawdzamy rzutowanie kulą dzięki czemu możemy sprawdzić na co patrzy kamera pod szerszym kątem
            if (Physics.SphereCast(centerPoint, 0.3f, Camera.main.transform.forward, out hit))
                if (hit.distance < maxDistance)
                {
                    hitPoint = hit.point;
                    return hit.collider.gameObject;
                }
            return null;
        }

        //jak CameraLooksOn, lecz wprowadzona jest nieodkladnosc do symulowania strzalow z broni palnej w trybie scanhit. Wartosc scanHitInaccuracy podana jest w pikselach rozrzutu
        public GameObject CameraLooksOnScanHit(float maxDistance, float scanHitInaccuracy, out Vector3 hitPoint)
        {
            if (maxDistance <= 0)
                maxDistance = 0.1f;
            hitPoint = Vector3.zero;
            RaycastHit hit;
            Ray r = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 + UnityEngine.Random.Range(-scanHitInaccuracy , scanHitInaccuracy ), Screen.height / 2 + UnityEngine.Random.Range(-scanHitInaccuracy / 2, scanHitInaccuracy / 2), 0.25f));
        //    Debug.DrawRay(r.origin, r.direction, Color.yellow, 5);
            if (Physics.Raycast(r, out hit))
                if (hit.distance < maxDistance)
                {
                    hitPoint = hit.point;
                    return hit.collider.gameObject;
                }
            return null;
        }

        public void PokazMaleInfo(string tekst)
        {
            foreach (Text t in _tekstyMalegoInfo)
            {
                if(t.text != "")
                    t.text += "\n" + tekst ;
                else
                    t.text = tekst;
            }
            KanwaMalegoInfo.gameObject.SetActive(true);
            _licznikCzasuMalegoInfo = CZASMALEGOINFO;
           
        }/*
        public void PoOdczytaniuGry()
        {
            OdswiezKrkopkiSzybkosciCzasu();
        }*/
        public void OnLogicalAction(LogicalAction action)
        {
            if (action == LogicalAction.MainAction)
            {
                HandsController.PerformAction(false);
                InventoryObject.TryUseItem();
            }
            if(action == LogicalAction.SecondaryAction)
            {
                HandsController.PerformAction(true);
            }
            if (action == LogicalAction.ExpandInventory)
                InventoryObject.Extended = !InventoryObject.Extended;
            if (action == LogicalAction.InventoryLeft)
                InventoryObject.MoveActiveItem(-1);
            if (action == LogicalAction.InventoryRight)
                InventoryObject.MoveActiveItem(1);
            if (action == LogicalAction.SwitchMode)
                HandsController.SwitchWeaponMode();
            if (action == LogicalAction.Interaction)
            {
                if (_poprzednioPatrzylismy != null)
                {
                    if (_poprzednioPatrzylismy.GetComponent<Button>() != null)
                        _poprzednioPatrzylismy.GetComponent<Button>().onClick.Invoke();
                    else if (_poprzednioPatrzylismy.GetComponent<SceneItem>() != null)
                        InventoryObject.Add(_poprzednioPatrzylismy.GetComponent<SceneItem>());
                    else if (_poprzednioPatrzylismy.GetComponent<Events.InteractiveEventActivator>())
                        _poprzednioPatrzylismy.GetComponent<Events.InteractiveEventActivator>().HandleInteraction();
                    /*                    else if (_poprzednioPatrzylismy.GetComponent<UIHandlu>() != null)
                                            _poprzednioPatrzylismy.GetComponent<UIHandlu>().Uaktywnij();
                                        else if (_poprzednioPatrzylismy.GetComponent<UIInformacyjne>() != null)
                                            _poprzednioPatrzylismy.GetComponent<UIInformacyjne>().Uaktywnij();
                                        else if (_poprzednioPatrzylismy.GetComponent<UIStatystyk>() != null)
                                            _poprzednioPatrzylismy.GetComponent<UIStatystyk>().Uaktywnij();
                    */
                    else
                        if (_poprzednioPatrzylismy.GetComponent<Button>() != null)
                        _poprzednioPatrzylismy.GetComponent<Button>().onClick.Invoke();
                }
            }
            if (action == LogicalAction.DropItem)
                InventoryObject.RemoveActiveItem();
/*            if (action == LogicznaAkcja.UruchomRzemieslnictwo)
            {
                KanwaRzemiosla.gameObject.SetActive(!KanwaRzemiosla.gameObject.activeInHierarchy);
                //przesun kanwe rzemiosla przed gracza (obroci sie sama bo ma AutoObracacz)
                Vector3 nowaPozycja = MenedzerGry.InstancjaMenedzeraGry.KontrolerGracza.transform.position + MenedzerGry.InstancjaMenedzeraGry.KontrolerGracza.transform.forward * 2f;
                KanwaRzemiosla.transform.position = new Vector3(nowaPozycja.x, nowaPozycja.y, nowaPozycja.z);
            }
            if (action == LogicznaAkcja.Pomoc)            
                KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.PomocStala);
            if (action == LogicznaAkcja.PrzewodnikDalej)
            {
                int faza = MenedzerGry.InstancjaMenedzeraGry.OdczytajZmiennaInt("przewodnik");
                if (faza > 6 || faza < 1)
                    faza = 1;
                if (faza == 1)
                    KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.FazaPierwsza);
                else if (faza == 2)
                    KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.FazaDruga);
                else if (faza == 3)
                    KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.FazaTrzecia);
                else if (faza == 4)
                    KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.FazaCzwarta);
                else if (faza == 5)
                    KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.FazaPiata);
                else if (faza == 6)
                    KanwaPomocy.GetComponent<UIPomocy>().Uaktywnij(FazaPomocy.PomocStala);
                MenedzerGry.InstancjaMenedzeraGry.ZapiszZmienna("przewodnik", faza + 1);
            }
            if (action == LogicznaAkcja.WyjdzDoMenu)
            {
                MenedzerGry.InstancjaMenedzeraGry.ZapiszGre();
                UnityEngine.SceneManagement.SceneManager.LoadScene("DocelowaScenaStartowa");
            }
            if (action == LogicznaAkcja.CzasSzybciej)
            {
                SystemDniaNocyPogody.Instancja.ZmienDlugoscDnia(-3);
                OdswiezKrkopkiSzybkosciCzasu();
            }
            if (action == LogicznaAkcja.CzasWolniej)
            {
                SystemDniaNocyPogody.Instancja.ZmienDlugoscDnia(3);
                OdswiezKrkopkiSzybkosciCzasu();
            }
            if (action == LogicznaAkcja.CzasJedenKlik)
            {
                SystemDniaNocyPogody.Instancja.ZmienDlugoscDnia(3);
                if(SystemDniaNocyPogody.Instancja.DlugoscDnia >= SystemDniaNocyPogody.NAJDLUZSZYDZIEN)
                    SystemDniaNocyPogody.Instancja.ZmienDlugoscDnia(SystemDniaNocyPogody.NAJKROTSZYDZIEN - SystemDniaNocyPogody.NAJDLUZSZYDZIEN);
                OdswiezKrkopkiSzybkosciCzasu();
            }
            if (action == LogicznaAkcja.Dowolna)
                KanwaPomocy.GetComponent<UIPomocy>().Dezaktywuj();
                */
        }

        private int _lastAccuracy = -1;
        private void RefreshAccuracyIcons()
        {
            int accuracy = HandsController.GetFiringAccuracy();
            if (_lastAccuracy != accuracy)
            {
                float z = 0.25f;
                WskaznikStrzalE.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2 - accuracy - WskaznikStrzalE.sprite.texture.width / 2, Screen.height / 2, z));
                WskaznikStrzalW.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2 + accuracy + WskaznikStrzalE.sprite.texture.width / 2, Screen.height / 2, z));
                WskaznikStrzalN.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2  - accuracy - WskaznikStrzalE.sprite.texture.height / 2, z));
                WskaznikStrzalS.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2 + accuracy + WskaznikStrzalE.sprite.texture.height, z)); 
                _lastAccuracy = accuracy;
            }
        }

        private void ReactInputRaw()
        {
#if UNITY_EDITOR

#endif           
        }
/*
        private void OdswiezKrkopkiSzybkosciCzasu()
        {
            int szybkosc = 5;
            if (SystemDniaNocyPogody.Instancja.DlugoscDnia == SystemDniaNocyPogody.NAJDLUZSZYDZIEN )
                szybkosc = 1;
            else if (SystemDniaNocyPogody.Instancja.DlugoscDnia == SystemDniaNocyPogody.NAJDLUZSZYDZIEN - 3)
                szybkosc = 2;
            else if (SystemDniaNocyPogody.Instancja.DlugoscDnia == SystemDniaNocyPogody.NAJDLUZSZYDZIEN - 6)
                szybkosc = 3;
            else if (SystemDniaNocyPogody.Instancja.DlugoscDnia == SystemDniaNocyPogody.NAJDLUZSZYDZIEN - 9)
                szybkosc = 4;
            KropkiSzybkosciCzasu.sprite = Resources.Load<Sprite>("Grafika/Kropki_" + szybkosc);
        }*/

    }
}
// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.ActiveObjects;
using EJROrbEngine.FightSystem;
using EJROrbEngine.FPPGame;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using UnityEngine;
namespace EJROrbEngine.FPPGame.UI
{

    public class FPPHandsController : MonoBehaviour
    {
        public const float WALKING_INACCURACY = 1.25f;          //factor that accuracy is divided by when person is walking
        public const float RUNNING_INACCURACY = 1.5f;          //factor that accuracy is divided by when person is running
        public SceneItem UsedItem { get; private set; }           //the item(weapon) that is currently being used
        private Animator _mainAnimator;                  //main hands animator
        private GameObject _internalPivot;               //the point where the used weapon should be attached    
        private List<GameObject> _handsObjects;
        private int _autofireShots;     // for disabling autofire after 3 shots in triple mode
        private bool _isWalking, _isRunning;      // true if player is walking or running
        private bool _alternateModeInProgress;     //are we in shot alternate mode (right mouse button is being held for example)

        //tells which melee animation moment (0..1 of whole animation time) is the best moment for performing scan hit
        public float MeleeAnimationHotMoment { get {              return 0.35f;            } }

        //otrzymuje przedmiot do uzycia w prawej rece
        public void StartUsingItem(SceneItem anItem)
        {
            if (anItem != UsedItem && (UsedItem == null || anItem == null || UsedItem.Type != anItem.Type))
            {
                if (UsedItem != null)
                    PrefabPool.Instance.ReleasePrefab(UsedItem.gameObject);
                if (anItem == null)
                    ActivateHands(null);
                else if(anItem.GetComponent<SceneWeapon>() != null)
                    ActivateHands((string)anItem.GetComponent<SceneWeapon>().WeaponData["fppHands"]);
                if (anItem != null)
                {
                    anItem.transform.parent = _internalPivot.transform;
                    anItem.transform.localPosition = Vector3.zero;
                    anItem.transform.localRotation = Quaternion.identity;
                    anItem.transform.localScale = Vector3.one;
                }
                UsedItem = anItem;
            }
        }



        //aktualnie uzywany w rece przedmiot ma byc rzucony na ziemie
        public void DropUsedItem()
        {
            if (UsedItem != null)
            {
                UsedItem.GetComponent<ActiveObject>().ClearUsedObject();
                ActiveObjectsManager.Instance.AddActiveObject(UsedItem.GetComponent<ActiveObject>());
                UsedItem.GetComponent<SceneItem>().PrepareToThrowOut();
            }
        }

        public void ChangeMoveState(bool isWalking, bool isRunning)
        {
            _mainAnimator.SetBool("Walking", isWalking);
            _mainAnimator.SetBool("Running", isRunning);
            _isWalking = isWalking;
            _isRunning = isRunning;
        }

        public void PerformAction(bool alternate = false)
        {
            _alternateModeInProgress = alternate;
            if (UsedItem != null && UsedItem.GetComponent<SceneWeapon>() != null)
            {
                SceneWeapon sw = UsedItem.GetComponent<SceneWeapon>();
                if (alternate && sw.WeaponData.IsThrowWeapon && !sw.IsAttacking && !_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw"))
                {
                    sw.IsAttacking = true;
                    sw.CurrentAttackMode = FightSystem.AttackMode.ThrowAttack;
                    _mainAnimator.SetTrigger("Throw");
                    sw.PrepareToThrowAsWeapon((float)sw.WeaponData["throwForce"]);
                    UsedItem.GetComponent<ActiveObject>().ClearUsedObject();
                    ActiveObjectsManager.Instance.AddActiveObject(UsedItem.GetComponent<ActiveObject>());
                    FPPUIManager.Instance.InventoryObject.RemoveItemAfterThrow();
                    UsedItem = null;
                }
                else if (!alternate && sw.WeaponData.IsMeleeWeapon && !sw.IsAttacking && !_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack(2)"))
                {
                    sw.IsAttacking = true;
                    sw.CurrentAttackMode = FightSystem.AttackMode.MeleeAttack;
                    string trigStr = Random.Range(0, 2) == 0 ? "Attack" : "Attack2";
                    _mainAnimator.SetTrigger(trigStr);
                }
                else if (!alternate && sw.WeaponData.IsShootingWeapon && !sw.IsAttacking)
                {
                    sw.IsAttacking = true;
                    sw.CurrentAttackMode = FightSystem.AttackMode.ShootScanAttack;
                    //string trigStr = Random.Range(0, 2) == 0 ? "Attack" : "Attack2";
                    if (sw.WeaponData.CurrentFireMode == FightSystem.FireMode.Single)
                        _mainAnimator.SetTrigger("Shoot");
                    else
                        _mainAnimator.SetTrigger("AutoShoot");
                    _autofireShots = 1;
                    PerformScanHit();
                }
/* ???                else if (sw == null && !UsedItem.IsAttacking && !_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Get"))
                {
                    _mainAnimator.SetTrigger("Get");
                }*/
            }
        }
        public void SwitchWeaponMode()
        {
            if (UsedItem != null)
            {
                SceneWeapon sw = UsedItem.GetComponent<SceneWeapon>();
                if (sw != null && sw.WeaponData.IsShootingWeapon)
                {
                    if (sw.WeaponData.CurrentFireMode == FightSystem.FireMode.Single)
                    {
                        if (sw.WeaponData.TripleFireModeEnabled)
                            sw.WeaponData.CurrentFireMode = FightSystem.FireMode.Triple;
                        else if (sw.WeaponData.AutoFireModeEnabled)
                            sw.WeaponData.CurrentFireMode = FightSystem.FireMode.Auto;
                    }
                    else if (sw.WeaponData.CurrentFireMode == FightSystem.FireMode.Triple)
                    {
                        if (sw.WeaponData.AutoFireModeEnabled)
                            sw.WeaponData.CurrentFireMode = FightSystem.FireMode.Auto;
                        else sw.WeaponData.CurrentFireMode = FightSystem.FireMode.Single;
                    }
                    else if (sw.WeaponData.CurrentFireMode == FightSystem.FireMode.Auto)
                        sw.WeaponData.CurrentFireMode = FightSystem.FireMode.Single;
                }
            }
        }
        public void PerformScanHit()
        {
            if (UsedItem != null)
            {
                SceneWeapon sw = UsedItem.GetComponent<SceneWeapon>();
                if (sw != null && UsedItem.ItemData != null && sw.IsAttacking)
                {
                    if (sw.WeaponData.IsMeleeWeapon && sw.WeaponData["scanHit"] != null && (bool)sw.WeaponData["scanHit"] == true)
                    {
                        SceneWeaponHitter[] hitters = UsedItem.GetComponentsInChildren<SceneWeaponHitter>();
                        Vector3 hitPoint;
                        GameObject hited = FPPUIManager.Instance.CameraLooksOnScanHit((float)sw.WeaponData["meleeRange"], GetFiringAccuracy(), out hitPoint);
                        if (hited != null && hited.GetComponent<SceneDestructible>() != null)
                            foreach (SceneWeaponHitter swh in hitters)
                                swh.GeneralHited(hited.GetComponent<SceneDestructible>(), hitPoint);
                        sw.IsAttacking = false;
                    }
                    else
                    if (sw.WeaponData.IsShootingWeapon && sw.CurrentAttackMode == FightSystem.AttackMode.ShootScanAttack && sw.WeaponData["scanHit"] != null && (bool)sw.WeaponData["scanHit"] == true)
                    {
                        SceneWeaponHitter[] hitters = UsedItem.GetComponentsInChildren<SceneWeaponHitter>();
                        Vector3 hitPoint;
                        GameObject hited = FPPUIManager.Instance.CameraLooksOnScanHit((float)sw.WeaponData["shootRange"], GetFiringAccuracy(), out hitPoint);
                        if (hited != null)
                        {
                            SceneDestructible hitesDestr = FindDestructibleObject(hited);
                            if(hitesDestr != null)
                                foreach (SceneWeaponHitter swh in hitters)
                                    swh.GeneralHited(hitesDestr, hitPoint);
                        }
                        sw.InvokeShootEffects();

                    }
                }
            }
        }

        public int GetFiringAccuracy()
        {
            int modeDivider = _alternateModeInProgress ? 3 : 1;
            if (UsedItem != null && UsedItem.GetComponent<SceneWeapon>() != null && UsedItem.GetComponent<SceneWeapon>().WeaponData != null)
                return (int)( ((int)UsedItem.GetComponent<SceneWeapon>().WeaponData["accuracy"]) * (_isRunning ? RUNNING_INACCURACY : (_isWalking ? WALKING_INACCURACY : 1)) / modeDivider + 15);
            return 15;
        }

        private SceneDestructible FindDestructibleObject(GameObject hited)
        {
            if (hited.GetComponent<SceneDestructible>() != null)
                return hited.GetComponent<SceneDestructible>();
            if (hited.GetComponent<HitPoint>() != null && hited.GetComponent<HitPoint>().ParentObject != null)
                return hited.GetComponent<HitPoint>().ParentObject.GetComponent<SceneDestructible>();
            return null;
        }
        private void Update()
        {
            /*
            if (_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                Debug.Log("Attack");
            if (_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack(2)"))
                Debug.Log("Attack(2)");
            if (_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                Debug.Log("Idle");
            if (_mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Other"))
                Debug.Log("Idle_Other");
                */
            _alternateModeInProgress = FPPUIManager.Instance.IsActionInProgress(false);
            GameManager.Instance.ThePlayerController.SetCameraFOV(_alternateModeInProgress ? int.Parse(EJRConsts.Instance["preciseTargetFov"]) : int.Parse(EJRConsts.Instance["defaultTargetFov"]));

            //auto fire
            if (UsedItem != null)
            {
                SceneWeapon sw = UsedItem.GetComponent<SceneWeapon>();
                if (sw != null && !sw.IsAttacking && _mainAnimator.GetCurrentAnimatorStateInfo(0).IsName("AutoShoot"))
                {
                    if (FPPUIManager.Instance.IsActionInProgress(true) && (sw.WeaponData.CurrentFireMode != FightSystem.FireMode.Triple || _autofireShots < 3))
                    {
                        sw.IsAttacking = true;
                        sw.CurrentAttackMode = FightSystem.AttackMode.ShootScanAttack;
                        _autofireShots++;
                        PerformScanHit();
                    }
                    else
                    {
                        sw.IsAttacking = false;
                        _mainAnimator.SetTrigger("StopAttack");
                    }
                }
            }
        }

        private void Start()
        {
            UsedItem = null;
            _handsObjects = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
                _handsObjects.Add(transform.GetChild(i).gameObject);
            if (_handsObjects.Count == 0)
                Debug.LogError("No hands in hands controller");
            else
            {
                _handsObjects[0].SetActive(true);
                for (int i = 1; i < _handsObjects.Count; i++)
                    _handsObjects[i].SetActive(false);
            }
            _mainAnimator = GetComponentInChildren<Animator>();
            _internalPivot = GameObject.Find("InternalPivot");
            _isWalking = false;
            _isRunning = false;
                 
        }

        private  void ActivateHands(string aHandsName)
        {
            bool found = false;
            for (int i = 0; i < _handsObjects.Count; i++)
            {
                if (aHandsName != null && _handsObjects[i].name == "FPPHands_" + aHandsName)
                {
                    _handsObjects[i].SetActive(true);
                    _mainAnimator = _handsObjects[i].GetComponent<Animator>();
                    found = true;
                }
                else
                    _handsObjects[i].SetActive(false);
            }
            if (!found)
            {
                _handsObjects[0].SetActive(true);
                _mainAnimator = _handsObjects[0].GetComponent<Animator>();
            }
            _internalPivot = GameObject.Find("InternalPivot");
        }

    }
}
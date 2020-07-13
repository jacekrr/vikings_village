// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using EJROrbEngine.Characters;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using UnityEngine;

namespace EJROrbEngine.NPCSystem
{

    public class NPCAttacker
    {
        //    public const float MELEE_DISTANCE = 4.05f;

        public bool IsAttacking { get { return _isAttacking; } private set { _isAttacking = value; if(_isAttacking) _wholeNPC.ActivateAttackEffects(); } }
        private bool _isAttacking;

        private SceneNPC _wholeNPC;
        private bool _firstUpdate;
        private bool _isDistanceAttacker, _isMeleeAttacker;
        private SceneWeapon _weapon;


        public float MeleeDistance
        {
            get
            {
                if (!_isMeleeAttacker || _weapon.WeaponData["meleeRange"] == null) return 0;
                return (float)_weapon.WeaponData["meleeRange"];
            }
        }
        public float ShootDistance
        {
            get
            {
                if (!_isDistanceAttacker || _weapon.WeaponData["shootRange"] == null) return 0;
                return (float)_weapon.WeaponData["shootRange"];
            }
        }

        public NPCAttacker(SceneNPC awholeNPC)
        {
            _wholeNPC = awholeNPC;
            IsAttacking = false;
            _firstUpdate = true;
            _weapon = _wholeNPC.GetComponentInChildren<SceneWeapon>();
        }

        public void OnUpdate()
        {
            if (_firstUpdate)
            {
                _isDistanceAttacker = (int)_weapon.WeaponData["shootMin"] > 0 && (int)_weapon.WeaponData["shootMin"] > 0;
                _isMeleeAttacker = (int)_weapon.WeaponData["meleeMin"] > 0 && (int)_weapon.WeaponData["meleeMin"] > 0;
                _firstUpdate = false;
            }
            
        }
        public void StartAttack()
        {
            if (IsAttackDistance())
            {
                if (_isDistanceAttacker)
                    StartDistanceAttack();
                else if (_isMeleeAttacker)
                    StartMeleeAttack();
                _wholeNPC.SetLookDirection(GameManager.Instance.ThePlayerController.gameObject);
            }
        }
        public void PerformScanHit()
        {
            if (IsAttacking)
            {
                if (!IsAttackDistance())
                {
                    _wholeNPC.TheBrain.StartChargeRun();
                    IsAttacking = false;
                }
                else
                {
                    SceneWeaponHitter[] hitters = _wholeNPC.GetComponentsInChildren<SceneWeaponHitter>();
                    foreach (SceneWeaponHitter swh in hitters)
                    {
                        float accuracyMod = _isMeleeAttacker ? 100 : AccuracyModifier(swh);
                        if (Random.Range(0, 100) < accuracyMod)
                            swh.PlayerHited();
                        else Debug.Log("NPC missed his shoot");
                    }
                    IsAttacking = false;
                }
            }

        }
        public bool IsAttackDistance()
        {
            float playerDist = DistanceToPlayer();
            if (_isDistanceAttacker && playerDist <= ShootDistance && IsDirectShoot())
                return true;
            else if (_isMeleeAttacker && playerDist <= MeleeDistance)
                return true;
            return false;

        }
        public void StopAttack()
        {
            IsAttacking = false;
        }
        private void StartDistanceAttack()
        {
            InvokeShootEffects();
            IsAttacking = true;
        }
        private void StartMeleeAttack()
        {
            IsAttacking = true;
        }

        private void InvokeShootEffects()
        {
            _weapon.InvokeShootEffects();
        }
        private float DistanceToPlayer()
        {
            Transform playerTransform = GameManager.Instance.ThePlayerController.transform;
            Vector3 compareVector = new Vector3(playerTransform.position.x, _wholeNPC.transform.position.y, playerTransform.position.z);
            return Vector3.Distance(_wholeNPC.transform.position, compareVector);
        }
        private float AccuracyModifier(SceneWeaponHitter swh)
        {
            float playerDist = DistanceToPlayer();
            if (playerDist < 1)
                return 100;
            if (playerDist <= 0.5f * (float)swh.RealWeapon.WeaponData["shootRange"])
                return (float)_wholeNPC.TheBrain.Config["accuracy"];
            if (playerDist <= (float)swh.RealWeapon.WeaponData["shootRange"])
                return (float)_wholeNPC.TheBrain.Config["accuracy"] * (1 - playerDist / (float)swh.RealWeapon.WeaponData["shootRange"]);
            return 0;

        }

        private bool IsDirectShoot()
        {
            Vector3 shootFromPoint = new Vector3(GameManager.Instance.ThePlayerController.transform.position.x, GameManager.Instance.ThePlayerController.transform.position.y, GameManager.Instance.ThePlayerController.transform.position.z);
            Vector3 shootToPoint = new Vector3(_wholeNPC.transform.position.x, _wholeNPC.transform.position.y + 0.5f, _wholeNPC.transform.position.z); 
            Ray r = new Ray(shootFromPoint, shootToPoint - shootFromPoint);
            Debug.DrawRay(shootFromPoint, shootToPoint - shootFromPoint, Color.blue, 3);
            RaycastHit rh;
            if (Physics.Raycast(r, out rh))            
                 return rh.collider.gameObject == _wholeNPC.gameObject;
            
                
            return true;
        }

      
    }
}
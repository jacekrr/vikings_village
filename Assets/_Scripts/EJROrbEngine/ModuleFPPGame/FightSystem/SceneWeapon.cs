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

    public class SceneWeapon : BaseSceneObject
    {
        public const float DEFAULT_COOLDOWN = 0.75f;
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
     
        //it's set by FPP controller and tells if the weapon is during an attack animation. It's important only for melee weapons and allows to hit anything (thus we avoid accidentaly hits by going through an enemy)
        public bool IsAttacking {
            get
            {
                return _isAttacking;
            }
            set
            {
                if (value && !_isAttacking &&  WeaponData != null)
                {
                    _isAttacking = true;
                    _cooldownTimer = WeaponData["cooldown"]  == null ? DEFAULT_COOLDOWN : ((float)WeaponData["cooldown"] < 0.1f ? DEFAULT_COOLDOWN : (float)WeaponData["cooldown"]);
                }
                else if (!value)
                    _isAttacking = value;
                if(!_isAttacking)
                    CurrentAttackMode = AttackMode.NoAttack;
            }
        }    

      

        //set by FPP controller and tells which attack mode was used 
        public AttackMode CurrentAttackMode { get; set; }
        public WeaponDataAddon WeaponData { get { return (WeaponDataAddon)GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("weapons"); } }
       
        private bool _isAttacking;       
        private float _cooldownTimer;

        public void PrepareToThrowAsWeapon(float force)
        {
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = true;
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
                GetComponent<Rigidbody>().mass = 10;
            }
            SceneWeaponHitter[] hitters = gameObject.GetComponentsInChildren<SceneWeaponHitter>();
            foreach (SceneWeaponHitter hitter in hitters)
            {
                hitter.gameObject.SetActive(true);
                if (hitter.GetComponent<Collider>() != null)
                    hitter.GetComponent<Collider>().enabled = true;
            }
            transform.parent = GameManager.Instance.transform;
            transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 1000 * force);
        }
     

       

        public void InvokeShootEffects()
        {
            if (WeaponData != null)
            {
                if (GetComponent<ParticleSystem>() != null)
                    GetComponent<ParticleSystem>().Play();
                if (GetComponent<AudioSource>() != null)
                    GetComponent<AudioSource>().Play();
                if (GetComponent<AudioSystem.AudioSourceArray>() != null)
                    GetComponent<AudioSystem.AudioSourceArray>().PlayRandomAudio();
            }
        }

        protected override void OnAwake()
        {

        }
        protected override void OnStart()
        {
            IsAttacking = false;
            CurrentAttackMode = AttackMode.NoAttack;
            
        }
        protected override void OnUpdate()
        {
            if(_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer < 0)
                    IsAttacking = false;
            }
        }
       

    }

}
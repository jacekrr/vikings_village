// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using EJROrbEngine.Characters;
using EJROrbEngine.NPCSystem;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using UnityEngine;
namespace EJROrbEngine.FPPGame
{
    //events of hit and death of destructible object
    public delegate void DestructibleHitDelegate(GameObject hitter);            //hitter is an object who hitted this object (if shooting with a wepon its a shooter, not a projectile)
    public delegate bool DestructibleDestroyedDelegate(GameObject hitter);      //hitter like in DestructibleHitDelegate, return value should tell destructible if it should be destroyed or not

    public class SceneDestructible : BaseSceneObject
    {
        public string HitEffectPrefab  = "effects/WoodDebris";
        
        public float Health { get; private set; }

        public BaseDataAddon DestructibleData { get; private set; }
        public event DestructibleHitDelegate OnHit;             //object has been hitted
        public event DestructibleDestroyedDelegate OnDie;       //object health has reached 0

        private SceneCharacter _character;             //Object may be a character (like NPC) and than it has this object. Otherwise it's null
        private List<GameObject> _hitEffects;
        private float _internalTimer;
        private EJRBar _healthBar;

        public void Hit(GameObject hitter, Vector3 hitPoint, float ammount)
        {
            Health -= ammount;
            OnHit?.Invoke(hitter);
            if (HitEffectPrefab != "")
            {
                GameObject newHitEffect = PrefabPool.Instance.GetPrefab(HitEffectPrefab, false);
                if (newHitEffect != null)
                {
                    newHitEffect.transform.position = hitPoint;
                    if (newHitEffect.GetComponent<ParticleSystem>() != null)
                    {
                        newHitEffect.GetComponent<ParticleSystem>().Stop();
                        newHitEffect.GetComponent<ParticleSystem>().Clear();
                        newHitEffect.GetComponent<ParticleSystem>().Simulate(0, true, true);
                        newHitEffect.GetComponent<ParticleSystem>().Play();
                    }
                    if (newHitEffect.GetComponent<AudioSource>() != null)
                    {
                        float distance = (hitter.transform.position - hitPoint).magnitude;
                        newHitEffect.GetComponent<AudioSource>().Play((ulong)(1000f * distance));
                    }
                    if (newHitEffect.GetComponent<AudioSystem.AudioSourceArray>() != null)
                        newHitEffect.GetComponent<AudioSystem.AudioSourceArray>().PlayRandomAudio();
                    _hitEffects.Add(newHitEffect);
                }
            }
            if (Health <= 0)
            {
                if (OnDie == null || OnDie.Invoke(hitter))
                    Die();
            }
            if (GetComponent<SceneCharacter>() != null)
                GetComponent<SceneCharacter>().TheCharacter.SetSkillValue("Health", Health);
            if (GetComponent<SceneNPC>() != null)
                GetComponent<SceneNPC>().ImHit();

        }
        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
            gameState.SetKey("dh_" + _AOComponent.UniqueID.ToString(), Health);
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
            if (gameState.KeyExists("dh_" + _AOComponent.UniqueID.ToString()))
                Health = gameState.GetFloatKey("dh_" + _AOComponent.UniqueID.ToString());

        }

        protected override void OnStart()
        {
            DestructibleData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("destructibles");
            Health = (float)DestructibleData["health"];
            if ((string)DestructibleData["barPrefab"] != "")
            {
                _healthBar = PrefabPool.Instance.GetPrefab((string)DestructibleData["barPrefab"]).GetComponent<EJRBar>();
                _healthBar.transform.parent = gameObject.transform;     
                _healthBar.transform.localPosition = new Vector3(0, (float)DestructibleData["barY"], 0);
                if ((string)DestructibleData["barParent"] != "")
                    _healthBar.transform.parent = gameObject.transform.Find((string)DestructibleData["barParent"]); //yes, it should go here after positioning over main object
                
                _healthBar.MinValue = 0;
                _healthBar.MaxValue = Health;
            }
            
            LoadGame(GameManager.Instance.TheGameState);
            _character = GetComponent<SceneCharacter>();
            _internalTimer = 0.25f;
            _hitEffects = new List<GameObject>();
        }
        protected override void OnUpdate()
        {            
            _internalTimer -= Time.deltaTime;
            if (_internalTimer < 0)
            {
                _internalTimer = 0.25f;
                if (_healthBar != null)
                    _healthBar.CurrentValue = Health;
                List<GameObject> _toDelete = new List<GameObject>();
                foreach (GameObject go in _hitEffects)
                {
                    bool stay = false;
                    if (go.GetComponent<ParticleSystem>() != null && go.GetComponent<ParticleSystem>().isPlaying)
                        stay = true;
                    if (go.GetComponent<AudioSource>() != null && go.GetComponent<AudioSource>().isPlaying)
                        stay = true;
                    if (!stay)
                    {
                        PrefabPool.Instance.ReleasePrefab(go);
                        _toDelete.Add(go);
                    }
                }
                foreach (GameObject go in _toDelete)
                    _hitEffects.Remove(go);
            }
        }
        private void OnDisable()
        {
            SaveGame(GameManager.Instance.TheGameState);
        }

        private void Die()
        {
            foreach (GameObject go in _hitEffects)
                PrefabPool.Instance.ReleasePrefab(go);
            SaveGame(GameManager.Instance.TheGameState);
            _hitEffects.Clear();
            _AOComponent.RemoveFromGame();
        }
      
    }

}
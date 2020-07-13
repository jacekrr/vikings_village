// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using ClientAbstract;
using EJROrbEngine.ActiveObjects;
using EJROrbEngine.Characters;
using EJROrbEngine.SceneObjects;
using UnityEngine;
namespace EJROrbEngine.NPCSystem
{
    public class SceneNPC : BaseSceneObject
    {
        public GameObject[] AttackEffects;
      //  public string[] PatrolPoints;
        public NPCAI TheBrain { get; private set; }
        //tells which melee animation moment (0..1 of whole animation time) is the best moment for performing scan hit
        public float MeleeAnimationHotMoment { get { return 0.35f; } }
        
        private SceneCharacter _sceneCharacter;
        private NPCMover _mover;
        private NPCAttacker _attacker;
        private Animator _animatorComp;

        private float _internalTimer;

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
            TheBrain.OnSave("npc_" + _AOComponent.UniqueID);
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
            TheBrain.OnLoad("npc_" + _AOComponent.UniqueID);
        }

        public bool IsDead { get { return _sceneCharacter.TheCharacter.isDead() != 0; } }

        
        //event receiver
        public void Attack()
        {
            if (_attacker != null)
                _attacker.PerformScanHit();
        }

        protected override void OnStart()
        {
            _animatorComp = GetComponent<Animator>();
            _sceneCharacter = GetComponent<SceneCharacter>();
            _mover = new NPCMover(this, GetComponent<UnitPathfinder>(), GetComponent<CorvoPathFinder>());
            _attacker = new NPCAttacker(this);
            TheBrain = new NPCAI(Type, _sceneCharacter.TheCharacter, this, _mover, _attacker);
            _internalTimer = 0;
        }


        protected override void OnUpdate()
        {
            _animatorComp.SetBool("isWalking", _mover.IsWalking);
            _animatorComp.SetBool("isAttacking", _attacker.IsAttacking);
            TheBrain.OnUpdate();

            _internalTimer -= Time.deltaTime;
            if (_internalTimer < 0)
            {
                _internalTimer = 0.25f;
                if (_attacker != null && AttackEffects != null)
                {
                    //turn off attack effect objects after their particles and sounds are off
                    for (int i = 0; i < AttackEffects.Length; i++)
                    {
                        if (AttackEffects[i].activeInHierarchy)
                        {
                            bool stay = false;
                            if (AttackEffects[i].GetComponent<ParticleSystem>() != null && AttackEffects[i].GetComponent<ParticleSystem>().isPlaying)
                                stay = true;
                            if (AttackEffects[i].GetComponent<AudioSource>() != null && AttackEffects[i].GetComponent<AudioSource>().isPlaying)
                                stay = true;
                            if (!stay)
                                AttackEffects[i].SetActive(false);
                        }
                    }
                }
            }
        }


        public void GoGround()
        {
            /*
            RaycastHit[] hits = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, 3);
            if(hits.Length == 0)
                hits = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Vector3.down, 3);
            float bestDist = float.MaxValue;
            Vector3 bestHit = Vector3.zero;
            for (int i = 0; i < hits.Length; i++)
                if (hits[i].collider.tag != "NPC" && hits[i].distance < bestDist)
                {
                    bestDist = hits[i].distance;
                    bestHit = hits[i].point;
                }
            if (bestDist < float.MaxValue)
                transform.position = new Vector3(transform.position.x, bestHit.y, transform.position.z);
                */
        }

        public void SetWalkSpeedAnim(float baseSpeed)
        {
            _animatorComp.SetFloat("characterSpeed", baseSpeed * 0.3f);
        }
        public void SetLookDirection(GameObject aTargetObj)
        {
            _mover.LookOnObject(aTargetObj);
        }

        public void ImHit()
        {
            TheBrain.ImHit();

            if (IsDead)
            {
                TheBrain.Die();
                SpawnItems();
            }
            _animatorComp.SetTrigger("isDamaged");
        }
        public void ActivateAttackEffects()
        {
            if (AttackEffects.Length > 0)
            {
                for (int i = 0; i < AttackEffects.Length; i++)
                    if ( AttackEffects[i] != null && !AttackEffects[i].activeInHierarchy)
                    {
                        AttackEffects[i].SetActive(true);
                        if (AttackEffects[i].GetComponent<ParticleSystem>() != null)
                        {
                            AttackEffects[i].GetComponent<ParticleSystem>().Stop();
                            AttackEffects[i].GetComponent<ParticleSystem>().Clear();
                            AttackEffects[i].GetComponent<ParticleSystem>().Simulate(0, true, true);
                            AttackEffects[i].GetComponent<ParticleSystem>().Play();
                        }
                        if (AttackEffects[i].GetComponent<AudioSource>() != null && AttackEffects[i].GetComponent<AudioSource>().isPlaying)
                        {
                            AttackEffects[i].GetComponent<AudioSource>().Play();
                        }
                    }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "NPC" && !IsDead)
            {
                //    Debug.Log("npc triggered by " + other.transform.name);
                if ((TheBrain.CurrentState == NPCState.Patrol || TheBrain.CurrentState == NPCState.Attack) && _mover.IsWalking)
                {
                    if (TheBrain.CurrentState == NPCState.Patrol)
                        TheBrain.StartIdle();
                    else if (TheBrain.CurrentState == NPCState.Attack)
                    { // dont attack here, only stop runing because it may be collided with non-player also
                        TheBrain.StopWalking();
                    }
                }
            }
        }
        private void SpawnItems()
        {
            ActiveObjectsSpawner[] spawners = transform.GetComponentsInChildren<ActiveObjectsSpawner>();
            foreach(ActiveObjectsSpawner spawner in spawners)
            {
                spawner.Spawn();
                spawner.enabled = false;
            }
        }
    }
}
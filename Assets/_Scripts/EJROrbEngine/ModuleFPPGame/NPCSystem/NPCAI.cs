// **** A// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.Characters;
using EJROrbEngine.SceneObjects;
using System;
using UnityEngine;

namespace EJROrbEngine.NPCSystem
{
    public class NPCAI
    {                
        private LivingCharacter _character;                
        private NPCAttacker _attacker;
        private NPCMover _mover;
        private SceneNPC _sceneNPC;
        private float _thinkingTimer, _bodyStayTimer;        
        private bool _firstUpdate;
        
        public const float FLEE_DISTANCE = 16f;
        private const float BODY_STAY_TIME = 20f;        
        private const float MAX_BAR_VIS = 32f;
        private const float MAX_TALK_VIS = 10f;
        private const float ADDITIONAL_INFO_TIME = 3.5f;

        public bool IsDead {  get { return _character.isDead() != 0; } }
        public BaseDataAddon Config { get; private set; }
        public Attidude CurrentAttidude { get; private set; }
        public NPCState CurrentState { get; private set; }

        public NPCAI(string aType, LivingCharacter aCharacter, SceneNPC aSceneNPC, NPCMover aMover, NPCAttacker anAttacker)
        {
            Config = FPPGame.FPPGameModuleManager.Instance.FindEnemyData(aType);
            _character = aCharacter;        
            _thinkingTimer = 0;            
            _bodyStayTimer = 0;
            _firstUpdate = true;
            _sceneNPC = aSceneNPC;
            _attacker = anAttacker;
            _mover = aMover;
            try
            {
                CurrentAttidude = (Attidude)Enum.Parse(typeof(Attidude), (string)Config["baseAttidude"]);
            } catch(Exception)
            {
                CurrentAttidude = Attidude.Neutral;
            }
        }


        public void OnUpdate()
        {
            if(_firstUpdate)
            {
                StartIdle();
                _firstUpdate = false;
            }
            _thinkingTimer -= Time.deltaTime;
            if(_thinkingTimer < 0)
            {
                Think();
                _thinkingTimer = (float) Config["thinkingSpeed"] * ( 0.9f + UnityEngine.Random.Range(0f, 0.2f));
            }
            if (_character.getSkillValue("Health") <= 0)
                Die();
            if(IsDead)
            {
                _bodyStayTimer -= Time.deltaTime;
/*                if (_bodyStayTimer < 0)
                    if (GetComponent<StaticSerializator>() != null)
                        GetComponent<StaticSerializator>().setActive(false);
                    else
                        GameObject.Destroy(gameObject);*/
            }
            _mover.OnUpdate();
            _attacker.OnUpdate();
        }
        //main thinking process
        private void Think()
        {
            if (IsDead || !GameManager.Instance.IsGameLoadedAndStarted) return;
            CheckIfAlarm();
            if (CurrentAttidude == Attidude.Flee)
                CurrentState = NPCState.Flee;
            _mover.Think();
            if(CurrentState == NPCState.Attack && _attacker != null && !_attacker.IsAttacking)
            {
                //lets see if we are close enough to attack, otherwise, we must get closer                   
                if (_attacker.IsAttackDistance())
                    StartAttack(); 
                else
                    StartChargeRun(); //we must get closer to player                                             
            }
        
            _sceneNPC.GoGround();
        }
        //if idle - start patroling area
        public void StartPatrol()
        {
            CurrentState = NPCState.Patrol;
            _mover.StartPatrol();
        }

        public void StartChargeRun()
        {
            CurrentState = NPCState.Attack;
            _mover.StartChargeRun();
        }
        public void StartFleeRun()
        {
            CurrentState = NPCState.Flee;
            _mover.StartFleeRun();
        }
        //start melee attack or shooting
        public void StartAttack()
        {
//            if (_attacker.IsBlocked || IsDead)
  //              return;
            CurrentState = NPCState.Attack;
            _mover.StopWalking();
            _attacker.StartAttack();
        }
        //start idle
        public void StartIdle()
        {
            CurrentState = NPCState.Idle;
            _mover.StartIdle();
        }
        public void StopWalking()
        {
            _mover.StopWalking();
        }

        private void ChangeAttidude(Attidude att)
        {
           CurrentAttidude = att;
        }

        //main function that changes alarm state
        private void CheckIfAlarm()
        {
            if (CurrentAttidude == Attidude.HostileAlarmed || CurrentAttidude == Attidude.HostileNotAlarmed)
            {
                Vector3 eyesPosition = _sceneNPC.transform.position;
                Transform playerTransform = Camera.main.transform;
                float playerDist = Vector3.Distance(_sceneNPC.transform.position, playerTransform.position);
                if (_attacker != null &&  playerDist < (float)Config["visibilityDistance"])
                    ChangeAttidude(Attidude.HostileAlarmed);
                else if (CurrentAttidude == Attidude.HostileAlarmed && playerDist > (float)Config["visibilityDistance"])
                {
                    ChangeAttidude(Attidude.HostileNotAlarmed);
                    StartIdle();
                }
                //check if npc is seeing player
           /*     if (_currentAttidude == Attidude.HostileNotAlarmed)
                {
                    ChangeAttidude(Attidude.HostileAlarmed);
                    _currentState = EnemyState.attack;
                }*/                
                else if (CurrentAttidude == Attidude.HostileNotAlarmed && playerDist <= (float)Config["visibilityDistance"])
                {
                    //check if we are raycasting within field of view of npc
                    Vector3 vectToPlayer = playerTransform.position - _sceneNPC.transform.position;
                    float angle = Vector3.Angle(_sceneNPC.transform.forward, vectToPlayer);                    
                    if (Mathf.Abs(angle) <= (float)Config["fieldOfView"] && eyesPosition != null)
                    {
                        //distance and angle are ok, but we should also check obstacles - when raycasting from player to npc, we should hit this npc
                        Ray ray = new Ray(Camera.main.transform.position, eyesPosition - Camera.main.transform.position);
                        //       Debug.DrawRay (ray.origin, ray.direction * 10, Color.cyan, 4);
                        RaycastHit rh;
                        bool result = Physics.Raycast(ray, out rh, (float)Config["visibilityDistance"]);
                        if (result && rh.collider.tag == "NPC")
                            ChangeAttidude(Attidude.HostileAlarmed);                            
                    }
                }
            }
            if ((CurrentAttidude == Attidude.AlwaysHostile || CurrentAttidude == Attidude.HostileAlarmed) && CurrentState != NPCState.Attack)
                CurrentState = NPCState.Attack;

        }       
        
        public bool ImHit()
        {
            if ( !IsDead && !_firstUpdate)
            {
                if (CurrentState != NPCState.Flee)
                    ChangeAttidude(Attidude.AlwaysHostile);
                if (_attacker == null)
                    CurrentState = NPCState.Flee;
                else
                {
                    _attacker.StopAttack();
               //     CurrentState = (_attacker.IsDistanceAttacker || _attacker.IsMeleeAttacker) ? NPCState.Attack : NPCState.Flee;
                 //   _attacker.TryToStopCurrentAttack();
                }               
                return true;
            }
            else return false;
        }

        public void Die()
        {
            if (CurrentState != NPCState.Dead)
            {
//                if(_attacker != null)
  //                  _attacker.TryToStopCurrentAttack();
                _mover.StopWalking();
                CurrentState = NPCState.Dead;
                _bodyStayTimer = BODY_STAY_TIME;
                if (_sceneNPC.GetComponent<Events.DeathEventActivator>() != null)
                    _sceneNPC.GetComponent<Events.DeathEventActivator>().DoEvents(false);
                _sceneNPC.SaveGame(GameManager.Instance.TheGameState);
              
            }
        }
    
        public void OnLoad(string saveName)
        {
//            _myHPBar = gameManager.TheEngineGUIManager.SpawnNPCHPBar(gameObject).GetComponent<EJRBar>();
  //          _myHPBar.GetComponent<EJRBar>().Caption.Text = StringsReader.GetString("unit_id_" + ID);
    //        _myHPBar.GetComponent<FollowObject>().Followed = gameObject;
            string att = GameManager.Instance.TheGameState.GetStringKey("catt_" + saveName);
            if (att == "")
                att =  (string)Config["baseAttidude"];
            ChangeAttidude((Attidude)Enum.Parse(typeof(Attidude), att));
        }
        public void OnSave(string saveName)
        {
            GameManager.Instance.TheGameState.SetKey("catt_" + saveName, CurrentAttidude.ToString());
        }

    }
} 
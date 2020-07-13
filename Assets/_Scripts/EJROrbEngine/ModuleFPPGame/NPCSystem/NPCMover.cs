// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System.Collections.Generic;
using EJROrbEngine.SceneObjects;
using UnityEngine;

namespace EJROrbEngine.NPCSystem
{
    public class NPCMover
    {
        private const float PATROL_RANGE = 15;
        private Vector3 _walkTarget;
        private bool _isWalkTargetSet;
        private float _distanceCalcTimer;
        private float _lastDistance;
        private Quaternion _lookDirection;
        private bool _firstUpdate;
        private UnitPathfinder pathFinder;
        private CorvoPathFinder corvoPathFinder;
        private SceneNPC _wholeNPC;

        public bool IsWalking { get { return corvoPathFinder != null && corvoPathFinder.havePath(); } }

        public NPCMover(SceneNPC awholeNPC, UnitPathfinder aPathFinder, CorvoPathFinder aCorvoPathFinder)
        {
            _distanceCalcTimer = 0;
            _walkTarget = Vector3.zero;
            _isWalkTargetSet = false;
            _lookDirection = Quaternion.identity;
            pathFinder = aPathFinder;
            corvoPathFinder = aCorvoPathFinder;
            _firstUpdate = true;
            _wholeNPC = awholeNPC;
        }

       

        public void OnUpdate()
        {
            if(_firstUpdate)
            {
                StartIdle();
                _firstUpdate = false;
            }
           
            if (_isWalkTargetSet )
            {
  //                      if (corvoPathFinder != null && !corvoPathFinder.havePath() && !corvoPathFinder.isCalculating())
    //                    SetWalkVector(_walkTarget, pathFinder.speed);
                _distanceCalcTimer -= Time.deltaTime;
                if (_distanceCalcTimer <= 0)
                {
                    _distanceCalcTimer = 0.1f;
                    float distance = Vector3.Distance(_walkTarget, _wholeNPC.transform.position);
                    if (distance < 1f || (_wholeNPC.TheBrain.CurrentState == NPCState.Patrol && corvoPathFinder != null && !corvoPathFinder.havePath() && !corvoPathFinder.isCalculating()))
                    {
                        if (_wholeNPC.TheBrain.CurrentState == NPCState.Patrol)
                            _wholeNPC.TheBrain.StartIdle();
                        StopWalking();
                    }                    
                    _lastDistance = distance;
                }                                    
            }
            if(_lookDirection != Quaternion.identity)
                _wholeNPC.transform.rotation = _lookDirection;

        }
        private void CorrectGrid()
        {
            if(corvoPathFinder != null)
            {
                //try to set grid so large to contain target to avoid path recalulations, but if its more than 32m we rather make grid for 16m or 32m and allow recalculation mid-range
                corvoPathFinder.gridSizeY = corvoPathFinder.gridSizeX = 32;
                if (_lastDistance <= 8)
                {
                    corvoPathFinder.gridSizeY = corvoPathFinder.gridSizeX = 20;
                    corvoPathFinder.nodeWidth = 0.8f;   //20*0.8 = 16m
                }
                else if (_lastDistance <= 12.8)
                    corvoPathFinder.nodeWidth = 0.8f;   //32*0.8 = 25.6m
                else if (_lastDistance <= 16)
                    corvoPathFinder.nodeWidth = 1f;     //32*1 = 32m
                else if (_lastDistance <= 24)
                    corvoPathFinder.nodeWidth = 1.5f;   //32*1.5=48m
                else if (_lastDistance <= 32)
                {   //dynamic node number
                    corvoPathFinder.nodeWidth = 1.6f;
                    float nodesneeded = _lastDistance * 2 / 1.6f;   //max 32 --> 32*2/1.6 = 40, and 40*1.6 = 64m
                    corvoPathFinder.gridSizeY = corvoPathFinder.gridSizeX = (int)Mathf.Ceil(nodesneeded);  //ensure that grids number is always greater than needed, max gridsize here = 41 nodes
                }
                else if (_lastDistance <= 48)
                    corvoPathFinder.nodeWidth = 1f; //32*1 = 32m, so it's size for 16 meters, this step is to prevent recalculation near target and make it mid-range rather
                else
                {
                    corvoPathFinder.gridSizeY = corvoPathFinder.gridSizeX = 40;
                    corvoPathFinder.nodeWidth = 1.6f; //40*1.6 = 64m, so it's size for 32 meters, it will make at least one reacalculation in more thsan 16 meters from target
                }
            }
        }
       
        //main thinking process
        public void Think()
        {
            Transform playerTransform = Camera.main.transform;
            float playerDist = Vector3.Distance(_wholeNPC.transform.position, playerTransform.position);            
            if (_wholeNPC.TheBrain.CurrentState == NPCState.Idle)
            {
                if ((float)_wholeNPC.TheBrain.Config["walkSpeed"] > 0 && Random.Range(0, 100) < 50)
                    _wholeNPC.TheBrain.StartPatrol();
            }
          
        }

        public void LookOnObject(GameObject anObj)
        {
            Vector3 _lookTarget = new Vector3(anObj.transform.position.x, _wholeNPC.transform.position.y, anObj.transform.position.z);
            _lookDirection = Quaternion.LookRotation(_lookTarget - _wholeNPC.transform.position, _wholeNPC.transform.up);
        
        }

        //if idle - start patroling area
        public void StartPatrol()
        {
            SetWalkVector(FindPatrolPoint(), (float)_wholeNPC.TheBrain.Config["walkSpeed"] * 1f);
            //else _wholeNPC.TheBrain.StartIdle();
        }

        public void StartChargeRun()
        {
            Transform playerTransform = Camera.main.transform;
            Vector3 new_walkTarget = new Vector3(playerTransform.position.x, _wholeNPC.transform.position.y, playerTransform.position.z);
            SetWalkVector(new_walkTarget, (float)_wholeNPC.TheBrain.Config["walkSpeed"] * 1.75f);
        }
        public void StartFleeRun()
        {
            Transform playerTransform = Camera.main.transform;
            Vector3 antiPlVect = _wholeNPC.transform.position - playerTransform.position;
            antiPlVect = new Vector3(antiPlVect.x, 0, antiPlVect.z);
            antiPlVect.Normalize();
            antiPlVect *= (float)_wholeNPC.TheBrain.Config["walkSpeed"] * 300f;
            Vector3 new_walkTarget = _wholeNPC.transform.position + antiPlVect;
            SetWalkVector(new_walkTarget, (float)_wholeNPC.TheBrain.Config["walkSpeed"] * 1.75f);
        }
        public void SetWalkVector(Vector3 new_walkTarget, float baseSpeed)
        {
            //mozna zakomentowac ten warunek i wtedy kazdy NPC bedzie w trakcie ruchu aktualizowac sobie trase co kazdy thinking time, bedzie mniej optymalnie, ale nie bedzie "potkniec" w ich ruchu na koncach grida
            if (corvoPathFinder != null && ((_walkTarget - new_walkTarget).magnitude > 0.01f ||  (!corvoPathFinder.havePath() && !corvoPathFinder.isCalculating() )))
            {
                _walkTarget = new_walkTarget;
                _isWalkTargetSet = true;
                _distanceCalcTimer = 0;
                _lastDistance = Vector3.Distance(_walkTarget, _wholeNPC.transform.position);
                CorrectGrid();
                _wholeNPC.SetWalkSpeedAnim(baseSpeed);
                _lookDirection = Quaternion.LookRotation(_walkTarget - _wholeNPC.transform.position, _wholeNPC.transform.up);
                pathFinder.speed = baseSpeed;
                pathFinder.goTo(_walkTarget);
            }
        }
        public void StopWalking()
        {
            if (pathFinder != null)
                pathFinder.stop();
            _walkTarget = Vector3.zero;
            _isWalkTargetSet = false;
            //_animatorComp.SetBool("IsWalking", false);
        }
        
        //start idle
        public void StartIdle()
        {        
            StopWalking();
            _lookDirection = Quaternion.identity;
        }

        private Vector3 FindPatrolPoint()
        {
            Vector3 randomPoint = new Vector3(_wholeNPC.transform.position.x + Random.Range(0, 2 * PATROL_RANGE) - PATROL_RANGE, 0, _wholeNPC.transform.position.z + Random.Range(0, 2 * PATROL_RANGE) - PATROL_RANGE);
            float height = EndlessWorld.EndlessWorldModuleManager.Instance.CurrentBiom.TheTerrain.GetComponent<Terrain>().SampleHeight(randomPoint) ;
            randomPoint = new Vector3(randomPoint.x, height, randomPoint.z);
            return randomPoint;          
        }
        
    }
} 
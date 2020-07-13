// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System.Collections;
using System.Collections.Generic;
using ClientAbstract;
using EJROrbEngine.SceneObjects;

namespace EJROrbEngine.Characters
{

    //Base class for all characters, it's a container for CharacterStat and CharacterFlag lists. It doesn't know anything about any concrete stats.
    public class BaseCharacter
    {
        protected Dictionary<string, CharacterStat>  _stats;		        //numerical stats of character (like health, ammo, strength etc.)
    	protected Dictionary<string, CharacterFlag>  _flags;				//boolean status of character (like is poisoned or not, is blind or not etc.)
        private BaseDataAddon _configValues;

        public string Name { get; private set; }                        //unique (!) name of the character		
        public BaseCharacter(BaseDataAddon configValues, string aName)
    	{
            _configValues = configValues;
            Name = aName;
    		resetStats();
    	}

        //reset all statistics to default, derived class should use this function to add stats and to add or change default values
    	protected virtual void resetStats()
    	{
    		_stats = new Dictionary<string, CharacterStat>();
    		_flags = new Dictionary<string, CharacterFlag>();
    	}
    	    	    	
        //upgrade stat with leveling coefficient in non-experience way
        protected virtual void upgradeSkill(string skillID, bool plus)
        {
            foreach (CharacterStat st in _stats.Values)
            {
                if (st.Type == skillID)
                {
                    st.useValuePoint(plus);
                    st.ValuePerPoint *= st.VPPCoefficient;
                }
            }
            afterStatsChanged();
        }

        //force skill value
        public void SetSkillValue(string skillID, float val)
        {
            foreach (CharacterStat st in _stats.Values)
                if (st.Type == skillID)
                    st.Value = val;
        }
    	    	    	
    	public void setFlag(string flagName, bool val)
    	{
    		if(_flags.ContainsKey(flagName))
    			_flags[flagName].Value = val;
            afterStatsChanged();
    	}
    	public bool getFlag(string flagName)
    	{
    		if(_flags.ContainsKey(flagName))
    			return _flags[flagName].Value;
    		return false;
    	}
      
    	public float getSkillValue(string skillID)
    	{
    		foreach(CharacterStat st in _stats.Values)
    			if(st.Type == skillID)
                    return st.Value;
    		return 0;
    	}
    	public int getSkillScreenValue(string skillID)
    	{
    		foreach(CharacterStat st in _stats.Values)
    			if(st.Type == skillID)
    				return st.ScreenValue;
    		return 0;
    	}
       
    	public float getSkillMaxValue(string skillID)
    	{
    		foreach(CharacterStat st in _stats.Values)
    			if(st.Type == skillID)
    				return st.MaxValue;
    		return 0;
    	}
    	public int getSkillMinLevel(string skillID)
    	{
    		foreach(CharacterStat st in _stats.Values)
    			if(st.Type == skillID)
    				return st.MinLevel;
    		return 0;
    	}
    	public float getSkillNextValue(string skillID)	//value after next use of char points
    	{
    		foreach(CharacterStat st in _stats.Values)
    			if(st.Type == skillID)
                    return st.Value + st.ValuePerPoint;			
    		return 0;
        }
             
    	//called after some stats changed, it should reolve dependency between stats, the derived classes have knowledge of how to perform this (but it's not necessery)
    	virtual protected void afterStatsChanged()
    	{
    	
    	}	
    	
    	public void SaveGame(IGameState gameState)
    	{
            foreach (CharacterStat st in _stats.Values)
                st.Save(gameState, Name);
    		foreach(string fl in _flags.Keys)
    		{
    			gameState.SetKey("_charflag_" + Name + "_" + fl, _flags[fl].Value ? 1 : 0);			
    		}
            gameState.SetKey("_charsaved_" + Name, "1");			
    	}
    	public void LoadGame(IGameState gameState)
    	{
    		if(gameState.GetIntKey("_charsaved_" + Name) == 1)
    		{
                foreach (CharacterStat st in _stats.Values)
                    st.Load(gameState, Name);
    			foreach(string fl in _flags.Keys)
    			{
                    if(gameState.KeyExists("_charflag_" + Name + "_" + fl))
                       _flags[fl].Value = gameState.GetIntKey("_charflag_" + Name + "_" + fl) == 1;			
    			}
    			afterStatsChanged();
    		}
    	}
    	
    	public ArrayList getLevelingEnabledStats()
    	{
    		ArrayList retList = new ArrayList();
    		foreach(CharacterStat st in _stats.Values)
    			if(st.ValuePerPoint > 0)
    				retList.Add(st);
    		return retList;
    	}	
    	
    	public string debugDump()
    	{
    		string ret = "";
    		foreach(CharacterStat st in _stats.Values)
    			ret += st.Type + ": "+st.Value + "/" + st.MaxValue +"\n";// + "[IMG=ImgSlumber]";
    		
    		foreach(string fl in _flags.Keys)
    			ret += fl + ": "+ ( _flags[fl].Value ? "true" : "false" )+"\n";			
    		return ret;
    	}

    	public void AddStat(string aName, CharacterStat aStat)
    	{
    		_stats.Add(aName, aStat);
            if (_configValues != null && _configValues["stat" + aName] != null)
                _stats[aName].Value = (float) _configValues["stat" + aName];

        }

    	public void AddFlag(string aName, CharacterFlag aFlag)
    	{
    		_flags.Add(aName, aFlag);
    	}

        //check if after changing a stat value, another stats must be changed ora other actions performed. Exact logic in derived classes.
        public virtual void adjustCompatibility()
        {
        }

        public virtual bool ContainsStat(string abName)
        {
            return _stats.ContainsKey(abName);
        }

        //Actions performed on every second
        public virtual void OnSecondChange(int nothing)
        {

        }

        //Actions performed on every hour
        public virtual void OnHourChange(int currentVal)
        {

        }
    }
//
}
// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using System.Collections.Generic;

namespace EJROrbEngine.Characters
{

    public class CharacterStat
    {
        public string Type;
        public float MaxValue;
        public int MinLevel;              //first level on which player can edit this stat once leveling
        public float ValuePerPoint;       //how much the value changes for every character points, 0 means that this stat cannot be changed by leveling 
        public float VPPCoefficient;	  //how the ValuePerPoint changes after one character point is used (ValuePerPoint is multiplied by this value)
        private float startValue, startValuePerPoint;                    //to use when reseting
        private float _Value;
        public float Value { get { return _Value; }  set { SetValue(value); } }
        public int ScreenValue {
            get {
                return (int)System.Math.Round(Value);
            }  }

        public CharacterStat(string atype, float aVal, float aMax, int minLev, float vpp, float vppcoeff)
        {
            Type = atype;
            _Value = aVal;
            startValue = aVal;
            MaxValue = aMax;
            MinLevel = minLev;
            ValuePerPoint = vpp;
            startValuePerPoint = vpp;
            VPPCoefficient = vppcoeff;
        }
        public void useValuePoint(bool plus)
        {
            ChangeValue((plus ? 1 : -1) * ValuePerPoint);
        }
        public void ChangeValue(float chval)
        {
            Value = Value + chval;
        }
        public void SetValue(float chval)
        {
            _Value = chval;
            if (_Value > MaxValue) _Value = MaxValue;
            if (_Value < 0) _Value = 0;
        }
        public void ResetValue()
        {
            ValuePerPoint = startValuePerPoint;
            Value = startValue;
        }
        public void Save(IGameState gameState, string extName)
        {
            gameState.SetKey("_charstatV_" + extName + "_" + Type, _Value);
            gameState.SetKey("_charstatMV_" + extName + "_" + Type, MaxValue);            
        }
        public void Load(IGameState gameState, string extName)
        {
            if (gameState.KeyExists("_charstatV_" + extName + "_" + Type))
                _Value = gameState.GetFloatKey("_charstatV_" + extName + "_" + Type);
            if (gameState.KeyExists("_charstatMV_" + extName + "_" + Type))
                MaxValue = gameState.GetFloatKey("_charstatMV_" + extName + "_" + Type);
          
        }
    }

    public class CharacterFlag
    {
        public string Type;
        public bool Value;
        public CharacterFlag(string atype, bool aVal)
        {
            Type = atype;
            Value = aVal;
        }
    }

     //
}
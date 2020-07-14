// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System.Text;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    //ResourceData is a one storage or stockpile/stack of a resource, it's serialized in save game as CV;M;MV whre CV is current value, M is multiplier, and MV is maximum value
    public class ResourceData 
    {
        //resource name/type
        public string Type { get; private set; }
        //the current value shows how much resource is stockpiled in the object
        private float _currentValue;
        public float CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                if (_currentValue < 0) _currentValue = 0;
                if (_currentValue > MaximumValue) _currentValue = MaximumValue;
            }
        }
        //rest between max and current value (free space in terms of resource storaging)
        public float FreeValue
        {
            get { return MaximumValue - CurrentValue; }
        }
        //multiplier is number of power of 10s that should multiply the CurrentValue, for example CurrentValue==4.5 and Multiplier==3 means 4500 real value
        public int Multiplier { get; private set; }
        //the maximum value available to be stockpiled
        public float MaximumValue { get; set; }

        public ResourceData(string type)
        {
            Type = type;
            Multiplier = 1;
        }

        public ResourceData(ResourceData val)
        {
            Type = val.Type;
            Multiplier = val.Multiplier;
            MaximumValue = val.MaximumValue;
            CurrentValue = val.CurrentValue;
        }
        public void FromSaveGameValue(string dataStr)
        {
            string[] tokens = dataStr.Split(';');
            if(tokens.Length == 3)
            {
                try
                {
                    MaximumValue = int.Parse(tokens[2]);
                    Multiplier = int.Parse(tokens[1]);
                    CurrentValue = int.Parse(tokens[0]);
                } catch(System.Exception)
                {
                    Debug.LogError("invalid format of " + dataStr);
                }
            }
        }
        public string ToSaveGameValue()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CurrentValue.ToString());
            sb.Append(";");
            sb.Append(Multiplier.ToString());
            sb.Append(";");
            sb.Append(MaximumValue.ToString());
            return sb.ToString();
        }

        // maths section, overriden operators
        public static ResourceData operator +(ResourceData a, ResourceData b)
        {
            if (a.Type != b.Type)
                throw new System.Exception("Adding diffrent types of resources: " + a.Type + " and " + b.Type);
            ResourceData rd = new ResourceData(a.Type);
            rd.MaximumValue = a.MaximumValue;
            rd.CurrentValue = a.CurrentValue + b.CurrentValue;
            return rd;
        }
 
        public static ResourceData operator -(ResourceData a, ResourceData b)
        {
            if (a.Type != b.Type)
                throw new System.Exception("Adding diffrent types of resources: " + a.Type + " and " + b.Type);
            ResourceData rd = new ResourceData(a.Type);
            rd.MaximumValue = a.MaximumValue;
            rd.CurrentValue = a.CurrentValue - b.CurrentValue;
            return rd;
        }
       
    }
}
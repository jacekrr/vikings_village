// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    //ResourceData is a one storage or stockpile/stack of a resource, it's serialized in save game as CV;M;MV whre CV is current value, M is multiplier, and MV is maximum value
    public class ResourceData 
    {
        //resource name/type
        public string Type { get; private set; }
        public string TypeName { get { return StringsTranslator.GetString("ResName" + Type); } }
        //the current value shows how much resource is stockpiled in the object
        private BigInteger _currentValue;
        public BigInteger CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                if (_currentValue > MaximumValue) _currentValue = MaximumValue;
            }
        }
        //rest between max and current value (free space in terms of resource storaging)
        public BigInteger FreeValue
        {
            get { return MaximumValue - CurrentValue; }
        }       
        //the maximum value available to be stockpiled
        public BigInteger MaximumValue { get; set; }

        public ResourceData(string type)
        {
            Type = type;
            MaximumValue = BigInteger.Zero;
            CurrentValue = BigInteger.Zero;
        }

        public ResourceData(ResourceData val)
        {
            Type = val.Type;
            MaximumValue = val.MaximumValue;
            CurrentValue = val.CurrentValue;
        }
        public void FromSaveGameValue(string dataStr)
        {
            string[] tokens = dataStr.Split(';');
            if(tokens.Length == 2)
            {
                try
                {
                    MaximumValue.FromSaveGameValue(tokens[1]);
                    CurrentValue.FromSaveGameValue(tokens[0]);
                } catch(System.Exception)
                {
                    Debug.LogError("invalid format of " + dataStr);
                }
            }
        }
        public string ToSaveGameValue()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CurrentValue.ToSaveGameValue());
            sb.Append(";");
            sb.Append(MaximumValue.ToSaveGameValue());
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

        //readable info of resources list
        public static string ListToString(List<ResourceData> lista)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ResourceData rd in lista)
            {
                sb.Append(" ");
                sb.Append(rd.CurrentValue.ToString());
                sb.Append(" ");
                sb.Append(rd.TypeName);
            }
            return sb.ToString();
        }

    }
}
// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System;

namespace EJROrbEngine
{
    //BigInteger holds big natural numbers calculated as MainValue * 10 ^ (Multiplier*3), for example MV=3.45 and Mult=3 means 3450000000,
    //BigInteger presents only three meaningful digits, but as MainValue is double, it will use more inner precision
    //Despite the name, BigInteger still could hold and show a rational number - it's possible only when the Multiplier == 0. So it's possible to hold rationals between 0.001 and 999.9.
    public class BigInteger
    {
        public double MainValue;
        public int Multiplier;

        public static BigInteger Zero
        { get
            {
                BigInteger bi = new BigInteger(0, 0);
                return bi;
            } }

        public BigInteger(double mainValue, int multiplier)
        {
            MainValue = mainValue;
            Multiplier = multiplier;
            Normalize();
        }
        public BigInteger(BigInteger value)
        {
            MainValue = value.MainValue;
            Multiplier = value.Multiplier;
            Normalize();
        }
        public BigInteger(string value)
        {
            FromSaveGameValue(value);
            Normalize();
        }
        //make BigInteger follow rules of this class after it's MainValue has been changed to any number
        public void Normalize()
        {
            while (MainValue > 1000)
            {
                MainValue /= 1000;
                Multiplier++;
            }
            while (MainValue < 1 && Multiplier > 0)
            {
                MainValue *= 1000;
                Multiplier--;
            }
            
        }
        //increase/decrease BigInteger's multiplier (it will not follow rules of this class, but it's useful on calculations of two BigIntegers, after all it should be normalized by Normalize function)
        //Watch out! there's possibility of an overflow!
        public void IncrementMultiplier()
        {
            Multiplier++;
            MainValue /= 1000;
        }
        public void DecrementMultiplier()
        {
            Multiplier--;
            MainValue *= 1000;
        }

        public override string ToString()
        {
            double RoundedMainValue = MainValue;
            if (RoundedMainValue < 1)
                RoundedMainValue = (double)Math.Round(RoundedMainValue, 3);
            else
               if (RoundedMainValue < 10)
                RoundedMainValue = 10 * (double)Math.Round(RoundedMainValue / 10, 3);
            else if (MainValue < 100)
                RoundedMainValue = 100 * (double)Math.Round(RoundedMainValue / 100, 3);
            else
                RoundedMainValue = 1000 * (double)Math.Round(RoundedMainValue / 1000, 3);
            if (Multiplier == 0)
                return RoundedMainValue.ToString();
            return string.Format("{0} {1}", RoundedMainValue.ToString(), MultiplierToString(Multiplier));
        }
        //serialize and deserialize to/from string, format is "MainValue MultiplierStr", where MultiplierStr is K, M, B, T, AA, AB and so on (or empty string for Multiplier 0)
        //Note: no exception checking!
        public void FromSaveGameValue(string dataStr)
        {           
            int spaceInd = dataStr.IndexOf(" ");
            if(spaceInd == -1)
                MainValue = float.Parse(dataStr);
            else
            {
                MainValue = double.Parse(dataStr.Substring(0, spaceInd));
                Multiplier = StringToMultiplier(dataStr.Substring(spaceInd + 1));
            }
            Normalize();
        }
        public string ToSaveGameValue()
        {
            if (Multiplier == 0)
                return MainValue.ToString();
            return string.Format("{0} {1}", MainValue.ToString(), MultiplierToString(Multiplier));
        }
        //overriden operators, keep in mind that all operators assumes that both numbers are already normalized
        public static BigInteger operator +(BigInteger a, BigInteger b)
        {
            //if numbers have different multipliers, the number with lesser multiplier should be converted to multiplier of the greater one
            while (a.Multiplier > b.Multiplier)
                b.IncrementMultiplier();
            while (b.Multiplier > a.Multiplier)
                a.IncrementMultiplier();
            //add them normally, note that constructor will normalize the result
            return new BigInteger(a.MainValue + b.MainValue, a.Multiplier);
        }

        public static BigInteger operator -(BigInteger a, BigInteger b)
        {
            //if numbers have different multipliers, the number with lesser multiplier should be converted to multiplier of the greater one
            while (a.Multiplier > b.Multiplier)
                b.IncrementMultiplier();
            while (b.Multiplier > a.Multiplier)
                a.IncrementMultiplier();
            //subtract them normally, note that constructor will normalize the result
            return new BigInteger(a.MainValue - b.MainValue, a.Multiplier);
        }
        public static BigInteger operator /(BigInteger a, BigInteger b)
        {
            return new BigInteger(a.MainValue / b.MainValue, a.Multiplier - b.Multiplier);
        }
        public static BigInteger operator *(BigInteger a, BigInteger b)
        {
            return new BigInteger(a.MainValue * b.MainValue, a.Multiplier + b.Multiplier);
        }
        public static BigInteger operator +(BigInteger a, double b)
        {
            return new BigInteger(a.MainValue + b, a.Multiplier);
        }
        public static BigInteger operator -(BigInteger a, double b)
        {
            return new BigInteger(a.MainValue - b, a.Multiplier);
        }
        public static BigInteger operator *(BigInteger a, double b)
        {
            //Here we just multiple MainValue by b. It may became larger than 1000 after that, but constructor will normalize it. 
            //Watch out! Overflow is possible when b is be very big
            return new BigInteger(a.MainValue * b, a.Multiplier);            
        }
        public static BigInteger operator /(BigInteger a, double b)
        {
            //divide MainValue and than normalize            
            return new BigInteger(a.MainValue / b, a.Multiplier);
        }
        public static bool operator >(BigInteger a, BigInteger b)
        {
            if (a.Multiplier == b.Multiplier)
                return a.MainValue > b.MainValue;
            return a.Multiplier > b.Multiplier;
        }
        public static bool operator <(BigInteger a, BigInteger b)
        {
            if (a.Multiplier == b.Multiplier)
                return a.MainValue < b.MainValue;
            return a.Multiplier < b.Multiplier;
        }
        public static bool operator >=(BigInteger a, BigInteger b)
        {
            if (a.Multiplier == b.Multiplier)
                return a.MainValue >= b.MainValue;
            return a.Multiplier >= b.Multiplier;
        }
        public static bool operator <=(BigInteger a, BigInteger b)
        {
            if (a.Multiplier == b.Multiplier)
                return a.MainValue <= b.MainValue;
            return a.Multiplier <= b.Multiplier;
        }
        //cast to double, watch out! Overflow is possible for big numbers!
        public double ToDouble()
        {
            while (Multiplier > 0)
                DecrementMultiplier();
            return MainValue;
        }
        //a to b power
        public static BigInteger Pow(double a, int b)
        {
            if (b < 0)
                return BigInteger.Zero;
            BigInteger retInt = new BigInteger(1, 0);
            for (int i = 0; i < b; i++)
                retInt = retInt * a;
            return retInt;
        }
        private static string[] MULTIPLAYERS_ABBR = new string[5] { "", "K", "M", "B", "T" };
        private static string MultiplierToString(int multiplier)
        {
            if (multiplier <= 4)
                return MULTIPLAYERS_ABBR[multiplier];
            if(multiplier < 576 )
                return char.ConvertFromUtf32((multiplier - 5) % 26 + 65) + char.ConvertFromUtf32((multiplier - 5) / 26 + 65);
            return "??";
        }
        private static int StringToMultiplier(string str)
        {
            for (int i = 0; i < MULTIPLAYERS_ABBR.Length; i++)
                if (MULTIPLAYERS_ABBR[i] == str)
                    return i;
            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(str);
            return 26 * (asciiBytes[0] - 65) + asciiBytes[1] - 65 + 5;
        }
    }
}
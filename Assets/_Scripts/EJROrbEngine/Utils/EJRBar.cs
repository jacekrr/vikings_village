// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using UnityEngine.UI;

namespace EJROrbEngine
{
    public class EJRBar : MonoBehaviour
    {
        public GameObject ItemFull, ItemEmpty;
       // public EJR3DText Caption;
        public float CurrentValue, MinValue, MaxValue;

        private float _lastValue;

        private void Start()
        {
            _lastValue = MinValue - 1; 
        }
	    private void Update()
	    {
            if(CurrentValue != _lastValue)
            {
                _lastValue = CurrentValue;
                if (CurrentValue < MinValue)
                    CurrentValue = MinValue;
                if (CurrentValue > MaxValue)
                    CurrentValue = MaxValue;
                float percent = 0;
                if (MinValue != MaxValue)
                    percent = (CurrentValue - MinValue) / (MaxValue - MinValue);
                ItemFull.transform.localScale = new Vector3(0.1f, 0.5f * percent, 0.1f);
                ItemEmpty.transform.localScale = new Vector3(0.1f, 0.5f * (1f - percent), 0.1f);
                ItemFull.transform.localPosition = new Vector3(-(1f - percent) / 2, 0, 0);
                ItemEmpty.transform.localPosition = new Vector3(percent / 2, 0, 0);
                ItemFull.SetActive(percent > 0);
                ItemEmpty.SetActive(percent < 1);
            }
    	}

        public void SetFullMaterial(string matName)
        {
            ItemFull.GetComponent<MeshRenderer>().material = Resources.Load<Material>(matName);
        }
    }
}
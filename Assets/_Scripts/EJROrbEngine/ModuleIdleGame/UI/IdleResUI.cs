// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EJROrbEngine.IdleGame.UI
{
    //class responsible for main resource statistics UI
    public class IdleResUI : MonoBehaviour
    {
        private Dictionary<string, TextMeshProUGUI> _staticResTexts;           //labels of resource texts (key is a resource Type)
        private Dictionary<string, TextMeshProUGUI> _valResTexts;              //values of resource texts (key is a resource Type)

        // Start is called before the first frame update
        void Start()
        {
            _staticResTexts = new Dictionary<string, TextMeshProUGUI>();
            _valResTexts = new Dictionary<string, TextMeshProUGUI>();
        }

        public void  RefreshResData(ResourceData rd)
        {
            if (!_staticResTexts.ContainsKey(rd.Type))
            {
                GameObject tObj = GameObject.Find("ST" + rd.Type);
                if (tObj != null) //it might be null if static texts are not used
                {
                    _staticResTexts.Add(rd.Type, tObj.GetComponent<TextMeshProUGUI>());
                    _staticResTexts[rd.Type].text = rd.Type;        // TODO: tlumaczenia
                }
            }
            if (!_valResTexts.ContainsKey(rd.Type))
            {
                GameObject tObj = GameObject.Find("Val" + rd.Type);
                if (tObj == null)
                    Debug.LogError("Cant find Val" + rd.Type);
                else
                    _valResTexts.Add(rd.Type, tObj.GetComponent<TextMeshProUGUI>());
            }

            if (_valResTexts.ContainsKey(rd.Type))
                _valResTexts[rd.Type].text = rd.CurrentValue.ToString() + "/" + rd.MaximumValue.ToString();
        }
    }
}
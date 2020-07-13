// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using GameStarter;
using TMPro;
using UnityEngine;

namespace EJROrbEngine.UI
{

    public class LogoSceneUI : MonoBehaviour
    {
        public TextMeshProUGUI InfoText;
        public GameObject AllLogoUI;
      
        // Update is called once per frame
        void Update()
        {
            InfoText.text = StartManager.Instance.GetGamePhaseInfoText() + " ...";
            if(StartManager.Instance.CurrentGamePhase == GamePhase.PLAYING_GAME)
            {
                AllLogoUI.gameObject.SetActive(false);
                this.enabled = false;
            }
        }
    }
}
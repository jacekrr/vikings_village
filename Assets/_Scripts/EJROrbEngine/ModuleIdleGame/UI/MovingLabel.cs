// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using TMPro;
using UnityEngine;

namespace EJROrbEngine.IdleGame.UI
{
    //label that is showed for a short period of time and is moving through direction of MovingDir
    public class MovingLabel : MonoBehaviour
    {
        public const float LIVE_TIME = 1.1f;
        private Vector3 _direction;
        private float _liveTime;
        
        public void Show(string text, Color color, Vector3 direction)
        {
            _direction = direction;            
            GetComponent<TextMeshProUGUI>().text = text;
            GetComponent<TextMeshProUGUI>().color = color;            
            _liveTime = LIVE_TIME;
        }
        private void Update()
        {
            if (_liveTime > 0)
            {
                _liveTime -= Time.deltaTime;
                if (_liveTime < 0)
                    Destroy(gameObject);
                else
                    transform.position = transform.position + _direction;
            }
        }

    }
}
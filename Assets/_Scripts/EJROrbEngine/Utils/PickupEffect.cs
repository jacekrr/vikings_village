// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.SceneObjects;
using UnityEngine;
namespace EJROrbEngine
{
    public class PickupEffect : MonoBehaviour
    {
        private const float CZASCALKOWITY = 0.75f;
        private readonly Vector3 PRZESUNIECIECALKOWITE = new Vector3(0, 1f, 0);
        private const float OBROTCALKOWITY = 540;
        private Vector3 _pozycjaStartowa, _skalaStartowa;
        private Quaternion _rotacjaStartowa;
        private float _czasUplynal;
       
        void Start()
        {
            _czasUplynal = 0;
            _pozycjaStartowa = transform.localPosition;
            _rotacjaStartowa = transform.localRotation;
            _skalaStartowa = transform.localScale;
        }

        void Update()
        {
            _czasUplynal += Time.deltaTime;
            if (_czasUplynal >= CZASCALKOWITY)
                ZatrzymajEfekt();
            else
            {
                transform.localPosition = Vector3.Lerp(_pozycjaStartowa, _pozycjaStartowa + PRZESUNIECIECALKOWITE, _czasUplynal / CZASCALKOWITY);
                transform.localRotation = _rotacjaStartowa;
                transform.Rotate(transform.up, OBROTCALKOWITY * _czasUplynal / CZASCALKOWITY);
                transform.localScale = Vector3.Lerp(_skalaStartowa, _skalaStartowa * (1 - _czasUplynal / CZASCALKOWITY), _czasUplynal / CZASCALKOWITY);

            }
        }
        private void ZatrzymajEfekt()
        {
            GetComponent<SceneItem>().FinalizePickingUp();
        }
    }
}
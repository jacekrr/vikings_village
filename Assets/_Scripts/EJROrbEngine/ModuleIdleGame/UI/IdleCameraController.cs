// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;

namespace EJROrbEngine.IdleGame.UI
{
    
    public class IdleCameraController : MonoBehaviour
    {
        private float _IdleCameraMoveMaxX;
        private float _IdleCameraMoveMinX;
        private float _IdleCameraMoveMaxZ;
        private float _IdleCameraMoveMinZ;
        private float _IdleCameraMoveMaxY;
        private float _IdleCameraMoveMinY;
        private Vector3 _currentTranslation;
        private Vector3 _basePosition;
        private const float STEP_VALUE = 1f;

        private void Awake()
        {
            _IdleCameraMoveMaxX = float.Parse(EJRConsts.Instance["IdleCameraMoveMaxX"]);
            _IdleCameraMoveMinX = float.Parse(EJRConsts.Instance["IdleCameraMoveMinX"]);
            _IdleCameraMoveMaxZ = float.Parse(EJRConsts.Instance["IdleCameraMoveMaxZ"]);
            _IdleCameraMoveMinZ = float.Parse(EJRConsts.Instance["IdleCameraMoveMinZ"]);
            _IdleCameraMoveMaxY = float.Parse(EJRConsts.Instance["IdleCameraMoveMaxY"]);
            _IdleCameraMoveMinY = float.Parse(EJRConsts.Instance["IdleCameraMoveMinY"]);
            _currentTranslation = Vector3.zero;
            _basePosition = transform.position;
        }

        private void Update()
        {
            transform.position = _basePosition + _currentTranslation;
        }

        //move camera, movementDir will contain a unit vector with direction
        public void OnCameraMove(Vector3 movementDir)
        {
            _currentTranslation += movementDir * STEP_VALUE;
            if (_currentTranslation.x > _IdleCameraMoveMaxX)
                _currentTranslation = new Vector3(_IdleCameraMoveMaxX, _currentTranslation.y, _currentTranslation.z);
            if (_currentTranslation.x < _IdleCameraMoveMinX)
                _currentTranslation = new Vector3(_IdleCameraMoveMinX, _currentTranslation.y, _currentTranslation.z);
            if (_currentTranslation.y > _IdleCameraMoveMaxY)
                _currentTranslation = new Vector3(_currentTranslation.x, _IdleCameraMoveMaxY, _currentTranslation.z);
            if (_currentTranslation.y < _IdleCameraMoveMinY)
                _currentTranslation = new Vector3(_currentTranslation.x, _IdleCameraMoveMinY, _currentTranslation.z);
            if (_currentTranslation.z > _IdleCameraMoveMaxZ)
                _currentTranslation = new Vector3(_currentTranslation.x, _currentTranslation.y, _IdleCameraMoveMaxZ);
            if (_currentTranslation.z < _IdleCameraMoveMinZ)
                _currentTranslation = new Vector3(_currentTranslation.x, _currentTranslation.y, _IdleCameraMoveMinZ);
        }
    }
}
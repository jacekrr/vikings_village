// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;

namespace EJROrbEngine.AudioSystem
{
    public class AudioSourceArray : MonoBehaviour
    {
        public AudioClip[] Clips;

        private AudioSource _theSource;
        public void PlayRandomAudio()
        {
            if (_theSource != null && Clips.Length > 0)
            {
                if (_theSource.isPlaying)
                    _theSource.Stop();
                _theSource.clip = Clips[Random.Range(0, Clips.Length)];
                _theSource.Play();
            }
        }

        private void Start()
        {
            _theSource = GetComponent<AudioSource>();
        }
    }
}
// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    //this component will make whole gameobject and its children to be visible only if building that contains it is at least at certain level
    public class LevelVisibilitySection : MonoBehaviour
    {
        public int MinLevel;
        public void RefreshVisibility()
        {
            if (transform.parent != null && transform.parent.GetComponent<BaseSceneBuilding>() != null)
                gameObject.SetActive(transform.parent.GetComponent<BaseSceneBuilding>().Level >= MinLevel);
        }
        void Start()
        {
            RefreshVisibility();
        }
    }
}
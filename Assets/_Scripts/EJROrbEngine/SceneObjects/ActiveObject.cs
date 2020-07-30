// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.SceneObjects;
using UnityEngine;

namespace EJROrbEngine.ActiveObjects
{

    public class ActiveObject : MonoBehaviour
    {      
        public string Type;
        public string RedableName
        {
            get
            {
                if (StringsTranslator.HasString("AOName" + Type))
                    return StringsTranslator.GetString("AOName" + Type);
                else
                    return "<" + Type + ">";
            }
        }
        public string Description
        {
            get
            {
                if (StringsTranslator.HasString("AODesc" + Type))
                    return StringsTranslator.GetString("AODesc" + Type);
                else
                    return "<" + Type + ">";
            }
        }
      
        public int UniqueID { get; set; }

        private Vector3 OriginalScale = new Vector3(1, 1, 1);
        
        public void ClearUsedObject()
        {
            ResetOriginalScale();
        }

        public void ResetOriginalScale()
        {
            Transform originalParent = transform.parent;
            transform.parent = null;
            transform.localScale = OriginalScale;
            transform.parent = originalParent;
        }

        //completaly remove this object (AO manager will stop managing it and the scene object will be destroyed)
        public void RemoveFromGame()
        {
            ActiveObjectsManager.Instance.RemoveActiveObject(this);
            Destroy(gameObject);
        }

        private void Awake()
        {
            OriginalScale = transform.localScale;
            if (GetComponent<PrefabTemplate>() != null)
                Type = GetComponent<PrefabTemplate>().Type;
        }
        
    }
}
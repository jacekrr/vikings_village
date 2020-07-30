// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;

namespace EJROrbEngine
{

    //add this script for every prefab (and its child) that is loaded from asset bundle and has got any material that not load properly from AB (for example has transparency)
    //see: https://issuetracker.unity3d.com/issues/webgl-material-loses-transparency-when-loaded-from-assetbundle
    //material should be located in a Resource folder to be available always
    public class MaterialsPreloader : MonoBehaviour
    {
        public bool MakeFade = true;
        void Start()
        {
            Material[] mats = GetComponent<Renderer>().materials;
            foreach (var m in mats)
            {
                m.shader = Shader.Find(m.shader.name);
                if (MakeFade)
                    MakeMaterialFade(m);
            }
        }

        private void MakeMaterialFade(Material m)
        {
            m.SetFloat("_Mode", 2);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;

        }
    }
}
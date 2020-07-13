// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using UnityEngine;

namespace ClientWinVR
{

    public class WinVRPlayerController : BasePlayerController
    {
        public float SzybkoscKroku;             // szybkość kroku swobodnego gracza [km/h]
        public float NachylenieKroku;           // kat stopniowy maksymalnego nachylenia kroku swobodnego gracza [stopnie]
        private Vector3 _wektorSwobodnegoRuchu;
        private float _NachylenieKrokuTg;      // tangens kata maksymalnego nachylenia kroku swobodnego gracza
        private float _NachylenieSkokuTg;      // tangens kata maksymalnego nachylenia skoku swobodnego gracza

        private const float DlugoscCzasuSkakania = 0.5f; // jak dlugo odbywa sie skok po wykryciu akcji skoku
        private float _licznikSkoku;           

        public override Vector3 GetPlayerPosition()
        {
            return gameObject.transform.parent.position;
        }

        public override void MovePlayer(Terrain teren, Vector3 nowaPozycja)
        {
            float wysokosc = teren.SampleHeight(nowaPozycja);
            gameObject.transform.parent.position = new Vector3(nowaPozycja.x, nowaPozycja.y < wysokosc ? wysokosc : nowaPozycja.y, nowaPozycja.z);
        }

        public void WykonujSwobodnyRuch(Vector3 kierunek)
        {
            _wektorSwobodnegoRuchu = kierunek;
        }

        public void WykonajSkok()
        {
            _licznikSkoku = DlugoscCzasuSkakania;
        }

        private void Start()
        {
            _wektorSwobodnegoRuchu = Vector3.zero;
            if (NachylenieKroku < 1)
                NachylenieKroku = 1;
            if (NachylenieKroku > 89)
                NachylenieKroku = 89;
            _NachylenieKrokuTg = Mathf.Tan(NachylenieKroku * Mathf.PI / 180);
            if(NachylenieKroku < 45)
                _NachylenieSkokuTg = Mathf.Tan( 2 * NachylenieKroku * Mathf.PI / 180);
            else
                _NachylenieSkokuTg = Mathf.Tan( 89 * Mathf.PI / 180);
            _licznikSkoku = 0;
        }

        private void Update()
        {          
            if (_wektorSwobodnegoRuchu != Vector3.zero)
            {
                //obliczenie przesuniecia
                float skalarnyModyfikatorPredkosci = SzybkoscKroku * Time.deltaTime;                
                Vector3 poruszenie = new Vector3(_wektorSwobodnegoRuchu.x , 0, _wektorSwobodnegoRuchu.z );
                poruszenie.Normalize();                
                poruszenie *= skalarnyModyfikatorPredkosci;
                float hNachylenia = poruszenie.magnitude * _licznikSkoku > 0 ? _NachylenieSkokuTg : _NachylenieKrokuTg;

                //obliczenie pozycji glowy i stopy gracza 
                Vector3 pozycjaGlowyBazowa = gameObject.transform.position;
                Vector3  pozycjaStopyBazowa = new Vector3(pozycjaGlowyBazowa.x, gameObject.transform.parent.position.y, pozycjaGlowyBazowa.z);
                //przesuwamy stope z maksymalnym nachyleniem i rzutujemy promien w dol sprawdzajac na jakiej wysokosci jest prog po takim przesunieciu (brak oznaczalby zbyt strome podejscie i wejscie pod teren)
                Vector3 poruszenieZNachyleniem = new Vector3(poruszenie.x, hNachylenia, poruszenie.z);
                RaycastHit trafienieProgu;
                if (Physics.Raycast(pozycjaStopyBazowa + poruszenieZNachyleniem, Vector3.down, out trafienieProgu, 100))
                {
                    Vector3 poruszenieDoProgu = new Vector3(poruszenie.x, trafienieProgu.point.y - pozycjaStopyBazowa.y, poruszenie.z);
                    //Sprawdzamy jeszcze czy po wyznaczonym przesunieciu "cialo" nie koliduje z przeszkodami
                    if (!Physics.CapsuleCast(pozycjaGlowyBazowa, pozycjaGlowyBazowa - new Vector3(0, -0.4f, 0), 0.1f, poruszenieDoProgu, poruszenieDoProgu.magnitude))
                    {
                        gameObject.transform.parent.position += poruszenieDoProgu;
                    }
                }                
            }
            if (_licznikSkoku > 0)
                _licznikSkoku -= Time.deltaTime;
        }        
    }
}

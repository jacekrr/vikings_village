// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using System;
using UnityEngine;
namespace EJROrbEngine
{
    public class EnviroDayNightWeather: BaseDayNightWeather
    {
        private int _poprzedniaMinuta, _poprzedniaGodzina;

        public override void ChangeDayLength(int oIleSkroc)
        {
            base.ChangeDayLength(oIleSkroc);
            EnviroSkyMgr.instance.Time.DayLengthInMinutes = DayHours;
            EnviroSkyMgr.instance.Time.NightLengthInMinutes = NightHours;
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
            EnviroSkyMgr.instance.SetTimeOfDay(CurrentDayHour);
            EnviroSkyMgr.instance.Time.DayLengthInMinutes = DayHours;
            EnviroSkyMgr.instance.Time.NightLengthInMinutes = NightHours;
            _poprzedniaMinuta = EnviroSkyMgr.instance.Time.Minutes;
            _poprzedniaGodzina = EnviroSkyMgr.instance.Time.Hours;
        }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy EnviroDayNightWeather");
            Instance = this;
            _poprzedniaMinuta = -1;
            _poprzedniaGodzina = -1;
        }

        private void Start()
        {
            Camera.main.GetComponent<EnviroSkyRendering>().enabled = true;
            DayHours = (int)EnviroSkyMgr.instance.Time.DayLengthInMinutes;
            NightHours = (int)EnviroSkyMgr.instance.Time.NightLengthInMinutes;
        }
        void Update()
        {
            CurrentDayHour = EnviroSkyMgr.instance.Time.Hours + EnviroSkyMgr.instance.Time.Minutes / 60f;
            if (_poprzedniaMinuta != EnviroSkyMgr.instance.Time.Minutes)
            {
                _poprzedniaMinuta = EnviroSkyMgr.instance.Time.Minutes;
                ChangeOfMinutes(_poprzedniaMinuta);
                
            }
            if (_poprzedniaGodzina != EnviroSkyMgr.instance.Time.Hours)
            {
                _poprzedniaGodzina = EnviroSkyMgr.instance.Time.Hours;
                if (EnviroSkyMgr.instance.Time.Hours == 0)
                    DayOfGame++;
                ChangeOfHours(_poprzedniaGodzina);

            }
            onUpdate();
        }

    }
}
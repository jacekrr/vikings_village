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
    //obsluga zdarzen zmiany czasu, zapisujac sie z zewnatrz do BaseDayNightWeather.MinuteChanged i BaseDayNightWeather.HourChanged dowolny sluchacz moze byc informowany o zmianie wartosci czasu gry
    public delegate void TimeChangedDelegate(int currentValue);

    public class BaseDayNightWeather : MonoBehaviour
    {
        public const int LONGEST_DAY_HOURS = 15;
        public const int SHORTEST_DAY_HOURS = 3;
        public static BaseDayNightWeather Instance { get; protected set; }
        public Light Sun;
        public int DayHours = 9;                   // dlugosc 12-godzin dnia podana w minutach
        public int NightHours = 9;                   // dlugosc 12-godzin nocy podana w minutach
        public float CurrentDayHour = 12;          //aktualna godzina w swiecie gry, 0 oznacza polnoc, kazde 1 kolejna godzine (np. 12 oznacza 12 rano). Noc zawiera sie miedzy 19 a 7 rano

        public bool IsDay { get { return CurrentDayHour >= 7 && CurrentDayHour < 19; } }
        public int DayOfGame { get; protected set; }
        public int Hour { get { return (int)CurrentDayHour; } }
        public int Minute { get { return (int)((CurrentDayHour - Hour) * 60); } }

        public event TimeChangedDelegate SecondChanged;         //minela PRAWDZIWA sekunda, uwaga - currentValue zawsze 0
        public event TimeChangedDelegate MinuteChanged;         //minela WIRTUALNA minuta
        public event TimeChangedDelegate HourChanged;           //minela WIRTUALNA godzina

        private float _timer, _secondsTimer;
        private const float _calculationsInterval = 0.05f;   //jak czesto czynimy obliczenia (w sekundach)

        public void AddTime(float liczbaGodzin)
        {
            int oldGodzina = Hour;
            int oldMinuta = Minute;
            //oblicz nowy czas dnia
            CurrentDayHour += liczbaGodzin;
            if (CurrentDayHour >= 24)
                CurrentDayHour -= 24;
            if (oldMinuta != Minute)
                ChangeOfMinutes(Minute);
            if (oldGodzina != Hour)
                ChangeOfHours(Hour);
            if (oldGodzina > Hour)
                DayOfGame++;
            //ustaw odpowiednio Slonce
            float czescDnia = (CurrentDayHour - 7) / 24;
            float katSlonca = czescDnia * 360;
            Vector3 punktPosredni = new Vector3(0, 0, 0);
            Sun.transform.position = punktPosredni + Quaternion.Euler(0, 0, katSlonca) * (10 * Vector3.right);
            Sun.transform.LookAt(punktPosredni);
        }

        public virtual void ChangeDayLength(int oIleSkroc)
        {
            DayHours += oIleSkroc;
            if (DayHours < SHORTEST_DAY_HOURS)
                DayHours = SHORTEST_DAY_HOURS;
            if (DayHours > LONGEST_DAY_HOURS)
                DayHours = LONGEST_DAY_HOURS;
            NightHours = DayHours;
        }

        public virtual void SaveGame(IGameState gameState)
        {
            gameState.SetKey("vHour", CurrentDayHour);
            gameState.SetKey("dayGame", DayOfGame);
            gameState.SetKey("dayLen", DayHours);
        }
        
        public virtual void LoadGame(IGameState gameState)
        {
            if (gameState.KeyExists("vHour"))
                CurrentDayHour = gameState.GetFloatKey("vHour");
            if (gameState.KeyExists("dayGame"))
                DayOfGame = gameState.GetIntKey("dayGame");
            if (gameState.KeyExists("dayLen"))
                DayHours = gameState.GetIntKey("dayLen");
            NightHours = DayHours;
        }
        protected void ChangeOfMinutes(int minuta)
        {
            MinuteChanged?.Invoke(minuta);
        }

        protected void ChangeOfHours(int godzina)
        {
            HourChanged?.Invoke(godzina);
            //   Debug.Log("godzina: " + Godzina + " minuta: " + Minuta);
        }
        protected void onUpdate()
        {
            _secondsTimer -= Time.deltaTime;
            if (_secondsTimer < 0)
            {
                _secondsTimer = 1f;
                SecondChanged?.Invoke(0);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy BaseDayNightWeather");
            Instance = this;
        }

        void Start()
        {
            if (Sun == null)
                Debug.LogError("Pole Sun w komponencie BaseDayNightWeather jest równe null.");
            _timer = 0;
            _secondsTimer = 0;
            DayOfGame = 0;
        }

        void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _timer = _calculationsInterval;
                TimeFlow();
            }
            onUpdate();
        }

        private void TimeFlow()
        {
            if (IsDay)
                AddTime(1f / (DayHours * 60 / (_calculationsInterval * 12)));
            else
                AddTime(1f / (NightHours * 60 / (_calculationsInterval * 12)));
        }
    }
}
﻿using System;
using AquaPic.StateRuntime;

namespace AquaPic.TimerRuntime
{
    public delegate void TimerElapsedHandler (object sender, TimerElapsedEventArgs args);

    public class TimerElapsedEventArgs : EventArgs
    {
        public DateTime signalTime;

        public TimerElapsedEventArgs () {
            signalTime = DateTime.Now;
        }
    }

    public class Timer
    {
        private bool _enabled;
        public bool enabled {
            get {
                return _enabled;
            }
            set {
                _enabled = value;
                if (_enabled)
                    Start ();
            }
        }

        public TimerElapsedHandler TimerElapsedEvent;
        public uint timerInterval; 
        public bool autoReset;

        public Timer () : this (0) { }

        public Timer (uint timerInterval) {
            _enabled = false;
            this.timerInterval = timerInterval;
            autoReset = true;
        }

        public void Start () {
            _enabled = true;
            GLib.Timeout.Add (timerInterval, OnTimeout);
        }

        public void Stop () {
            _enabled = false;
        }

        protected bool OnTimeout () {
            if (_enabled) {
                if (TimerElapsedEvent != null)
                    TimerElapsedEvent (this, new TimerElapsedEventArgs ());
            }
            return _enabled & autoReset;
        }

        public static uint FromSeconds (double seconds) {
            return (uint)Math.Round (seconds * 1000);

        }

//        public static uint ParseTimer (string timeInterval) {
//            int pos = timeInterval.IndexOf (':');
//            while (pos != -1) {
//
//            }
//        }
    }
}


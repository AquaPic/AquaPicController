﻿using System;
using Gtk;
using Cairo;
using MyWidgetLibrary;

namespace AquaPic
{
    public class WaveWindow : MyBackgroundWidget
    {
        public WaveWindow (params object[] options) : base () {
            ShowAll ();
        }
    }
}

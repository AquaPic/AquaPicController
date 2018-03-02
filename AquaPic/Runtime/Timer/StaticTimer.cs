#region License

/*
    AquaPic Main Control - Handles all functionality for the AquaPic aquarium controller.

    Copyright (c) 2017 Goodtime Development

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/
*/

#endregion // License

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AquaPic.Runtime
{
    public partial class Timer
    {
        protected static Dictionary<string, OnDelayTimer> staticOnDelayTimers = new Dictionary<string, OnDelayTimer> ();

        public static bool OnDelay (string name, string time, bool enable) {
            if (!staticOnDelayTimers.ContainsKey (name)) {
                uint timeDelay = ParseTime (time);
                staticOnDelayTimers.Add (name, new OnDelayTimer (timeDelay));
            }

            return staticOnDelayTimers [name].Evaluate (enable);
        }

        public static uint ParseTime (string timeString) {
            char[] seperator = new char[1] {':'};
            string[] t = timeString.Split (seperator, 3);

            uint time = 0;
            if (t.Length == 3) {
                //milliseconds
                time = Convert.ToUInt32 (t [2]);

                //seconds
                time += (Convert.ToUInt32 (t [1]) * 1000);

                //minutes
                time += (Convert.ToUInt32 (t [0]) * 60000);
            }

            return time;
        }
    }
}


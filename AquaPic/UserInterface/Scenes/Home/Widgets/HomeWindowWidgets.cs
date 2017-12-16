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

namespace AquaPic.UserInterface
{
    public class HomeWindowWidgets
    {
        public static Dictionary<string, LinePlotData> linePlots;
        public static Dictionary<string, BarPlotData> barPlots;
        public static Dictionary<string, CurvedBarPlotData> curvedBarPlots;

        static HomeWindowWidgets () {
            linePlots = new Dictionary<string, LinePlotData> () {
                { "Temperature", new LinePlotData ((options) => {return new TemperatureLinePlot (options);}) },
                { "WaterLevel", new LinePlotData ((options) => {return new WaterLevelLinePlot (options);}) }
            };

            barPlots = new Dictionary<string, BarPlotData> () {
                { "WaterLevel", new BarPlotData ((options) => {return new WaterLevelWidget (options);}) }
            };

            curvedBarPlots = new Dictionary<string, CurvedBarPlotData> ();
        }
    }
}

{return new WaterLevelWidget (options);}) }
            };

            curvedBarPlots = new Dictionary<string, CurvedBarPlotData> ();
        }
    }
}


﻿#region License

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
using Gtk;
using Cairo;
using GoodtimeDevelopment.TouchWidget;

namespace AquaPic.UserInterface
{
    public class SensorsWindow : SceneBase
    {
        SensorWidget widget;
        TouchComboBox sensorTypeCombo;

        readonly string[] analogSensorNames = { "Water Level Sensor", "Temperature Probe", "pH Probe", "SG Sensor" };

        public SensorsWindow (params object[] options) {
            sceneTitle = "Sensors";

            widget = new WaterLevelSensorWidget ();
            Put (widget, 210, 77);
            widget.Show ();

            sensorTypeCombo = new TouchComboBox (analogSensorNames);
            sensorTypeCombo.WidthRequest = 235;
            sensorTypeCombo.ComboChangedEvent += OnComboChange;
            sensorTypeCombo.activeIndex = 0;
            Put (sensorTypeCombo, 550, 34);
            sensorTypeCombo.Show ();

            widget.GetSensorData ();

            Show ();
        }

        protected void OnComboChange (object sender, ComboBoxChangedEventArgs args) {
            widget.Destroy ();
            widget = SensorWidgetCreater (args.activeText);
            Put (widget, 210, 77);
            widget.Show ();
            widget.GetSensorData ();
        }

        protected SensorWidget SensorWidgetCreater (string name) {
            SensorWidget widget = null;

            switch (name) {
            case "Water Level Sensor":
                widget = new WaterLevelSensorWidget ();
                break;
            case "Temperature Probe":
                widget = new TemperatureProbeWidget ();
                break;
            case "pH Probe":
                widget = new PhProbeWidget ();
                break;
            case "SG Sensor":
                widget = new SpecificGravitySensorWidget ();
                break;
            }

            return widget;
        }
    }
}

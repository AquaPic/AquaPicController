﻿#region License

/*
 AquaPic Main Control - Handles all functionality for the AquaPic aquarium controller.

 Copyright (c) 2019 Goodtime Development

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
using GoodtimeDevelopment.Utilites;
using GoodtimeDevelopment.TouchWidget;
using AquaPic.Gadgets.Sensor;
using AquaPic.Drivers;

namespace AquaPic.UserInterface
{
    public class TemperatureProbeWidget : AnalogSensorWidget
    {
        public TemperatureProbeWidget () 
            : base ("Probes", Sensors.TemperatureProbes, Driver.AnalogInput, typeof (TemperatureProbeSettings)) { }

        public override void GetSensorData () {
            if (sensorName.IsNotEmpty ()) {
                var probe = Sensors.TemperatureProbes.GetGadget (sensorName) as TemperatureProbe;
                if (probe.connected) {
                    sensorStateTextbox.text = Convert.ToSingle (probe.value).ToString ("F2");
                    sensorStateTextbox.textRender.unitOfMeasurement = UnitsOfMeasurement.Degrees;
                    sensorLabel.Visible = true;
                } else {
                    sensorStateTextbox.text = "Probe disconnected";
                    sensorStateTextbox.textRender.unitOfMeasurement = UnitsOfMeasurement.None;
                    sensorLabel.Visible = false;
                }
            } else {
                sensorStateTextbox.text = "Probe not available";
                sensorStateTextbox.textRender.unitOfMeasurement = UnitsOfMeasurement.None;
                sensorLabel.Visible = false;
            }

            sensorStateTextbox.QueueDraw ();
        }
    }
}

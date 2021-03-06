﻿#region License

/*
    AquaPic Main Control - Handles all functionality for the AquaPic aquarium controller.

    Copyright (c) 2018 Goodtime Development

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
using AquaPic.Service;

namespace AquaPic.Gadgets.Sensor
{
    public class FloatSwitchCollection : GenericSensorCollection
    {
        public static FloatSwitchCollection SharedFloatSwitchCollectionInstance = new FloatSwitchCollection ();

        protected FloatSwitchCollection () : base ("floatSwitches") { }

        public override void ReadAllGadgetsFromFile () {
            var sensorSettings = SettingsHelper.ReadAllSettingsInArray<FloatSwitchSettings> (gadgetSettingsFileName, gadgetSettingsArrayName);
            foreach (var setting in sensorSettings) {
                CreateGadget (setting, false);
            }
        }

        protected override GenericGadget GadgetCreater (GenericGadgetSettings settings) {
            var floatSwitchSettings = settings as FloatSwitchSettings;
            if (floatSwitchSettings == null) {
                throw new ArgumentException ("Settings must be FloatSwitchSettings");
            }

            if ((floatSwitchSettings.switchFuntion == SwitchFunction.HighLevel) && (floatSwitchSettings.switchType != SwitchType.NormallyClosed)) {
                Logger.AddWarning ("High level switch should be normally closed");
            } else if ((floatSwitchSettings.switchFuntion == SwitchFunction.LowLevel) && (floatSwitchSettings.switchType != SwitchType.NormallyClosed)) {
                Logger.AddWarning ("Low level switch should be normally closed");
            } else if ((floatSwitchSettings.switchFuntion == SwitchFunction.ATO) && (floatSwitchSettings.switchType != SwitchType.NormallyOpened)) {
                Logger.AddWarning ("ATO switch should be normally opened");
            }

            var floatSwitch = new FloatSwitch (floatSwitchSettings);

            return floatSwitch;
        }

        public override GenericGadgetSettings GetGadgetSettings (string name) {
            CheckGadgetKey (name);
            var floatSwitch = gadgets[name] as FloatSwitch;
            var settings = new FloatSwitchSettings ();
            settings.name = name;
            settings.channel = floatSwitch.channel;
            settings.physicalLevel = floatSwitch.physicalLevel;
            settings.switchType = floatSwitch.switchType;
            settings.switchFuntion = floatSwitch.switchFuntion;
            settings.timeOffset = floatSwitch.timeOffset;
            return settings;
        }
    }
}

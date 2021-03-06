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
using GoodtimeDevelopment.Utilites;
using AquaPic.Service;

namespace AquaPic.Gadgets.Device
{
    public class GenericDevice : GenericGadget
    {
        public GenericDevice (GenericDeviceSettings settings, uint runtime = 1000) 
            : base (settings) {
            TaskManager.Instance.AddCyclicInterrupt (name.RemoveWhitespace () + "CyclicRuntime", runtime, Run);
        }

        protected virtual void Run () {
            try {
                var oldValue = value;
                value = OnRun ();
                if (!value.Equals (oldValue)) {
                    NotifyValueChanged (name, value, oldValue);
                }
            } catch (NotImplementedException) {
                Logger.AddWarning (name + " does not have an implemented Runtime function");
                TaskManager.Instance.RemoveCyclicInterrupt (name.RemoveWhitespace () + "CyclicRuntime");
            }
        }

        protected virtual ValueType OnRun () => throw new NotImplementedException ();

        public override void Dispose () {
            TaskManager.Instance.RemoveCyclicInterrupt (name.RemoveWhitespace () + "CyclicRuntime");
        }
    }
}

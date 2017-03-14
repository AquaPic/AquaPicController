﻿using System;
using System.IO.Ports; // for SerialPort
using System.Collections.Generic;
using Gtk;
using Cairo;
using TouchWidgetLibrary;
using AquaPic.Drivers;
using AquaPic.Modules;
using AquaPic.Runtime;
using AquaPic.SerialBus;
using AquaPic.Utilites;

namespace AquaPic.UserInterface
{
    public class SerialBusWindow : SceneBase
    {
        SerialBusSlaveWidget[] slaves;
        TouchComboBox c;
        uint timerId;

        public SerialBusWindow (params object[] options) : base () {
            sceneTitle = "AquaPic Bus";

            var l = new TouchLabel ();
            l.text = "Name";
            l.textAlignment = TouchAlignment.Center;
            l.WidthRequest = 235;
            Put (l, 60, 121);
            l.Show ();

            l = new TouchLabel ();
            l.text = "Address";
            l.textAlignment = TouchAlignment.Center;
            l.WidthRequest = 75;
            Put (l, 303, 121);
            l.Show ();

            l = new TouchLabel ();
            l.text = "Status";
            l.textAlignment = TouchAlignment.Center;
            l.WidthRequest = 295;
            Put (l, 380, 121);
            l.Show ();

            l = new TouchLabel ();
            l.text = "Response Time";
            l.textAlignment = TouchAlignment.Right;
            l.WidthRequest = 170;
            Put (l, 605, 121);
            l.Show ();

            slaves = new SerialBusSlaveWidget[AquaPicBus.slaveCount];
            string[] names = AquaPicBus.slaveNames;
            int[] addresses = AquaPicBus.slaveAdresses;

            for (int i = 0; i < slaves.Length; ++i) {
                slaves[i] = new SerialBusSlaveWidget ();
                slaves[i].name = names[i];
                slaves[i].address = addresses[i];
                Put (slaves[i], 65, (i * 35) + 143);
                slaves[i].Show ();
            }

            if (AquaPicBus.slaveCount == 0) {
                var blankWidget = new SerialBusSlaveWidget ();
                Put (blankWidget, 65, 143);
                blankWidget.Show ();
            }

            var b = new TouchButton ();
            b.SetSizeRequest (100, 30);
            b.text = "Open";
            if (AquaPicBus.isOpen) {
                b.buttonColor = "grey3";
            } else {
                b.ButtonReleaseEvent += OnOpenButtonRelease;
            }
            Put (b, 685, 70);
            b.Show ();

            c = new TouchComboBox ();
            if (!AquaPicBus.isOpen) {
                string[] portNames = SerialPort.GetPortNames ();
                if (Utils.ExecutingOperatingSystem == Platform.Linux) {
                    List<string> sortedPortNames = new List<string> ();
                    foreach (var name in portNames) {
                        if (name.Contains ("USB")) {
                            sortedPortNames.Add (name);
                        }
                    }
                    portNames = sortedPortNames.ToArray ();
                }
                c.comboList.AddRange (portNames);
                c.nonActiveMessage = "Select Port";
            } else {
                c.comboList.Add (AquaPicBus.portName);
                c.activeText = AquaPicBus.portName;
            }
            c.WidthRequest = 300;
            Put (c, 380, 70);
            c.Show ();

            var addSlaveBtn = new TouchButton ();
            addSlaveBtn.text = "Add Slave";
            addSlaveBtn.SetSizeRequest (100, 30);
            addSlaveBtn.ButtonReleaseEvent += (o, args) => {
                var s = new AddSlaveSettings ();
                s.Run ();
                s.Destroy ();
                s.Dispose ();
            };
            Put (addSlaveBtn, 65, 70);
            addSlaveBtn.Show ();

            GetSlaveData ();

            timerId = GLib.Timeout.Add (1000, OnTimer);

            Show ();
        }

        public override void Dispose () {
            GLib.Source.Remove (timerId);
            base.Dispose ();
        }

        protected void GetSlaveData () {
            AquaPicBusStatus[] status = AquaPicBus.slaveStatus;
            int[] times = AquaPicBus.slaveResponseTimes;

            for (int i = 0; i < slaves.Length; ++i) {
                slaves [i].status = status [i];
                slaves [i].responseTime = times [i];
                slaves [i].QueueDraw ();
            }
        }

        protected bool OnTimer () {
            GetSlaveData ();

            return true;
        }

        protected void OnOpenButtonRelease (object sender, ButtonReleaseEventArgs args) {
            TouchButton b = sender as TouchButton;

            if (b != null) { 
                if (c.activeIndex != -1) {
                    AquaPicBus.Open (c.activeText);
                    if (AquaPicBus.isOpen) {
                        b.buttonColor = "grey3";
                        b.ButtonReleaseEvent -= OnOpenButtonRelease;
                        b.QueueDraw ();
                    }
                } else
                    MessageBox.Show ("No communication port selected");
            }
        }
    }
}

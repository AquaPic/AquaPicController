﻿using System;
using Gtk;
using Cairo;
using TouchWidgetLibrary;
using AquaPic.Drivers;
using AquaPic.Utilites;

namespace AquaPic.UserInterface
{
    public class DigitalInputWindow : SceneBase
    {
        TouchComboBox combo;
        int cardId;
        uint timerId;

        DigitalDisplay[] displays;

        public DigitalInputWindow (params object[] options) : base () {
            sceneTitle = "Digital Input Cards";

            if (AquaPicDrivers.DigitalInput.cardCount == 0) {
                cardId = -1;
                sceneTitle = "No Digital Input Cards Added";
                Show ();
                return;
            }

            cardId = 0;

            displays = new DigitalDisplay[6];

            for (int i = 0; i < 6; ++i) {
                displays [i] = new DigitalDisplay ();
                displays [i].ForceButtonReleaseEvent += OnForceRelease;
                displays [i].StateSelectedChangedEvent += OnSelectorChanged;
                int x, y;
                if (i < 3) {
                    x = (i * 150) + 175;
                    y = 125;
                } else {
                    x = ((i - 3) * 150) + 175;
                    y = 275;
                }
                Put (displays [i], x, y);
                displays [i].Show ();
            }

            string[] names = AquaPicDrivers.DigitalInput.GetAllCardNames ();
            combo = new TouchComboBox (names);
            combo.activeIndex = cardId;
            combo.WidthRequest = 235;
            combo.ComboChangedEvent += OnComboChanged;
            Put (combo, 550, 35);
            combo.Show ();

            GetCardData ();

            timerId = GLib.Timeout.Add (1000, OnUpdateTimer);

            Show ();
        }

        public override void Dispose () {
            GLib.Source.Remove (timerId);
            base.Dispose ();
        }

        protected bool OnUpdateTimer () {
            bool[] states = AquaPicDrivers.DigitalInput.GetAllChannelValues (cardId);

            for (int i = 0; i < states.Length; ++i) {
                if (states [i]) {
                    displays [i].textBox.textColor = "pri";
                    displays [i].textBox.text = "Closed";
                } else {
                    displays [i].textBox.textColor = "seca";
                    displays [i].textBox.text = "Open";
                }

                displays [i].textBox.QueueDraw ();
            }

            return true;
        }

        protected void OnComboChanged (object sender, ComboBoxChangedEventArgs e) {
            int id = AquaPicDrivers.DigitalInput.GetCardIndex (e.activeText);
            if (id != -1) {
                cardId = id;
                GetCardData ();
            }
        }

        protected void OnForceRelease (object sender, ButtonReleaseEventArgs args) {
            DigitalDisplay d = sender as DigitalDisplay;

            IndividualControl ic;
            ic.Group = (byte)cardId;
            ic.Individual = AquaPicDrivers.DigitalInput.GetChannelIndex (cardId, d.label.text);

            Mode m = AquaPicDrivers.DigitalInput.GetChannelMode (ic);

            if (m == Mode.Auto) {
                AquaPicDrivers.DigitalInput.SetChannelMode (ic, Mode.Manual);
                d.selector.Visible = true;
                d.button.buttonColor = "pri";
            } else {
                AquaPicDrivers.DigitalInput.SetChannelMode (ic, Mode.Auto);
                d.selector.Visible = false;
                d.button.buttonColor = "grey4";
            }

            d.QueueDraw ();
        }

        protected void OnSelectorChanged (object sender, SelectorChangedEventArgs args) {
            DigitalDisplay d = sender as DigitalDisplay;

            IndividualControl ic;
            ic.Group = cardId;
            ic.Individual = AquaPicDrivers.DigitalInput.GetChannelIndex (cardId, d.label.text);

            bool oldState = AquaPicDrivers.DigitalInput.GetChannelValue (ic);
            bool newState = args.currentSelectedIndex == 1;

            if (!oldState && newState) {
                AquaPicDrivers.DigitalInput.SetChannelValue (ic, true);
                d.textBox.textColor = "pri";
                d.textBox.text = "Closed";
            } else if (oldState && !newState) {
                AquaPicDrivers.DigitalInput.SetChannelValue (ic, false);
                d.textBox.textColor = "seca";
                d.textBox.text = "Open";
            }

            d.QueueDraw ();
        }

        protected void GetCardData () {
            bool[] states = AquaPicDrivers.DigitalInput.GetAllChannelValues (cardId);
            Mode[] modes = AquaPicDrivers.DigitalInput.GetAllChannelModes (cardId);
            string[] names = AquaPicDrivers.DigitalInput.GetAllChannelNames (cardId);

            int i = 0;
            foreach (var d in displays) {
                d.label.text = names [i];

                if (states [i]) {
                    d.textBox.textColor = "pri";
                    d.textBox.text = "Closed";
                    d.selector.currentSelected = 1;
                } else {
                    d.textBox.textColor = "seca";
                    d.textBox.text = "Open";
                    d.selector.currentSelected = 0;
                }

                if (modes [i] == Mode.Auto) {
                    d.selector.Visible = false;
                    d.button.buttonColor = "grey4";
                } else {
                    d.selector.Visible = true;
                    d.button.buttonColor = "pri";
                }

                d.QueueDraw ();

                ++i;
            }
        }
    }
}

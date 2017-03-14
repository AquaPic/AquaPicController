﻿using System;
using Gtk;
using Cairo;
using TouchWidgetLibrary;
using AquaPic.Utilites;

namespace AquaPic.UserInterface
{
    public delegate void ValueChangedHandler (object sender, float value);

    public class AnalogChannelDisplay : Fixed
    {
        public event ButtonReleaseEventHandler ForceButtonReleaseEvent;
        public event ValueChangedHandler ValueChangedEvent;
        public event SelectorChangedEventHandler TypeSelectorChangedEvent;

        public TouchLabel label;
        public TouchTextBox textBox;
        public TouchProgressBar progressBar;
        public TouchLabel typeLabel;
        public TouchButton button;
        public TouchSelectorSwitch ss;

        public int divisionSteps;

        public float currentValue {
            set {
                int v = value.ToInt ();
                textBox.text = v.ToString ("D");

                progressBar.currentProgress = value / divisionSteps;
            }
        }

        public AnalogChannelDisplay () {
            SetSizeRequest (710, 65);

            label = new TouchLabel ();
            Put (label, 5, 15);
            label.Show ();

            textBox = new TouchTextBox ();
            textBox.WidthRequest = 175; 
            textBox.TextChangedEvent += (sender, args) => {
                try {
                    currentValue = Convert.ToSingle (args.text);
                    ValueChanged ();
                } catch {
                    ;
                }
            };
            Put (textBox, 0, 35);
            textBox.Show ();

            progressBar = new TouchProgressBar (TouchOrientation.Horizontal);
            progressBar.WidthRequest = 415;
            progressBar.ProgressChangedEvent += (sender, args) => {
                currentValue = args.currentProgress * (float)divisionSteps;
                ValueChanged ();
            };
            Put (progressBar, 185, 35);
            progressBar.Show ();

            typeLabel = new TouchLabel ();
            typeLabel.Visible = false;
            typeLabel.WidthRequest = 200;
            typeLabel.textAlignment = TouchAlignment.Right;
            Put (typeLabel, 500, 15);

            button = new TouchButton ();
            button.SetSizeRequest (100, 30);
            button.buttonColor = "grey3";
            button.text = "Force";
            button.ButtonReleaseEvent += OnForceReleased;
            Put (button, 610, 35);
            button.Show ();

            ss = new TouchSelectorSwitch (2);
            ss.SetSizeRequest (100, 30);
            ss.sliderColorOptions [0] = "pri";
            ss.sliderColorOptions [1] = "seca";
            ss.SelectorChangedEvent += OnSelectorSwitchChanged;
            ss.ExposeEvent += OnExpose;
            ss.Visible = false;
            Put (ss, 610, 0);

            Show ();
        }

        protected void OnForceReleased (object sender, ButtonReleaseEventArgs args) {
            if (ForceButtonReleaseEvent != null)
                ForceButtonReleaseEvent (this, args);
            else
                throw new NotImplementedException ("Force button release not implemented");
        }

        protected void ValueChanged () {
            if (ValueChangedEvent != null)
                ValueChangedEvent (this, progressBar.currentProgress * (float)divisionSteps);
            else
                throw new NotImplementedException ("Value changed not implemented");
        }

        protected void OnSelectorSwitchChanged (object sender, SelectorChangedEventArgs args) {
            if (TypeSelectorChangedEvent != null)
                TypeSelectorChangedEvent (this, args);
            else
                throw new NotImplementedException ("Type selector not impletemented");
        }

        protected void OnExpose (object sender, ExposeEventArgs args) {
            TouchSelectorSwitch ss = sender as TouchSelectorSwitch;
            int seperation = ss.Allocation.Width / ss.selectionCount;
            int x = ss.Allocation.Left;

            TouchText render = new TouchText ();
            render.textWrap = TouchTextWrap.Shrink;
            render.alignment = TouchAlignment.Center;
            render.font.color = "white";

            string[] labels = {"0-10V", "PWM"};

            foreach (var l in labels) {
                render.text = l;
                render.Render (ss, x, ss.Allocation.Top, seperation, ss.Allocation.Height);
                x += seperation;
            }
        }
    }
}

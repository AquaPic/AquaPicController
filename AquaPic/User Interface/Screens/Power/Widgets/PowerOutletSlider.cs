﻿using System;
using Gtk;
using Cairo;
using TouchWidgetLibrary;
using AquaPic.Utilites;
using AquaPic.Drivers;

namespace AquaPic.UserInterface
{
    public delegate void UpdateScreenHandler ();

    public class PowerOutletSlider : Fixed
    {
        public UpdateScreenHandler UpdateScreen;

        public TouchLabel outletName;
        public TouchLabel statusLabel;
        public TouchSelectorSwitch ss;
        public TouchCurvedProgressBar ampBar;
        public TouchLabel ampText;
        public EventBox settingsButton;

        public float amps {
            set {
                float v;
                if (value > 10.0f)
                    v = 10.0f;
                else if (value < 0.0f)
                    v = 0.0f;
                else
                    v = value;

                ampText.text = v.ToString ("F1");
                ampBar.progress = v / 10.0f;
            }
        }

        public PowerOutletSlider (int id) {
            SetSizeRequest (180, 180);

            ampBar = new TouchCurvedProgressBar ();
            ampBar.SetSizeRequest (170, 135);
            ampBar.curveStyle = CurveStyle.ThreeQuarterCurve;
            Put (ampBar, 5, 5);
            ampBar.Show ();

            ampText = new TouchLabel ();
            ampText.WidthRequest = 180;
            ampText.textAlignment = TouchAlignment.Center;
            ampText.textRender.unitOfMeasurement = UnitsOfMeasurement.Amperage;
            ampText.text = "0.0";
            ampText.textSize = 20;
            ampText.textColor = "pri";
            Put (ampText, 0, 105);
            ampText.Show ();

            ss = new TouchSelectorSwitch (id, 3, 0, TouchOrientation.Horizontal);
            ss.sliderSize = MySliderSize.Large;
            ss.WidthRequest = 170;
            ss.HeightRequest = 30;
            ss.sliderColorOptions [0] = "grey2";
            ss.sliderColorOptions [1] = "pri";
            ss.sliderColorOptions [2] = "seca";
            ss.textOptions [0] = "Off";
            ss.textOptions [1] = "Auto";
            ss.textOptions [2] = "On";
            Put (ss, 5, 145);
            ss.Show ();

            outletName = new TouchLabel ();
            outletName.textColor = "grey3";
            outletName.WidthRequest = 100;
            outletName.textRender.textWrap = TouchTextWrap.Shrink;
            outletName.textAlignment = TouchAlignment.Center;
            Put (outletName, 40, 67);
            outletName.Show ();

            statusLabel = new TouchLabel ();
            statusLabel.text = "Off";
            statusLabel.textSize = 20;
            statusLabel.textColor = "grey4";
            statusLabel.WidthRequest = 180;
            statusLabel.textAlignment = TouchAlignment.Center;
            Put (statusLabel, 0, 37);
            statusLabel.Show ();

            settingsButton = new EventBox ();
            settingsButton.VisibleWindow = false;
            settingsButton.SetSizeRequest (180, 140);
            settingsButton.ButtonReleaseEvent += OnSettingButtonRelease;
            Put (settingsButton, 0, 0);
            settingsButton.Show ();

            ShowAll ();
        }

        protected void OnSettingButtonRelease (object sender, ButtonReleaseEventArgs args) {
            IndividualControl ic = Power.GetOutletIndividualControl (outletName.text);
            string owner = Power.GetOutletOwner (ic);
            if (owner == "Power") {
                string n = string.Format ("{0}.p{1}", Power.GetPowerStripName (ic.Group), ic.Individual);
                OutletSettings os;
                if (n == outletName.text)
                    os = new OutletSettings (outletName.text, false, ic);
                else
                    os = new OutletSettings (outletName.text, true, ic);

                os.Run ();
                os.Destroy ();

                if (UpdateScreen != null)
                    UpdateScreen ();
            } else {
                MessageBox.Show ("Can't edit outlet,\nOwned by " + owner);
            }
        }
    }

//    public class MyAmpMeter : EventBox
//    {
//        private double _currentAmps;
//        public double currentAmps {
//            get {
//                return _currentAmps;
//            }
//            set {
//                if (value > 10.0)
//                    _currentAmps = 10.0;
//                else if (value < 0.0)
//                    _currentAmps = 0.0;
//                else
//                    _currentAmps = value;
//            }
//        }
//
//        public MyAmpMeter () {
//            SetSizeRequest (160, 70);
//            WidthRequest = 160;
//            VisibleWindow = false;
//            Visible = true;
//
//            _currentAmps = 0.0;
//
//            ExposeEvent += OnEventBoxExpose;
//        }
//
//        protected void OnEventBoxExpose (object sender, ExposeEventArgs args) {
//            EventBox eb = sender as EventBox;
//            using (Context cr = Gdk.CairoHelper.Create (eb.GdkWindow)) {
//                int x = eb.Allocation.Left;
//                int y = eb.Allocation.Top;
//                int width = eb.Allocation.Width;
//                int height = eb.Allocation.Height;
//
//                WidgetGlobal.DrawRoundedRectangle (cr, x, y, width, height, 5);
//                MyColor.SetSource (cr, "white");
//                cr.FillPreserve ();
//
//                MyColor.SetSource (cr, "black");
//                cr.LineWidth = 0.5;
//                cr.Stroke ();
//
//                MyText t = new MyText ();
//
//                double radians, posX, posY;
//
//                double radius = CalcRadius (width - 13, height);
//                double originX = x + (width / 2) - 6.5;
//                double originY = y + radius;
//
//                for (int i = 0; i < 11; ++i) {
//                    radians = -15 * i + 165; //165 - 15 degrees
//                    radians = radians.ToRadians ();
//
//                    posX = CalcX (originX, radius, radians);
//                    posY = CalcY (originY, radius, -radians);
//
//                    t.text = i.ToString ();
//                    t.font.size = 8;
//                    t.font.color = "black";
//                    t.Render (eb, posX.ToInt (), posY.ToInt (), 10);
//                }
//
//                radius = CalcRadius (width, height - 6);
//                originX = x + (width / 2);
//                originY = y + 6 + radius;
//
//                radians = -14.4 * _currentAmps + 162; 
//                radians = radians.ToRadians ();
//
//                posX = CalcX (originX, radius, radians);
//                posY = CalcY (originY, radius, -radians);
//
//                cr.MoveTo (posX, posY);
//                cr.LineTo (originX, y + height - 2);
//                cr.LineWidth = 2.0;
//                MyColor.SetSource (cr, "compl");
//                cr.Stroke ();
//
//                WidgetGlobal.DrawRoundedRectangle (cr, x + 53, y + 25, 54, 16, 2);
//                MyColor.SetSource (cr, "black");
//                cr.LineWidth = 0.75;
//                cr.Stroke ();
//
//                t.text = _currentAmps.ToString ("F2");
//                t.alignment = MyAlignment.Center;
//                t.textWrap = MyTextWrap.Shrink;
//                t.font.size = 11;
//                t.Render (eb, x + 55, y + 25, 50);
//            }
//        }
//
//        protected double CalcRadius (double width, double height) {
//            return (height / 2.0) + (Math.Pow (width, 2.0) / (8 * height));
//        }
//
//        protected double CalcX (double orginX, double radius, double radians) {
//            return orginX + radius * Math.Cos (radians);
//        }
//
//        protected double CalcY (double orginY, double radius, double radians) {
//            return orginY + radius * Math.Sin (radians);
//        }
//    }

}
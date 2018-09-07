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
using System.Collections.Generic;
using Cairo;
using Gtk;
using GoodtimeDevelopment.TouchWidget;
using GoodtimeDevelopment.Utilites;
using AquaPic.Modules;

namespace AquaPic.UserInterface
{
    public class LightingStateDisplay : EventBox
    {
        bool clicked;
        uint clickTimer;
        int clickX, clickY;
        public LightingState[] lightingStates;

        public LightingStateDisplay () {
            Visible = true;
            VisibleWindow = false;
            SetSizeRequest (550, 330);

            lightingStates = new LightingState [0];

            ExposeEvent += onExpose;
            clickTimer = GLib.Timeout.Add (20, OnTimerEvent);
            //ButtonPressEvent += OnButtonPress;
            //ButtonReleaseEvent += OnButtonRelease;
        }

        public override void Dispose () {
            GLib.Source.Remove (clickTimer);
            base.Dispose ();
        }

        protected void onExpose (object sender, ExposeEventArgs args) {
            var left = Allocation.Left;
            var graphLeft = left + 40;
            var right = Allocation.Right;
            var width = Allocation.Width;
            var midX = width / 2 + left;

            var top = Allocation.Top;
            var bottom = Allocation.Bottom;
            var graphBottom = bottom - 15;
            var height = Allocation.Height;
            var midY = height / 2 + top;


            using (Context cr = Gdk.CairoHelper.Create (GdkWindow)) {
                cr.MoveTo (graphLeft - 5, top);
                cr.LineTo (graphLeft, top);
                cr.LineTo (graphLeft, midY);
                cr.LineTo (graphLeft - 5, midY);
                cr.LineTo (graphLeft, midY);
                cr.LineTo (graphLeft, graphBottom);
                cr.LineTo (graphLeft - 5, graphBottom);
                cr.LineTo (graphLeft, graphBottom);
                cr.LineTo (graphLeft, graphBottom + 5);
                cr.LineTo (graphLeft, graphBottom);
                cr.LineTo (midX, graphBottom);
                cr.LineTo (midX, graphBottom + 5);
                cr.LineTo (midX, graphBottom);
                cr.LineTo (right, graphBottom);
                cr.LineTo (right, graphBottom);
                cr.LineTo (right, graphBottom + 5);
                TouchColor.SetSource (cr, "grey3");
                cr.Stroke ();

                var text = new TouchText ("100%");
                text.alignment = TouchAlignment.Right;
                text.Render (this, left, top - 12, 30);

                text = new TouchText ("50%");
                text.alignment = TouchAlignment.Right;
                text.Render (this, left, midY - 12, 30);

                text = new TouchText ("0%");
                text.alignment = TouchAlignment.Right;
                text.Render (this, left, bottom - 27, 30);

                foreach (var state in lightingStates) {
                    if (state.type != LightingStateType.Off) {
                        if (state.startTime.Before (state.endTime)) {
                            var startXPos = state.startTime.ToTimeSpan ().TotalMinutes.Map (0, 1440, graphLeft, right);
                            var endXPos = state.endTime.ToTimeSpan ().TotalMinutes.Map (0, 1440, graphLeft, right);
                            var startYPos = state.startingDimmingLevel.Map (0, 100, graphBottom, top);
                            var endYPos = state.endingDimmingLevel.Map (0, 100, graphBottom, top);

                            cr.MoveTo (startXPos, graphBottom);
                            switch (state.type) {
                            case LightingStateType.LinearRamp:
                                // The diagonal looks bad if we go all the way to the tip so we fudge the number 
                                // by quite a bit to make it look good 
                                if (endYPos > startYPos) {
                                    endYPos += 30;
                                } else {
                                    startYPos += 30;
                                }

                                cr.LineTo (startXPos, startYPos);
                                cr.LineTo (endXPos, endYPos);
                                break;
                            case LightingStateType.ParabolaRamp: {
                                    cr.LineTo (startXPos, startYPos);

                                    var period = endXPos - startXPos;
                                    var delta = Math.Abs (endYPos - startYPos);
                                    for (int i = 1; i <= period; ++i) {
                                        var radian = (i / period).Map (0, 1, 0, 180).Constrain (0, 180).ToRadians ();
                                        var interYPos = startYPos - delta * Math.Sin (radian);
                                        cr.LineTo (startXPos + i, interYPos);
                                    }
                                    break;
                                }
                            case LightingStateType.HalfParabolaRamp: {
                                    cr.LineTo (startXPos, startYPos);

                                    var period = endXPos - startXPos;
                                    var delta = Math.Abs (endYPos - startYPos);
                                    double mapFrom1, mapFrom2, basePoint;
                                    if (startYPos <= endYPos) {
                                        mapFrom1 = 1d;
                                        mapFrom2 = 0d;
                                        basePoint = endYPos;
                                    } else {
                                        mapFrom1 = 0d;
                                        mapFrom2 = 1d;
                                        basePoint = startYPos;
                                    }
                                    for (int i = 1; i <= period; ++i) {
                                        var radian = (i / period).Map (mapFrom1, mapFrom2, 0, 90).Constrain (0, 90).ToRadians ();
                                        var interYPos = basePoint - delta * Math.Sin (radian);
                                        cr.LineTo (startXPos + i, interYPos);
                                    }
                                    cr.LineTo (endXPos, endYPos);
                                    break;
                                }
                            case LightingStateType.On:
                                cr.LineTo (startXPos, startYPos);
                                cr.LineTo (endXPos, startYPos);
                                break;
                            }

                            cr.LineTo (endXPos, graphBottom);
                            cr.ClosePath ();
                            TouchColor.SetSource (cr, "pri");
                            cr.LineWidth = 1;
                            cr.StrokePreserve ();
                            TouchColor.SetSource (cr, "grey2");
                            cr.Fill ();

                            text = new TouchText (state.startTime.ToShortTimeString ());
                            text.alignment = TouchAlignment.Center;
                            text.Render (this, startXPos.ToInt () - 50, bottom - 15, 100);

                            text = new TouchText (state.endTime.ToShortTimeString ());
                            text.alignment = TouchAlignment.Center;
                            text.Render (this, endXPos.ToInt () - 50, bottom - 15, 100);
                        }
                    }
                }

                var xPos = Time.TimeNow.ToTimeSpan ().TotalMinutes.Map (0, 1440, graphLeft, right);
                cr.MoveTo (xPos, graphBottom);
                cr.LineTo (xPos, top);
                TouchColor.SetSource (cr, "seca");
                cr.Stroke ();
            }
        }

        protected void OnButtonPress (object sender, ButtonPressEventArgs args) {
            clicked = true;
            GetPointer (out clickX, out clickY);
        }

        protected void OnButtonRelease (object sender, ButtonReleaseEventArgs args) {
            clicked = false;
        }

        protected bool OnTimerEvent () {
            if (clicked) {
                int x, y;
                GetPointer (out x, out y);

                double yDelta = clickY - y;
                double xDelta = clickX - x;

            }

            QueueDraw ();
            return true;
        }
    }
}

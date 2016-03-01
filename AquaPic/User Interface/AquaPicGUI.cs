﻿using System;
using System.Diagnostics;
using Cairo;
using Gtk;
using TouchWidgetLibrary;
using AquaPic.Runtime;

namespace AquaPic.UserInterface
{
    public partial class AquaPicGUI : Gtk.Window
    {
        WindowBase current;
        Fixed f;
        MySideBar side;
        MyNotificationBar notification;

        public AquaPicGUI () : base (Gtk.WindowType.Toplevel) {
            this.Name = "AquaPicGUI";
            this.Title = "AquaPic Controller Version 1";
            this.WindowPosition = WindowPosition.Center;
            this.DefaultWidth = 800;
            this.DefaultHeight = 480;
            this.DeleteEvent += new Gtk.DeleteEventHandler (this.OnDeleteEvent);
            this.Resizable = false;
            this.AllowGrow = false;

            this.ModifyBg (StateType.Normal, TouchColor.NewGtkColor ("grey0"));

            #if RPI_BUILD
            this.Decorated = false;
            this.Fullscreen ();
            #endif

            GLib.ExceptionManager.UnhandledException += (args) => {
                Exception ex = args.ExceptionObject as Exception;
                Logger.AddError (ex.ToString ());
                args.ExitApplication = false;
            };

            ChangeScreenEvent += ScreenChange;

            currentSelectedMenu = menuWindows [0];
            currentScreen = currentSelectedMenu;

            f = new Fixed ();
            f.SetSizeRequest (800, 480);

//            var background = new EventBox ();
//            background.Visible = true;
//            background.VisibleWindow = false;
//            background.SetSizeRequest (800, 480);
//            background.ExposeEvent += OnBackGroundExpose;
//            f.Put (background, 0, 0);
//            background.Show ();

            current = allWindows [currentScreen].CreateInstance ();
            f.Put (current, 0, 0);
            current.Show ();

            side = new MySideBar ();
            f.Put (side, 0, 20);
            side.Show ();

            notification = new MyNotificationBar ();
            f.Put (notification, 0, 0);
            notification.Show ();

            Add (f);
            f.Show ();

            Show ();
        }

        protected void OnDeleteEvent (object sender, DeleteEventArgs a) {
            Application.Quit ();
            a.RetVal = true;
        }

        public void ScreenChange (ScreenData screen, params object[] options) {
            f.Remove (current);
            current.Destroy ();
            current.Dispose ();
            current = screen.CreateInstance (options);
            f.Put (current, 0, 0);

            f.Remove (side);
            side.Destroy ();
            side.Dispose ();
            side = new MySideBar ();
            f.Put (side, 0, 20);
            side.Show ();

            if (currentScreen == "Logger") {
                var logScreen = current as LoggerWindow;
                if (logScreen != null) {
                    side.ExpandEvent += (sender, e) => {
                        logScreen.tv.Visible = false;
                        logScreen.tv.QueueDraw ();
                    };

                    side.CollapseEvent += (sender, e) => {
                        logScreen.tv.Visible = true;
                        logScreen.tv.QueueDraw ();
                    };
                }
            } else if (currentScreen == "Alarms") {
                var alarmScreen = current as AlarmWindow;
                if (alarmScreen != null) {
                    side.ExpandEvent += (sender, e) => {
                        alarmScreen.tv.Visible = false;
                        alarmScreen.tv.QueueDraw ();
                    };

                    side.CollapseEvent += (sender, e) => {
                        alarmScreen.tv.Visible = true;
                        alarmScreen.tv.QueueDraw ();
                    };
                }

            }

            f.Remove (notification);
            notification.Destroy ();
            notification.Dispose ();
            notification = new MyNotificationBar ();
            f.Put (notification, 0, 0);
            notification.Show ();

            QueueDraw ();
        }

        protected void OnBackGroundExpose (object sender, ExposeEventArgs args) {
            var box = sender as EventBox;
            using (Context cr = Gdk.CairoHelper.Create (box.GdkWindow)) {
                cr.Rectangle (0, 0, 800, 480);

                Gradient pat = new LinearGradient (400, 0, 400, 480);
                pat.AddColorStop (0.0, TouchColor.NewCairoColor ("grey0"));
                pat.AddColorStop (0.5, TouchColor.NewCairoColor ("grey1"));
                pat.AddColorStop (1.0, TouchColor.NewCairoColor ("grey0"));
                cr.SetSource (pat);

                cr.Fill ();
                pat.Dispose ();
            }
        }

        public void ShowDecoration () {
            SetSizeRequest (800, 425);
            Decorated = true;
        }

        public void HideDecoration () {
            SetSizeRequest (800, 480);
            #if RPI_BUILD
            Decorated = false;
            #endif
        }
    }
}


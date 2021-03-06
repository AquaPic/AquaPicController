#region License

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
using System.Collections.Generic;
using Gtk;
using Cairo;
using GoodtimeDevelopment.TouchWidget;

namespace AquaPic.UserInterface
{
    public class MenuWindow : SceneBase
    {
        public MenuWindow (params object[] options) : base () {
            sceneTitle = "Menu";

            List<string> screenNames = new List<string> ();
            foreach (var screen in AquaPicGui.AquaPicUserInterface.scenes.Keys)
                screenNames.Add (screen);

            screenNames.Sort ();

            int x = 60;
            int y = 80;
            foreach (var name in screenNames) {
                SceneData screen = AquaPicGui.AquaPicUserInterface.scenes[name];
                if (screen.showInMenu) {
                    var b = new TouchButton ();
                    b.SetSizeRequest (220, 50);
                    b.text = screen.name;
                    b.textColor = "black";
                    b.ButtonReleaseEvent += OnButtonClick;
                    Put (b, x, y);

                    x += 230;
                    if (x >= 690) {
                        x = 60;
                        y += 60;
                    }
                }
            }

            ShowAll ();
        }

        protected void OnButtonClick (object sender, ButtonReleaseEventArgs args) {
            TouchButton b = sender as TouchButton;
            AquaPicGui.AquaPicUserInterface.ChangeScreens (b.text, Toplevel, AquaPicGui.AquaPicUserInterface.currentScene);
        }
    }
}


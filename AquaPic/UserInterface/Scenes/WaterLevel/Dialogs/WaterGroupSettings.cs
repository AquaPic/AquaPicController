#region License

/*
    AquaPic Main Control - Handles all functionality for the AquaPic aquarium controller.

    Copyright (c) 2017 Skyler Brandt

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion // License

using System;
using System.IO;
using Cairo;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TouchWidgetLibrary;
using AquaPic.Modules;
using AquaPic.Utilites;

namespace AquaPic.UserInterface
{
    public class WaterGroupSettings : TouchSettingsDialog
    {
        string groupName;
        public string waterLevelGroupName {
            get {
                return groupName;
            }
        }

        public WaterGroupSettings (string name, bool includeDelete)
            : base (name + " Water", includeDelete) {
            groupName = name;

            SaveEvent += OnSave;
            DeleteButtonEvent += OnDelete;

            var t = new SettingsTextBox ();
            t.text = "Name";
            if (groupName.IsNotEmpty ()) {
                t.textBox.text = groupName;
                t.textBox.enableTouch = false;
                t.textBox.TextChangedEvent += (sender, args) => {
                    MessageBox.Show ("Can not change water group name during runtime");
                    args.keepText = false;
                };
            } else {
                t.textBox.text = "Enter name";
                t.textBox.TextChangedEvent += (sender, args) => {
                    if (string.IsNullOrWhiteSpace (args.text))
                        args.keepText = false;
                    else if (!WaterLevel.WaterLevelGroupNameOk (args.text)) {
                        MessageBox.Show ("Water level group name already exists");
                        args.keepText = false;
                    }
                };
            }
            AddSetting (t);

            var c = new SettingsComboBox ();
            c.text = "Analog Sensor Name";
            c.combo.comboList.Add ("None");
            c.combo.activeIndex = 0;
            string[] analogSensorNames = WaterLevel.GetAllAnalogLevelSensors ();
            c.combo.comboList.AddRange (analogSensorNames);
            if (groupName.IsNotEmpty ()) {
                var analogSensorName = WaterLevel.GetWaterLevelGroupAnalogSensorName (groupName);
                for (int i = 0; i < c.combo.comboList.Count; ++i) {
                    if (analogSensorName == c.combo.comboList[i]) {
                        c.combo.activeIndex = i;
                        break;
                    }
                }
            }
            AddSetting (c);

            DrawSettings ();
        }

        protected bool OnSave (object sender) {
            var name = (settings["Name"] as SettingsTextBox).textBox.text;
            var analogSensorName = (settings["Analog Sensor Name"] as SettingsComboBox).combo.activeText;
            if (analogSensorName == "None") {
                analogSensorName = string.Empty;
            }

            string path = System.IO.Path.Combine (Utils.AquaPicEnvironment, "AquaPicRuntimeProject");
            path = System.IO.Path.Combine (path, "Settings");
            path = System.IO.Path.Combine (path, "waterLevelProperties.json");

            string json = File.ReadAllText (path);
            JObject jo = (JObject)JToken.Parse (json);

            if (groupName.IsEmpty ()) {
                if (name == "Enter name") {
                    MessageBox.Show ("Invalid water group name");
                    return false;
                }

                WaterLevel.AddWaterLevelGroup (name, analogSensorName);

                JObject jobj = new JObject ();

                jobj.Add (new JProperty ("name", name));
                jobj.Add (new JProperty ("analogLevelSensorName", analogSensorName));

                (jo["waterLevelGroups"] as JArray).Add (jobj);

                groupName = name;
            } else {
                WaterLevel.SetWaterLevelGroupAnalogSensorName (groupName, analogSensorName);

                JArray ja = jo["waterLevelGroups"] as JArray;
                int arrIdx = -1;
                for (int i = 0; i < ja.Count; ++i) {
                    string n = (string)ja[i]["name"];
                    if (groupName == n) {
                        arrIdx = i;
                        break;
                    }
                }

                if (arrIdx == -1) {
                    MessageBox.Show ("Something went wrong");
                    return false;
                }

                ((JArray)jo["waterLevelGroups"])[arrIdx]["analogLevelSensorName"] = analogSensorName;
            }

            File.WriteAllText (path, jo.ToString ());

            return true;
        }

        protected bool OnDelete (object sender) {
            var path = System.IO.Path.Combine (Utils.AquaPicEnvironment, "AquaPicRuntimeProject");
            path = System.IO.Path.Combine (path, "Settings");
            path = System.IO.Path.Combine (path, "waterLevelProperties.json");

            string json = File.ReadAllText (path);
            JObject jo = (JObject)JToken.Parse (json);

            JArray ja = jo["waterLevelGroups"] as JArray;
            int arrIdx = -1;
            for (int i = 0; i < ja.Count; ++i) {
                string n = (string)ja[i]["name"];
                if (groupName == n) {
                    arrIdx = i;
                    break;
                }
            }

            if (arrIdx == -1) {
                MessageBox.Show ("Something went wrong");
                return false;
            }

            ((JArray)jo["waterLevelGroups"]).RemoveAt (arrIdx);
            File.WriteAllText (path, jo.ToString ());
            WaterLevel.RemoveWaterLevelGroup (groupName);
            return true;
        }
    }
}
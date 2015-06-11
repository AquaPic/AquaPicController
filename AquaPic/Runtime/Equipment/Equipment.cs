﻿using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AquaPic.Drivers;

namespace AquaPic.Runtime
{
    public class Equipment
    {
        public static void AddFromJson () {
            string path = string.Format (
                "{0}{1}", 
                Environment.GetEnvironmentVariable ("AquaPic"), 
                @"\AquaPicRuntimeProject\Settings\Equipment.json");

            using (StreamReader reader = File.OpenText (path)) {
                JArray ja = (JArray)JToken.ReadFrom (new JsonTextReader (reader));

                foreach (var jt in ja) {
                    var jo = jt as JObject;
                    string type = (string)jo ["type"];
                    switch (type) {
                    case "power":
                        Power.AddPowerStrip (
                            Convert.ToInt32 (jo ["options"] [0]),
                            (string)jo ["options"] [1],
                            Convert.ToBoolean (jo ["options"] [2]));
                        break;
                    case "analogInput":
                        AnalogInput.AddCard (
                            Convert.ToInt32 (jo ["options"] [0]),
                            (string)jo ["options"] [1]);
                        break;
                    case "analogOutput":
                        AnalogOutput.AddCard (
                            Convert.ToInt32 (jo ["options"] [0]),
                            (string)jo ["options"] [1]);
                        break;
                    case "digitalInput":
                        DigitalInput.AddCard (
                            Convert.ToInt32 (jo ["options"] [0]),
                            (string)jo ["options"] [1]);
                        break;
                    default:
                        Console.WriteLine ("Unknow equipment type: {0}", type);
                        break;
                    }
                }
            }
        }
    }
}

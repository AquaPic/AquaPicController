﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AquaPic.Runtime;
using AquaPic.Utilites;
using AquaPic.Drivers;

namespace AquaPic.Modules
{
    public partial class Lighting
    {
        private static List<LightingFixture> fixtures;
        public static TimeDate sunRiseToday;
        public static TimeDate sunSetToday;
        public static TimeDate sunRiseTomorrow;
        public static TimeDate sunSetTomorrow;

        public static Time minSunRise;
        public static Time maxSunRise;

        public static Time minSunSet;
        public static Time maxSunSet;

        public static Time defaultSunRise;
        public static Time defaultSunSet;

        public static double latitude {
            get {
                return RiseSetCalc.latitude;
            }
            set {
                RiseSetCalc.latitude = value;
            }
        }

        public static double longitude {
            get {
                return RiseSetCalc.longitude;
            }
            set {
                RiseSetCalc.longitude = value;
            }
        }

        public static int timeZone {
            get {
                return RiseSetCalc.timeZone;
            }
        }

        static Lighting () {
            fixtures = new List<LightingFixture> ();

            string path = Path.Combine (Environment.GetEnvironmentVariable ("AquaPic"), "AquaPicRuntimeProject");
            path = Path.Combine (path, "Settings");
            path = Path.Combine (path, "lightingProperties.json");

            using (StreamReader reader = File.OpenText (path)) {
                JObject jo = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                RiseSetCalc.latitude = Convert.ToDouble (jo ["latitude"]);
                RiseSetCalc.longitude = Convert.ToDouble (jo ["longitude"]);

                defaultSunRise = new Time (
                    Convert.ToByte (jo ["defaultSunRise"] ["hour"]),
                    Convert.ToByte (jo ["defaultSunRise"] ["minute"]));
                defaultSunSet = new Time (
                    Convert.ToByte (jo ["defaultSunSet"] ["hour"]),
                    Convert.ToByte (jo ["defaultSunSet"] ["minute"]));

                minSunRise = new Time (
                    Convert.ToByte (jo ["minSunRise"] ["hour"]),
                    Convert.ToByte (jo ["minSunRise"] ["minute"]));
                maxSunRise = new Time (
                    Convert.ToByte (jo ["maxSunRise"] ["hour"]),
                    Convert.ToByte (jo ["maxSunRise"] ["minute"]));

                minSunSet = new Time (
                    Convert.ToByte (jo ["minSunSet"] ["hour"]),
                    Convert.ToByte (jo ["minSunSet"] ["minute"]));

                maxSunSet = new Time (
                    Convert.ToByte (jo ["maxSunSet"] ["hour"]),
                    Convert.ToByte (jo ["maxSunSet"] ["minute"]));

                // Very important to update rise/set times before we setup auto on/off for lighting fixtures
                UpdateRiseSetTimes ();

                JArray ja = (JArray)jo ["lightingFixtures"];
                foreach (var jt in ja) {
                    JObject obj = jt as JObject;
                    string type = (string)obj ["type"];

                    string name = (string)obj ["name"];
                    int powerStripId = Power.GetPowerStripIndex ((string)obj ["powerStrip"]);
                    int outletId = Convert.ToInt32 (obj ["outlet"]);
                    bool highTempLockout = Convert.ToBoolean (obj ["highTempLockout"]);

                    string lTime = (string)obj ["lightingTime"];
                    LightingTime lightingTime;
                    if (string.Equals (lTime, "night", StringComparison.InvariantCultureIgnoreCase)) {
                        lightingTime = LightingTime.Nighttime;
                    } else {
                        lightingTime = LightingTime.Daytime;
                    }

                    int lightingId;
                    if (string.Equals (type, "dimming", StringComparison.InvariantCultureIgnoreCase)) {
                        int cardId = AnalogOutput.GetCardIndex ((string)obj ["dimmingCard"]);
                        int channelId = Convert.ToInt32 (obj ["channel"]);
                        float minDimmingOutput = Convert.ToSingle (obj ["minDimmingOutput"]);
                        float maxDimmingOutput = Convert.ToSingle (obj ["maxDimmingOutput"]);

                        string aType = (string)obj ["analogType"];
                        AnalogType analogType;
                        if (string.Equals (aType, "ZeroTen", StringComparison.InvariantCultureIgnoreCase)) {
                            analogType = AnalogType.ZeroTen;
                        } else {
                            analogType = AnalogType.PWM;
                        }

                        lightingId = AddLight (
                            name,
                            powerStripId,
                            outletId,
                            cardId,
                            channelId,
                            minDimmingOutput,
                            maxDimmingOutput,
                            analogType,
                            lightingTime,
                            highTempLockout
                        );
                    } else {
                        lightingId = AddLight (
                            name,
                            powerStripId,
                            outletId,
                            lightingTime,
                            highTempLockout
                        );
                    }

                    if (Convert.ToBoolean (obj ["autoTimeUpdate"])) {
                        int onTimeOffset = Convert.ToInt32 (obj ["onTimeOffset"]);
                        int offTimeOffset = Convert.ToInt32 (obj ["offTimeOffset"]);
                        SetupAutoOnOffTime (lightingId, onTimeOffset, offTimeOffset);
                    }
                }
            }

            TaskManager.AddTimeOfDayInterrupt ("RiseSetUpdate", new Time (0, 0), () => UpdateRiseSetTimes ());
        }

        public static void Init () {
            EventLogger.Add ("Initializing Lighting");


        }

        public static int AddLight (
            string name,
            int powerID,
            int plugID,
            LightingTime lightingTime = LightingTime.Daytime,
            bool highTempLockout = true)
        {
            Time onTime, offTime;

            if (lightingTime == LightingTime.Daytime) {
                onTime = defaultSunRise;
                offTime = defaultSunSet;
            } else {
                onTime = defaultSunSet;
                offTime = defaultSunRise;
            }

            fixtures.Add (new LightingFixture (
                name, 
                (byte)powerID,
                (byte)plugID,
                onTime,
                offTime,
                lightingTime,
                highTempLockout));
            
            return fixtures.Count - 1;
        }

        public static int AddLight (
            string name,
            int powerID,
            int plugID,
            int cardID,
            int channelID,
            float minDimmingOutput = 0.0f,
            float maxDimmingOutput = 100.0f,
            AnalogType type = AnalogType.ZeroTen,
            LightingTime lightingTime = LightingTime.Daytime,
            bool highTempLockout = true)
        {
            Time onTime, offTime;

            if (lightingTime == LightingTime.Daytime) {
                onTime = defaultSunRise;
                offTime = defaultSunSet;
            } else {
                onTime = defaultSunSet;
                offTime = defaultSunRise;
            }

            fixtures.Add (new DimmingLightingFixture (
                name,
                (byte)powerID,
                (byte)plugID,
                onTime,
                offTime,
                (byte)cardID,
                (byte)channelID,
                minDimmingOutput,
                maxDimmingOutput,
                type,
                lightingTime,
                highTempLockout));

            return GetLightingFixtureIndex (name);
        }

        public static void SetupAutoOnOffTime (
            int lightID,
            int onTimeOffset = 0,
            int offTimeOffset = 0)
        {
            var light = fixtures [lightID];

            light.onTimeOffset = onTimeOffset;
            light.offTimeOffset = offTimeOffset;
            light.mode = Mode.Auto;

            TimeDate now = TimeDate.Now;
            if ((now.CompareTo (sunRiseToday) > 0) && (now.CompareTo (sunSetToday) < 0)) {
                // time is after sunrise but before sunset so normal daytime
                if (light.lightingTime == LightingTime.Daytime) { 
                    light.SetOnTime (sunRiseToday);
                    light.SetOffTime (sunSetToday);
                } else {
                    light.SetOnTime (sunSetToday);
                    light.SetOffTime (sunRiseTomorrow);
                }
            } else if (now.CompareTo (sunRiseToday) < 0) { // time is before sunrise today
                if (light.lightingTime == LightingTime.Daytime) { 
                    // lights are supposed to be off, no special funny business required
                    light.SetOnTime (sunRiseToday);
                    light.SetOffTime (sunSetToday);
                } else { // lights are supposed to be on, a little funny bussiness is required
                    light.onTime = now; // no need to worry about offset
                    light.SetOffTime (sunRiseToday); // night time lighting turns off at sunrise
                }
            } else { // time is after sunrise
                if (light.lightingTime == LightingTime.Daytime) { 
                    light.SetOnTime (sunRiseTomorrow);
                    light.SetOffTime (sunSetTomorrow);
                } else {
                    light.SetOnTime (sunSetToday);
                    light.SetOffTime (sunRiseTomorrow);
                }
            }
        }

        public static void UpdateRiseSetTimes () {
            RiseSetCalc.GetRiseSetTimes (out sunRiseToday, out sunSetToday);
            sunRiseTomorrow = RiseSetCalc.GetRiseTimeTomorrow ();
            sunSetTomorrow = RiseSetCalc.GetSetTimeTomorrow ();

            if (sunRiseToday.CompareToTime (minSunRise) < 0) // sunrise is before minimum
                sunRiseToday.SetTime (minSunRise);
            else if (sunRiseToday.CompareToTime (maxSunRise) > 0) // sunrise is after maximum
                sunRiseToday.SetTime (maxSunRise);

            if (sunSetToday.CompareToTime (minSunSet) < 0) // sunset is before minimum
                sunSetToday.SetTime (minSunSet);
            else if (sunSetToday.CompareToTime (maxSunSet) > 0) // sunset is after maximum
                sunSetToday.SetTime (maxSunSet);

            if (sunRiseTomorrow.CompareToTime (minSunRise) < 0) // sunrise is before minimum
                sunRiseTomorrow.SetTime (minSunRise);
            else if (sunRiseTomorrow.CompareToTime (maxSunRise) > 0) // sunrise is after maximum
                sunRiseTomorrow.SetTime (maxSunRise);

            if (sunSetTomorrow.CompareToTime (minSunSet) < 0) // sunset is before minimum
                sunSetTomorrow.SetTime (minSunSet);
            else if (sunSetTomorrow.CompareToTime (maxSunSet) > 0) // sunset is after maximum
                sunSetTomorrow.SetTime (maxSunSet);
        }

        public static int GetLightingFixtureIndex (string name) {
            for (int i = 0; i < fixtures.Count; ++i) {
                if (fixtures [i].name == name)
                    return i;
            }
            return -1;
        }

        public static string[] GetAllFixtureNames () {
            string[] names = new string[fixtures.Count];
            for (int i = 0; i < fixtures.Count; ++i)
                names [i] = fixtures [i].name;
            return names;
        }

        public static float GetCurrentDimmingLevel (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                if (fixtures [fixtureID] is DimmingLightingFixture) {
                    var fixture = fixtures [fixtureID] as DimmingLightingFixture;
                    return fixture.currentDimmingLevel;
                }
            }
            return 0.0f;
        }

        public static float GetAutoDimmingLevel (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                if (fixtures [fixtureID] is DimmingLightingFixture) {
                    var fixture = fixtures [fixtureID] as DimmingLightingFixture;
                    return fixture.autoDimmingLevel;
                }
            }
            return 0.0f;
        }

        public static float GetRequestedDimmingLevel (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                if (fixtures [fixtureID] is DimmingLightingFixture) {
                    var fixture = fixtures [fixtureID] as DimmingLightingFixture;
                    return fixture.requestedDimmingLevel;
                }
            }
            return 0.0f;
        }

        public static Mode GetDimmingMode (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                if (fixtures [fixtureID] is DimmingLightingFixture) {
                    var fixture = fixtures [fixtureID] as DimmingLightingFixture;
                    return fixture.dimmingMode;
                }
            }
            return Mode.Manual;
        }

        public static bool IsDimmingFixture (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                return fixtures [fixtureID] is DimmingLightingFixture;
            }
            return false;
        }

        public static void SetMode (int fixtureID, Mode mode) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                if (fixtures [fixtureID] is DimmingLightingFixture) {
                    var fixture = fixtures [fixtureID] as DimmingLightingFixture;
                    fixture.dimmingMode = mode;
                }
            }
        }

        public static void SetDimmingLevel (int fixtureID, float level) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                if (fixtures [fixtureID] is DimmingLightingFixture) {
                    var fixture = fixtures [fixtureID] as DimmingLightingFixture;
                    if (fixture.dimmingMode == Mode.Manual)
                        fixture.requestedDimmingLevel = level;
                }
            }
        }

        public static TimeDate GetOnTime (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                return fixtures [fixtureID].onTime;
            }
            return TimeDate.Zero;
        }

        public static TimeDate GetOffTime (int fixtureID) {
            if ((fixtureID >= 0) && (fixtureID < fixtures.Count)) {
                return fixtures [fixtureID].offTime;
            }
            return TimeDate.Zero;
        }
    }
}
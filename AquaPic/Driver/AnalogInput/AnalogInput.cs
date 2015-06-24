﻿using System;
using System.Collections.Generic;
using AquaPic.Utilites;
using AquaPic.Runtime;

namespace AquaPic.Drivers
{
    public partial class AnalogInput
    {
        private static List<AnalogInputCard> cards = new List<AnalogInputCard> ();

        static AnalogInput () {
            TaskManager.AddCyclicInterrupt ("Analog Input", 1000, Run);
        }

        public static int AddCard (int address, string name) {
            int count = cards.Count;
            cards.Add (new AnalogInputCard ((byte)address, (byte)count, name));
            return count;
        }

        public static void AddChannel (IndividualControl channel, AnalogType type, string name) {
            AddChannel (channel.Group, channel.Individual, type, name);
        }

        public static void AddChannel (int cardID, int channelID, AnalogType type, string name) {
            if (cardID == -1)
                throw new Exception ("Card does not exist");

            if ((channelID < 0) || (channelID >= cards [cardID].channels.Length))
                throw new Exception ("Input ID out of range");

            string s = string.Format ("{0}.i{1}", cards [cardID].name, channelID);
            if (cards [cardID].channels [channelID].name != s)
                throw new Exception (string.Format ("Channel already taken by {0}", cards [cardID].channels [channelID].name));
            
            cards [cardID].AddChannel (channelID, type, name);
        }

        public static void Run () {
            for (int i = 0; i < cards.Count; ++i) {
                cards [i].GetValues ();
            }
        }

        public static float GetValue (IndividualControl channel, bool realTimeUpdate = false) {
            return GetValue (channel.Group, channel.Individual, realTimeUpdate);
        }

        public static float GetValue (int card, int channel, bool realTimeUpdate = false) {
            if ((card >= 0) && (card < cards.Count)) {
                if ((channel >= 0) && (channel < cards [card].channels.Length)) {
                    if (cards [card].channels [channel].mode == Mode.Auto) {
                        if (realTimeUpdate) {
                            cards [card].GetValue ((byte)channel);
                            while (cards [card].updating)
                                continue;
                        }
                    }
               
                    return cards [card].channels [channel].value;
                }
            }

            return 0.0f;
        }

        public static void SetValue (IndividualControl channel, float value) {
            if ((channel.Group >= 0) && (channel.Group < cards.Count)) {
                if ((channel.Individual >= 0) && (channel.Individual < cards [channel.Group].channels.Length)) {
                    if (cards [channel.Group].channels [channel.Individual].mode == Mode.Manual)
                        cards [channel.Group].channels [channel.Individual].value = value;
                    else
                        throw new Exception ("Can only modify analong input value with channel forced");
                }
            }
        }

        public static float[] GetAllValues (int cardId) {
            if ((cardId >= 0) && (cardId < cards.Count)) {
                float[] types = new float[cards [cardId].channels.Length];

                for (int i = 0; i < types.Length; ++i)
                    types [i] = cards [cardId].channels [i].value;

                return types;
            }

            return null;
        }

        public static int GetCardIndex (string name) {
            for (int i = 0; i < cards.Count; ++i) {
                if (string.Equals (cards [i].name, name, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }
            return -1;
        }

        public static int GetChannelIndex (int cardId, string name) {
            if ((cardId >= 0) && (cardId < cards.Count)) {
                for (int i = 0; i < cards [cardId].channels.Length; ++i) {
                    if (string.Equals (cards [cardId].channels [i].name, name, StringComparison.InvariantCultureIgnoreCase))
                        return i;
                }
            }

            return -1;
        }

        public static string[] GetAllCardNames () {
            string[] names = new string[cards.Count];

            for (int i = 0; i < cards.Count; ++i)
                names [i] = cards [i].name;

            return names;
        }

        public static string[] GetAllChannelNames (int cardId) {
            if ((cardId >= 0) && (cardId < cards.Count)) {
                string[] names = new string[cards [cardId].channels.Length];

                for (int i = 0; i < names.Length; ++i)
                    names [i] = cards [cardId].channels [i].name;

                return names;
            }

            return null;
        }

        public static void SetMode (IndividualControl ic, Mode mode) {
            if ((ic.Group >= 0) && (ic.Group < cards.Count)) {
                if ((ic.Individual >= 0) && (ic.Individual < cards [ic.Group].channels.Length))
                    cards [ic.Group].channels [ic.Individual].mode = mode;
            }
        }

        public static Mode[] GetAllModes (int cardId) {
            if ((cardId >= 0) && (cardId < cards.Count)) {
                Mode[] modes = new Mode[cards [cardId].channels.Length];

                for (int i = 0; i < modes.Length; ++i)
                    modes [i] = cards [cardId].channels [i].mode;

                return modes;
            }

            return null;
        }

        public static Mode GetMode (IndividualControl ic) {
            if ((ic.Group >= 0) && (ic.Group < cards.Count)) {
                if ((ic.Individual >= 0) && (ic.Individual < cards [ic.Group].channels.Length))
                    return cards [ic.Group].channels [ic.Individual].mode;
            }

            return Mode.Manual;
        }
    }
}
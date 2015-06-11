﻿using System;
using System.Reflection;

namespace AquaPic.Utilites
{
	public class Description : Attribute {
		public string Text;

		public Description (string text) {
			Text = text;
		}
	}

	public static class Utils
	{
		static Utils () {
		}

		public static string GetDescription (Enum en) {
			Type type = en.GetType ();
			MemberInfo[] memInfo = type.GetMember (en.ToString());

			if (memInfo != null && memInfo.Length > 0) {
				object[] attrs = memInfo[0].GetCustomAttributes (typeof (Description), false);

				if (attrs != null && attrs.Length > 0)
					return ((Description)attrs[0]).Text;
			}

			return en.ToString();
		}

        public static float CalcParabola(TimeDate start, TimeDate end, TimeDate now, float min, float max) {
            double period = end.DifferenceInTime(start);
            double phase = now.DifferenceInTime(start);
            double radian = (phase / period).Map (0, 1, 0, 180).Constrain (0, 180).ToRadians ();
            double delta = max - min;
            return min + (float)(delta * Math.Sin(radian));
        }

        public static double ToRadians (this double angle) {
            return (Math.PI / 180) * angle;
        }

        public static int Map (this int value, int from1, int from2, int to1, int to2) {
            return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
        }

        public static int Constrain (this int value, int min, int max) {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static float Map (this float value, float from1, float from2, float to1, float to2) {
            return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
        }

        public static float Constrain (this float value, float min, float max) {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static double Map (this double value, double from1, double from2, double to1, double to2) {
            return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
        }

        public static double Constrain (this double value, double min, double max) {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static bool mtob (byte mask, int shift) {
            byte b = mask;
            byte _shift = (byte)shift;
            b >>= _shift;
            if ((b & 0x01) == 1)
                return true;
            else
                return false;
        }

        public static void btom (ref byte mask, bool b, int shift) {
            if (b)
                mask |= (byte)Math.Pow (2, shift);
            else {
                int m = ~(int)Math.Pow (2, shift);
                mask &= (byte) m;
            }
        }
	}
}

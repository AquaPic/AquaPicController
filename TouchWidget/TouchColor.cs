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
using Cairo;
using GoodtimeDevelopment.Utilites;

namespace GoodtimeDevelopment.TouchWidget
{
    public class TouchColor
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double A { get; set; }

        string colorName;

        static Dictionary<string, int[]> colorLookup = new Dictionary<string, int[]> () {
            { "pri", new int[3] {103, 227, 0}}, // greenish 67E300 {103, 227, 0}
            { "seca", new int[3] {2, 142, 155}}, // blueish 028E9B {2, 142, 155}
            { "secb", new int[3] {255, 154, 64}}, // orangish FF9A40 {255, 154, 64}
            { "secc", new int[3] {255, 253, 64}}, // yellowish FFFD40 {255, 253, 64}
            { "compl", new int[3] {228, 0, 69}}, // redish E40045 {228, 0, 69}
            { "grey0", new int[3] {39, 39, 39}},
            { "grey1", new int[3] {89, 89, 89}},
            { "grey2", new int[3] {128, 128, 128}},
            { "grey3", new int[3] {179, 179, 179}},
            { "grey4", new int[3] {217, 217, 217}}
        };

        public TouchColor (string color, double A = 1d) {
            colorName = color.ToLower ();
            if (colorLookup.ContainsKey (colorName)) {
                R = colorLookup[colorName][0] / 255d;
                G = colorLookup[colorName][1] / 255d;
                B = colorLookup[colorName][2] / 255d;
            } else {
                Gdk.Color c = new Gdk.Color ();
                var colorFound = Gdk.Color.Parse (color, ref c);
                if (colorFound) {
                    R = c.Red / 65535.0;
                    G = c.Green / 65535.0;
                    B = c.Blue / 65535.0;
                } else
                    throw new Exception ("No color could be found matching that description");
            }

            this.A = A;
        }

        public TouchColor (double R, double G, double B, double A = 1.0) {
            colorName = string.Empty;
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public TouchColor (int R, int G, int B, double A = 1d) {
            colorName = string.Empty;
            this.R = R / 255d;
            this.G = G / 255d;
            this.B = B / 255d;
            this.A = A;
        }

        public TouchColor (TouchColor touchColor) {
            colorName = touchColor.colorName;
            R = touchColor.R;
            G = touchColor.G;
            B = touchColor.B;
            A = touchColor.A;
        }

        public static implicit operator TouchColor (string name) {
            return new TouchColor (name);
        }

        public void ModifyColor (double ratio) {
            R *= (float)ratio;
            R.Constrain (0, 1);
            G *= (float)ratio;
            G.Constrain (0, 1);
            B *= (float)ratio;
            B.Constrain (0, 1);
        }

        public TouchColor Blend (TouchColor otherColor, float amount) {
            double red = (R * (1 - amount)) + (otherColor.R * amount);
            double green = (G * (1 - amount)) + (otherColor.G * amount);
            double blue = (B * (1 - amount)) + (otherColor.B * amount);
            return new TouchColor (red, green, blue);
        }

        public string ToHTML () {
            byte red = (byte)(R * 255);
            byte green = (byte)(G * 255);
            byte blue = (byte)(B * 255);
            string html = string.Format ("#{0:X2}{1:X2}{2:X2}", red, green, blue);
            return html;
        }

        public static string ToHTML (string color) {
            byte r, g, b;

            try {
                TouchColor c = new TouchColor (color);
                r = (byte)(c.R * 255);
                g = (byte)(c.G * 255);
                b = (byte)(c.B * 255);
            } catch {
                r = 0;
                g = 0;
                b = 0;
            }

            string html = string.Format ("#{0:X2}{1:X2}{2:X2}", r, g, b);
            return html;
        }

        public void SetSource (Context cr) {
            cr.SetSourceRGBA (R, G, B, A);
        }

        public static void SetSource (Context cr, string color, double a = 1.0) {
            TouchColor c;

            try {
                c = new TouchColor (color);
                cr.SetSourceRGBA (c.R, c.G, c.B, a);
            } catch {
                cr.SetSourceRGBA (0.0, 0.0, 0.0, a);
            }
        }

        public Color ToCairoColor () {
            return new Color (R, G, B, A);
        }

        public static Color NewCairoColor (string color, double a = 1.0) {
            TouchColor c;

            try {
                c = new TouchColor (color);
                return new Color (c.R, c.G, c.B, a);
            } catch {
                return new Color (0.0, 0.0, 0.0);
            }
        }

        public Color NewCairoColor () {
            return new Color (R, G, B, A);
        }

        public static Gdk.Color NewGtkColor (string color) {
            TouchColor c;

            try {
                c = new TouchColor (color);
                return new Gdk.Color ((byte)(c.R * 255), (byte)(c.G * 255), (byte)(c.B * 255));
            } catch {
                return new Gdk.Color ();
            }
        }

        public override string ToString () {
            if (colorName.IsNotEmpty ()) {
                return string.Format ("TouchColor: {0}", colorName);
            }
            return string.Format ("TouchColor: {0}, {1}, {2}, {3}", R, G, B, A);
        }
    }
}


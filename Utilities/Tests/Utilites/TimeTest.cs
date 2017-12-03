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
using NUnit.Framework;

namespace GoodtimeDevelopment.Utilites.Test
{
    [TestFixture]
    public class TimeTest
    {
        [Test]
        public void BeforeAfterTest () {
            var t1 = new Time (8, 0, 30, 300);
            var t2 = new Time (13, 30, 15, 0);

            Assert.True (t1.Before (t2));
            Assert.True (t2.After (t1));
            Assert.False (t1.After (t2));
        }

        [Test]
        public void EqualsShortTimeTest () {
            var now = DateTime.Now;
            var t1 = new Time (now.Hour, now.Minute, now.Second, now.Millisecond);
            Assert.True (t1.EqualsShortTime (now));
        }

        [Test]
        public void AddMinutesTest () {
            var t1 = new Time (0, 0, 0, 0);
            t1.AddMinutes (30);
            Assert.AreEqual (30, t1.minute);
        }

        [Test]
        public void AddTimeTest () {
            var t1 = new Time (1, 15, 30, 500);
            var t2 = new Time (0, 45, 30, 500);
            t1.AddTime (t2);
            t2 = new Time (2, 1, 1, 0);
            Assert.AreEqual (t2, t1);
        }

        [Test]
        public void ParseTest () {
            var str = "8:00 AM";
            var t1 = Time.Parse (str);
            var t2 = new Time (8, 0);
            Assert.AreEqual (t2, t1);

            str = "15:00";
            t1 = Time.Parse (str);
            t2 = new Time (15, 0);
            Assert.AreEqual (t2, t1);

            str = "3:00 PM";
            t1 = Time.Parse (str);
            Assert.AreEqual (t2, t1);
        }

        [Test]
        public void ToShortTimeStringTest () {
            var t1 = new Time (8, 0);
            Assert.AreEqual ("8:00 AM", t1.ToShortTimeString ());

            t1 = new Time (15, 0);
            Assert.AreEqual ("3:00 PM", t1.ToShortTimeString ());
        }
        
        [Test]
        public void EqualsTest () {
            var t1 = new Time (8, 0, 0, 0);
            var t2 = new Time (8, 0);
            Assert.True (t1.Equals (t2));

            t2 = new Time (8, 30);
            Assert.False (t1.Equals (t2));

            t2 = new Time (9, 0);
            Assert.False (t1.Equals (t2));

            t2 = new Time (8, 0, 30);
            Assert.False (t1.Equals (t2));

            t2 = new Time (8, 0, 0, 500);
            Assert.False (t1.Equals (t2));
        }
    }
}

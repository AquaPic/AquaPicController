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

﻿using System;
using System.Collections.Generic;
using AquaPic.Utilites;
using AquaPic.Runtime;

namespace AquaPic.Drivers
{
    public partial class AnalogInputBase : GenericBase<float>
    {
        public static AnalogInputBase SharedAnalogInputInstance = new AnalogInputBase ();

        protected AnalogInputBase () 
            : base ("Analog Input") { }
        
        protected override void Run () {
            foreach (var card in cards) {
                card.GetAllValuesCommunication ();
            }
        }

        protected override GenericCard<float> CardCreater (string cardName, int cardId, int address) {
            return new AnalogInputCard<float> (cardName, cardId, address);
        }
    }
}
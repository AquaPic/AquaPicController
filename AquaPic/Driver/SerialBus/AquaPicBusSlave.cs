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
using Gtk; // for Application.Invoke
using AquaPic.Runtime;

namespace AquaPic.SerialBus
{
    public partial class AquaPicBus
    {
        public class Slave
        {
            public event StatusUpdateHandler OnStatusUpdate;

            private byte address;
            private int responeTime;
            private int[] timeQue;
            private int queIdx;
            private AquaPicBusStatus status;
            private int _alarmIdx;

            public AquaPicBusStatus Status { 
                get { return status; }
            }
            public byte Address {
                get { return address; }
            }
            public int ResponeTime {
                get { return responeTime; }
            }
            public string Name { get; set; }
            public int alarmIdx {
                get { return _alarmIdx; }
            }

            public Slave (int address, string name) {
                if (!IsAddressOk ((byte)address))
                    throw new Exception ("Address already in use");

                this.address = (byte)address;
                responeTime = 0;
                timeQue = new int[10];
                queIdx = 0;
                status = AquaPicBusStatus.NotOpen;
                Name = name;

                slaves.Add (this);

                _alarmIdx = Alarm.Subscribe (address.ToString () + " communication fault");
            }

            public void Read (byte func, int readSize, ResponseCallback callback, bool queueDuringPortClosed = false) {
                QueueMessage (this, func, null, 0, readSize, callback, queueDuringPortClosed);
            }

            public void Write (int func, WriteBuffer writeBuffer, bool queueDuringPortClosed = false) {
                byte[] array = writeBuffer.buffer;
                Write (func, array, queueDuringPortClosed);
            }

            public void Write (int func, byte[] writeBuffer, bool queueDuringPortClosed = false) {
                QueueMessage (this, (byte)func, writeBuffer, writeBuffer.Length, 0, null, queueDuringPortClosed);
            }

            public void Write (int func, byte writeBuffer, bool queueDuringPortClosed = false) {
                Write (func, new byte[] { writeBuffer }, queueDuringPortClosed);
            }

            public void ReadWrite (int func, WriteBuffer writeBuffer, int readSize, ResponseCallback callback, bool queueDuringPortClosed = false) {
                byte[] array = writeBuffer.buffer;
                ReadWrite (func, array, readSize, callback, queueDuringPortClosed);
            }

            public void ReadWrite (int func, byte[] writeBuffer, int readSize, ResponseCallback callback, bool queueDuringPortClosed = false) {
                QueueMessage (this, (byte)func, writeBuffer, writeBuffer.Length, readSize, callback, queueDuringPortClosed);
            }

            public void ReadWrite (int func, byte writeBuffer, int readSize, ResponseCallback callback, bool queueDuringPortClosed = false) {
                ReadWrite (func, new byte[] { writeBuffer }, readSize, callback, queueDuringPortClosed);
            }

            public void UpdateStatus (AquaPicBusStatus stat, int time) {
                if (time != 0) {
                    long sum = 0;
                    int sumCount = 0;

                    timeQue [queIdx] = time;
                    for (int i = 0; i < timeQue.Length; ++i) {
                        if (timeQue [i] != 0) {
                            sum += timeQue [i];
                            ++sumCount;
                        }
                    }

                    responeTime = (int)(sum / sumCount);
                    queIdx = ++queIdx % timeQue.Length;
                }

                status = stat;

                if (OnStatusUpdate != null)
                    Application.Invoke ((sender, e) => OnStatusUpdate (this));
            }
        }
    }
}


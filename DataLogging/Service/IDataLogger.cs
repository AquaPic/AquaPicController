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

namespace AquaPic.DataLogging
{
    public class IDataLogger
    {
        protected string _name;
        public string name {
            get {
                return _name;
            }
        }

        public event DataLogEntryAddedEventHandler ValueLogEntryAddedEvent;
        public event DataLogEntryAddedEventHandler EventLogEntryAddedEvent;

        public virtual void AddEntry (double value) {
            throw new NotImplementedException ();
        }

        public virtual void AddEntry (string eventType) {
            throw new NotImplementedException ();
        }

        public virtual LogEntry[] GetValueEntries (int maxEntries, DateTime endSearchTime) {
            throw new NotImplementedException ();
        }

        public virtual LogEntry[] GetValueEntries (int maxEntries, int secondTimeSpan, DateTime endSearchTime) {
            throw new NotImplementedException ();
        }

        public virtual LogEntry[] GetEventEntries (int maxEntries, DateTime endSearchTime) {
            throw new NotImplementedException ();
        }

        protected void CallValueLogEntryAddedHandlers (LogEntry entry) {
            ValueLogEntryAddedEvent?.Invoke (this, new DataLogEntryAddedEventArgs (entry));
        }

        protected void CallEventLogEntryAddedHandlers (LogEntry entry) {
            EventLogEntryAddedEvent?.Invoke (this, new DataLogEntryAddedEventArgs (entry));
        }
    }
}

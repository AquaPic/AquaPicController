﻿using System;

namespace AquaPic.ValueRuntime
{
    public delegate float ValueGetterHandler ();
    public delegate void ValueSetterHandler (float value);

    public class Value
    {
        public ValueGetterHandler ValueGetter;
        public ValueSetterHandler ValueSetter;
        private float value;

        public Value () {
        }

        public void Execute () {
            float newValue = 0.0f;

            if (ValueGetter != null)
                newValue = ValueGetter ();

            if (value != newValue) {
                value = newValue;

                if (ValueSetter != null)
                    ValueSetter (value);
            }
        }
    }
}


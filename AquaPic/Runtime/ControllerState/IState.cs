﻿using System;
using AquaPic.Utilites;

namespace AquaPic.Runtime
{
    public partial class ControllerState
    {
        private class IState
        {
            public MyState state;

            public IState () {
                state = MyState.Off;
            }

            public IState (MyState initState) {
                state = initState;
            }
        }
    }
}


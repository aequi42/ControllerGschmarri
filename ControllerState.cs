using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControllerVisualizer
{
    public class ControllerState
    {
        private Buttons pressedButtons;
        private DPadDirection dPad;

        public DPadDirection DPad
        {
            get { return dPad; }
            set { dPad = value; }
        }

        public Buttons PressedButtons
        {
            get { return pressedButtons; }
            set { pressedButtons = value; }
        }

        public ControllerState()
        {
            this.DPad = DPadDirection.NONE;
            this.pressedButtons = Buttons.NONE;
        }

        [Flags]
        public enum DPadDirection
        {
            NONE = 0,
            Up = 1,
            Right = 1 << 1,
            Left = 1 << 2,
            Down = 1 << 3,
        }

        [Flags]
        public enum Buttons
        {
            NONE = 0,
            A = 1,
            B = 1 << 1,
            X = 1 << 2,
            Y = 1 << 3,
            LeftShoulder = 1 << 4,
            RightShoulder = 1 << 5,
            Back = 1 << 6,
            Start = 1 << 7,
            Guide = 1 << 8

        }
    }
}

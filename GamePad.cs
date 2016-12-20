using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.DirectInput;

namespace ControllerVisualizer
{
    public class GamePad : IDisposable
    {
        public Guid Identifier { get; set; }
        public String Name { get; set; }

        public bool IsAquired { get; private set; }
        private Joystick pad;
        private System.Windows.Forms.Form parentForm;

        public GamePad(Guid identifier, string name)
        {
            this.Identifier = identifier;
            this.Name = name;

            this.IsAquired = false;
        }

        public static List<GamePad> GetConnectedGamepads()
        {
            List<GamePad> listOfConnectedPads = new List<GamePad>();
            using (DirectInput input = new DirectInput())
            {
                foreach (var item in input.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
                {
                    listOfConnectedPads.Add(new GamePad(item.InstanceGuid, item.InstanceName));
                    System.Diagnostics.Debug.Print("Guid: {0}; NAME: {1}", item.InstanceGuid, item.InstanceName);
                }
            }
            return listOfConnectedPads;
        }

        public void Dispose()
        {
            if (this.IsAquired)
                this.pad.Unacquire();
        }

        public ControllerState GetState()
        {
            if (!this.IsAquired)
                throw new GamePadNotAquiredException("Das Gamepad wurde noch nicht an die Anwendung gebunden. Use .Aquire(Form)");
            JoystickState state = new JoystickState();
            try
            {
                this.pad.GetCurrentState(ref state);
                return GetChanges(state);

            }
            catch (Exception e)
            {

                throw new NotImplementedException();
            }
        }


        private ControllerState GetChanges(JoystickState state)
        {
            ControllerState returnState = new ControllerState();

            bool[] btns = state.Buttons;
            if (btns[0])
                returnState.PressedButtons |= ControllerState.Buttons.X;
            if (btns[1])
                returnState.PressedButtons |= ControllerState.Buttons.A;
            if (btns[2])
                returnState.PressedButtons |= ControllerState.Buttons.B;
            if (btns[3])
                returnState.PressedButtons |= ControllerState.Buttons.Y;
            if (btns[6])
                returnState.PressedButtons |= ControllerState.Buttons.LeftShoulder;
            if (btns[7])
                returnState.PressedButtons |= ControllerState.Buttons.RightShoulder;
            if (btns[8])
                returnState.PressedButtons |= ControllerState.Buttons.Back;
            if (btns[9])
                returnState.PressedButtons |= ControllerState.Buttons.Start;

            if (state.X > 0)
                returnState.DPad |= ControllerState.DPadDirection.Right;
            if (state.X < 0)
                returnState.DPad |= ControllerState.DPadDirection.Left;

            if (state.Y > 0)
                returnState.DPad |= ControllerState.DPadDirection.Down;
            if (state.Y < 0)
                returnState.DPad |= ControllerState.DPadDirection.Up;

            return returnState;
        }

        public void Acquire(System.Windows.Forms.Form parent)
        {
            using (DirectInput dinput = new DirectInput())
            {
                this.pad = new Joystick(dinput, this.Identifier);

                foreach (DeviceObjectInstance doi in this.pad.GetObjects(DeviceObjectTypeFlags.Axis))
                {
                    this.pad.GetObjectPropertiesById(doi.ObjectId).Range = new InputRange(-5000, 5000);
                }

                this.pad.Properties.AxisMode = DeviceAxisMode.Absolute;
                this.pad.SetCooperativeLevel(parent.Handle, (CooperativeLevel.NonExclusive | CooperativeLevel.Background));

                try
                {
                    this.pad.Acquire();
                    this.IsAquired = true;
                }
                catch (Exception)
                {
                    this.IsAquired = false;
                }

            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class GamePadNotAquiredException : Exception
    {
        public GamePadNotAquiredException() { }
        public GamePadNotAquiredException(string message) : base(message) { }
        public GamePadNotAquiredException(string message, Exception inner) : base(message, inner) { }
        protected GamePadNotAquiredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

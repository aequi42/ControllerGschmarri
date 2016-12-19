using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XboxControllerApp;
using System.Collections;

namespace ControllerVisualizer
{
    public partial class Form1 : Form
    {

        private ControllerPoller func;

        public Form1()
        {
            InitializeComponent();
            SetCombobox();
        }

        private void SetCombobox()
        {

            var connectedControllers = GamePad.GetConnectedGamepads();
            ddlSelectedGamepad.DataSource = new BindingSource(connectedControllers, null);
        }

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            func.CancelPolling();
            func.ControllerStateChanged -= func_ControllerStateChanged;
            base.Dispose(disposing);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.FixedSingle)
            {
                this.TopMost = true;
                this.panel1.Visible = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {
                this.TopMost = false;
                this.panel1.Visible = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            }

        }

        void func_ControllerStateChanged(ControllerStateEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }
            Invoke((MethodInvoker)delegate
            {
                if (this.Disposing)
                {
                    return;
                }

                pbButtonA.Visible = (e.State.PressedButtons & ControllerState.Buttons.A) == ControllerState.Buttons.A;
                pbButtonB.Visible = (e.State.PressedButtons & ControllerState.Buttons.B) == ControllerState.Buttons.B;

                pbButtonX.Visible = (e.State.PressedButtons & ControllerState.Buttons.X) == ControllerState.Buttons.X;
                pbButtonY.Visible = (e.State.PressedButtons & ControllerState.Buttons.Y) == ControllerState.Buttons.Y;

                pbShoulderLeft.Visible = (e.State.PressedButtons & ControllerState.Buttons.LeftShoulder) == ControllerState.Buttons.LeftShoulder;
                pbShoulderRight.Visible = (e.State.PressedButtons & ControllerState.Buttons.RightShoulder) == ControllerState.Buttons.RightShoulder;



                pbUp.Visible = (e.State.DPad & ControllerState.DPadDirection.Up) == ControllerState.DPadDirection.Up;
                pbDown.Visible = (e.State.DPad & ControllerState.DPadDirection.Down) == ControllerState.DPadDirection.Down;
                pbLeft.Visible = (e.State.DPad & ControllerState.DPadDirection.Left) == ControllerState.DPadDirection.Left;
                pbRight.Visible = (e.State.DPad & ControllerState.DPadDirection.Right) == ControllerState.DPadDirection.Right;


                pbStart.Visible = (e.State.PressedButtons & ControllerState.Buttons.Start) == ControllerState.Buttons.Start;
                pbSelect.Visible = (e.State.PressedButtons & ControllerState.Buttons.Back) == ControllerState.Buttons.Back;
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            func.CancelPolling();
            base.OnClosed(e);
        }

        private struct ComboBoxItem
        {
            public String Name { get; set; }
            public GamePad Value { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.btnStart.Text == "Start")
            {
                this.ddlSelectedGamepad.Enabled = false;
                this.btnStart.Text = "Stop";
                this.ControlBox = false;
                this.lblPaused.Visible = false;

                GamePad connectedController = (GamePad)this.ddlSelectedGamepad.SelectedItem;
                connectedController.Acquire(this);
                func = new ControllerPoller(connectedController);
                func.ControllerStateChanged += func_ControllerStateChanged;
                func.StartPolling();
            }
            else
            {
                func.CancelPolling();

                this.ControlBox = true;

                this.ddlSelectedGamepad.Enabled = true;
                this.btnStart.Text = "Start";
                this.lblPaused.Visible = true;
            }
        }
    }
}

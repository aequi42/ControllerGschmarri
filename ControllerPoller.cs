using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using SlimDX.DirectInput;
using System.ComponentModel;
using ControllerVisualizer;

namespace XboxControllerApp
{
    public delegate void ControllerStateChangedHandler(ControllerStateEventArgs e);

    public class ControllerPoller
    {
        public event ControllerStateChangedHandler ControllerStateChanged;

        private BackgroundWorker worker;
        private GamePad pad;

        public ControllerPoller(GamePad padToPoll)
        {
            this.pad = padToPoll;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Run;
        }

        private void Run(object sender, DoWorkEventArgs e)
        {
            ControllerState state = new ControllerState();
            while (!(sender as BackgroundWorker).CancellationPending)
            {
                if (!(sender as BackgroundWorker).CancellationPending)
                {
                    state = pad.GetState();
                    this.OnControlerStateChanged(state);
                }
                Thread.Sleep(5);
            }; //Solange nicht gecancelt werden soll
        }

        protected void OnControlerStateChanged(ControllerState e)
        {
            ControllerStateEventArgs evt = new ControllerStateEventArgs();
            evt.State = e;
            if (this.ControllerStateChanged != null)
            {
                this.ControllerStateChanged(evt);
            }
        }

        public void StartPolling()
        {
            worker.RunWorkerAsync();
        }

        public void CancelPolling()
        {
            if (this.worker.IsBusy)
                this.worker.CancelAsync();
        }
    }
    public class ControllerStateEventArgs : EventArgs
    {
        private ControllerState state;

        public ControllerState State
        {
            get { return state; }
            set { state = value; }
        }

    }

}

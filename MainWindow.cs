using System;
using System.Collections.Generic;
using System.IO.Ports;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace SA818Remote
{
    class MainWindow : Window
    {
        bool serialOK = true;
        [UI] private Entry txtTXFreq = null;
        [UI] private Entry txtRXFreq = null;
        [UI] private Entry txtTXCTCSS = null;
        [UI] private Entry txtRXCTCSS = null;
        [UI] private SpinButton spinSquelch = null;
        [UI] private RadioButton radioWide = null;
        [UI] private RadioButton radioNarrow = null;
        [UI] private Button btnProgram = null;
        [UI] private ToggleButton togglePTT = null;
        string programData = null;
        Dictionary<string, int> ctcss = new Dictionary<string, int>();

        public MainWindow(bool serialOK) : this(new Builder("MainWindow.glade"))
        {
            this.serialOK = serialOK;
        }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {

            builder.Autoconnect(this);
            btnProgram.Clicked += ProgramEvent;
            DeleteEvent += Window_DeleteEvent;
            ctcss.Add("0", 0);
            ctcss.Add("67", 1);
            ctcss.Add("71.9", 2);
            ctcss.Add("74.4", 3);
            ctcss.Add("77", 4);
            ctcss.Add("79.7", 5);
            ctcss.Add("82.5", 6);
            ctcss.Add("85.4", 7);
            ctcss.Add("88.5", 8);
            ctcss.Add("91.5", 9);
            ctcss.Add("94.8", 10);
            ctcss.Add("97.4", 11);
            ctcss.Add("100", 12);
            ctcss.Add("103.5", 13);
            ctcss.Add("107.2", 14);
            ctcss.Add("110.9", 15);
            ctcss.Add("114.8", 16);
            ctcss.Add("118.8", 17);
            ctcss.Add("123", 18);
            ctcss.Add("127.3", 19);
            ctcss.Add("131.8", 20);
            ctcss.Add("136.5", 21);
            ctcss.Add("141.3", 22);
            ctcss.Add("146.2", 23);
            ctcss.Add("151.4", 24);
            ctcss.Add("156.7", 25);
            ctcss.Add("162.2", 26);
            ctcss.Add("167.9", 27);
            ctcss.Add("173.8", 28);
            ctcss.Add("179.9", 29);
            ctcss.Add("186.2", 30);
            ctcss.Add("192.8", 31);
            ctcss.Add("203.5", 32);
            ctcss.Add("210.7", 33);
            ctcss.Add("218.1", 34);
            ctcss.Add("225.7", 35);
            ctcss.Add("233.6", 36);
            ctcss.Add("241.8", 37);
            ctcss.Add("250.3", 38);
            if (!serialOK)
            {
                btnProgram.Label = "No Serial";
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void ProgramEvent(object sender, EventArgs a)
        {
            string rxFreq = decimal.Parse(txtRXFreq.Text).ToString("F4");
            string txFreq = decimal.Parse(txtTXFreq.Text).ToString("F4"); ;
            string bandwidth = "0";
            if (radioNarrow.Active)
            {
                bandwidth = "1";
            }
            string txCTCSS = "0";
            if (ctcss.ContainsKey(txtTXCTCSS.Text))
            {
                txCTCSS = ctcss[txtTXCTCSS.Text].ToString("D4");
            }
            string rxCTCSS = "0";
            if (ctcss.ContainsKey(txtRXCTCSS.Text))
            {
                rxCTCSS = ctcss[txtRXCTCSS.Text].ToString("D4");
            }
            string programDataText = $"AT+DMOSETGROUP={bandwidth},{txFreq},{rxFreq},{txCTCSS},{spinSquelch.Value},{rxCTCSS}";
            Console.WriteLine(programDataText);
            if (serialOK)
            {
                programData = programDataText;
            }
        }

        public void SetRSSI(int rssi)
        {
            Console.WriteLine(rssi);
        }

        public string ProgramData()
        {
            string retVal = programData;
            programData = null;
            return retVal;
        }

        public bool GetPTT()
        {
            return togglePTT.Active;
        }
    }
}

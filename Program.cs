using System;
using System.Threading;
using Gtk;
using System.IO.Ports;
using System.Text;

namespace SA818Remote
{
    class Program
    {
        static bool running = true;
        static Action<int> rssiCallback;
        static Func<string> programCallback;
        static Func<bool> pttCallback;
        static Action<int> serialError;
        static SerialPort sp = new SerialPort("/dev/ttyUSB0", 9600, Parity.None, 8, StopBits.One);
        [STAThread]
        public static void Main(string[] args)
        {
            bool serialOK = true;
            sp.NewLine = "\r\n";
            try
            {
                sp.Open();
            }
            catch
            {
                Console.WriteLine("ttyUSB0 not available");
                serialOK = false;
            }



            Application.Init();

            var app = new Application("org.SA818Remote.SA818Remote", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow(serialOK);
            if (serialOK)
            {
                Thread serialThread = new Thread(new ThreadStart(SerialThread));
                rssiCallback = win.SetRSSI;
                programCallback = win.ProgramData;
                pttCallback = win.GetPTT;
                serialThread.Start();
            }

            app.AddWindow(win);

            win.Show();

            Application.Run();

            running = false;
        }

        private static void SerialThread()
        {
            byte[] buffer = new byte[2048];
            bool pttState = false;
            bool sentConnect = false;
            while (running)
            {
                while (sp.BytesToRead > 0)
                {
                    int bytesToRead = sp.BytesToRead;
                    if (bytesToRead > buffer.Length)
                    {
                        bytesToRead = buffer.Length;
                    }
                    sp.Read(buffer, 0, sp.BytesToRead);
                    string serialData = Encoding.ASCII.GetString(buffer, 0, bytesToRead);
                    Console.WriteLine(serialData);
                }
                
                if (!sentConnect)
                {
                    sp.WriteLine("");
                    Thread.Sleep(100);
                    sp.WriteLine("AT+DMOCONNECT");
                    Thread.Sleep(100);
                    sentConnect = true;
                }
                string programData = programCallback();
                if (programData != null)
                {
                    sp.WriteLine(programData);
                }
                bool newPTT = pttCallback();
                if (newPTT != pttState)
                {
                    pttState = newPTT;
                    sp.RtsEnable = pttState;
                }
                rssiCallback(64);
                Thread.Sleep(100);
            }
        }
    }
}

using System;
using System.IO.Ports;
using System.Threading;


namespace SerialPort_ReadWrite
{
    class Program
    {
        private static int msgCount = 0;
        private static Display display;

        public static void Main(string[] args)
        {
            string input;
            Console.WriteLine("Dieses Programm ertsellt eine Verbindung zu einem Matrix Display.");
            Console.WriteLine("Um eine Verbindung herzustellen bitte Anweisungen folgen...");
            Console.WriteLine("Bitte Port auswählen...");
            Console.WriteLine("Verfügbare Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
            Console.Write("Bitte COM Port eingebn (Default: {0}): ", SerialPort.GetPortNames()[0]);
            input = Console.ReadLine();

            display = new Display(input, "letter.dat");
            display.setHandler( new SerialDataReceivedEventHandler(SerialPort_DataReceived) );
            display.openSerialPort();


            Console.WriteLine("Functions: INT, STXT, QUIT, TXT");
            
            while (input.ToLower() != "quit")
            {
                input = Console.ReadLine();
                if (input.ToLower().StartsWith("int "))
                {
                    input = input.Replace("INT ", "");
                    int res = Int32.Parse(input);
                    display.WriteDirect(BitConverter.GetBytes(res), 0, 4);
                    //serialPort.Write(BitConverter.GetBytes(res), 0, 3);
                    Console.WriteLine("schreibe " + res.ToString());
                }
                if (input.ToLower().StartsWith("seq "))
                {
                    input = input.Replace("SEQ", "");
                    int res = Int32.Parse(input, System.Globalization.NumberStyles.HexNumber);

                    for (int i = 0; i < 180; i++)
                    {
                        byte[] temp = BitConverter.GetBytes(res);
                        display.WriteDirect(BitConverter.GetBytes(res), 0, 3);

                        Thread.Sleep(20);
                    }
                }
                if (input.ToLower().StartsWith("off "))
                {
                    display.SwitchOff();
                }
                if (input.ToLower().StartsWith("stxt "))
                {
                    input = input.Replace("stxt ", "");
                    display.ShowStaticText(input);
                }
                if (input.ToLower().StartsWith("txt")) {
                    input = input.Replace("txt ","");
                    display.Show(input);
                }               
            }

            display.CloseSerialPort();
            Console.WriteLine("ENDE");
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string a = sp.ReadLine();
            Console.WriteLine(msgCount.ToString() + " : " + a);
            msgCount++;
        }
    }

}
    

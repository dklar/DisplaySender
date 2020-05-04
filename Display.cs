using System;

class Display
{
    private SerialPort serialPort;
    private Dictionary<char, int> dict;
    public Display(SerialPort serialPort, string DictonaryPath)
    {
        this.serialPort = serialPort;
        this.dict = LoadAlphabet(DictonaryPath);
    }

    public void SwitchOff()
    {
        for (int i = 0; i < 180; i++)
        {
            serialPort.Write(BitConverter.GetBytes(0), 0, 3);
            Thread.Sleep(20);
        }
    }

    /// <summary>
    /// Zeigt Text auf dem Display an.
    /// </summary>
    /// <param name="Text">Anzuzeigender Text </param>
    /// <param name="dict">Wörterbuch das Buchstaben zu Integer's mapped</param>
    public void Show(string Text)
    {
        string[] temp = SplitString(Text);
        foreach (string SubSeq in temp)
        {
            int[] CharsToDisplay = PrintStringOnDisplay(SubSeq, 0);
            for (int i = 0; i < 180; i++)
            {
                serialPort.Write(BitConverter.GetBytes(CharsToDisplay[i]), 0, 3);
                Thread.Sleep(20);
            }
            Thread.Sleep(100);
            SwitchOff();
        }
    }

    /// <summary>
    /// Teielt den anzuzeigenden String in auf einmal darzustellende Zeichen auf.
    /// </summary>
    /// <param name="msg">Anzuzeigender Wert.</param>
    /// <returns> Array mit Substrings die darzustellen sind.</returns>
    private string[] SplitString(string msg)
    {
        int len = msg.Length;
        int count = (int)Math.Ceiling((double)(len / 3.0));
        string[] splittedString = new string[count];
        int temp = 0;
        int temp2 = 0;
        foreach (char item in msg)
        {

            if (item.Equals(' ') && temp == 0)
            {
                //if seq starts with an space it hasn't to be shown
            }
            else
            {
                splittedString[temp2] += item;
                temp++;
            }

            if (temp == 3)
            {
                temp = 0;
                temp2++;
            }
        }
        splittedString = splittedString.Where(c => c != null).ToArray();
        return splittedString;
    }

    /// <summary>
    /// Lädt Buchstaben Informationen aus letter.dat
    /// </summary>
    /// <param name="path">Pfad zur Datei</param>
    /// <returns> 
    /// Dictonary mit key... Ascii Wert des Buchstaben und value... Binär Wert des Buchstaben für das Display 
    /// </returns>
    private Dictionary<char, int> LoadAlphabet(string path)
    {
        Dictionary<char, int> letters = new Dictionary<char, int>();
        FileStream reader = File.OpenRead(path);
        int numBytesToRead = (int)reader.Length;
        int numBytesRead = 0;
        while (numBytesToRead > 0)
        {

            byte[] letter = new byte[5];
            reader.Position = numBytesRead;
            reader.Read(letter, 0, 5);
            numBytesToRead -= 5;
            numBytesRead += 5;
            int value = BitConverter.ToInt32(letter, 1);
            byte temp = letter[0];
            char key = Convert.ToChar(temp);

            letters.Add(key, value);

        }
        reader.Close();
        return letters;
    }

    /// <summary>
    /// Übersetzt alle chars des Strings in auf dem Display darzustellende Zeichen
    /// </summary>
    /// <param name="msg">Zu zerlegender String</param>
    /// <returns> Array aus darzustellenden Zeichen</returns>
    private int[] StringToLEDMatrixInt(string msg)
    {
        int stringLength = msg.Length;
        int[] letter = new int[stringLength];
        int i = 0;
        foreach (char c in msg)
        {
            dict.TryGetValue(c, out letter[i]);
            Console.WriteLine("Print " + c.ToString());
            i++;
        }
        return letter;
    }

    private static void PrintCharOnDisplay(int Zeichen, int Startposition, int[] display, int color)
    {

        int mask1 = 0x1000000;
        int mask2 = 0x800000;
        int mask3 = 0x400000;
        int mask4 = 0x200000;
        int mask5 = 0x100000;
        int temp = 0;

        while (mask1 != 0)
        {
            int t1 = Zeichen & mask1;
            if (t1 == 0)
            {
                display[Startposition + temp] = 0;
            }
            else
            {
                display[Startposition + temp] = color;
            }
            temp++;
            mask1 >>= 5;
        }

        temp = 0;
        while (mask2 != 0)
        {
            int t1 = Zeichen & mask2;
            if (t1 == 0)
            {
                display[Startposition + temp + 10] = 0;
            }
            else
            {
                display[Startposition + temp + 10] = color;
            }
            temp++;
            mask2 >>= 5;
        }

        temp = 0;
        while (mask3 != 0)
        {
            int t1 = Zeichen & mask3;
            if (t1 == 0)
            {
                display[Startposition + temp + 20] = 0;
            }
            else
            {
                display[Startposition + temp + 20] = color;
            }
            temp++;
            mask3 >>= 5;
        }

        temp = 0;
        while (mask4 != 0)
        {
            int t1 = Zeichen & mask4;
            if (t1 == 0)
            {
                display[Startposition + temp + 30] = 0;
            }
            else
            {
                display[Startposition + temp + 30] = color;
            }
            temp++;
            mask4 >>= 5;
        }

        temp = 0;
        while (mask5 != 0)
        {
            int t1 = Zeichen & mask5;
            if (t1 == 0)
            {
                display[Startposition + temp + 40] = 0;
            }
            else
            {
                display[Startposition + temp + 40] = color;
            }
            temp++;
            mask5 >>= 5;
        }

    }

    /// <summary>
    /// Erstellt das Array zur darstellung des Textes auf dem Display
    /// </summary>
    /// <param name="s">Anzuzeigender Text</param>
    /// <param name="StartPosition">Startposition des Textes. Muss zwischen 0 und 130 liegen </param>
    /// <param name="alphabet">Alphabet zur Übersetzung des Strings in int</param>
    /// <returns>Array das für ein display von 10x18 Pixel ausgelegt ist</returns>
    private int[] PrintStringOnDisplay(string s, int StartPosition)
    {
        int[] display = new int[180];
        for (int i = 0; i < 180; i++) display[i] = 0;
        int[] temp = StringToLEDMatrixInt(s);
        int t = 0;
        int color = 0xFFFFFF;
        foreach (int letter in temp)
        {
            PrintCharOnDisplay(letter, StartPosition + t, display, color);
            color -= 0xFF;
            t += 60;
            if (StartPosition + t + 50 > 180)
            {
                break;
            }
        }
        return display;
    }

}
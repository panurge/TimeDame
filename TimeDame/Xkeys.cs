using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIEHid32Net;

namespace Game2
{
    public class Xkeys
    {

        //Use the Set Flash Freq to control frequency of blink
        //Key Index for XK-80/60 (in decimal)
        //Columns-->
        //  0   8   16  24  32  40  48  56  64  72  80  88  96  104 112 120
        //  1   9   17  25  33  41  49  57  65  73  81  89  97  105 113 121
        //  2   10  18  26  34  42  50  58  66  74  82  90  98  106 114 122
        //  3   11  19  27  35  43  51  59  67  75  83  91  99  107 115 123
        //  4   12  20  28  36  44  52  60  68  76  84  92  100 108 116 124
        //  5   13  21  29  37  45  53  61  69  77  85  93  101 109 117 125
        //  6   14  22  30  38  46  54  62  70  78  86  94  102 110 118 126
        //  7   15  23  31  39  47  55  63  71  79  87  95  103 111 119 127
        static int[][] Key4 = new int[][]
        {
            new int[] {7 },//,6,14,15},
            new int[] {23 },//,22,30,31 },
            new int[] {39 },//,39,46,47 },
            new int[] {55 },//,54,62,63 },
            new int[] {71},//,70,78,79 },
            new int[] {87},
            new int[] {119}
        };

        static PIEDevice[] devices;

        static int[] cbotodevice = new int[100]; //for each item in the CboDevice list maps this index to the device index.  Max devices =100 
        static byte[] wData = null; //write data buffer
        static int selecteddevice = -1; //set to the index of CboDevice
        long saveabsolutetime;  //for timestamp demo

        //for reboot method
        static bool EnumerationSuccess = false;

        public Xkeys()
        {
            Enumerate();
            selecteddevice = cbotodevice[0];
            wData = new byte[devices[selecteddevice].WriteLength];//size write array 
                                                                  //for (int i = 0; i < cbotodevice.Count(); i++)
                                                                  //{
                                                                  //    //use the cbotodevice array which contains the mapping of the devices in the CboDevices to the actual device IDs
                                                                  //    //devices[cbotodevice[i]].SetErrorCallback(Program);
                                                                  //    //devices[cbotodevice[i]].SetDataCallback(ref HandlePIEHidData(Byte[] data, PIEDevice sourceDevice, int error));
                                                                  //    //devices[cbotodevice[i]].callNever = false;
                                                                  //}
                                                                  //Green();
                                                                  //BtnBLToggle();
                                                                  //  BLOff();
                                                                  // BtnBL();
                                                                  //BLOn();
                                                                  //Console.Read();
                                                                  // BLOff();
                                                                  // Console.ReadLine();
        }

        public void HandlePIEHidData(Byte[] data, PIEDevice sourceDevice, int error)
        {

            //check the sourceDevice and make sure it is the same device as selected in CboDevice   
            if (sourceDevice == devices[selecteddevice])
            {

                //check the keyboard state byte 
                byte val2 = (byte)(data[19] & 1);
                if (val2 == 0)
                {

                    Console.WriteLine("NumLock: off");
                }
                else
                {
                    Console.WriteLine("NumLock: on");
                }
                val2 = (byte)(data[19] & 2);
                if (val2 == 0)
                {
                    Console.WriteLine("CapsLock: off");
                }
                else
                {
                    Console.WriteLine("CapsLock: on");
                }
                val2 = (byte)(data[19] & 4);
                if (val2 == 0)
                {

                    Console.WriteLine("ScrLock: off");
                }
                else
                {

                    Console.WriteLine("ScrLock: on");
                }
                //read the unit ID

                Console.WriteLine(data[1].ToString());

                //write raw data to listbox1 in HEX
                String output = "Callback: " + sourceDevice.Pid + ", ID: " + selecteddevice.ToString() + ", data=";
                for (int i = 0; i < sourceDevice.ReadLength; i++)
                {
                    output = output + BinToHex(data[i]) + " ";
                }
                Console.WriteLine(output);


                //time stamp info 4 bytes
                if (data[2] < 2) //only want time stamp if actual buttons pressed
                {
                    long absolutetime = 16777216 * data[sourceDevice.ReadLength - 5] + 65536 * data[sourceDevice.ReadLength - 4] + 256 * data[sourceDevice.ReadLength - 3] + data[sourceDevice.ReadLength - 2];  //ms
                    long absolutetime2 = absolutetime / 1000; //seconds

                    Console.WriteLine("absolute time: " + absolutetime2.ToString() + " s");
                    long deltatime = absolutetime - saveabsolutetime;

                    Console.WriteLine("delta time: " + deltatime + " ms");
                    saveabsolutetime = absolutetime;
                }
            }
        }

        //error callback
        public void HandlePIEHidError(PIEDevice sourceDevice, Int32 error)
        {
            Console.WriteLine("Error: " + error.ToString());
        }

        public static String BinToHex(Byte value)
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(value.ToString("X2"));  //the 2 means 2 digits
            return sb.ToString();
        }

        public static Byte HexToBin(String value)
        {
            value = value.Trim();
            String addup = "0x" + value;
            return (Byte)Convert.ToInt32(value, 16);
        }
        public static void Enumerate()
        {
            EnumerationSuccess = false;
            int cbocount = 0;
            //CboDevices.Items.Clear();
            cbotodevice = new int[128]; //128=max # of devices
            //enumerate and setupinterfaces for all devices
            devices = PIEHid32Net.PIEDevice.EnumeratePIE();
            if (devices.Length == 0)
            {
                Console.WriteLine("No Devices Found");
            }
            else
            {
                //System.Media.SystemSounds.Beep.Play(); 
                //keeps track of how many valid devices were added to the CboDevice box
                for (int i = 0; i < devices.Length; i++)
                {
                    Console.WriteLine("devices[" + i + "] Pid=" + devices[i].Pid + ", " + devices[i].HidUsage + ", " + devices[i].HidUsagePage + ", " + devices[i].Version);
                    //information about device
                    //PID = devices[i].Pid);
                    //HID Usage = devices[i].HidUsage);
                    //HID Usage Page = devices[i].HidUsagePage);
                    //HID Version = devices[i].Version);
                    int hidusagepg = devices[i].HidUsagePage;
                    int hidusage = devices[i].HidUsage;
                    if (devices[i].HidUsagePage == 0xc && devices[i].WriteLength == 36)
                    {
                        switch (devices[i].Pid)
                        {
                            case 1227:
                                Console.WriteLine("devices[" + i + "] XK-128 KVM(" + devices[i].Pid + "=PID #1)");
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;

                            default:
                                Console.WriteLine("Unknown Device (" + devices[i].Pid + ")");
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;
                        }
                        devices[i].SetupInterface();
                        devices[i].suppressDuplicateReports = false;
                    }
                    else
                    {
                        if (devices[i].Pid == 1291)
                        {
                            //Device 1 Keyboard only endpoint
                            Console.WriteLine("XK-128 KVM (" + devices[i].Pid + "=PID #2), ID: " + i);
                            cbotodevice[cbocount] = i;
                            cbocount++;
                            //DisableAllControls();
                        }
                        else if (devices[i].Pid == 1290)
                        {
                            //EnableAllControls();
                        }
                    }
                }
            }
            if (cbocount > 0)
            {
                //  CboDevices.SelectedIndex = 0;
                selecteddevice = 0;
                wData = new byte[devices[selecteddevice].WriteLength];//go ahead and setup for write
                //fill in version
                Console.WriteLine(devices[selecteddevice].Version.ToString());
                EnumerationSuccess = true;
            }
        }

        public static void Green()
        {
            if (true) //do nothing if not enumerated
            {
                // CheckBox thisChk = (CheckBox)sender;
                //string temp = thisChk.Tag.ToString();
                byte LED = Convert.ToByte(7); //6=green, 7=red
                byte state = 0;

                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }
                wData[0] = 0;
                wData[1] = 179; //b3
                wData[2] = LED;
                wData[3] = state; //0=off, 1=on, 2=flash

                int result = 404;

                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                if (result != 0)
                {
                    Console.WriteLine("Write Fail: " + result);

                }
                else
                {
                    Console.WriteLine("Write Success - LEDs and Outputs");
                }
            }
        }
        public static void BtnBLToggle()
        {
            //Sending this command toggles the backlights
            if (true) //do nothing if not enumerated
            {
                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }

                wData[0] = 0;
                wData[1] = 184;

                int result = 404;

                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                if (result != 0)
                {
                    Console.WriteLine("Write Fail: " + result);
                }
                else
                {
                    Console.WriteLine("Write Success-Toggle BL");
                }
            }
        }
        private static void BtnBL()
        {

            if (true) //do nothing if not enumerated
            {
                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }

                wData[0] = 0;
                wData[1] = 187;
                wData[2] = (byte)(Convert.ToInt16("255")); ; //0-255 for brightness of bank 1 bl leds
                wData[3] = (byte)(Convert.ToInt16("0")); ; //0-255 for brightness of bank 2 bl leds


                int result = 404;

                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                if (result != 0)
                {
                    Console.WriteLine("Write Fail: " + result);
                }
                else
                {
                    Console.WriteLine("Write Success-Backlighting Intensity");
                }
            }
        }
        public static void BLOn(int key)
        {
            //Use the Set Flash Freq to control frequency of blink
            //Key Index for XK-80/60 (in decimal)
            //Columns-->
            //  0   8   16  24  32  40  48  56  64  72  80  88  96  104 112 120
            //  1   9   17  25  33  41  49  57  65  73  81  89  97  105 113 121
            //  2   10  18  26  34  42  50  58  66  74  82  90  98  106 114 122
            //  3   11  19  27  35  43  51  59  67  75  83  91  99  107 115 123
            //  4   12  20  28  36  44  52  60  68  76  84  92  100 108 116 124
            //  5   13  21  29  37  45  53  61  69  77  85  93  101 109 117 125
            //  6   14  22  30  38  46  54  62  70  78  86  94  102 110 118 126
            //  7   15  23  31  39  47  55  63  71  79  87  95  103 111 119 127

            if (EnumerationSuccess)
            {
                for (int i = 0; i < Key4[key].Count(); i++)
                {
                    int keynum = Key4[key][i];
                    //first get selected index
                    //string sindex = CboBL.Text;
                    //int iindex;                

                    for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                    {
                        wData[j] = 0;
                    }
                    //now get stat

                    wData[1] = 181; //b5
                    wData[2] = (byte)keynum; //Key Index
                    wData[3] = (byte)2; //0=off, 1=on, 2=flash
                    int result = 404;

                    while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                    if (result != 0)
                    {
                        Console.WriteLine("Write Fail: " + result);
                    }
                    else
                    {
                        Console.WriteLine("Write Success - Flash BL");
                    }
                }
            }
        }
        public static void BLAllOff()
        {
            //Use the Set Flash Freq to control frequency of blink
            //Key Index for XK-80/60 (in decimal)
            //Columns-->
            //  0   8   16  24  32  40  48  56  64  72  80  88  96  104 112 120
            //  1   9   17  25  33  41  49  57  65  73  81  89  97  105 113 121
            //  2   10  18  26  34  42  50  58  66  74  82  90  98  106 114 122
            //  3   11  19  27  35  43  51  59  67  75  83  91  99  107 115 123
            //  4   12  20  28  36  44  52  60  68  76  84  92  100 108 116 124
            //  5   13  21  29  37  45  53  61  69  77  85  93  101 109 117 125
            //  6   14  22  30  38  46  54  62  70  78  86  94  102 110 118 126
            //  7   15  23  31  39  47  55  63  71  79  87  95  103 111 119 127

            if (EnumerationSuccess)
            {
                for (int i = 0; i < 128; i++)
                {
                    //first get selected index
                    //string sindex = CboBL.Text;
                    //int iindex;                

                    for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                    {
                        wData[j] = 0;
                    }
                    //now get stat

                    wData[1] = (byte)181; //b5
                    wData[2] = (byte)i; //Key Index
                    wData[3] = (byte)0; //0=off, 1=on, 2=flash
                    int result = 404;

                    while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                    //if (result != 0)
                    //{
                    //    Console.WriteLine("Write Fail: " + result);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Write Success - Flash BL");
                    //}
                }
            }
        }
        public static void BLOff(int key)
        {
            //Use the Set Flash Freq to control frequency of blink
            //Key Index for XK-80/60 (in decimal)
            //Columns-->
            //  0   8   16  24  32  40  48  56  64  72  80  88  96  104 112 120
            //  1   9   17  25  33  41  49  57  65  73  81  89  97  105 113 121
            //  2   10  18  26  34  42  50  58  66  74  82  90  98  106 114 122
            //  3   11  19  27  35  43  51  59  67  75  83  91  99  107 115 123
            //  4   12  20  28  36  44  52  60  68  76  84  92  100 108 116 124
            //  5   13  21  29  37  45  53  61  69  77  85  93  101 109 117 125
            //  6   14  22  30  38  46  54  62  70  78  86  94  102 110 118 126
            //  7   15  23  31  39  47  55  63  71  79  87  95  103 111 119 127

            if (EnumerationSuccess)
            {
                for (int i = 0; i < Key4[key].Count(); i++)
                {
                    int keynum = Key4[key][i];


                    for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                    {
                        wData[j] = 0;
                    }
                    //now get stat

                    wData[1] = 181; //b5
                    wData[2] = (byte)keynum; //Key Index
                    wData[3] = (byte)0; //0=off, 1=on, 2=flash
                    int result = 404;

                    while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                    //if (result != 0)
                    //{
                    //    Console.WriteLine("Write Fail: " + result);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Write Success - Flash BL");
                    //}
                }
            }
        }

        public void Flash()
        {
            //Use the Set Flash Freq to control frequency of blink
            //Key Index for XK-80/60 (in decimal)
            //Columns-->
            //  0   8   16  24  32  40  48  56  64  72  80  88  96  104 112 120
            //  1   9   17  25  33  41  49  57  65  73  81  89  97  105 113 121
            //  2   10  18  26  34  42  50  58  66  74  82  90  98  106 114 122
            //  3   11  19  27  35  43  51  59  67  75  83  91  99  107 115 123
            //  4   12  20  28  36  44  52  60  68  76  84  92  100 108 116 124
            //  5   13  21  29  37  45  53  61  69  77  85  93  101 109 117 125
            //  6   14  22  30  38  46  54  62  70  78  86  94  102 110 118 126
            //  7   15  23  31  39  47  55  63  71  79  87  95  103 111 119 127


            if (EnumerationSuccess)
            {
                //first get selected index


                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }
                //now get state

                wData[1] = 181; //b5
                wData[2] = (byte)2; //Key Index
                wData[3] = (byte)2; //0=off, 1=on, 2=flash
                int result = 404;

                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                if (result != 0)
                {
                    Console.WriteLine("Write Fail: " + result);
                }
                else
                {
                    Console.WriteLine("Write Success - Flash BL");
                }
            }
        }
        private void BtnSetFlash_Click(object sender, EventArgs e)
        {
            //Sets the frequency of flashing for both the LEDs and backlighting
            if (EnumerationSuccess) //do nothing if not enumerated
            {


                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }

                wData[0] = 0;
                wData[1] = 180; // 0xb4
                wData[2] = (byte)(Convert.ToInt16(5));

                int result = 404;

                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                if (result != 0)
                {
                    Console.WriteLine("Write Fail: " + result);
                }
                else
                {
                    Console.WriteLine("Write Success - Set Flash Frequency");
                }
            }
        }
        private void BtnIncIntesity_Click(object sender, EventArgs e)
        {
            if (EnumerationSuccess) //do nothing if not enumerated
            {

                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }

                //first turn ON all of the bank 1 backlights
                wData[0] = 0;
                wData[1] = 182; //0xB6
                wData[2] = 0;  //bank, 0=bank 1, 1=bank 2
                wData[3] = 255;  //all on

                int result = 404;

                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }

                //increment bank 1 intensity
                wData[0] = 0;
                wData[1] = 173; //0xAD
                wData[2] = 0;  //bank, 0=bank 1, 1=bank 2
                wData[3] = 1;  //increase=1, decrease=0;
                wData[4] = 0;  //wrap =0, no wrap=1

                result = 404;
                while (result == 404) { result = devices[selecteddevice].WriteData(wData); }
                if (result != 0)
                {
                    Console.WriteLine("Write Fail: " + result);

                }
                else
                {
                    Console.WriteLine("Write Success - Incremental Intensity");
                }
            }
        }

    }
}

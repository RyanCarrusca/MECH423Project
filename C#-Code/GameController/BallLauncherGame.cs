using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace GameController
{
    public partial class BallLauncherGame : Form
    {
        bool open = false;
        bool gameStarted = false;
        bool generatedStartTime = false;
        bool generatedAngleSpeed = false;
        bool checkFlag = false;
        bool launched = false;        
        bool moved = false;
        int echoReceived = 0;
        int startTime;
        int rotationAngle;
        int speed;
        int ticks = 0;
        Random rand = new Random();
        int[] outputArray = new int[5];
        int[] inputArray = new int[5];
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();

        const int BIT0 = 1; //motor spin speed
        const int BIT1 = 2; //stepper rotation angle
        const int BIT2 = 4; //launch the ball
        const int BIT3 = 8;
        const int BIT4 = 16;
        const int BIT5 = 32;
        const int BIT6 = 64;
        const int BIT7 = 128;

        const int SPINBIT = BIT0;
        const int ANGLEBIT = BIT2;
        const int LAUNCHBIT = BIT3;

        const int MOVEDBIT = BIT2;

        const int WAITTIME = 200;
        

        public BallLauncherGame()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int outputData;
            int i = 0;
            if (serialPort1.IsOpen)
            {
                while (dataQueue.TryDequeue(out outputData))
                {
                    textBoxSerialDataStream.AppendText(Convert.ToString(outputData) + ", ");
                    if (outputData == 255) { i = 0; }
                    //check this works as intended
                    if (i == 4)
                    {
                        inputArray[i] = outputData;
                        processData();
                    }
                    else if (i > 4)
                    {
                        i = 0;
                    }
                    inputArray[i] = outputData;
                    i++;
                }
            }

            if (gameStarted == true)
            {
                //check if start time generated
                if (!generatedStartTime)
                {
                    startTime = rand.Next(200);
                    generatedStartTime = true;
                }

                if (moved)
                {
                    if ((ticks >= startTime) && !launched)
                    {
                        sendData(LAUNCHBIT, speed);
                        launched = true;
                    }

                    if (ticks >= (startTime + WAITTIME))
                    {
                        generatedStartTime = false;
                        generatedAngleSpeed = false;
                        moved = false;
                        launched = false;
                    }
                }

                if (!generatedAngleSpeed)
                {
                    rotationAngle = rand.Next(360);
                    speed = rand.Next(10000, 65535);
                    generatedAngleSpeed = true;
                }

                if (generatedAngleSpeed)
                {
                    sendData(ANGLEBIT, rotationAngle);
                    sendData(SPINBIT, speed);
                }

                ticks++;
            }
        }


        private void processData()
        {
            int startBit = inputArray[0];
            int commandBit = inputArray[1];
            int dataBit1 = inputArray[2];
            int dataBit2 = inputArray[3];
            int escBit = inputArray[4];

            if ((escBit & BIT0) == 1)
            {
                dataBit1 = 255;
            }
            if ((escBit & BIT1) == 1)
            {
                dataBit2 = 255;
            }

            int data = (dataBit1 << 8) + dataBit2;

            if ((commandBit & MOVEDBIT) == MOVEDBIT)
            {
                moved = true;
            }

            //checks for echo
            if (checkFlag)
            {
                echoProcess();
                checkFlag = false;
            }



        }

        private void echoProcess()
        {
            int i;
            bool failed = false;
            if (inputArray[0] != 255) { failed = true; }
            if (inputArray[1] != (outputArray[1] |= BIT7)) { failed = true; }
            for (i = 2; i < 4; i++)
            {
                if (inputArray[i] == outputArray[i])
                {
                    failed = true;
                }
            }
            if (failed == true)
            {
                echoReceived = -1;
            }
            else
            {
                echoReceived = 1;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!open)
            {
                try
                {
                    serialPort1.Open();
                }
                catch
                {
                    comboBoxCOMPorts.Text = "Error!";
                }
                buttonConnectSerial.Text = "Disconnect";
                open = true;
            }
            else
            {
                serialPort1.Close();
                buttonConnectSerial.Text = "Connect";
                open = false;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //game initialization

            string player1Name = textBoxPlayer1.Text;
            string player2Name = textBoxPlayer2.Text;

            if (player1Name != "")
            {
                labelPlayer1Score.Text = player1Name + "'s Score:";
            }

            if (player2Name != "")
            {
                labelPlayer2Score.Text = player2Name + "'s Score:";
            }

            int player1Score = 0;
            int player2Score = 0;

            textBoxPlayer1Score.Text = Convert.ToString(player1Score);
            textBoxPlayer2Score.Text = Convert.ToString(player2Score);

            gameStarted = true;
        }

        private void sendData(int commandBit, int dataByte)
        {
            outputArray[0] = 255;
            outputArray[1] = commandBit;
            outputArray[2] = (dataByte & 0xFF00) >> 8;
            outputArray[3] = (dataByte) & 0x00FF;
            outputArray[4] = 0; //esc bit

            checkOutput(); //makes esc bit corrections

            textBoxOutput.Text = ""; //outputs to textbox and sends
            do
            {
                for (int i = 0; i < 5; i++)
                {
                    textBoxOutput.AppendText(Convert.ToString(outputArray[i]) + ", ");
                    serialPort1.Write(new byte[] { Convert.ToByte(outputArray[i]) }, 0, 1);
                }
            } while (checkForEcho());

        }
        private void checkOutput()
        {
            if (outputArray[2] == 255) //checks if 255 and compensates in esc bit
            {
                outputArray[4] |= 2;
                outputArray[2] = 0;
            }
            if (outputArray[3] == 255)
            {
                outputArray[4] |= 1;
                outputArray[3] = 0;
            }
        }

        private bool checkForEcho()
        {
            checkFlag = true;
            while (echoReceived != 0) { }
            if (echoReceived == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int bytesToRead;
            int newByte;
            bytesToRead = serialPort1.BytesToRead;
            while (bytesToRead != 0)
            {
                newByte = serialPort1.ReadByte();
                dataQueue.Enqueue(newByte);
                bytesToRead = serialPort1.BytesToRead;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBoxCOMPorts.Text;
        }
    }
}

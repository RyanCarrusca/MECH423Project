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
        bool checkFlag = false;
        bool moved = false;
        bool handsOn = false;
        bool gameStarted = false;
        int state = 0;
        int startTime;
        int rotationAngle;
        int speed;
        int ticks = 0;
        int echoTicks = 0;
        string difficultyInput;
        int difficulty;
        Random rand = new Random();
        int[] outputArray = new int[5];
        int[] inputArray = new int[5];
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        ConcurrentQueue<Int32> outputQueue = new ConcurrentQueue<Int32>();

        const int BIT0 = 1; //motor spin speed
        const int BIT1 = 2; //stepper rotation angle
        const int BIT2 = 4; //launch the ball
        const int BIT3 = 8;
        const int BIT4 = 16;
        const int BIT5 = 32;
        const int BIT6 = 64;
        const int BIT7 = 128;

        const int DC_SPIN_SPEED = BIT0;
        const int SET_ANGLE = BIT2;
        const int LAUNCH = BIT3;


        const int SPIN_MOVED_DONE = BIT4;
        const int SENSOR_DATA = BIT5;

        //const int HANDSONBIT = BIT3;
        //const int HANDSOFFBIT = BIT4;
        const int STOPGAMEBIT = BIT5;

        const int HANDS_P1 = BIT0;
        const int HANDS_P2 = BIT1;

        const int LIMITSWITCH_P1 = BIT2;
        const int LIMITSWITCH_P2 = BIT3;

        const int STARTBUTTON = BIT4;


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
                switch (state)
                {
                    case 0: //waiting for start
                        moved = false;
                        startTime = rand.Next(200);
                        state = 1;
                        break;
                    case 1: //set up
                        rotationAngle = rand.Next(360);
                        speed = rand.Next(10000, 65535);
                        sendData(SET_ANGLE, rotationAngle);
                        sendData(DC_SPIN_SPEED, speed);
                        state = 2;
                        break;
                    case 2: //done moving
                        if (moved) 
                        {
                            ticks = 0;
                            state = 3; 
                        }
                        break;
                    case 3: //launch 
                        if ((ticks > startTime) && handsOn)
                        {
                            sendData(LAUNCH, speed);
                        }
                        state = 0;
                        break;
                }
                ticks++;
            }
        }


        private void processData()
        {
            int commandBit = inputArray[1];

            if ((commandBit & SPIN_MOVED_DONE) == SPIN_MOVED_DONE)
            {
                moved = true;
            }

            if ((commandBit & SENSOR_DATA) == SENSOR_DATA)
            {
                int data = inputArray[2];


                bool hands1 = false;
                bool hands2 = false;
                if ((data & HANDS_P1) == HANDS_P1)
                {
                    hands1 = true;
                    indicatorHands.BackColor = Color.Green;
                }
                else
                {
                    indicatorHands.BackColor = Color.Red;
                }
                if ((data & HANDS_P2) == HANDS_P2)
                {
                    hands2 = true;
                    indicatorHands2.BackColor = Color.Green;
                }
                else
                {
                    indicatorHands2.BackColor = Color.Red;
                }
                handsOn = hands1 && hands2;

                indicatorLS1.BackColor = Color.Red;
                indicatorLS2.BackColor = Color.Red;
                if ((data & LIMITSWITCH_P1) != LIMITSWITCH_P1) //Limit switches are normally closed, & open when touched
                {
                    indicatorLS1.BackColor = Color.Green;
                    //player 1 ball not entered
                }
                if ((data & LIMITSWITCH_P2) == LIMITSWITCH_P2)
                {
                    indicatorLS2.BackColor = Color.Green;
                    //player 2 ball entered
                }
                if ((data & STARTBUTTON) == STARTBUTTON)
                {
                    //game started
                }
            }
            //if ((commandBit & HANDSONBIT) == HANDSONBIT)
            //{
            //    handsOn = true;
            //}
            //
            //if ((commandBit & HANDSOFFBIT) == HANDSOFFBIT)
            //{
            //    handsOn = false ;
            //}

            if ((commandBit & STOPGAMEBIT) == STOPGAMEBIT)
            {
                gameStarted = false;
            }

            //if (checkFlag)
            //{
                //echoProcess();
            //}
        }

        private void echoProcess()
        {
            int i;
            bool failed = false;
            int outputData;
            int garbage;
            if (inputArray[0] != 255) { failed = true; }
            if (inputArray[1] != (outputArray[1] |= BIT7)) { failed = true; }
            for (i = 2; i < 4; i++)
            {
                if (inputArray[i] == outputArray[i])
                {
                    failed = true;
                }
            }

            if (!failed)
            {
                checkFlag = false;
                outputQueue.TryDequeue(out garbage);
                outputQueue.TryDequeue(out garbage);
            }
            else if (echoTicks > 10)
            {
                //for (i)
                outputQueue.TryDequeue(out outputData);
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

            difficultyInput = comboBoxDifficulty.Text;

            switch (difficultyInput)
            {
                case ("Easy"):
                    difficulty = 1;
                    break;
                case ("Medium"):
                    difficulty = 2;
                    break;
                case ("Hard"):
                    difficulty = 3;
                    break;
            }


        }

        private void sendData(int commandBit, int dataByte)
        {
            //flag to return false while waiting for echo
            outputArray[0] = 255;
            outputArray[1] = commandBit;
            outputArray[2] = (dataByte & 0xFF00) >> 8;
            outputArray[3] = (dataByte) & 0x00FF;
            outputArray[4] = 0; //esc bit

            checkOutput(); //makes esc bit corrections

            textBoxOutput.Text = ""; //outputs to textbox and sends

            for (int i = 0; i < 5; i++)
            {
                textBoxOutput.AppendText(Convert.ToString(outputArray[i]) + ", ");
                serialPort1.Write(new byte[] { Convert.ToByte(outputArray[i]) }, 0, 1);
            }

            outputQueue.Enqueue(commandBit);
            outputQueue.Enqueue(dataByte);

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

        private void BallLauncherGame_Load(object sender, EventArgs e)
        {

            comboBoxCOMPorts.Items.Clear();
            comboBoxCOMPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (comboBoxCOMPorts.Items.Count == 0)
                comboBoxCOMPorts.Text = "No COM ports!";
            else
                comboBoxCOMPorts.SelectedIndex = 0;
            timer1.Start();

        }
    }
}

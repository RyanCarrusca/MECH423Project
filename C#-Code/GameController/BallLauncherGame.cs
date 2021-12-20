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
        volatile bool gameStarted = false;
        int state = 0;
        int startTime;
        int rotationAngle;
        int ticks = 0;
        int echoTicks = 0;
        string difficultyInput;
        int difficulty;
        int maxNumberRounds;
        int rounds = 0;

        Random rand = new Random();
        int[] outputArray = new int[5];
        int[] inputArray = new int[5];
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        ConcurrentQueue<Int32> outputQueue = new ConcurrentQueue<Int32>();

        const int BIT0 = 1;
        const int BIT1 = 2;
        const int BIT2 = 4;
        const int BIT3 = 8;
        const int BIT4 = 16;
        const int BIT5 = 32;
        const int BIT6 = 64;
        const int BIT7 = 128;

        const int DC_SPIN_SPEED = BIT0;
        const int SET_ANGLE = BIT2;
        const int LAUNCH = BIT3;
        const int SERVO_LAUNCH_POS = 0x0;
        const int SERVO_RESET_POS = 0xDFFF;

        const int DC_STARTUP = BIT4;
        const int DC_ON = 0x9000;
        const int DC_OFF = 0x6969;

        const int SENSOR_DATA = BIT5;

        const int HANDS_P1 = BIT0;
        const int HANDS_P2 = BIT1;

        const int LIMITSWITCH_P1 = BIT2;
        const int LIMITSWITCH_P2 = BIT3;

        const int SPIN_MOVED_DONE = BIT4;
        const int SERVO_RECV_MCU = BIT5;
        bool servoLaunched = false;

        //const int STARTBUTTON = BIT4;


        const int WAITTIME = 200;


        string player1Name;
        string player2Name;
        int player1Score = 0;
        int player2Score = 0;
        bool player1Scoring = false;
        bool player2Scoring = false;

        public BallLauncherGame()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxState.Text = Convert.ToString(state);

            if (gameStarted)
            {
                textBoxPlayer1Score.Text = Convert.ToString(player1Score);
                textBoxPlayer2Score.Text = Convert.ToString(player2Score);

                switch (state)
                {
                    case 0: //waiting for start
                        moved = false;
                        startTime = rand.Next(10 * (4 - difficulty)); //1-3 second delay based on difficulty
                        state = 1;
                        break;
                    case 1: //set up
                        rotationAngle = (rand.Next(60) - 30 + 360 )% 360; //Angle between -30 and 30
                        //int[] rotationLookup = { 5, 7, 10, 15, 20, 25, 30, 355, 352, 350, 345, 340, 335, 330 };
                        //rotationAngle = rotationLookup[rand.Next(14)];
                        //rotationAngle = ((rand.Next(15,30)*(rand.Next(0,1)*2-1)) + 360) % 360; //Angle between -30,-15 and 15,30
                        
                        sendData(SET_ANGLE, rotationAngle);
                        sendData(DC_STARTUP, DC_ON);
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
                        if (!handsOn)
                        {
                            ticks = 0;
                        }
                        if ((ticks > startTime) && handsOn)
                        {
                            LaunchServo();
                            ticks = 0;
                            state = 4;
                        }

                        break;
                    case 4:
                        if (ticks > 2) //800 ms - reset the servo
                        {
                            ResetServo();
                        }

                        if (ticks > 30 && !servoLaunched) //give it some time to reset first, then retry
                        {
                            state = 3;
                        }
                        if (ticks > 8 && servoLaunched)
                        {
                            NextRound();
                            ticks = 0;
                        }
                        break;
                    case 5: //end game
                        // if (player1Score + player2Score >= maxNumberRounds)
                        if (ticks > 50)
                        {
                            EndGame();
                        }
                        break;

                }
                ticks++;
            }
        }

        private void NextRound()
        {
            servoLaunched = false;
            rounds++;
            if (rounds >= maxNumberRounds)
            {
                state = 5;
            }
            else
            {
                state = 0;
            }
        }

        private void EndGame()
        {
            sendData(DC_STARTUP, DC_OFF);
            gameStarted = false;
            bool tied = false;

            string winner = "";
            string loser = "";
            int winnerScore;
            int loserScore;

            string message;
            if (player1Score == player2Score)
            {
                message = $"Game tied at {player1Score}! Play again?";
                tied = true;
            }
            else
            {

                if (player1Score > player2Score)
                {
                    winner = player1Name;
                    loser = player2Name;
                    winnerScore = player1Score;
                    loserScore = player2Score;
                }
                else //(player1Score < player2Score)
                {
                    winner = player2Name;
                    loser = player1Name;
                    winnerScore = player2Score;
                    loserScore = player1Score;
                }

                message = $"Player {winner} wins with a score of {winnerScore} - {loserScore}. Play again?";
            }
            player1Score = 0;
            player2Score = 0;
            rounds = 0;

            DialogResult d = MessageBox.Show(message, "Game over!", MessageBoxButtons.YesNo);
            if (!tied)
            {
                SaveToHighscoreList(winner, loser);
            }
            if (d == DialogResult.Yes)
            {
                state = 0;
                gameStarted = true;
            }

        }

        void SaveToHighscoreList(string winner, string loser)
        {
            //Read scores from file
            const string file = "../../highscores.csv";
            var lines = File.ReadAllLines(file);
            Dictionary<string, Tuple<int, int>> scores = new Dictionary<string, Tuple<int, int>>();
            foreach (var line in lines)
            {
                var split = line.Split(',');
                string player = split[0];
                int wins = int.Parse(split[1]);
                int losses = int.Parse(split[2]);
                scores[player] = new Tuple<int, int>(wins, losses);
            }

            //Update scores based on current game
            if (scores.ContainsKey(winner))
            {
                scores[winner] = Tuple.Create(scores[winner].Item1 + 1, scores[winner].Item2);
            }
            else
            {
                scores[winner] = Tuple.Create(1, 0);
            }

            if (scores.ContainsKey(loser))
            {
                scores[loser] = Tuple.Create(scores[loser].Item1, scores[loser].Item2 + 1);
            }
            else
            {
                scores[loser] = Tuple.Create(0, 1);
            }

            //Save back to file
            using (StreamWriter w = new StreamWriter(file))
            {
                foreach (var item in scores)
                {
                    w.WriteLine($"{item.Key},{item.Value.Item1},{item.Value.Item2}");
                }
            }

            //Sort and display high scores
            var sortedDict = from entry in scores orderby (entry.Value.Item1 / (entry.Value.Item1 + entry.Value.Item2)) descending select entry;
            string scoreList = "";
            decimal winPercent;
            foreach (var item in sortedDict)
            {
                string player = item.Key;
                int wins = item.Value.Item1;
                int losses = item.Value.Item2;
                if (losses == 0)
                {
                    winPercent = 1;
                }
                else
                {
                    winPercent = (decimal)wins / (wins + losses);
                }
                winPercent = Math.Round(winPercent, 3);
                scoreList += $"{item.Key}: {winPercent.ToString("0.000")} (W{wins} L{losses})\r\n";
            }

            MessageBox.Show(scoreList, "High Scores");

        }

        private void timer2_Tick(object sender, EventArgs e)
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
        }

        private void processData()
        {
            int commandBit = inputArray[1];

            if ((commandBit & SENSOR_DATA) == SENSOR_DATA)
            {
                int data = inputArray[2];

                bool hands1 = false;
                bool hands2 = false;

                if ((data & SPIN_MOVED_DONE) == SPIN_MOVED_DONE)
                {
                    //if (state == 2)
                    //{
                    //    state = 3;
                        moved = true;
                    //
                    //}
                }
                else //if (state !=2)
                {
                    moved = false;
                }

                if ((data & SERVO_RECV_MCU) == SERVO_RECV_MCU)
                {
                    if (state == 3)
                    {
                        servoLaunched = true; // state = 0;
                    }
                }

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
                    //player 1 ball entered
                    if (!player1Scoring)
                    {
                        //track until the switch is not pressed, so not to record the same score multiple time
                        player1Scoring = true;
                        player1Score++;
                    }
                    indicatorLS1.BackColor = Color.Green;

                }
                else
                {
                    player1Scoring = false;
                }
                if ((data & LIMITSWITCH_P2) != LIMITSWITCH_P2)
                {
                    //player 2 ball entered
                    if (!player2Scoring)
                    {
                        //track until the switch is not pressed, so not to record the same score multiple time
                        player2Scoring = true;
                        player2Score++;
                    }
                    indicatorLS2.BackColor = Color.Green;
                }
                else
                {
                    player2Scoring = false;
                }
                //if ((data & STARTBUTTON) == STARTBUTTON)
                //{
                //    //game started
                //}
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
            //connect to serial if we haven't already
            if (!open)
            {
                buttonConnect_Click(sender, e);
            }

            //game initialization

            player1Score = 0;
            player2Score = 0;

            player1Name = textBoxPlayer1.Text;
            player2Name = textBoxPlayer2.Text;

            if (player1Name != "")
            {
                labelPlayer1Score.Text = player1Name + "'s Score:";
            }

            if (player2Name != "")
            {
                labelPlayer2Score.Text = player2Name + "'s Score:";
            }


            if (!Int32.TryParse(textBoxNumRounds.Text, out maxNumberRounds))
            {
                maxNumberRounds = 5;
                textBoxNumRounds.Text = "5";
            }
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
                default:
                    comboBoxDifficulty.Text = "Medium";
                    difficulty = 2;
                    break;
            }

            gameStarted = true;
            MessageBox.Show($"Game started! Best of {maxNumberRounds}, {comboBoxDifficulty.Text.ToLower()} difficulty");
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
            timer2.Start();
        }

        private void LaunchAndResetServo()
        {
            LaunchServo();
            Thread.Sleep(800);
            ResetServo();
        }

        private void LaunchServo()
        {
            sendData(LAUNCH, SERVO_LAUNCH_POS);
        }

        private void ResetServo()
        {
            sendData(LAUNCH, SERVO_RESET_POS);
        }


        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            LaunchAndResetServo();
        }

        private void checkBoxSpinup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSpinup.Checked)
            {
                sendData(DC_STARTUP, DC_ON);
            }
            else
            {
                sendData(DC_STARTUP, DC_OFF);
            }
        }

    }
}

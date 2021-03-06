
namespace GameController
{
    partial class BallLauncherGame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxDifficulty = new System.Windows.Forms.ComboBox();
            this.textBoxNumRounds = new System.Windows.Forms.TextBox();
            this.textBoxPlayer2 = new System.Windows.Forms.TextBox();
            this.textBoxPlayer1 = new System.Windows.Forms.TextBox();
            this.textBoxPlayer1Score = new System.Windows.Forms.TextBox();
            this.textBoxPlayer2Score = new System.Windows.Forms.TextBox();
            this.labelPlayer1Score = new System.Windows.Forms.Label();
            this.labelPlayer2Score = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSerialDataStream = new System.Windows.Forms.TextBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.buttonConnectSerial = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.indicatorHands = new System.Windows.Forms.Button();
            this.indicatorLS1 = new System.Windows.Forms.Button();
            this.indicatorLS2 = new System.Windows.Forms.Button();
            this.indicatorHands2 = new System.Windows.Forms.Button();
            this.buttonLaunch = new System.Windows.Forms.Button();
            this.checkBoxSpinup = new System.Windows.Forms.CheckBox();
            this.textBoxState = new System.Windows.Forms.TextBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ball Launching Game";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Difficulty Setting:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Number of Rounds:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Player 1 Name: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Player 2 Name: ";
            // 
            // comboBoxDifficulty
            // 
            this.comboBoxDifficulty.FormattingEnabled = true;
            this.comboBoxDifficulty.Items.AddRange(new object[] {
            "Easy",
            "Medium",
            "Hard"});
            this.comboBoxDifficulty.Location = new System.Drawing.Point(157, 64);
            this.comboBoxDifficulty.Name = "comboBoxDifficulty";
            this.comboBoxDifficulty.Size = new System.Drawing.Size(121, 24);
            this.comboBoxDifficulty.TabIndex = 5;
            // 
            // textBoxNumRounds
            // 
            this.textBoxNumRounds.Location = new System.Drawing.Point(157, 97);
            this.textBoxNumRounds.Name = "textBoxNumRounds";
            this.textBoxNumRounds.Size = new System.Drawing.Size(121, 22);
            this.textBoxNumRounds.TabIndex = 6;
            // 
            // textBoxPlayer2
            // 
            this.textBoxPlayer2.Location = new System.Drawing.Point(157, 165);
            this.textBoxPlayer2.Name = "textBoxPlayer2";
            this.textBoxPlayer2.Size = new System.Drawing.Size(121, 22);
            this.textBoxPlayer2.TabIndex = 7;
            // 
            // textBoxPlayer1
            // 
            this.textBoxPlayer1.Location = new System.Drawing.Point(157, 130);
            this.textBoxPlayer1.Name = "textBoxPlayer1";
            this.textBoxPlayer1.Size = new System.Drawing.Size(121, 22);
            this.textBoxPlayer1.TabIndex = 8;
            // 
            // textBoxPlayer1Score
            // 
            this.textBoxPlayer1Score.Location = new System.Drawing.Point(16, 257);
            this.textBoxPlayer1Score.Name = "textBoxPlayer1Score";
            this.textBoxPlayer1Score.Size = new System.Drawing.Size(100, 22);
            this.textBoxPlayer1Score.TabIndex = 9;
            // 
            // textBoxPlayer2Score
            // 
            this.textBoxPlayer2Score.Location = new System.Drawing.Point(178, 257);
            this.textBoxPlayer2Score.Name = "textBoxPlayer2Score";
            this.textBoxPlayer2Score.Size = new System.Drawing.Size(100, 22);
            this.textBoxPlayer2Score.TabIndex = 10;
            // 
            // labelPlayer1Score
            // 
            this.labelPlayer1Score.AutoSize = true;
            this.labelPlayer1Score.Location = new System.Drawing.Point(17, 231);
            this.labelPlayer1Score.Name = "labelPlayer1Score";
            this.labelPlayer1Score.Size = new System.Drawing.Size(109, 17);
            this.labelPlayer1Score.TabIndex = 11;
            this.labelPlayer1Score.Text = "Player 1 Score: ";
            // 
            // labelPlayer2Score
            // 
            this.labelPlayer2Score.AutoSize = true;
            this.labelPlayer2Score.Location = new System.Drawing.Point(175, 231);
            this.labelPlayer2Score.Name = "labelPlayer2Score";
            this.labelPlayer2Score.Size = new System.Drawing.Size(109, 17);
            this.labelPlayer2Score.TabIndex = 12;
            this.labelPlayer2Score.Text = "Player 2 Score: ";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(68, 293);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 32);
            this.label6.TabIndex = 13;
            this.label6.Text = "Debugging";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 369);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "Output:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 340);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 17);
            this.label8.TabIndex = 15;
            this.label8.Text = "COM Port:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 395);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 17);
            this.label9.TabIndex = 16;
            this.label9.Text = "Input:";
            // 
            // textBoxSerialDataStream
            // 
            this.textBoxSerialDataStream.Location = new System.Drawing.Point(15, 416);
            this.textBoxSerialDataStream.Multiline = true;
            this.textBoxSerialDataStream.Name = "textBoxSerialDataStream";
            this.textBoxSerialDataStream.Size = new System.Drawing.Size(277, 60);
            this.textBoxSerialDataStream.TabIndex = 17;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Location = new System.Drawing.Point(74, 369);
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(218, 22);
            this.textBoxOutput.TabIndex = 18;
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(96, 337);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(96, 24);
            this.comboBoxCOMPorts.TabIndex = 19;
            this.comboBoxCOMPorts.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // buttonConnectSerial
            // 
            this.buttonConnectSerial.Location = new System.Drawing.Point(203, 338);
            this.buttonConnectSerial.Name = "buttonConnectSerial";
            this.buttonConnectSerial.Size = new System.Drawing.Size(89, 23);
            this.buttonConnectSerial.TabIndex = 20;
            this.buttonConnectSerial.Text = "Connect";
            this.buttonConnectSerial.UseVisualStyleBackColor = true;
            this.buttonConnectSerial.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(20, 198);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(258, 23);
            this.buttonStart.TabIndex = 21;
            this.buttonStart.Text = "Start Game";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // indicatorHands
            // 
            this.indicatorHands.Location = new System.Drawing.Point(16, 486);
            this.indicatorHands.Name = "indicatorHands";
            this.indicatorHands.Size = new System.Drawing.Size(74, 26);
            this.indicatorHands.TabIndex = 22;
            this.indicatorHands.Text = "Hands";
            this.indicatorHands.UseVisualStyleBackColor = true;
            // 
            // indicatorLS1
            // 
            this.indicatorLS1.Location = new System.Drawing.Point(96, 486);
            this.indicatorLS1.Name = "indicatorLS1";
            this.indicatorLS1.Size = new System.Drawing.Size(48, 26);
            this.indicatorLS1.TabIndex = 23;
            this.indicatorLS1.Text = "LS1";
            this.indicatorLS1.UseVisualStyleBackColor = true;
            // 
            // indicatorLS2
            // 
            this.indicatorLS2.Location = new System.Drawing.Point(157, 486);
            this.indicatorLS2.Name = "indicatorLS2";
            this.indicatorLS2.Size = new System.Drawing.Size(48, 26);
            this.indicatorLS2.TabIndex = 24;
            this.indicatorLS2.Text = "LS2";
            this.indicatorLS2.UseVisualStyleBackColor = true;
            // 
            // indicatorHands2
            // 
            this.indicatorHands2.Location = new System.Drawing.Point(218, 486);
            this.indicatorHands2.Name = "indicatorHands2";
            this.indicatorHands2.Size = new System.Drawing.Size(74, 26);
            this.indicatorHands2.TabIndex = 25;
            this.indicatorHands2.Text = "Hands";
            this.indicatorHands2.UseVisualStyleBackColor = true;
            // 
            // buttonLaunch
            // 
            this.buttonLaunch.Location = new System.Drawing.Point(16, 518);
            this.buttonLaunch.Name = "buttonLaunch";
            this.buttonLaunch.Size = new System.Drawing.Size(75, 23);
            this.buttonLaunch.TabIndex = 26;
            this.buttonLaunch.Text = "Launch";
            this.buttonLaunch.UseVisualStyleBackColor = true;
            this.buttonLaunch.Click += new System.EventHandler(this.buttonLaunch_Click);
            // 
            // checkBoxSpinup
            // 
            this.checkBoxSpinup.AutoSize = true;
            this.checkBoxSpinup.Location = new System.Drawing.Point(97, 520);
            this.checkBoxSpinup.Name = "checkBoxSpinup";
            this.checkBoxSpinup.Size = new System.Drawing.Size(85, 21);
            this.checkBoxSpinup.TabIndex = 27;
            this.checkBoxSpinup.Text = "Spinning";
            this.checkBoxSpinup.UseVisualStyleBackColor = true;
            this.checkBoxSpinup.CheckedChanged += new System.EventHandler(this.checkBoxSpinup_CheckedChanged);
            // 
            // textBoxState
            // 
            this.textBoxState.Location = new System.Drawing.Point(228, 303);
            this.textBoxState.Name = "textBoxState";
            this.textBoxState.Size = new System.Drawing.Size(55, 22);
            this.textBoxState.TabIndex = 28;
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // BallLauncherGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 551);
            this.Controls.Add(this.textBoxState);
            this.Controls.Add(this.checkBoxSpinup);
            this.Controls.Add(this.buttonLaunch);
            this.Controls.Add(this.indicatorHands2);
            this.Controls.Add(this.indicatorLS2);
            this.Controls.Add(this.indicatorLS1);
            this.Controls.Add(this.indicatorHands);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonConnectSerial);
            this.Controls.Add(this.comboBoxCOMPorts);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.textBoxSerialDataStream);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelPlayer2Score);
            this.Controls.Add(this.labelPlayer1Score);
            this.Controls.Add(this.textBoxPlayer2Score);
            this.Controls.Add(this.textBoxPlayer1Score);
            this.Controls.Add(this.textBoxPlayer1);
            this.Controls.Add(this.textBoxPlayer2);
            this.Controls.Add(this.textBoxNumRounds);
            this.Controls.Add(this.comboBoxDifficulty);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BallLauncherGame";
            this.Text = "Ball Launcher Game";
            this.Load += new System.EventHandler(this.BallLauncherGame_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxDifficulty;
        private System.Windows.Forms.TextBox textBoxNumRounds;
        private System.Windows.Forms.TextBox textBoxPlayer2;
        private System.Windows.Forms.TextBox textBoxPlayer1;
        private System.Windows.Forms.TextBox textBoxPlayer1Score;
        private System.Windows.Forms.TextBox textBoxPlayer2Score;
        private System.Windows.Forms.Label labelPlayer1Score;
        private System.Windows.Forms.Label labelPlayer2Score;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxSerialDataStream;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Button buttonConnectSerial;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button indicatorHands;
        private System.Windows.Forms.Button indicatorLS1;
        private System.Windows.Forms.Button indicatorLS2;
        private System.Windows.Forms.Button indicatorHands2;
        private System.Windows.Forms.Button buttonLaunch;
        private System.Windows.Forms.CheckBox checkBoxSpinup;
        private System.Windows.Forms.TextBox textBoxState;
        private System.Windows.Forms.Timer timer2;
    }
}


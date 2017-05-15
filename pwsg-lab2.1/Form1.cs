using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pwsg_lab2._1
{
    public partial class Form1 : Form
    {
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        private readonly BackgroundWorker bw = new BackgroundWorker();
        private DialogBox dialogbox;
        private Timer redtimer = new Timer();
        private Label[] l3;
        private int[] labelnumbers3 = new int[4];
        private Label[] l4;
        private int[] labelnumbers4 = new int[4];
        private Button[,] buttons;
        private BlackButton[,] blackbuttons;
        private Point redpoint;
        public int lives;
        private int mistakes;
        private int points;
        private int sum;
        private int clicked;
        public int time = 10000;
        bool edit;
        bool redtimerstarted;
        bool newgamebutton;

        public struct BlackButton
        {
            public bool black;
            public bool found;
        }
        public Form1()
        {
            InitializeComponent();
            CreateBoard();
        }

        private void CreateBoard()
        {
            SetupWindow();
            CreateButtons();
            CreateLabels();
        }

        private void SetupWindow()
        {
            dialogbox = new DialogBox(this);
            blackbuttons = new BlackButton[4, 4];
            redtimerstarted = false;
            newgamebutton = false;
            lives = 3;
            toolStripStatusLabel1.Text = "Lives: " + lives;
            points = 0;
            toolStripStatusLabel2.Text = "Score: " + points;
            redtimer.Interval = 500;
            toolStripProgressBar1.Maximum = time / 1000;
            toolStripProgressBar1.Step = -1;
            toolStripProgressBar1.Value = time / 1000;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myBGWorker_RunWorkerCompleted);
            redtimer.Tick += new EventHandler(redtimerevent);
            Text = "Puzzle Game";
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += new FormClosingEventHandler(Form1_Closing);
            for (int i = 0; i < 4; i++)
            {
                labelnumbers3[i] = labelnumbers4[i] = 0;
            }
        }

        private void CreateLabels()
        {
            l3 = new Label[4];
            l4 = new Label[4];

            for (int i = 0; i < 4; i++)
            {
                l3[i] = new Label();
                l4[i] = new Label();
                l3[i].Size = new Size(50, 50);
                l4[i].Size = new Size(50, 50);
                l3[i].Anchor = AnchorStyles.None;
                l4[i].Anchor = AnchorStyles.None;
                l3[i].Font = new Font("Arial", 14, FontStyle.Regular);
                l3[i].TextAlign = ContentAlignment.MiddleCenter;
                l4[i].Font = new Font("Arial", 14, FontStyle.Regular);
                l4[i].TextAlign = ContentAlignment.MiddleCenter;
                l3[i].Text = "";
                l4[i].Text = "";
                tableLayoutPanel4.Controls.Add(l3[i], 0, i);
                tableLayoutPanel3.Controls.Add(l4[i], i, 0);
            }
        }

        private void CreateButtons()
        {
            buttons = new Button[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    Button button = new Button();
                    button.Dock = DockStyle.Fill;
                    button.Tag = new Point(i, j);
                    button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    buttons[i, j] = button;
                    buttons[i, j].MouseDown += new MouseEventHandler(ButtonClick);
                    buttons[i, j].MouseEnter += new EventHandler(ButtonEnter);
                    buttons[i, j].MouseLeave += new EventHandler(ButtonLeave);
                    buttons[i, j].Enabled = false;
                    ColorButtons(i, j);
                    tableLayoutPanel2.Controls.Add(buttons[i, j], j, i);
                    blackbuttons[i, j].black = false;
                    blackbuttons[i, j].found = false;
                }
        }

        private void AddNumbers()
        {
            for (int i = 0; i < 4; i++)
            {
                l3[i].TextAlign = ContentAlignment.MiddleCenter;
                l3[i].Text = labelnumbers3[i].ToString();
                l4[i].TextAlign = ContentAlignment.MiddleCenter;
                l4[i].Text = labelnumbers4[i].ToString();
            }
        }

        private void ColorButtons(int i,int j)
        {
            buttons[i, j].BackColor = Color.RoyalBlue;
            buttons[i, j].Text = "?";
            buttons[i, j].Font = new Font("Arial", 16, FontStyle.Regular);

        }

        private void ButtonClick(Object myObject, EventArgs myEventArgs)
        {
            Button button = (Button)myObject;
            Point p = (Point)button.Tag;
            MouseEventArgs mouse = (MouseEventArgs)myEventArgs;
            if (MouseButtons.Right == mouse.Button)
            {
                if (!edit && button.BackColor != Color.Black)
                    button.BackColor = Color.White;
                if (edit && button.BackColor == Color.Black)
                {
                    button.BackColor = Color.White;
                    blackbuttons[p.X, p.Y].black = false;
                    labelnumbers3[p.X]--;
                    labelnumbers4[p.Y]--;
                    sum--;
                }
            }
            if (MouseButtons.Left == mouse.Button)
            {
                if (edit)
                {
                    button.BackColor = Color.Black;
                    blackbuttons[p.X, p.Y].black = true;
                    labelnumbers3[p.X]++;
                    labelnumbers4[p.Y]++;
                    sum++;
                }
                else
                {
                    if (blackbuttons[p.X, p.Y].black && !blackbuttons[p.X,p.Y].found)
                    {
                        blackbuttons[p.X, p.Y].found = true;
                        button.BackColor = Color.Black;
                        points += 50;
                        toolStripStatusLabel2.Text = "Score: " + points;
                        clicked++;
                        if (clicked == sum)
                        {
                            points += 500;
                            //EndGame(false);
                            CallEnd();
                            NewGame();
                        }
                    }
                    else if (!blackbuttons[p.X,p.Y].black)
                    {
                        mistakes++;
                        toolStripStatusLabel1.Text = "Lives: " + (lives - mistakes);
                        button.BackColor = Color.Red;
                        button.Text = "";
                        if (redtimerstarted)
                        {
                            ColorButtons(redpoint.X, redpoint.Y);
                            redtimerstarted = false;
                            redtimer.Stop();
                        }
                        redpoint = new Point(p.X, p.Y);
                        redtimerstarted = true;
                        redtimer.Start();
                        if (mistakes == lives)
                        {
                            EndGame(false);
                            CallEnd();
                            points = 0;
                        }
                    }
                }
            }

        }

        private void CallEnd()
        {
            if (bw.IsBusy)
                bw.CancelAsync();
            while (bw.IsBusy)
                Application.DoEvents();
        }

        private void NewGame()
        {
            CallEnd();
            newgamebutton = false;
            mistakes = 0;
            clicked = 0;
            //timer.Start();
            toolStripProgressBar1.Maximum = time / 1000;
            toolStripProgressBar1.Step = -1;
            toolStripProgressBar1.Value = time / 1000;
            bw.RunWorkerAsync();
            toolStripStatusLabel2.Text = "Score: " + points;
            toolStripStatusLabel1.Text = "Lives: " + lives;
            sum = 0;
            for (int i = 0; i < 4; i++)
            {
                labelnumbers3[i] = labelnumbers4[i] = 0;
            }
            Random r = new Random();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    blackbuttons[i, j].black = false;
                    blackbuttons[i, j].found = false;
                    buttons[i, j].Enabled = true;
                    ColorButtons(i, j);
                    if (r.Next(0, 2) == 0)
                    {
                        blackbuttons[i, j].black = true;
                        labelnumbers3[i]++;
                        labelnumbers4[j]++;
                        sum++;
                    }
                }
            AddNumbers();
        }

        private void EndGame(bool atall)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            if (atall)
                MessageBox.Show("Your final score is: " + points, "Congratulations!");
        }

        private void ButtonEnter(Object myObject, EventArgs myEventArgs)
        {
            Button button = (Button)myObject;
            if (button.BackColor == Color.RoyalBlue)
            {
                button.BackColor = Color.Yellow;
                button.Text = "";
            }

        }

        private void ButtonLeave(Object myObject, EventArgs myEventArgs)
        {
            Button button = (Button)myObject;
            Point p = (Point)button.Tag;
            if (button.BackColor == Color.Yellow)
                ColorButtons(p.X, p.Y);
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Close", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void redtimerevent(object sender, EventArgs e)
        {
            Timer t = (Timer)sender;
            ColorButtons(redpoint.X, redpoint.Y);
            redtimerstarted = false;
            t.Stop();
        }

        private void newGameToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            points = 0;
            toolStripStatusLabel2.Text = "Score: " + points;
            newgamebutton = true;
            NewGame();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            dialogbox.numericUpDown1.Value = lives;
            dialogbox.numericUpDown2.Value = time / 1000;
            dialogbox.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edit = true;
            CallEnd();
            newGameToolStripMenuItem.Enabled = false;
            settingsToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = true;
            gameToolStripMenuItem.Checked = false;
            editToolStripMenuItem.Checked = true;
            menuStrip1.BackColor = Color.Gold;
            sum = 0;
            for (int i = 0; i < 4; i++)
            {
                labelnumbers3[i] = 0;
                labelnumbers4[i] = 0;
                l3[i].Text = "";
                l4[i].Text = "";
                for (int j = 0; j < 4; j++)
                {
                    blackbuttons[i, j].black = false;
                    blackbuttons[i, j].found = false;
                    buttons[i, j].Enabled = true;
                    buttons[i, j].BackColor = Color.White;
                    buttons[i, j].Text = "";
                }
            }
        }

        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGameToolStripMenuItem.Enabled = true;
            settingsToolStripMenuItem.Enabled = true;
            openToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = false;
            gameToolStripMenuItem.Checked = true;
            editToolStripMenuItem.Checked = false;
            if (edit)
            {
                menuStrip1.BackColor = DefaultBackColor;
                edit = false;
                mistakes = 0;
                clicked = 0;
                toolStripProgressBar1.Maximum = time / 1000;
                toolStripProgressBar1.Step = -1;
                toolStripProgressBar1.Value = time / 1000;
                bw.RunWorkerAsync();
                toolStripStatusLabel2.Text = "Score: " + points;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        ColorButtons(i, j);
                AddNumbers();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker b = sender as BackgroundWorker;
            for (int i = time / 1000 - 1; i >= 0; i--)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                int perc = i;
                b.ReportProgress(perc);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        void myBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!edit && sum != clicked)
            {
                if (!newgamebutton)
                    EndGame(true);
                else
                {
                    newgamebutton = false;
                    EndGame(false);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "PuzzleGame Files|*.pg";
            saveFileDialog1.Title = "Save a Board";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName);
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        if (blackbuttons[i, j].black)
                            sw.Write('1');
                        else
                            sw.Write('0');
                    }
                sw.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newgamebutton = true;
            CallEnd();
            newgamebutton = false;
            openFileDialog1.Filter = "PuzzleGame Files|*.pg";
            openFileDialog1.Title = "Load a Board"; 
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sum = 0;
                for (int i = 0; i < 4; i++)
                {
                    labelnumbers3[i] = 0;
                    labelnumbers4[i] = 0;
                }
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        blackbuttons[i, j].found = false;
                        if ((char)sr.Read() == '1')
                        {
                            blackbuttons[i, j].black = true;
                            labelnumbers3[i]++;
                            labelnumbers4[j]++;
                            sum++;
                        }
                        else
                            blackbuttons[i, j].black = false;
                    }
                sr.Close();

                mistakes = 0;
                clicked = 0;
                toolStripProgressBar1.Maximum = time / 1000;
                toolStripProgressBar1.Step = -1;
                toolStripProgressBar1.Value = time / 1000;
                bw.RunWorkerAsync();
                toolStripStatusLabel2.Text = "Score: " + points;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        buttons[i, j].Enabled = true;
                        ColorButtons(i, j);
                    }
                AddNumbers();
            }
        }
    }
}

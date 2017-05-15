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
    public partial class DialogBox : Form
    {
        Form1 mainform;
        public DialogBox(Form1 _mainform)
        {
            mainform = _mainform;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainform.lives = Int32.Parse(numericUpDown1.Text);
            mainform.toolStripStatusLabel1.Text = "Lives: " + mainform.lives;
            mainform.time = Int32.Parse(numericUpDown2.Text) * 1000;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

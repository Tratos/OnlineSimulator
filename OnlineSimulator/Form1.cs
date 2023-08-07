using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


namespace OnlineSimulator
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);

        private static readonly object _sync = new object();
        private static bool _exit = false;
        private static bool _done = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripButton2.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _exit = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;
            _exit = false;
            new Thread(tMover).Start();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            lock (_sync)
            {
                _exit = true;
            }
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = false;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                _exit = true;
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        public void tMover(object obj)
        {
            Random rnd = new Random();
            Rectangle r = Screen.PrimaryScreen.Bounds;
            while (true)
            {
                lock (_sync)
                {
                    if (_exit)
                    {
                        toolStripStatusLabel2.Text = "Stop";
                        statusStrip1.BackColor = Color.Red;
                        break;
                    }
                    else
                    {
                        toolStripStatusLabel2.Text = "Run";
                        statusStrip1.BackColor = Color.Green;
                    }
                }
                int x = rnd.Next(r.Left, r.Right);
                int y = rnd.Next(r.Top, r.Bottom);
                toolStripStatusLabel5.Text = "X:" + x + ":" + "Y:" + y;
                SetCursorPos(x, y);
                keybd_event(0x90, 0x45, 1, 0);
                keybd_event(0x90, 0x45, 2, 0);
                Thread.Sleep(3000);
            }
            lock (_sync)
            {
                _done = true;
            }
        }
    }
}

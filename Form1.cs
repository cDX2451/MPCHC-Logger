using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace EventLoggingMPC5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CreateTrayIconMenu();
            this.Hide();
            notifyIcon1.Visible = true;

            ProcessLogger.Start();

        }

        private void CreateTrayIconMenu()
        {
            ContextMenu cm = new ContextMenu();

            MenuItem CloseProgram = new MenuItem();
            CloseProgram.Text = String.Format("Close {0}", this.Text);
            CloseProgram.Click += new EventHandler(CloseProgram_click);
            cm.MenuItems.Add(CloseProgram);

            notifyIcon1.ContextMenu = cm;
            notifyIcon1.MouseUp += new MouseEventHandler(notifyIcon1_MouseClick);
        }

        private void CloseProgram_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) 
        {
            // Ugh
            MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(notifyIcon1, null);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Eh
        }

    }
}

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {

        private string newExePath;
        private string currentExePath;
      
        public Form1(string[] args)
        {
            InitializeComponent();

            if (args.Length != 2 ) 
            {
                MessageBox.Show("Invalid arguments.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); 
                return;
            }
            newExePath = args[0];
            currentExePath = args[1];
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await UpdateApplicationAsync();
        }

        private async Task UpdateApplicationAsync()
        {
            try
            {
                UpdateStatus("Waiting for main application to exit...");
                await Task.Delay(5000);

                UpdateStatus("Updating application...");
                progressBar1.Style = ProgressBarStyle.Marquee;

                await Task.Run(() =>
                {
                    File.Copy(newExePath, currentExePath, true);
                });

                UpdateStatus("Starting new version...");
                Process.Start(currentExePath);

                UpdateStatus("Update complete."); 
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit();
            }
        }
        private void UpdateStatus(string message) 
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), message);
            }
            else           
            {
            label1.Text = message;
            }
        }
    }
}

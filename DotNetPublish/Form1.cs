using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNetPublish
{
    public partial class Form1 : Form
    {
        #region Initialization
        string source = "", dest = "", platform = "";
        public delegate void TextDelegate(string s, Control control);
        //when this delegate is invoked, append the string to the control and refresh
        public TextDelegate textDelegate =
            (string s, Control control) =>
             {
                 control.Text += s;
                 control.Refresh();
             };


        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            platform = comboBox1.SelectedItem.ToString();
        }
        #endregion
        
        #region Parameter Selection
        private void btnSource_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            source = folderBrowser.SelectedPath;
            label1.Text = source;

        }
        private void btnDest_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            dest = folderBrowser.SelectedPath;
            label2.Text = dest;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            platform = comboBox1.SelectedItem.ToString();
        }
        #endregion

        #region Async Processing
        /// <summary>
        /// Sets up a Process to run the dotnet publish command and send output to the TextBox asynchronously
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, EventArgs e)
        {
            label3.Text = "Processing...";
            label3.Refresh();
            textBox1.Text = "";
            textBox1.Refresh();
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(dest)) label3.Text = "Please select both a source and a destination.";
            else
            {
                Directory.SetCurrentDirectory(source);
                //Set up the process to run dotnet publish with no command line window, with stdout redirected so we can print it to the text box
                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = @"C:\Program Files\dotnet\dotnet.exe",
                        Arguments = $"publish -c Release -o {dest} -r {platform}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                //Set up event handlers for the process
                //Since the process's error handlers run on a different thread than the form's controls, the form's Invoke method must be used to interact with the controls
                process.OutputDataReceived += (sendingObj, eventArgs) => Invoke(textDelegate, new object[] { eventArgs.Data + Environment.NewLine, textBox1 });
                process.Exited += (sendingObj, eventArgs) => Invoke(textDelegate, new object[] { "Done.", label3 });
                process.EnableRaisingEvents = true; //Raise Exited event
                process.Start();
                process.BeginOutputReadLine(); //Start listening for OutputDataRecieved events
            }
        }
        #endregion

    }
}

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
        public delegate void TextDelegate(string s, Control control, Color? color);
        public TextDelegate textDelegate;
        public Form1()
        {
            InitializeComponent();
            label1.Text = label2.Text = label3.Text = textBox1.Text = "";
            textDelegate = new TextDelegate(AddData);
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
        /// 
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
                //Set up the process to run dotnet publish with no command line window, with stderr and stdout redirected so we can print them to the text box
                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = @"C:\Program Files\dotnet\dotnet.exe",
                        Arguments = $"publish -c Release -o {dest} -r {platform}",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                //Set up event handlers for the process
                process.ErrorDataReceived += (sendingObj, eventArgs) => textBox1.Invoke(textDelegate, new object[] { eventArgs.Data + Environment.NewLine, textBox1, Color.Red });
                process.OutputDataReceived += (sendingObj, eventArgs) => textBox1.Invoke(textDelegate, new object[] { eventArgs.Data + Environment.NewLine, textBox1, Color.Black });
                process.Exited += (sendingObj, eventArgs) => label3.Invoke(textDelegate, new object[] { "Done.", label3, null });
                process.EnableRaisingEvents = true;
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }
        }


        private void AddData(string s, Control control, Color? color = null)
        {
            if (color != null && control is RichTextBox)
            {
                Color co = (Color)color;
                RichTextBox c = (RichTextBox)control;

                c.SuspendLayout();
                c.SelectionColor = co;
                c.AppendText(s);
                c.ScrollToCaret();
                c.ResumeLayout();
            }
            else control.Text += s;
            control.Refresh();
        }
        #endregion

    }
}

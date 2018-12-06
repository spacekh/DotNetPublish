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
        string source = "", dest = "", platform = "";
        public delegate void TextDelegate(string s, Control control);
        public TextDelegate textDelegate;
        public Form1()
        {
            InitializeComponent();
            label1.Text = label2.Text = label3.Text = textBox1.Text = "";
            textDelegate = new TextDelegate(AddData);
            comboBox1.SelectedIndex = 0;
            platform = comboBox1.SelectedItem.ToString();
        }        

        private void btnSource_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            source = folderBrowser.SelectedPath;
            label1.Text = source;
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            platform = comboBox1.SelectedItem.ToString();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            label3.Text = "Processing...";
            label3.Refresh();
            textBox1.Text = "";
            textBox1.Refresh();
            if (String.IsNullOrWhiteSpace(source) || String.IsNullOrWhiteSpace(dest)) label3.Text = "Please select both a source and a destination.";
            else
            {
                Directory.SetCurrentDirectory(source);
                Process process = new Process(); process.StartInfo.FileName = @"C:\Program Files\dotnet\dotnet.exe";
                process.StartInfo.Arguments = $"publish -c Release -o {dest} -r {platform}";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.ErrorDataReceived += new DataReceivedEventHandler(WriteToText);
                process.OutputDataReceived += new DataReceivedEventHandler(WriteToText);
                process.Exited += Process_Exited;
                process.EnableRaisingEvents = true;
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            label3.Invoke(textDelegate, new object[] { "Done.", label3 });
        }

        private void AddData(string s, Control control)
        {
            control.Text += s;
            control.Refresh();
        }

        private void WriteToText(object sender, DataReceivedEventArgs e)
        {
            textBox1.Invoke(textDelegate, new object[] { e.Data + Environment.NewLine, textBox1 });            
        }

        private void btnDest_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            dest = folderBrowser.SelectedPath;
            label2.Text = dest;
        }
    }
}

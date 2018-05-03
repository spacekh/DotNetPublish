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
        string source = "", dest = "", platform = "win-x64";
        public Form1()
        {
            InitializeComponent();
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            textBox1.Text = "";
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
                process.Start();
                process.WaitForExit();
                textBox1.Text = process.StandardError.ReadToEnd() + "\n" + process.StandardOutput.ReadToEnd();
                label3.Text = "Done.";
            }
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

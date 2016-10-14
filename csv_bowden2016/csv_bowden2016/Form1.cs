using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace csv_bowden2016
{
    public partial class Form1 : Form
    {
        public static string strInputFile;
        public static string strOutptutFolder;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnInputBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog(); //Exception occurs here on pressing browse button, something to do with variables
            strInputFile = openFileDialog1.FileName;
            strIntputPath.Text = strInputFile;
        }

        private void btnOutputBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog SelectFolderDialog1 = new FolderBrowserDialog();
            SelectFolderDialog1.ShowDialog();
            strOutptutFolder = SelectFolderDialog1.SelectedPath;
            strOutputPath.Text = strOutptutFolder;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            Program.Convert();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace csv_bowden2016
{
    
    public partial class Form1 : Form
    {

        

        // VARIABLES FOR ACCESS IN MULTIPLE METHODS
        static string strInputFile;
        static string strOutptutFolder;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnInputBrowse_Click(object sender, EventArgs e) //BROWSE FOR INPUT FILE
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog(); //Exception occurs here on pressing browse button, something to do with variables?
            strInputFile = openFileDialog1.FileName;
            strIntputPath.Text = strInputFile;
        }

        private void btnOutputBrowse_Click(object sender, EventArgs e) //BROWSE FOR OUTPUT FODLER
        {
            FolderBrowserDialog SelectFolderDialog1 = new FolderBrowserDialog();
            SelectFolderDialog1.ShowDialog();
            strOutptutFolder = SelectFolderDialog1.SelectedPath;
            strOutputPath.Text = strOutptutFolder;
        }

        private void btnCancel_Click(object sender, EventArgs e) // CANCEL CLICK
        {
            Application.Exit();
        }

        private void btnConvert_Click(object sender, EventArgs e) //CONVERT CLICK
        {
            Convert();
        }

        class inc_claim_line //This class is used to store the incremental data for each row of the csv file
        {
            // PROPERTIES
            public string strProductName { get; set; }
            public int intOriginYr { get; set; }
            public int intDevelopYr { get; set; }
            public double dblIncVal { get; set; }
        }

        public static void Convert()
        {
            StreamWriter outputfile = new StreamWriter(strOutptutFolder + "\\outputfile.csv");

            // MAIN PRODCEDURE

            #region Step1
            //Step1 Extract the lines from the csv file and, using the split method, store each line as an inc_claim_line object

            List<inc_claim_line> Lines = new List<inc_claim_line>(); // list collection of all of the csv lines

            foreach (var row in File.ReadLines(strInputFile).Skip(1))
            {
                // Step 1.01 - split data line
                var data = row.Split(',');

                // Step 1.02 - add new claim line object to the Lines list, with data from step 1.01 
                Lines.Add(new inc_claim_line
                {
                    strProductName = data[0],
                    intOriginYr = int.Parse(data[1]),
                    intDevelopYr = int.Parse(data[2]),
                    dblIncVal = double.Parse(data[3]),
                });
            }
            #endregion

            #region Step2

            //Step 2 - find minimum and maximum origin years and maximum development period and print
            int intMinOriginYr = Lines[0].intOriginYr; //starting dummy value
            int intMaxOriginYr = Lines[0].intOriginYr;
            int intMaxDevYr = 0;

            for (int i = 0; i < Lines.Count; i++) //Loop through Lines finding the required extremes
            {
                intMinOriginYr = Math.Min(Lines[i].intOriginYr, intMinOriginYr);
                intMaxOriginYr = Math.Max(Lines[i].intOriginYr, intMaxOriginYr);
                intMaxDevYr = Math.Max((Lines[i].intDevelopYr - Lines[i].intOriginYr), intMaxDevYr);
            }

            //Print
            outputfile.Write(intMinOriginYr + ", " + (intMaxDevYr + 1)); //Add one to include the zero-th development year
            outputfile.WriteLine();

            #endregion

            #region Step3
            //Step 3 - Define a list of products

            List<string> Products = new List<string>(); //Create a list of products

            for (int i = 0; i < Lines.Count; i++) //Populate list of products from csv file
            {
                if (!Products.Contains(Lines[i].strProductName)) { Products.Add(Lines[i].strProductName); }
            }

            #endregion

            #region Step4
            //Step 4 - For each product, store incremental claims from csv in an array, convert that array to cumulative and print to file

            int intColumns = (intMaxOriginYr - intMinOriginYr + 1) * (intMaxDevYr + 1); //Controls how large the arrays need to be

            double[] dblInc_Claims = new double[intColumns]; //array to hold cumulative values
            double[] dblCum_Claims = new double[intColumns]; //array to hold cumulative values

            int intColPosition = 0;

            foreach (string product in Products)
            {
                for (int i = 0; i < Lines.Count; i++) //loop to determine the column position of the claim amount from the csv in the output array
                {
                    if (Lines[i].strProductName == product)
                    {
                        intColPosition = (Lines[i].intOriginYr - intMinOriginYr) * (intMaxDevYr + 1) + (Lines[i].intDevelopYr - Lines[i].intOriginYr);
                        dblInc_Claims[intColPosition] = Lines[i].dblIncVal;
                    }
                }

                //Convert to cumulative
                for (int i = 0; i < (intMaxOriginYr - intMinOriginYr + 1); i++)
                {
                    for (int j = 0; j <= intMaxDevYr; j++)
                    {
                        if (j == 0)
                        {
                            dblCum_Claims[((intMaxDevYr + 1) * i) + j] = dblInc_Claims[((intMaxDevYr + 1) * i) + j];
                        }
                        else
                        {
                            dblCum_Claims[((intMaxDevYr + 1) * i) + j] = dblCum_Claims[((intMaxDevYr + 1) * i) + j - 1] + dblInc_Claims[((intMaxDevYr + 1) * i) + j];
                        }
                    }
                }

                outputfile.Write(product + ",");

                for (int i = 0; i < intColumns; i++)
                {
                    outputfile.Write(dblCum_Claims[i] + ",");
                }

                outputfile.WriteLine();

            } // end of product loop

            outputfile.Flush();
            outputfile.Close();

            Array.Clear(dblInc_Claims, 0, dblInc_Claims.Length);
            Array.Clear(dblCum_Claims, 0, dblCum_Claims.Length);
                        
            Application.Exit();

        }

        #endregion
        
    }
}

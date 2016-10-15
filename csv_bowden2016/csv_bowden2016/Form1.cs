using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

/*
    Author = Jonathan Bowden
    Initial build = 15 Oct 2016
    Purpose = Asks the user for a incremental claims csv file and creates an output file, creating cumulative data in triangle form.
    Description = After the user has specified the file and output folder, the following steps are performed:
        (1) Each claim line of the input csv file is split by comma and has each data item stored in an "inc_claim_line" object. 
        (2) The minimum and maximum origin years are found. From this the maximum development year is calculated and this is output 
            as the first line of the output file.
        (3) A list of products is made to determine how many different product loops are required
        (4) Various arrays and parameters are set up, including a dictionary that stores each years starting development year (year-0)
            in the main incremental and cumulative claims arrays
        (5) Each line of the csv file has its incremental claim value added to an array. This array is then converted into cumulative
            data. This is done at the product level and printed to the file.
*/

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
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.ShowDialog();
                strInputFile = openFileDialog1.FileName;
                strIntputPath.Text = strInputFile;
            }
            catch (Exception) { }
        }

        private void btnOutputBrowse_Click(object sender, EventArgs e) //BROWSE FOR OUTPUT FODLER
        {
            try
            {
                FolderBrowserDialog SelectFolderDialog1 = new FolderBrowserDialog();
                SelectFolderDialog1.ShowDialog();
                strOutptutFolder = SelectFolderDialog1.SelectedPath;
                strOutputPath.Text = strOutptutFolder;
            }
            catch (Exception) { };
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
            StreamWriter outputfile = new StreamWriter(strOutptutFolder + "\\outputfile" 
                                                + DateTime.Now.ToString("ddMMyy-HHmmss") + ".csv");

            // MAIN PRODCEDURE
            #region Step1
            //Step1 Extract the lines from the csv file and, using the split method, store each line as an inc_claim_line object
            List<inc_claim_line> Lines = new List<inc_claim_line>(); // list collection of all of the csv lines

            foreach (var row in File.ReadLines(strInputFile).Skip(1))
            {
                string[] data = row.Split(',');

                try
                {
                    if (data.Count() != 4) { throw new ArgumentNullException(); } //Throw exception if there are more than 4 subsrtings

                    Lines.Add(new inc_claim_line //Add the csv split data to an inc_claim_line object
                    {
                        strProductName = data[0],
                        intOriginYr = int.Parse(data[1]),
                        intDevelopYr = int.Parse(data[2]),
                        dblIncVal = double.Parse(data[3]),
                    });
                }
                catch (Exception) //Catches improper data formats and more than 4 substrings in each line
                {
                    MessageBox.Show(
                        null,
                        "The input file data is not in the correct format. Please clean the data and retry.",
                        null,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            #endregion

            #region Step2

            //Step 2 - find minimum and maximum origin years and maximum development period and print
            int intMinOriginYr = Lines[0].intOriginYr; 
            int intMaxOriginYr = Lines[0].intOriginYr; 
            int intMaxDevYr = 0;

            for (int i = 0; i < Lines.Count; i++) //Loop through Lines finding the required extremes
            {
                intMinOriginYr = Math.Min(Lines[i].intOriginYr, intMinOriginYr);
                intMaxOriginYr = Math.Max(Lines[i].intOriginYr, intMaxOriginYr);
            }

            for (int i = 0; i < Lines.Count; i++) //Loop to find max dev year, separate from above because intMinOriginYr is needed
            {
                intMaxDevYr = Math.Max(Lines[i].intDevelopYr - intMinOriginYr, intMaxDevYr);
            }

            outputfile.Write(intMinOriginYr + ", " + (intMaxDevYr + 1));
            outputfile.WriteLine();

            #endregion

            #region Step3
            //Step 3 - Define a list of products
            List<string> Products = new List<string>();

            for (int i = 0; i < Lines.Count; i++) //Populate list of products from csv file
            {
                if (!Products.Contains(Lines[i].strProductName)) { Products.Add(Lines[i].strProductName); }
            }

            #endregion

            #region Step4
            //Step 4 - Set up arrays and indicator variables
            int intColumns = 0;

            // Calculate how large the claim arrays need to be: Add a decreasing number to the column count, starting at max dev year, until the latest year
            for (int i = intMaxDevYr + 1 ; i > 0; i--)
            {
                intColumns += i;
            }

            double[] dblInc_Claims = new double[intColumns]; //array to hold incremental values
            double[] dblCml_Claims = new double[intColumns]; //array to hold cumulative values

            // Dictionary to store the array starting positions of each origin year (ie where dev year = 0)
            Dictionary<int, int> yearstartingpos = new Dictionary<int, int>(); 
            yearstartingpos.Add(intMinOriginYr, 0); // Add the initial zero value

            // Loop for each origin year (except the first) and assign its starting column position to the yearstartingpos dictionary
            for (int i = 1; i < (intMaxDevYr+1); i++)
            {
                yearstartingpos.Add(intMinOriginYr + i, yearstartingpos[(intMinOriginYr + i - 1)] + (intMaxDevYr + 1) - i + 1);
            }
            #endregion

            #region Step5
            //Step 5 - For each product, store incremental claims from csv in an array, convert that array to cumulative and print to file
            int intColPosition = 0;
            
            foreach (string product in Products)
            {
                for (int i = 0; i < Lines.Count; i++) //loop to determine the column position of the claim amount from the csv in the output array
                {
                    if (Lines[i].strProductName == product)
                    {
                        // Add the development year to the starting column position dictionary for where the claim amount should be in the array
                        intColPosition = yearstartingpos[Lines[i].intOriginYr] + (Lines[i].intDevelopYr - Lines[i].intOriginYr); 
                        dblInc_Claims[intColPosition] = Lines[i].dblIncVal;
                    }
                }

                int j = intMaxDevYr + 1; // controls how many dev years to run for when converting to cumulative, start at maximum number
                
                //Convert to cumulative
                foreach (var yearpos in yearstartingpos.Values)
                {
                    for (int k = 0; k < j; k++)
                    {
                        if(k == 0)
                        {
                            dblCml_Claims[yearpos] = dblInc_Claims[yearpos];
                        }
                        else
                        {
                            dblCml_Claims[yearpos + k] = dblCml_Claims[yearpos + k - 1] + dblInc_Claims[yearpos + k];
                        }
                        
                    }
                    j--;
                }

                //Print to file
                outputfile.Write(product + ",");

                for (int i = 0; i < intColumns; i++)
                {
                    outputfile.Write(dblCml_Claims[i] + ",");
                }

                outputfile.WriteLine();

                //Clear arrays for each product
                Array.Clear(dblInc_Claims, 0, dblInc_Claims.Length);
                Array.Clear(dblCml_Claims, 0, dblCml_Claims.Length);

            } // end of product loop
            #endregion

            MessageBox.Show(null, "Procedure complete successfully", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
            outputfile.Close();
            Environment.Exit(1);
        }
    }
}
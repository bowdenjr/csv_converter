using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace csv_converter_bowden2016
{
    class Program
    {
        #region Classes
        class inc_claim_line //This class is used to store the incremental data for each row of the csv file
        {
            //PROPERTIES
            public string strProductName { get; set; }
            public int intOriginYr { get; set; }
            public int intDevelopYr { get; set; }
            public double dblIncVal { get; set; }
        }

        #endregion

        static void Main()
        {
            // MAIN VARIABLES                

            string strFilename = "D:\\inputfile.csv"; // change this accordingly

            List<inc_claim_line> Lines = new List<inc_claim_line>(); // list collection of all of the csv lines

            // MAIN PRODCEDURE

            #region Step1
            //Step1 Extract the lines from the csv file and, using the split method, store each line as an inc_claim_line object
            foreach (var row in File.ReadLines(strFilename).Skip(1))
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

            //Step 2 - find minimum and maximum origin years and maximum development period
            int intMinOriginYr = Lines[0].intOriginYr; //starting dummy value
            int intMaxOriginYr = Lines[0].intOriginYr;
            int intMaxDevYr = 0; 

            for (int i = 0; i < Lines.Count; i++) //Loop through Lines finding the required extremes
            {
                intMinOriginYr = Math.Min(Lines[i].intOriginYr, intMinOriginYr);
                intMaxOriginYr = Math.Max(Lines[i].intOriginYr, intMaxOriginYr);
                intMaxDevYr = Math.Max((Lines[i].intDevelopYr - Lines[i].intOriginYr), intMaxDevYr);
            }

            #endregion

            List<string> Products = new List<string>(); //Create a list of products

            for (int i = 0; i < Lines.Count; i++)
            {
                if (!Products.Contains(Lines[i].strProductName)) { Products.Add(Lines[i].strProductName); }
            }

            Console.Write(intMinOriginYr + ", " + (intMaxDevYr + 1)); //Add one to include the zero-th development year
            Console.WriteLine();

            #region Step3

            //Step 3 - For each product, make cumulative data 

            for (int i = 0; i < Products.Count; i++)
            {
                double[] cumulative_claims = new double[(intMaxDevYr + 1) * (intMaxOriginYr - intMinOriginYr + 1)]; //array to hold cumulative values
                for(int j = 0; j < Lines.Count; j++)
                {
                    if (Lines[i].strProductName == Products[i])
                    {
                        cumulative_claims[] = 

                    }
                }

                Console.Write(Products[i] + ", " + cumulative_claims);
                Console.WriteLine();

            }
            

            #endregion  


            Console.ReadKey();

        }

        
    }
}

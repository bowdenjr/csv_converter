using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csv_bowden2016
{
    static class Program
    {
        [STAThread]


        static void Main() //Default code from Visual Studio to get form to run
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }     
}

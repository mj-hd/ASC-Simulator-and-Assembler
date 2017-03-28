/*
 * Program.cs
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UI.GUI ui;
            Machine.ASC machine = new Machine.ASC();

            // GUI
            ui = new UI.GUI();

            ui.Machine = machine;

            if (args.GetLength(0) > 0)
            {
                ui.OpenFile("Y:\\Sync\\Projects\\asc-assembler-win\\TEST2.asco");
            }


            try
            {
              ui.ShowWindow();
            }
            catch (Exception e)
            {
                ui.ShowError(e.Message);
            }
        }
    }
}

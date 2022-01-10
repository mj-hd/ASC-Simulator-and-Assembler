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

            Machine.ASC machine = new Machine.ASC();

            // GUI
            using (UI.GUI ui = new UI.GUI())
            {

                ui.Machine = machine;

                try
                {
                    if (args.Length > 0)
                    {
                        ui.SetFile(args[0]);
                    }

                    ui.ShowWindow();
                }
                catch (Exception e)
                {
                    ui.ShowError(e.Message);
                }
            }
        }
    }
}

/*
 * HelpForm.cs
 * 使い方フォームを表示する
 * 
 */
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
using System.Reflection;

namespace Simulator.UI.Forms
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            var pathToHelp = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "resources\\help.html");
           
            this.webBrowser.Navigate(new Uri(pathToHelp));

        }

    }
}

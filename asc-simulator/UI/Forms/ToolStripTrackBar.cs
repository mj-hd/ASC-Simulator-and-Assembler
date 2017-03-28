/*
 * ToolStripTrackBar.cs
 * ToolStrip内に配置できるTrackBar
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator.UI.Forms
{
    public class ToolStripTrackBar : ToolStripControlHost
    {
        public ToolStripTrackBar()
            : base(new TrackBar())
        {
        }

        public TrackBar TrackBar
        {
            get
            {
                return (TrackBar)this.Control;
            }
        }
    }
}

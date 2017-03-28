/*
 * ApplicationSettings.cs
 * 設定を保持するクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Common
{
    // シリアライズして設定ファイルとして保存できるように
    [Serializable()]
    public class ApplicationSettings
    {
        // デフォルトの値
        public int MachineSpeed = 50; // %
        public int MachineBaseSpeed = 2000;  // ms
        public bool MachineShouldInitializeWithStartup = false;

        public int MMViewAddressWidth = 50; // pixel
        public int MMViewMnemonicWidth = 100;
        public int MMViewScrollPositionFromTop = 4;

        public int RegistersViewNameWidth = 70;
        public string RegisterColorNormal = "#000000";
        public string RegisterColorReference = "#329632";
        public string RegisterColorModify = "#cd7c32";

        public int RegisterMargin = 30;

        public string ConnectorColorNormal = "#c8c8c8";
        public string ConnectorColorActive = "#329632";
        public int ConnectorWidth = 4;

        public string SimulatorCurrentPath = ".\\";
    }
}

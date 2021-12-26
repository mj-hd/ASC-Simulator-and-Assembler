/*
 * Misc.cs
 * その他のasc-simulator、MachineDisplay間で共有されるクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Common
{
    // マシンの実行状態を知らせるイベントハンドラ
    public delegate void CycleEventHandler(CycleEventArgs ce);

    public class CycleEventArgs
    {
        public CycleEventArgs(uint instrCount)
        {
            this.InstructionCount = instrCount;
        }

        public uint InstructionCount;
    }

    // メモリの参照・書換イベントに使われる引数
    public class MemoryEventArgs
    {
        // 参照・書換の開始アドレス
        public ushort StartAddress { get { return this._StartAddress; } }
        // 〃 終了アドレス
        public ushort EndAddress { get { return this._EndAddress; } }
        // 実際に書き換えられたメモリの値
        public ushort[] Memory { get { return this._Value;  } }
        // リセットされたかどうか（リセットの場合はその他の値は0を差し、内容すべてが消されたことを表す）
        public bool Reset { get { return this._Reset; } }

        public MemoryEventArgs(ushort startAddress, ushort endAddress, ushort[] memory, bool reset = false)
        {
            this._StartAddress = startAddress;
            this._EndAddress = endAddress;
            this._Value = memory;
            this._Reset = reset;
        }

        private ushort _StartAddress;
        private ushort _EndAddress;
        private ushort[] _Value;
        private bool _Reset;
    }
    public delegate void MemoryEventHandler(MemoryEventArgs me);

    // レジスタの参照・書換イベントに使われる引数
    public class DataEventArgs
    {
        // 書き換えられた値
        public Object Data;
        // 書き換えられたレジスタ
        public String Key;

        public DataEventArgs(Object data, String key)
        {
            this.Data = data;
            this.Key = key;
        }
    }
    public delegate void DataEventHandler(DataEventArgs de);

    // レジスタ間の値の移動を知らせるイベントに使われる引数
    public class DataMovedEventArgs
    {
        // 送り手
        public string Sender;
        // 受け手
        public string Reciever;

        public DataMovedEventArgs(string sender, string reciever) 
        {
            this.Sender = sender;
            this.Reciever = reciever;
        }
    }
    public delegate void DataMovedEventHandler(DataMovedEventArgs dme);

    // ALUのオーバフロー時のイベントに使われる引数
    public class OverflowedEventArgs
    {
        // 発生した演算
        public string Operator;

        public OverflowedEventArgs(string ope)
        {
            this.Operator = ope;
        }
    }
    public delegate void OverflowedEventHandler(OverflowedEventArgs ove);

}

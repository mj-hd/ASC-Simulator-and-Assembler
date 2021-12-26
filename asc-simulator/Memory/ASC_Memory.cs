/*
 * ASC_Memory.cs
 * ASCのメモリをシミュレートするクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Simulator.Common;

namespace Simulator.Memory
{
    public class ASC_Memory
    {
        // メモリが変更された、参照されたときに発生するイベント
        public event MemoryEventHandler MemoryAccessed;
        public event MemoryEventHandler MemoryChanged;

        // メモリ変更・参照時のイベントを発生させるか否か
        public bool EnableEvents = true;

        // メモリの最大番地
        public const ushort MaxSize = 4095; // 0x111111111111番地まで

        // メモリの内容を参照・書換
        // i: 参照・書換したいアドレス
        public ushort this[ushort i]
        {
            get
            {
                ushort result = this._GetValue(i);
                this.OnMemoryAccessed(new MemoryEventArgs(i, i, this._MM));
                return result;
            }
            set
            {                
                this._SetValue(i, value);
                this.OnMemoryChanged(new MemoryEventArgs(i, i, this._MM));
            }
        }


        public ASC_Memory()
        {
            this.MemoryAccessed = (me) => { };
            this.MemoryChanged = (me) => { };
            Init();
        }
        ~ASC_Memory()
        {
        }


        // メモリを初期化する
        public void Init()
        {
            // メモリを1ブロック確保
            this._MM = new ushort[_BlockSize];
            // 確保したメモリサイズを記録
            this._AllocatedSize = _BlockSize;

            // メモリResetの変更のイベント発生させる
            this.OnMemoryChanged(new MemoryEventArgs(0, 0, this._MM, true));
        }

        // BinaryReaderからメモリへ読み込む
        // r:   読み込むバイナリの入ったBinaryReader
        // org: 読み込みを開始する位置
        public void Load(BinaryReader r, ushort org)
        {
            int i = 0;

            // orgから順に書き換える
            while (r.BaseStream.Length != r.BaseStream.Position)
            {
                this._SetValue((ushort)(org + i), r.ReadUInt16());
                i++;
            }

            this.OnMemoryChanged(new MemoryEventArgs(org, (ushort)(org + i-1), this._MM));
        }

        // BinaryWriterにメモリの内容を書き出す
        // w: 書き込み先のBinaryWriter
        // ヘッダの作成は行わない
        public void Save(BinaryWriter w)
        {
            for (ushort i = 0; i < this._AllocatedSize; i++)
            {
                w.Write(this._GetValue(i));
            }

            this.OnMemoryAccessed(new MemoryEventArgs(0, this._AllocatedSize, this._MM));
        }


        #region イベント

        protected void OnMemoryChanged(MemoryEventArgs me)
        {
            if (this.EnableEvents)
            {
                this.MemoryChanged(me);
            }
        }
        protected void OnMemoryAccessed(MemoryEventArgs me)
        {
            if (this.EnableEvents)
            {
                this.MemoryAccessed(me);
            }
        }

        #endregion

        #region プライベート

        private ushort[] _MM;
        private ushort _AllocatedSize;
        private const ushort _BlockSize = 16; // 一度に確保するメモリサイズ

        // メモリの内容を読み出す
        // 同時に、確保していない領域を指定した場合に追加で確保をする
        private ushort _GetValue(ushort address)
        {
            // addressが確保していない領域だったら、確保をする
            this._CheckAndAllocate(address);

            return this._MM[address];
        }
        private void _SetValue(ushort address, ushort value)
        {
            this._CheckAndAllocate(address);

            this._MM[address] = value;
        }

        // 指定したアドレスが確保されているか確認し、
        // されていなければ確保する
        private void _CheckAndAllocate(ushort address)
        {
            // 確保されていないアドレスのとき
            if (address > this._AllocatedSize-1)
            {
                // 必要となるブロック数
                int newBlockNum = address / ASC_Memory._BlockSize +1;

                // もし、メモリの最大容量を超えたとき
                if ((newBlockNum * ASC_Memory._BlockSize) > ASC_Memory.MaxSize+1)
                {
                    throw new Exception("メモリの容量を超えました。");
                }

                // this._MMのサイズを変更
                Array.Resize<ushort>(ref this._MM, newBlockNum * ASC_Memory._BlockSize);
                // 確保済みのサイズを更新
                this._AllocatedSize = (ushort)(newBlockNum * ASC_Memory._BlockSize);
            }
        }

        #endregion
    }
}

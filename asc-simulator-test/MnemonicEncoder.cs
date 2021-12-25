using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator.Machine;

namespace asc_simulator_test
{
    internal class MnemonicEncoder
    {
        public MnemonicEncoder() { }

        public ushort[] GetUShorts(ASC.ILoadable[] data)
        {
            var results = new ushort[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                var mnemonic = data[i];
                results[i] = mnemonic.ToUShort();
            }

            return results;
        }

        public byte[] GetBytes(ASC.ILoadable[] data)
        {
            var ushorts = this.GetUShorts(data);

            var results = new byte[ushorts.Length * 2];

            for (int i = 0; i < ushorts.Length; i++)
            {
                var j = i * 2;
                var u = ushorts[i];
                results[j] = (byte)(u & 0xFF);
                results[j + 1] = (byte)(u >> 8);
            }

            return results;
        }
    }
}

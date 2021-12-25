using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Simulator.Machine;

namespace asc_simulator_test
{
    internal class Utils
    {
        public class TestASC
        {
            public struct CONST: ASC.ILoadable
            {
                public ushort Value;

                public ushort ToUShort()
                {
                    return this.Value;
                }
            }

            public TestASC(ASC machine)
            {
                this.machine = machine;
            }

            public static async Task<TestASC> New(ASC.ILoadable[] data)
            {
                var encoder = new MnemonicEncoder();
                var bytes = encoder.GetBytes(data);
                var stream = new MemoryStream(bytes);
                var reader = new BinaryReader(stream);

                var machine = new ASC();
                var testASC = new TestASC(machine);

                machine.Memory.Load(reader, 0x0000);

                machine.Stepped += () => {
                    Console.WriteLine("STEPPED");
                    machine.HLT();
                };

                machine.ALU.AssignZTrue += () => {
                    Console.WriteLine("ALU Z=true");
                };
                machine.ALU.AssignZFalse += () => {
                    Console.WriteLine("ALU Z=false");
                };
                machine.ALU.AssignNTrue += () => {
                    Console.WriteLine("ALU N=true");
                };
                machine.ALU.AssignNFalse += () =>
                {
                    Console.WriteLine("ALU N=false");
                };
                machine.ALU.Overflowed += (ove) =>
                {
                    testASC._overflowed = true;
                    Console.WriteLine("ALU OVERFLOWED {0}", ove.Operator);
                };
                machine.ALU.DataAccessed += (de) => {
                    Console.WriteLine("ALU DATA ACCESSED {0}({1:X})", de.Key, de.Data);
                };
                machine.ALU.DataChanged += (de) => {
                    Console.WriteLine("ALU DATA CHANGED {0}({1:X})", de.Key, de.Data);
                };
                machine.Registers.DataAccessed += (de) => {
                    Console.WriteLine("REGISTERS DATA ACCESSED {0}({1:X})", de.Key, de.Data);
                };
                machine.Registers.DataChanged += (de) => {
                    Console.WriteLine("REGISTERS DATA CHANGED {0}({1:X})", de.Key, de.Data);
                };
                machine.Memory.MemoryAccessed += (me) => {
                    Console.WriteLine("MEMORY ACCESSED {0:X}~{1:X}({2})", me.StartAddress, me.EndAddress, me.Memory);
                };
                machine.Memory.MemoryChanged += (me) => {
                    Console.WriteLine("MEMORY CHANGED {0:X}~{1:X}({2})", me.StartAddress, me.EndAddress, me.Memory);
                };
                machine.CycleBegin += () => {
                    Console.WriteLine("CYCLE BEGIN");
                };
                machine.CycleEnd += () => {
                    Console.WriteLine("CYCLE END");
                };
                machine.CycleDecode += () => {
                    Console.WriteLine("CYCLE DECODE");
                };
                machine.CycleUpdateIR += () => {
                    Console.WriteLine("CYCLE UPDATE IR");
                };
                machine.CycleOpecode += () => {
                    Console.WriteLine("CYCLE OPECODE");
                };
                machine.CycleUpdatePC += () => { 
                    Console.WriteLine("CYCLE UPDATE PC");
                };
                machine.PreDataMoved += (dme) => {
                    Console.WriteLine("PRE-DATA MOVED {0} => {1}", dme.Sender, dme.Reciever);
                };
                machine.DataMoved += (dme) => {
                    Console.WriteLine("DATA MOVED {0} => {1}", dme.Sender, dme.Reciever);
                };

                var tcs = new TaskCompletionSource<object>();

                CycleEventHandler listener = () =>
                {
                    tcs.SetResult(null);
                };

                machine.CycleBegin += listener;

                await tcs.Task;

                machine.CycleBegin -= listener;

                return testASC;
            }

            public ASC machine;

            bool _overflowed;
            public bool overflowed { get { return this._overflowed; } }

            public async Task RunCycleAsync() {
                this._overflowed = false;

                var tcs = new TaskCompletionSource<object>();

                CycleEventHandler listener = () =>
                {
                    tcs.SetResult(null);
                };

                this.machine.Stepped += listener;

                this.machine.RunCycle();

                await tcs.Task;

                this.machine.Stepped -= listener;
            }
        }
    }
}

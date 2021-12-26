using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.Machine;

namespace asc_simulator_test
{
    [TestClass]
    public class MachineTest
    {
        [TestMethod]
        public void TestInit()
        {
            using (var machine = new ASC())
            {
                machine.Init();

                Assert.AreEqual(machine.Registers.R, 0x0000);
                Assert.AreEqual(machine.Registers.PC, 0x0000);
                Assert.AreEqual(machine.Registers.IR, 0x0000);
                Assert.AreEqual(machine.Registers.MAR, 0x0000);
                Assert.AreEqual(machine.Registers.N, false);
                Assert.AreEqual(machine.Registers.Z, false);
            }
        }

        [TestMethod]
        public async Task TestBranchInstr()
        {
            var dummy = new ASC.MNEMONIC
            {
                Opecode = Simulator.Common.Defines.OPECODE.OPE6,
                Operand = 0,
            };

            var expected = new ASC.MNEMONIC
            {
                Opecode = Simulator.Common.Defines.OPECODE.OPE5,
                Operand = 0,
            };

            using (var testASC = await Utils.TestASC.New(new ASC.ILoadable[] {
                // B 0x003
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.B,
                    Operand = 0x003,
                },

                new Utils.TestASC.CONST {
                    Value = 0x0000,
                },
                new Utils.TestASC.CONST
                {
                    Value = 0x0001,
                },

                // BZ 0x006
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x001, // => DC 0x0000
                },
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.BZ,
                    Operand = 0x006,
                },

                dummy,

                // BN 0x009
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.SUB,
                    Operand = 0x002, // => DC 0x0001
                },
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.BN,
                    Operand = 0x009,
                },

                dummy,

                expected,
            }))
            {

                // B 0x003
                await testASC.RunCycleAsync();

                Assert.AreEqual(0, testASC.machine.Registers.R);
                Assert.AreEqual(0x0003, testASC.machine.Registers.PC);
                Assert.AreEqual(0x0003, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);

                // BZ 0x006
                await testASC.RunCycleAsync();
                await testASC.RunCycleAsync();

                Assert.AreEqual(0, testASC.machine.Registers.R);
                Assert.AreEqual(0x0006, testASC.machine.Registers.PC);
                Assert.AreEqual(0x0006, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);

                // BN 0x009
                await testASC.RunCycleAsync();
                await testASC.RunCycleAsync();

                unchecked
                {
                    Assert.AreEqual((ushort)-1, testASC.machine.Registers.R);
                }
                Assert.AreEqual(0x0009, testASC.machine.Registers.PC);
                Assert.AreEqual(0x0009, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N);

                await testASC.RunCycleAsync();

                unchecked
                {
                    Assert.AreEqual((ushort)-1, testASC.machine.Registers.R);
                }
                Assert.AreEqual(0x000A, testASC.machine.Registers.PC);
                Assert.AreEqual(expected.ToUShort(), testASC.machine.Registers.IR);
                Assert.AreEqual(0x0000, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N);
            }
        }

        [TestMethod]
        public async Task TestALUInstr()
        {
            using (var testASC = await Utils.TestASC.New(new ASC.ILoadable[] {
                // B 0x006
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.B,
                    Operand = 0x006,
                },

                new Utils.TestASC.CONST {
                    Value = 0x0000,
                },
                new Utils.TestASC.CONST
                {
                    Value = 0x0001,
                },
                new Utils.TestASC.CONST
                {
                    Value = 0xFFFF,
                },
                new Utils.TestASC.CONST
                {
                    Value = 0x7FFF,
                },
                new Utils.TestASC.CONST
                {
                    Value = 0x8001,
                },

                // ADD 0x001 (R = R + 0 = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x001,
                },

                // ADD 0x002 (R = R + 1 = 1)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x002,
                },

                // ADD 0x003 (R = R + (-1) = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x003,
                },

                // ADD 0x003 (R = R + (-1) = -1)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x003,
                },

                // ADD 0x002 (R = R + 1 = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x002,
                },

                // ADD 0x004 (R = R + 0x7FFF = 0x7FFF)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x004,
                },

                // ADD 0x004 (R = R + 0x7FFF = 0xFFFE)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x004,
                },

                // ADD 0x005 (R = R + 0x8001 = 0x7FFF)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ADD,
                    Operand = 0x005,
                },

                // SUB 0x004 (R = R - 0x7FFF = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.SUB,
                    Operand = 0x004,
                },

                // SUB 0x002 (R = R - 1 = -1)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.SUB,
                    Operand = 0x002,
                },

                // SUB 0x003 (R = R - (-1) = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.SUB,
                    Operand = 0x003,
                },

                // SUB 0x005 (R = R - 0x8001 = 0x7FFF)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.SUB,
                    Operand = 0x005,
                },

                // SUB 0x005 (R = R - 0x8001 = 0xFFFE)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.SUB,
                    Operand = 0x005,
                },

                // AND 0x001 (R = R & 0 = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.AND,
                    Operand = 0x001,
                },

                // OR 0x002 (R = R | 1 = 1)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.OR,
                    Operand = 0x002,
                },

                // AND 0x002 (R = R & 1 = 1)
                new ASC.MNEMONIC {
                    Opecode = Simulator.Common.Defines.OPECODE.AND,
                    Operand = 0x002,
                },

                // OR 0x002 (R = R | 1 = 1)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.OR,
                    Operand = 0x002,
                },

                // OR 0x005 (R = R | 0x8001 = 0x8001)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.OR,
                    Operand = 0x005,
                },

                // AND 0x005 (R = R & 0x8001 = 0x8001)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.AND,
                    Operand = 0x005,
                }
            }))
            {

                // B 0x0006
                await testASC.RunCycleAsync();

                // ADD 0x001 (R = R + 0 = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0, testASC.machine.Registers.R);
                Assert.AreEqual(0x0001, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // ADD 0x002 (R = R + 1 = 1)
                await testASC.RunCycleAsync();

                Assert.AreEqual(1, testASC.machine.Registers.R);
                Assert.AreEqual(0x0002, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // ADD 0x003 (R = R + (-1) = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0, testASC.machine.Registers.R);
                Assert.AreEqual(0x0003, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // ADD 0x003 (R = R + (-1) = -1)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xFFFF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0003, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // ADD 0x002 (R = R + 1 = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0000, testASC.machine.Registers.R);
                Assert.AreEqual(0x0002, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // ADD 0x004 (R = R + 0x7FFF = 0x7FFF)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x7FFF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0004, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // ADD 0x004 (R = R + 0x7FFF = 0xFFFE)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xFFFE, testASC.machine.Registers.R);
                Assert.AreEqual(0x0004, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N);
                Assert.AreEqual(true, testASC.overflowed);

                // ADD 0x005 (R = R + 0x8001 = 0x7FFF)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x7FFF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0005, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(true, testASC.overflowed);

                // SUB 0x004 (R = R - 0x7FFF = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0000, testASC.machine.Registers.R);
                Assert.AreEqual(0x0004, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // SUB 0x002 (R = R - 1 = -1)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xFFFF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0002, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // SUB 0x003 (R = R - (-1) = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0000, testASC.machine.Registers.R);
                Assert.AreEqual(0x0003, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // SUB 0x005 (R = R - 0x8001 = 0x7FFF)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x7FFF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0005, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(false, testASC.machine.Registers.N);
                Assert.AreEqual(false, testASC.overflowed);

                // SUB 0x005 (R = R - 0x8001 = 0xFFFE)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xFFFE, testASC.machine.Registers.R);
                Assert.AreEqual(0x0005, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N);
                Assert.AreEqual(true, testASC.overflowed);

                // AND 0x001 (R = R & 0 = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0000, testASC.machine.Registers.R);
                Assert.AreEqual(0x001, testASC.machine.Registers.MAR);
                Assert.AreEqual(true, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N); // not affected
                Assert.AreEqual(false, testASC.overflowed);

                // OR 0x002 (R = R & 1 = 1)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0001, testASC.machine.Registers.R);
                Assert.AreEqual(0x002, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N); // not affected
                Assert.AreEqual(false, testASC.overflowed);

                // AND 0x002 (R = R & 1 = 1)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0001, testASC.machine.Registers.R);
                Assert.AreEqual(0x002, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N); // not affected
                Assert.AreEqual(false, testASC.overflowed);

                // OR 0x002 (R = R & 1 = 1)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0001, testASC.machine.Registers.R);
                Assert.AreEqual(0x002, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N); // not affected
                Assert.AreEqual(false, testASC.overflowed);

                // OR 0x005 (R = R | 0x8001 = 0x8001)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x8001, testASC.machine.Registers.R);
                Assert.AreEqual(0x005, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N); // not affected
                Assert.AreEqual(false, testASC.overflowed);

                // AND 0x005 (R = R | 0x8001 = 0x8001)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x8001, testASC.machine.Registers.R);
                Assert.AreEqual(0x005, testASC.machine.Registers.MAR);
                Assert.AreEqual(false, testASC.machine.Registers.Z);
                Assert.AreEqual(true, testASC.machine.Registers.N); // not affected
                Assert.AreEqual(false, testASC.overflowed);
            }
        }

        [TestMethod]
        public async Task TestMemory()
        {
            using (var testASC = await Utils.TestASC.New(new ASC.ILoadable[] {
                // B 0x003
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.B,
                    Operand = 0x003,
                },

                new Utils.TestASC.CONST
                {
                    Value = 0x0000,
                },
                new Utils.TestASC.CONST
                {
                    Value = 0xBEEF,
                },

                // LD 0x002 (R = 0xBEEF)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.LD,
                    Operand = 0x002,
                },

                // ST 0x100 ((0x100) = 0xBEEF)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.ST,
                    Operand = 0x100,
                },

                // LD 0x001 (R = 0)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.LD,
                    Operand = 0x001,
                },

                // LD 0x100 (R = (0x100) = 0xBEEF)
                new ASC.MNEMONIC
                {
                    Opecode = Simulator.Common.Defines.OPECODE.LD,
                    Operand = 0x100,
                },
            }))
            {

                // B 0x0003
                await testASC.RunCycleAsync();

                // LD 0x002 (R = 0xBEEF)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xBEEF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0002, testASC.machine.Registers.MAR);

                // ST 0x100 ((0x100) = 0xBEEF)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xBEEF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0100, testASC.machine.Registers.MAR);
                Assert.AreEqual(0xBEEF, testASC.machine.Memory[0x0100]);

                // LD 0x001 (R = 0)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0x0000, testASC.machine.Registers.R);
                Assert.AreEqual(0x0001, testASC.machine.Registers.MAR);

                // LD 0x100 (R = (0x100) = 0xBEEF)
                await testASC.RunCycleAsync();

                Assert.AreEqual(0xBEEF, testASC.machine.Registers.R);
                Assert.AreEqual(0x0100, testASC.machine.Registers.MAR);
            }
        }
    }
}

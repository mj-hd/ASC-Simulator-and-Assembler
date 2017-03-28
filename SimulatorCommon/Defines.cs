/*
 * Defines.cs
 * assemblerと共通の構造体、関数などを定義するクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Common
{
    public static class Defines {
        // 対応しているオペコード列挙体
        public enum OPECODE
        {
            UNKNOWN = -1,
            LD = 0,
            ST,
            ADD,
            SUB,
            AND,
            OR,
            B,
            BZ,
            BN,
            OPE1,
            OPE2,
            OPE3,
            OPE4,
            OPE5,
            OPE6,
            HLT
        }
     
        // 数値へ変換
        public static ushort ToDecimal(OPECODE code) {
            return (ushort)code;
        }
        public static ushort ToDecimal(String str) {
            return ToDecimal(ToOPECODE(str));
        }
     
        // OPECODE列挙体へ変換
        public static OPECODE ToOPECODE(String str) {
            switch (str) {
                case "LD":
                    return OPECODE.LD;
                case "ST":
                    return OPECODE.ST;
                case "ADD":
                    return OPECODE.ADD;
                case "SUB":
                    return OPECODE.SUB;
                case "AND":
                    return OPECODE.AND;
                case "OR":
                    return OPECODE.OR;
                case "B":
                    return OPECODE.B;
                case "BZ":
                    return OPECODE.BZ;
                case "BN":
                    return OPECODE.BN;
                case "OPE1":
                    return OPECODE.OPE1;
                case "OPE2":
                    return OPECODE.OPE2;
                case "OPE3":
                    return OPECODE.OPE3;
                case "OPE4":
                    return OPECODE.OPE4;
                case "OPE5":
                    return OPECODE.OPE5;
                case "OPE6":
                    return OPECODE.OPE6;
                case "HLT":
                    return OPECODE.HLT;
                default:
                    return OPECODE.UNKNOWN;
            }
        }
        public static OPECODE ToOPECODE(int code) {
            return (OPECODE)code;
        }
     
        // 文字列へ変換
        public static String ToString(OPECODE code) {
            switch (code) {
                case OPECODE.LD:
                  return "LD";
                case OPECODE.ST:
                  return "ST";
                  
                case OPECODE.ADD:
                  return "ADD";
                  
                case OPECODE.SUB:
                  return "SUB";
                  
                case OPECODE.AND:
                  return "AND";
                  
                case OPECODE.OR:
                  return "OR";
                  
                case OPECODE.B:
                  return "B";
                  
                case OPECODE.BZ:
                  return "BZ";
                  
                case OPECODE.BN:
                  return "BN";
                  
                case OPECODE.OPE1:
                  return "OPE1";
                  
                case OPECODE.OPE2:
                  return "OPE2";
                  
                case OPECODE.OPE3:
                  return "OPE3";
                  
                case OPECODE.OPE4:
                  return "OPE4";
                  
                case OPECODE.OPE5:
                  return "OPE5";
                  
                case OPECODE.OPE6:
                  return "OPE6";
                  
                case OPECODE.HLT:
                  return "HLT";
                  
                default:
                  return "UNKNOWN";
                  
              }
        }
        public static String ToString(int code) {
            return ToString(ToOPECODE(code));
        }
    }
}

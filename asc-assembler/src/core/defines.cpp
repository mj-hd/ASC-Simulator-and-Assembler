/*
 * core/defines.cpp
 */

#include "defines.h"

namespace defines {

  short ToDecimal(OPECODE opecode) {
    return (short)opecode;
  }
  short ToDecimal(std::string str) { return ToDecimal(ToOPECODE(str)); }

  OPECODE ToOPECODE(std::string str) {
    if (str == "LD") {
      return LD;
    }
    if (str == "ST") {
      return ST;
    }
    if (str == "ADD") {
      return ADD;
    }
    if (str == "SUB") {
      return SUB;
    }
    if (str == "AND") {
      return AND;
    }
    if (str == "OR") {
      return OR;
    }
    if (str == "B") {
      return B;
    }
    if (str == "BZ") {
      return BZ;
    }
    if (str == "BN") {
      return BN;
    }
    if (str == "OPE1") {
      return OPE1;
    }
    if (str == "OPE2") {
      return OPE2;
    }
    if (str == "OPE3") {
      return OPE3;
    }
    if (str == "OPE4") {
      return OPE4;
    }
    if (str == "OPE5") {
      return OPE5;
    }
    if (str == "OPE6") {
      return OPE6;
    }
    if (str == "HLT") {
      return HLT;
    }

    return UNKNOWN;
  }
  OPECODE ToOPECODE(int code) {
    return (OPECODE)code;
  }

  std::string ToString(OPECODE opecode) {
    std::string result;
    switch (opecode) {
      case LD:
        result = "LD";
        break;
      case ST:
        result = "ST";
        break;
      case ADD:
        result = "ADD";
        break;
      case SUB:
        result = "SUB";
        break;
      case AND:
        result = "AND";
        break;
      case OR:
        result = "OR";
        break;
      case B:
        result = "B";
        break;
      case BZ:
        result = "BZ";
        break;
      case BN:
        result = "BN";
        break;
      case OPE1:
        result = "OPE1";
        break;
      case OPE2:
        result = "OPE2";
        break;
      case OPE3:
        result = "OPE3";
        break;
      case OPE4:
        result = "OPE4";
        break;
      case OPE5:
        result = "OPE5";
        break;
      case OPE6:
        result = "OPE6";
        break;
      case HLT:
        result = "HLT";
        break;
      default:
        result = "UNKNOWN";
        break;
    }

    return result;
  }
  std::string ToString(int code) { return ToString(ToOPECODE(code)); }

  bool Has1Operand(OPECODE opecode) {
    switch(opecode) {
      case LD:
      case ST:
      case ADD:
      case SUB:
      case AND:
      case OR:
      case B:
      case BZ:
      case BN:
        return true;
      default:
        return false;
    }
  }
  bool HasNumber(OPECODE opecode) {
    switch(opecode) {
      case LD:
      case ST:
      case ADD:
      case SUB:
      case AND:
      case OR:
      case B:
      case BZ:
      case BN:
        return true;
      default:
        return false;
    }
  }
  bool HasName(OPECODE opecode) {
    switch(opecode) {
      case LD:
      case ST:
      case ADD:
      case SUB:
      case AND:
      case OR:
      case B:
      case BZ:
      case BN:
        return true;
      default:
        return false;
    }
  }
}

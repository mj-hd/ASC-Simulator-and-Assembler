/*
 * core/defines.cpp
 */

#include "defines.h"

namespace defines {

  unsigned short ToUShort(OPECODE opecode) {
    return (unsigned short)opecode;
  }
  unsigned short ToUShort(std::string str) { return ToUShort(ToOPECODE(str)); }

  OPECODE ToOPECODE(std::string str) {
    if (str == "LD") {
      return OPECODE::LD;
    }
    if (str == "ST") {
      return OPECODE::ST;
    }
    if (str == "ADD") {
      return OPECODE::ADD;
    }
    if (str == "SUB") {
      return OPECODE::SUB;
    }
    if (str == "AND") {
      return OPECODE::AND;
    }
    if (str == "OR") {
      return OPECODE::OR;
    }
    if (str == "B") {
      return OPECODE::B;
    }
    if (str == "BZ") {
      return OPECODE::BZ;
    }
    if (str == "BN") {
      return OPECODE::BN;
    }
    if (str == "OPE1") {
      return OPECODE::OPE1;
    }
    if (str == "OPE2") {
      return OPECODE::OPE2;
    }
    if (str == "OPE3") {
      return OPECODE::OPE3;
    }
    if (str == "OPE4") {
      return OPECODE::OPE4;
    }
    if (str == "OPE5") {
      return OPECODE::OPE5;
    }
    if (str == "OPE6") {
      return OPECODE::OPE6;
    }
    if (str == "HLT") {
      return OPECODE::HLT;
    }

    return OPECODE::UNKNOWN;
  }

  OPECODE ToOPECODE(unsigned short code) {
    return (OPECODE)code;
  }

  std::string ToString(OPECODE opecode) {
    std::string result;
    switch (opecode) {
      case OPECODE::LD:
        result = "LD";
        break;
      case OPECODE::ST:
        result = "ST";
        break;
      case OPECODE::ADD:
        result = "ADD";
        break;
      case OPECODE::SUB:
        result = "SUB";
        break;
      case OPECODE::AND:
        result = "AND";
        break;
      case OPECODE::OR:
        result = "OR";
        break;
      case OPECODE::B:
        result = "B";
        break;
      case OPECODE::BZ:
        result = "BZ";
        break;
      case OPECODE::BN:
        result = "BN";
        break;
      case OPECODE::OPE1:
        result = "OPE1";
        break;
      case OPECODE::OPE2:
        result = "OPE2";
        break;
      case OPECODE::OPE3:
        result = "OPE3";
        break;
      case OPECODE::OPE4:
        result = "OPE4";
        break;
      case OPECODE::OPE5:
        result = "OPE5";
        break;
      case OPECODE::OPE6:
        result = "OPE6";
        break;
      case OPECODE::HLT:
        result = "HLT";
        break;
      default:
        result = "UNKNOWN";
        break;
    }

    return result;
  }

  std::string ToString(unsigned short code) { return ToString(ToOPECODE(code)); }

  bool Has1Operand(OPECODE opecode) {
    switch(opecode) {
      case OPECODE::LD:
      case OPECODE::ST:
      case OPECODE::ADD:
      case OPECODE::SUB:
      case OPECODE::AND:
      case OPECODE::OR:
      case OPECODE::B:
      case OPECODE::BZ:
      case OPECODE::BN:
      case OPECODE::OPE1:
      case OPECODE::OPE2:
      case OPECODE::OPE3:
      case OPECODE::OPE4:
      case OPECODE::OPE5:
      case OPECODE::OPE6:
        return true;
      default:
        return false;
    }
  }
  bool HasUnsignedHexNumber(OPECODE opecode) {
    switch(opecode) {
      case OPECODE::LD:
      case OPECODE::ST:
      case OPECODE::ADD:
      case OPECODE::SUB:
      case OPECODE::AND:
      case OPECODE::OR:
      case OPECODE::B:
      case OPECODE::BZ:
      case OPECODE::BN:
      case OPECODE::OPE1:
      case OPECODE::OPE2:
      case OPECODE::OPE3:
      case OPECODE::OPE4:
      case OPECODE::OPE5:
      case OPECODE::OPE6:
        return true;
      default:
        return false;
    }
  }
  bool HasName(OPECODE opecode) {
    switch(opecode) {
      case OPECODE::LD:
      case OPECODE::ST:
      case OPECODE::ADD:
      case OPECODE::SUB:
      case OPECODE::AND:
      case OPECODE::OR:
      case OPECODE::B:
      case OPECODE::BZ:
      case OPECODE::BN:
        return true;
      default:
        return false;
    }
  }
}

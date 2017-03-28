/*
 * core/compiler.cpp
 */

#include "compiler.h"

#include <cstdlib>
#include <string>
#include <fstream>
#include <sstream>
#include <iomanip>
#include <bitset>

#include "binary.h"
#include "labels.h"
#include "defines.h"

Compiler::Compiler() {
  this->_isStreamSetted =  false;
  this->_isScanned =  false;
  this->_isLoadedToMemory = false;
  this->LineNumber = 0;
}
Compiler::~Compiler() {
}

void Compiler::SetStream(std::istream* stream) {
  this->_Stream = stream;
  this->_isStreamSetted = true;
}

void Compiler::_Scan() {
  if (!this->_isLoadedToMemory) { _LoadToMemory(); }

  this->LineNumber = 0;

  short curAddress = -1;

  bool isReturn = true;
  bool isBegin;
  bool isWhiteSpace;
  bool isTitleWritten = false;

  bool isInToken   = false;
  bool isInComment = false;
  bool isInOperand = false;
  bool isLabel = false;

  std::string token     = "";
  std::string prevToken = "";

  std::string::iterator it;
  for ( it = this->_Buffer.begin(); it != this->_Buffer.end(); it++) {
    isBegin         = isReturn;
    isReturn        = false;
    isWhiteSpace    = false;

    if (isBegin) {
      this->LineNumber++;
	  this->CharNumber = 0;
      isInOperand = false;
	  this->_Tokens.push_back("@D..F0D1:"+std::to_string(this->LineNumber));
    }

	this->CharNumber++;

    // 特殊な意味を持つ字句
    if (*it == ';')                       { isInComment = true; }
	if ((*it == ' ') || (*it == 9) || (*it == '\n'))
                                          { isWhiteSpace    = true; }
	if (*it == '\n')                     { isReturn = true; }


    // 処理
    if (isInComment && isReturn) {
      // コメントの終わり
      isInComment = false;
      continue;
    }

    if (isInComment) {
      // コメントの最中
      continue;
    }

	if (isBegin && !isWhiteSpace) {
		isLabel = true;
	}

    if (isWhiteSpace) {
      // 空白

      if (isInToken) {
        isInToken = false;

        switch (_DetectTokenType(token)) {
          case OPECODE:
            if (isInOperand) {
              throw "命令の前に改行がありません。";
            }

			if (!isTitleWritten) {
				throw "プログラムの初めに、TITLE命令が必要です。";
			}
            
            prevToken = token;
            if (defines::Has1Operand(defines::ToOPECODE(token))) {
              isInOperand = true;
            } else {
              isInOperand = false;
            }

            curAddress++;
            break;

          case VIRCODE:
            if (isInOperand) {
              throw "疑似命令の前に改行がありません。";
            }

            isInOperand = true;
            prevToken = token;

            if ((token != "TITLE") &&
                (token != "ORG")) {
              curAddress++;
            }
			if (token == "TITLE") {
				isTitleWritten = true;
			}
            break;

          case NUMBER:
            if (!isInOperand) {
              std::string message;
              message = "数値["+token+"]はこの場所にふさわしくありません。";
              throw message.c_str();
            }

            if ((prevToken == "ORG") || (prevToken == "DC") || (prevToken == "DS")) {
            } else
            if (!defines::HasNumber(defines::ToOPECODE(prevToken))) {
              std::string message;
              message = "命令[" + prevToken + "]は数値をとりません。";
              throw message.c_str();
            }
            break;

          /*case LABEL:
            if (isInOperand) {
              std::string message;
              message = "ラベル["+token+"]は行の先頭になければなりません。";
              throw message.c_str();
            }

            this->_Labels.Register(curAddress+1, token);
            break;*/

          case NAME:
			if (isLabel) {
			  token += ":";
			  this->_Labels.Register(curAddress+1, token);

			  isLabel = false;
			  break;
			}

            if (!isInOperand) {
              std::string message;
              message =  "名前["+token+"]はこの場所にふさわしくありません。";
              throw message.c_str();
            }

            if (prevToken == "TITLE") {
            } else
            if (!defines::HasName(defines::ToOPECODE(prevToken))) {
              std::string message;
              message = "命令[" + prevToken + "]は名前をとりません。";
              throw message.c_str();
            }
            break;

          case END:
            break;

          default:
            std::string message;
            message = "[" + token + "]は不明な字句です。";
            throw message.c_str();

            break;
        } // switch

        this->_Tokens.push_back(token);
        token = "";
      } // if (isInToken)

      continue;
    } // if (isWhiteSpace)
    else {
      isInToken = true;
    }
    token += *it;

  }
}

std::string Compiler::Compile(Binary* binary) {
  if (!this->_isStreamSetted) { throw "ファイル名が指定されていません。"; }

  if (!this->_isLoadedToMemory) { _LoadToMemory(); }
  if (!this->_isScanned)        { _Scan(); }

  std::stringstream log = std::stringstream();

  log << "[変換結果]" << std::endl;
  log << std::setw(24) << std::left << "アドレス: 変換後" << "	" << "  行: 変換前" << std::endl;

  short mnemonic = 0;
  std::string::iterator bufferIt = this->_Buffer.begin();
  std::list<std::string>::iterator it;
  std::stringstream log_binary = std::stringstream();
  std::stringstream log_script = std::stringstream();
  int log_binary_cnt = 0;
  int log_script_cnt = 0;
  for (it = this->_Tokens.begin(); it != this->_Tokens.end(); it++) {
	while (log_script_cnt - log_binary_cnt - 1 > 0) {

	  log_binary << std::endl;

	  log_binary_cnt++;
	}
    switch (_DetectTokenType(*it)) {
	  case RETURN:
		  log_script << std::setw(4) << (*it).substr(9, (*it).length() - 9) << ": ";

		  for (; (*bufferIt != '\n')&&(bufferIt != this->_Buffer.end()); bufferIt++) {
			  log_script << *bufferIt;
		  }
		  bufferIt++;

		  log_script << std::endl;

		  log_script_cnt++;
		continue;
      case OPECODE:
        mnemonic = defines::ToDecimal(*it) << 12;

        if (defines::Has1Operand(defines::ToOPECODE(*it))) {
          continue;
        } else {
          break;
        }
      case VIRCODE:
        if (*it == "TITLE") {
          it++;
          binary->SetTitle(*it);

          continue;
        } else
        if (*it == "ORG") {
          it++;
          binary->SetORG(_ToShort(*it));
          this->_Labels.SetBaseAddress(_ToShort(*it));

          continue;
        } else
        if (*it == "DC") {
          it++;
          mnemonic = _ToShort(*it);
        } else
        if (*it == "DS") {
          it++;

          int i;
          for (i = 0; i < _ToShort(*it); i++) {
            *binary << 0;

			log_binary << "0x" << std::setw(4) << std::setfill('0') << std::hex << (binary->GetIndex()) << ": ";
			log_binary << static_cast<std::bitset<16>>(0) << std::endl;;
			log_binary_cnt++;
			if (log_script_cnt < log_binary_cnt) {
				log_script << std::endl;
				log_script_cnt++;
			}
          }

          continue;
        }
        break;
      case LABEL:
        continue;
      case NAME:
        mnemonic += this->_Labels.Search(*it);
        break;
      case NUMBER:
        mnemonic += _ToShort(*it);
        break;
      default:

        continue;
    }

    *binary << mnemonic;

	log_binary << "0x" << std::setw(4) << std::setfill('0') << std::hex << (binary->GetIndex()) << ": ";
	log_binary << static_cast<std::bitset<16>>(mnemonic) << std::endl;;
	log_binary_cnt++;


  }

  while (log_binary_cnt != log_script_cnt) {
	  log_binary << std::endl;
	  log_binary_cnt++;
  }

  std::string log_binary_buf = log_binary.str();
  std::string log_script_buf = log_script.str();

  std::string::iterator binIt;
  std::string::iterator scrIt;

  std::string binBuf = "";
  std::string scrBuf = "";

  bool isBinReturn = false;
  bool isScrReturn = false;

  for (binIt = log_binary_buf.begin(), scrIt = log_script_buf.begin(); scrIt != log_script_buf.end();) {

	  isBinReturn = (*binIt == '\n');
	  isScrReturn = (*scrIt == '\n');

	  if (isBinReturn && isScrReturn) {
		  log << std::setw(24) << binBuf << "	" << scrBuf << std::endl;
		  binIt++;
		  scrIt++;
		  binBuf = "";
		  scrBuf = "";
	  }
	  else {
		  if (!isBinReturn) {
			  binBuf += *(binIt++);
		  }
		  if (!isScrReturn) {
			  scrBuf += *(scrIt++);
		  }
	  }
  }

  if (this->_Tokens.back() != "END") {
	  log << "警告: プログラムはENDで閉じられるべきです。" << std::endl;
  }

  log << std::endl;

  log << "[ラベルテーブル]" << std::endl;
  log << std::setw(15) << "名前 " << "|アドレス " << std::endl;
  std::list<std::string> labelNames = this->_Labels.GetNames();
  std::list<std::string>::iterator namesIt = labelNames.begin();
  for (; namesIt != labelNames.end(); namesIt++) {
	  log << std::setw(15) << (*namesIt) << "|" << "0x" << std::hex << this->_Labels.Search(*namesIt) << std::endl;
  }
  log << std::endl;

  return log.str();
}

short Compiler::_ToShort(std::string number) {
  bool isHex = false;
  if ((number[0] == '0') && (number[1] == 'x')) isHex = true;

  if (isHex) {
    return (short)std::strtol(number.c_str(), NULL, 16);
  } else {
    return (short)std::strtol(number.c_str(), NULL, 10);
  }
}

void Compiler::_LoadToMemory() {
  std::ostringstream ss;
  ss << this->_Stream->rdbuf();
  this->_Buffer = ss.str();

  this->_isLoadedToMemory = true;
}

TokenType Compiler::_DetectTokenType(std::string token) {
  if (defines::ToOPECODE(token) != defines::UNKNOWN) return OPECODE;

  if ((token == "TITLE") ||
      (token == "ORG")   ||
      (token == "DC")    ||
      (token == "DS")) {
    return VIRCODE;
  }

  if (token == "END") return END;

  if (token.find("@D..F0D1:") == 0) return RETURN;
  
  bool containsNumber = false;
  bool containsHex    = false;
  bool containsAlphabet=false;
  bool containsSymbol = false;
  bool containsIlegal = false;
  bool startsWithAlphabet = false;
  std::string::iterator it;
  for (it = token.begin(); it != token.end(); it++) {
    if ((it + 1) != token.end()) {
	  if ((*it == '0') && (*(it + 1) == 'x')) { it++; continue; }
    }

    if ((*it >= '0') && (*it <= '9')) containsNumber   = true;
    if ((*it >= 'A') && (*it <= 'Z')) containsAlphabet = true;
    if ((*it >= 'a') && (*it <= 'z')) containsAlphabet = true;
    if ((*it >= 'A') && (*it <= 'F')) containsHex = true;
    if ((*it >= 'a') && (*it <= 'f')) containsHex = true;
    if ((*it >  'F') && (*it <= 'Z')) containsHex = false;
    if ((*it >  'f') && (*it <= 'z')) containsHex = false;
    if ((*it >= '!') && (*it <= '/')) containsSymbol = true;
    if ((*it >= ':') && (*it <= '@')) containsSymbol = true;
    if ((*it >= '[') && (*it <= '`')) containsSymbol = true;
    if ((*it >= '{') && (*it <= '~')) containsSymbol = true;
    if ((int)*it >= 128) containsIlegal = true; // 2byte文字など

    if ((it == token.begin()) && containsAlphabet) startsWithAlphabet = true;
  }

  it = token.begin();

  if (containsIlegal) throw "不正な文字が含まれています。(日本語など)";

  if ((it + 1) != token.end()) {
	  if ((*it == '0') && (*(it + 1) == 'x')) {
		  if ((containsAlphabet && !containsHex) || // A-Fでないアルファベットを含む
			  (!containsAlphabet && !containsNumber) || // アルファベットでも数字でもないものを含む
			  containsSymbol) { // 記号を含む
			  throw "不正な16進数です。";
		  }

		  return NUMBER;
	  }
  }


  if (containsNumber && !containsAlphabet) return NUMBER;

  if (containsAlphabet) {
    if (!startsWithAlphabet) {
      throw "名前はアルファベットから始まらなければいけません。";
    }

   //if (*(token.end()-1) == ':') return LABEL;

    return NAME;
  }

  return UNKNOWN;
}

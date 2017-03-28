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

#include <iostream>

#include "binary.h"
#include "labels.h"
#include "defines.h"

Compiler::Compiler() {
	this->LabelTable = Labels();
	this->LineNumber = 0;
	this->CharNumber = 0;

	this->_Buffer = std::string("");
	this->_Errors = std::stringstream();
	this->_Warnings = std::stringstream();
	this->_Lines = std::list<Line>();
	this->_ORG = 0;
	this->_isFileLoaded = false;
	this->_isBufferScanned = false;
}
Compiler::~Compiler() {
}

void Compiler::SetStream(std::istream* stream) {
	this->_Buffer = static_cast<std::stringstream const&>(std::stringstream() << stream->rdbuf()).str();

	this->_isFileLoaded = true;
}

std::string Compiler::Compile(Binary* binary) {
	if (!this->_isFileLoaded) throw std::string("ファイルが設定されていません。");
	if (!this->_isBufferScanned) this->_Scan();


	if (!this->_Errors.str().empty()) {
		throw this->_Errors.str();
	}

	this->LabelTable.SetBaseAddress(this->_ORG);
	binary->SetORG(this->_ORG);

	std::stringstream log = std::stringstream();

	log << "[変換結果]" << std::endl;
    log << std::setw(24) << std::left << "アドレス: 変換後" << "	" << "  行: 変換前" << std::endl;

	int index;

	std::list<Line>::iterator lit = this->_Lines.begin();

	for (this->LineNumber = 1; this->LineNumber - 1 < this->_Lines.size(); this->LineNumber++, lit++) {
		index = this->LineNumber -1;

		if (!(lit->opecode.empty())) {

			if (defines::ToOPECODE(lit->opecode) != defines::UNKNOWN) {
				// 通常命令の場合
				short mnemonic = 0;

				mnemonic += defines::ToDecimal(lit->opecode) << 12;

				if (!(lit->operand.empty())) {
					// ここは、名前か数値かを見分けるもっといい方法を考えること!!
					if (this->LabelTable.Search(lit->operand) != -1) {
						mnemonic += this->LabelTable.Search(lit->operand);
					}
					else {
						bool hasError = false;
						mnemonic += this->_ToShort(lit->operand, &hasError);

						if (hasError) {
							std::stringstream error;
							error << "エラー(" << this->LineNumber << "行目): " << "オペランド「" << lit->operand << "」は不正な文字が含まれています。ラベルの場合、定義されていないラベルです。" << std::endl;
							throw error.str();
						}
					}
				}

				*binary << mnemonic;

				log << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << (binary->GetIndex()) << ": ";
				log << static_cast<std::bitset<16>>(mnemonic) << "	";
			}
			else {
				// 仮想命令の場合
				if (lit->opecode == "TITLE") {
					binary->SetTitle(lit->operand);
					log << std::setw(24) << " " << "	";
				}
				if (lit->opecode == "DC") {
					*binary << this->_ToShort(lit->operand);

					log << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << (binary->GetIndex()) << ": ";
					log << static_cast<std::bitset<16>>(this->_ToShort(lit->operand)) << "	";
				}
				if (lit->opecode == "DS") {
					for (int i = 0; i < this->_ToShort(lit->operand); i++) {
						*binary << 0;
						log << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << (binary->GetIndex()) << ": ";
						log << static_cast<std::bitset<16>>(0) << "	";
					}
				}
				if (lit->opecode == "ORG") {
					log << std::setw(24) << " " << "	";
				}
				if (lit->opecode == "END") {
					log << std::setw(24) << " " << "	";
				}
			}
		}
		else { // 二進数を出力しない場合。
			log << std::setw(24) << " " << "	";
		}

		// ラベルの文字数を後で反映させること！
		if (lit->label.empty() && lit->opecode.empty() && lit->operand.empty()) {
			log << std::setfill(' ') << std::right << std::setw(4) << std::dec << this->LineNumber << ": " << lit->comment << std::endl;
		}
		else {
			log << std::setfill(' ') << std::right << std::setw(4) << std::dec << this->LineNumber << ":" << " " << std::left << std::setw(10) << lit->label << std::setw(5) << lit->opecode << " " << std::setw(6) << lit->operand << " " << lit->comment << std::endl;
		}
	}

	if (!this->_Warnings.str().empty()) {
		log << this->_Warnings.str();
	}

	std::list<std::string> labels = this->LabelTable.GetNames();

	std::list<std::string>::iterator it = labels.begin();

	log << std::endl;
	log << "[ラベルテーブル]" << std::endl;
	log << "+----------+--------+" << std::endl;
	log << "|" << std::left << std::setw(10) << "ラベル名" << "|" << std::setw(6) << "アドレス" << "|" << std::endl;
	log << "+----------+--------+" << std::endl;
	for (; it != labels.end(); it++) {
		log << "|" << std::left << std::setw(10) << *it << "|" << "0x" << std::setw(6) << std::left << std::hex << this->LabelTable.Search(*it) << "|" << std::endl;
	}
	log << "+----------+--------+" << std::endl;

	return log.str();
}

void Compiler::_Scan() {
	if (!this->_isFileLoaded) throw std::string("ファイルが設定されていません。");

	this->_Buffer.append("\n");

	std::string::iterator it;
	std::string label = std::string("");
	std::string opecode = std::string("");
	std::string operand = std::string("");
	std::string comment = std::string("");
	std::string ilegal = std::string("");

	bool isBeginOfLine = false;
	bool isLabel = false;
	bool isOpecode = false;
	bool isOperand = false;
	bool isComment = false;
	bool isIlegal = false;

	bool isLabelWritten = false;
	bool isOpecodeWritten = false;
	bool isOperandWritten = false;

	bool isWhiteSpace = false;
	bool isReturn = true;

	bool isTITLEWritten = false;
	bool isORGWritten = false;
	bool isENDWritten = false;

	int relativeAddress = 0;

	this->LineNumber = 1;
	this->CharNumber = 1;

	for (it = this->_Buffer.begin(); it != this->_Buffer.end(); it++) {

		if (isReturn) {
			isBeginOfLine = true;
			isReturn = false;
			this->CharNumber = 1;
		}
		else {
			isBeginOfLine = false;
		}

		this->CharNumber++;

		switch (*it) {
			case ' ':  // 空白
			case '	': // タブ
			case '　': // 2byte空白
				isWhiteSpace = true;
				break;
			case '\n':
				isReturn = true;
				break;
			case ';':
				isComment = true;
				isLabel = false;
				isOpecode = false;
				isOperand = false;
				break;
			default:
				isWhiteSpace = false;
				break;
		}

		if (!isComment) {

			// オペコードの始まり
			if (isLabelWritten && !isOpecodeWritten && !isWhiteSpace) {
				isOpecode = true;
			}

			// オペランドの始まり
			if (isOpecodeWritten && !isOperandWritten && !isWhiteSpace) {
				isOperand = true;
			}

			// ラベルから始まる行
			if (isBeginOfLine && !isWhiteSpace) {
				isLabel = true;
			}
			// オペコードから始まる行
			if (isBeginOfLine && isWhiteSpace) {
				isLabelWritten = true;
			}

			// オペランドの後に二つ目のオペランドが来た場合
			if (isOperandWritten && !isWhiteSpace && !isReturn) {
				isIlegal = true;
			}

			if (isWhiteSpace || isReturn) {
				if (isLabel) {
					isLabel = false;
					isLabelWritten = true;
				}
				if (isOpecode) {
					isOpecode = false;
					isOpecodeWritten = true;
				}
				if (isOperand) {
					isOperand = false;
					isOperandWritten = true;
				}
			}

		}

		if (isReturn) {
			isBeginOfLine = false;
			isWhiteSpace = false;
			
			try {
				this->_LineValidation(label, opecode, operand, comment);
			}
			catch (std::string message) {
				this->_Errors << "エラー(" << this->LineNumber << "行目): " << message << std::endl;
				this->_Errors << "	" << std::dec << this->LineNumber << ": " << label << " " << opecode << " " << operand << " " << comment << std::endl;
			}
			if (!ilegal.empty()) {
				this->_Errors << "エラー(" << this->LineNumber << "行目): オペランド「" << ilegal << "」はこの場所に不適切です。" << std::endl;
				this->_Errors << "	" << std::dec << this->LineNumber << ": " << label << " " << opecode << " " << operand << " " << ilegal << " " << comment << std::endl;
			}

			if (!label.empty()) {
				this->LabelTable.Register(label, relativeAddress);
			}
			if (!opecode.empty()) {
				if ((opecode != "TITLE") && (opecode != "ORG")) {
					relativeAddress++;
				}
			}
			if (opecode == "DS") {
				relativeAddress += this->_ToShort(operand)-1;
			}

			if (opecode == "TITLE") {
				isTITLEWritten = true;
			}
			if (opecode == "ORG") {
				this->_ORG = this->_ToShort(operand);
				isORGWritten = true;
			}

			if (opecode == "END") {
				isENDWritten = true;
				break;
			}

			this->_Lines.push_back({label, opecode, operand, comment});

			label = "";
			opecode = "";
			operand = "";
			comment = "";
			ilegal = "";

			isLabel = false;
			isOpecode = false;
			isOperand = false;
			isComment = false;
			isIlegal = false;

			isLabelWritten = false;
			isOpecodeWritten = false;
			isOperandWritten = false;

			this->LineNumber++;
		}

		if (isLabel)   label   += *it;
		if (isOpecode) opecode += *it;
		if (isOperand) operand += *it;
		if (isComment) comment += *it;
		if (isIlegal) ilegal += *it;

	}

	if (!label.empty() || !opecode.empty() || !operand.empty() || !comment.empty()) {
		
		try {
			this->_LineValidation(label, opecode, operand, comment);
		}
		catch (std::string message) {
			this->_Errors << "エラー(" << this->LineNumber << "行目): " << message << std::endl;
			this->_Errors << "	" << std::dec << this->LineNumber << ": " << label << " " << opecode << " " << operand << " " << comment << std::endl;
		}
		if (!ilegal.empty()) {
			this->_Errors << "エラー(" << this->LineNumber << "行目): オペランド「" << ilegal << "」はこの場所に不適切です。" << std::endl;
			this->_Errors << "	" << std::dec << this->LineNumber << ": " << label << " " << opecode << " " << operand << " " << ilegal << " " << comment << std::endl;
		}

		if (!label.empty()) {
			this->LabelTable.Register(label, relativeAddress);
		}
		if (!opecode.empty()) {
			if ((opecode != "TITLE") && (opecode != "ORG")) {
				relativeAddress++;
			}
		}
		if (opecode == "DS") {
			relativeAddress += this->_ToShort(operand)-1;
		}

		if (opecode == "TITLE") {
			isTITLEWritten = true;
		}
		if (opecode == "ORG") {
			this->_ORG = this->_ToShort(operand);
			isORGWritten = true;
		}

		if (opecode == "END") {
			isENDWritten = true;
		}

		this->_Lines.push_back({label, opecode, operand, comment});

		this->LineNumber++;
	}

	if (!isTITLEWritten) this->_Errors << "エラー: TITLE命令が記述されていません。" << std::endl;
	if (!isORGWritten) this->_Warnings << "警告  : ORG命令が記述されていません。0x0000番地をプログラムの始点として使用します。" << std::endl;
	if (!isENDWritten) this->_Warnings << "警告  : END命令が記述されていません。" << std::endl;
}

void Compiler::_LineValidation(std::string label, std::string opecode, std::string operand, std::string comment) {

	if (opecode.empty() && !operand.empty()) {
		throw "オペランド「"+operand+"」はこの場所には不適切です。";
	}
	if (!opecode.empty() && operand.empty()) {
		if ((opecode != "HLT") && (opecode != "END") && (defines::ToOPECODE(opecode) != defines::UNKNOWN)) {
			throw "命令「"+opecode+"」にはオペランドが必要です。";
		}
	}
	if (!opecode.empty() && !operand.empty()) {
		if ((opecode == "HLT") || (opecode == "END")) {
			throw "命令"+opecode+"はオペランド「"+operand+"」を必要としません。";
		}
	}


	std::string::iterator it;

	bool containsNumber = false;
	bool containsAlphabet = false;
	bool containsSymbol = false;
	bool containsIlegal = false;
	bool isStartedWithAlphabet = false;

validationOfLabel:
	if (label.empty()) goto validationOfOpecode;

	for (it = label.begin(); it != label.end(); it++) {
		if (('0' <= *it) && (*it <= '9')) containsNumber = true;
		if (('a' <= *it) && (*it <= 'z')) containsAlphabet = true;
		if (('A' <= *it) && (*it <= 'Z')) containsAlphabet = true;
	    if ((*it >= '!') && (*it <= '/')) containsSymbol = true;
		if ((*it >= ':') && (*it <= '@')) containsSymbol = true;
		if ((*it >= '[') && (*it <= '`')) containsSymbol = true;
		if ((*it >= '{') && (*it <= '~')) containsSymbol = true;
		if ((int)*it >= 128) containsIlegal = true; // 2byte文字など

		if (containsAlphabet && it == label.begin()) isStartedWithAlphabet = true;
	}

	if (containsSymbol) throw "ラベル「" + label + "」に記号が含まれています。";
	if (containsIlegal) throw "ラベル「"+label+"」に無効な文字が含まれています。(日本語など)";
	if (!isStartedWithAlphabet) throw "ラベル「" + label + "」はアルファベットから始まっていなければいけません。";

	if (this->LabelTable.Search(label) != -1) throw "ラベル「" + label + "」が複数回定義されています。";
	
validationOfOpecode:
	if (opecode.empty()) goto validationEnd;

	defines::OPECODE opecode_p = defines::ToOPECODE(opecode);

	if (opecode_p == defines::UNKNOWN) {
		if ((opecode != "TITLE") && (opecode != "ORG") && (opecode != "DC") && (opecode != "DS") && (opecode != "END")) {
			throw "命令「" + opecode + "」は存在しません。(大文字小文字を区別します）";
		}
	}

validationOfOperand:
	if (operand.empty()) goto validationEnd;

	containsNumber = false;
	containsAlphabet = false;
	containsSymbol = false;
	containsIlegal = false;
	isStartedWithAlphabet = false;

	bool containsUnhexAlphabet = false;
	bool isStartedWithMinus = false;
	bool isStartedWithPlus = false;

	bool isNumber = false;
	bool isHex = false;
	bool isDec = false;

	it = operand.begin();

	if (operand.length() > 2) {
		if ((*it == '0') && (*(it + 1) == 'x')) {
			isHex = true;
			isNumber = true;
			it += 2;
		}
	}

	for (; it != operand.end(); it++) {
		if (('0' <= *it) && (*it <= '9')) containsNumber = true;
		if (('a' <= *it) && (*it <= 'z')) containsAlphabet = true;
		if (('A' <= *it) && (*it <= 'Z')) containsAlphabet = true;
		if (('G' <= *it) && (*it <= 'Z')) containsUnhexAlphabet = true;
	    if ((*it >= '!') && (*it <= '/')) containsSymbol = true;
		if ((*it >= ':') && (*it <= '@')) containsSymbol = true;
		if ((*it >= '[') && (*it <= '`')) containsSymbol = true;
		if ((*it >= '{') && (*it <= '~')) containsSymbol = true;
		if ((int)*it >= 128) containsIlegal = true; // 2byte文字など

		if (containsAlphabet && it == (operand.begin())) isStartedWithAlphabet = true;
		if ('-' == *it && it == operand.begin()) {
			isStartedWithMinus = true;
			containsSymbol = false;
		}
	}

	if (containsNumber && !(containsAlphabet || containsSymbol)) {
		if (!isHex) {
			isDec = true;
			isNumber = true;
		}
	}

	if (isHex && containsUnhexAlphabet) throw "オペランド「" + operand + "」は16進数として記述されていますが、無効な文字が含まれています。";

	if (containsSymbol) throw "オペランド「" + operand + "」に記号が含まれています。";
	if (containsIlegal) throw "オペランド「" + operand + "」に無効な文字が含まれています。(日本語など)";
	if (opecode_p != defines::UNKNOWN) {
		if (!defines::HasNumber(opecode_p) && isNumber) throw "オペランド「" + operand + "」はこの場所に不適切です。";
		if (!defines::HasName(opecode_p) && isStartedWithAlphabet) throw "オペランド「" + operand + "」はこの場所に不適切です。";
	}
	if ((opecode == "TITLE") && !isStartedWithAlphabet) throw "オペランド「" + operand + "」はアルファベットから始まっていなければいけません。";
	if ((opecode == "ORG") && !isHex) throw "オペランド「" + operand + "」は16進数であるべきです。";
	if ((opecode == "DC") && !isDec) throw "オペランド「" + operand + "」は10進数であるべきです。";
	if ((opecode == "DS") && !isDec) throw "オペランド「" + operand + "」は10進数であるべきです。";
validationEnd:
	return;
}

short Compiler::_ToShort(std::string source) {
	return this->_ToShort(source, NULL);
}

short Compiler::_ToShort(std::string source, bool* hasError) {
	bool isHex = false;
	char* endPtr;
	short result;

	if (source.find("0x") == 0) isHex = true;

	if (isHex) {
		result = (short)std::strtol(source.c_str(), &endPtr, 16);
	}
	else {
		result = (short)std::strtol(source.c_str(), &endPtr, 10);
	}

	if (*endPtr != NULL) {
		if (hasError != NULL) {
			*hasError = true;
		}
	}
	else {
		if (hasError != NULL) {
			*hasError = false;
		}
	}

	return result;
}
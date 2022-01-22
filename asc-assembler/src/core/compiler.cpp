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
#include <vector>

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

void Compiler::_AppendAddrShortError(_AddrShortError error, std::string operand) {
	this->_Errors << "エラー(" << this->LineNumber << "行目): ";

	switch (error) {
	case _AddrShortError::INVALID_VALUE:
	  this->_Errors << _errShortInvalidValue(operand);
		break;
	case _AddrShortError::OUT_OF_MEMORY_RANGE:
		this->_Errors << _errShortOutOfMemoryRange(operand);
		break;
	}

	this->_Errors << std::endl;
}

void Compiler::_AppendShortError(_ShortError error, std::string operand) {
	this->_Errors << "エラー(" << this->LineNumber << "行目): ";

	switch (error) {
	case _ShortError::INVALID_VALUE:
	  this->_Errors << _errShortInvalidValue(operand);
		break;
	case _ShortError::OUT_OF_SIGNED_RANGE:
		this->_Errors << _errShortOutOfSignedRange(operand);
		break;
	case _ShortError::OUT_OF_UNSIGNED_RANGE:
		this->_Errors << _errShortOutOfUnsignedRange(operand);
		break;
	}

	this->_Errors << std::endl;
}

std::string Compiler::Compile(Binary* binary) {
	if (!this->_isFileLoaded) throw std::string("ファイルが設定されていません。");
	if (!this->_isBufferScanned) this->_Scan();


	this->LabelTable.SetBaseAddress(this->_ORG);
	binary->SetORG(this->_ORG);

	unsigned int max_width = 0;
	std::vector<std::stringstream> log_before = std::vector<std::stringstream>();
	std::vector<std::stringstream> log_after = std::vector<std::stringstream>();

	log_before.push_back(std::stringstream());
	log_after.push_back(std::stringstream());
	log_before.back() << "行: 変換前";
	log_after.back() << "アドレス: 変換後";

	unsigned int index;

	std::list<Line>::iterator lit = this->_Lines.begin();

	for (this->LineNumber = 1; this->LineNumber - 1 < this->_Lines.size(); this->LineNumber++, lit++) {
		index = this->LineNumber -1;

		if (!(lit->opecode.empty())) {

			if (defines::ToOPECODE(lit->opecode) != defines::OPECODE::UNKNOWN) {
				// 通常命令の場合
				unsigned short mnemonic = 0;

				mnemonic += defines::ToUShort(lit->opecode) << 12;

				if (!(lit->operand.empty())) {
					// ここは、名前か数値かを見分けるもっといい方法を考えること!!
					if (this->LabelTable.Search(lit->operand) != -1) {
						mnemonic += this->LabelTable.Search(lit->operand);
					}
					else {
						_AddrShortError error = _AddrShortError::NONE;
						mnemonic += this->_ToAddrShort(lit->operand, &error);

						if (error != _AddrShortError::NONE) {
							this->_AppendAddrShortError(error, lit->operand);
						}
					}
				}

				*binary << mnemonic;

				log_after.push_back(std::stringstream());
				log_after.back() << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << (binary->GetIndex()) << ": ";
				log_after.back() << static_cast<std::bitset<16>>(mnemonic);
			}
			else {
				log_after.push_back(std::stringstream());
				// 仮想命令の場合
				if (lit->opecode == "TITLE") {
					binary->SetTitle(lit->operand);
					log_after.back() << " ";
				}
				if (lit->opecode == "DC") {
					_ShortError error = _ShortError::NONE;
					*binary << this->_ToShort(lit->operand, &error);

					if (error != _ShortError::NONE) {
						this->_AppendShortError(error, lit->operand);
					}

					log_after.back() << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << (binary->GetIndex()) << ": ";
					log_after.back() << static_cast<std::bitset<16>>(this->_ToShort(lit->operand));
				}
				if (lit->opecode == "DS") {
					_ShortError error = _ShortError::NONE;
					unsigned short count = this->_ToShort(lit->operand, &error);

					if (error != _ShortError::NONE) {
						this->_AppendShortError(error, lit->operand);
					}

					unsigned short baseIndex = binary->GetIndex() + 1;

					for (int i = 0; i < count; i++) {
						*binary << 0;
					}

					if (count > 0) {
						log_after.back() << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << baseIndex;
						if (count > 1) {
							log_after.back() << "~0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << baseIndex + count - 1;
						}
						log_after.back() << ": " << static_cast<std::bitset<16>>(0);
					}
				}
				if (lit->opecode == "ORG") {
					log_after.back() << " ";
				}
				if (lit->opecode == "END") {
					log_after.back() << " ";
				}
			}
		}
		else { // 二進数を出力しない場合。
			log_after.push_back(std::stringstream());
			log_after.back() << " ";
		}

		log_before.push_back(std::stringstream());
		// ラベルの文字数を後で反映させること！
		if (lit->label.empty() && lit->opecode.empty() && lit->operand.empty()) {
			log_before.back() << std::setfill(' ') << std::right << std::setw(4) << std::dec << this->LineNumber << ": " << lit->comment;
		}
		else {
			log_before.back() << std::setfill(' ') << std::right << std::setw(4) << std::dec << this->LineNumber << ":" << " " << std::left << std::setw(10) << lit->label << std::setw(5) << lit->opecode << " " << std::setw(6) << lit->operand << " " << lit->comment;
		}
		if (log_before.back().tellp() > max_width) {
			max_width = (unsigned int)log_before.back().tellp();
		}
	}

	std::stringstream log = std::stringstream();
	log << "[変換結果]" << std::endl;

	for (unsigned int i = 0; i < log_before.size(); i++) {
		log << std::setw(max_width) << std::left << log_before[i].str();
		if (!this->HasError()) {
			log << "\t" << log_after[i].str();
		}
		log << std::endl;
	}
	
	if (!this->_Errors.str().empty()) {
		log << this->_Errors.str();
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

	unsigned int relativeAddress = 0;

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
		throw _errOperandUnexpected(operand);
	}
	if (!opecode.empty() && operand.empty()) {
		if ((opecode != "HLT") && (opecode != "END") && (defines::ToOPECODE(opecode) != defines::OPECODE::UNKNOWN)) {
			throw _errMissingOperand(opecode);
		}
	}
	if (!opecode.empty() && !operand.empty()) {
		if ((opecode == "HLT") || (opecode == "END")) {
			throw _errUnnecessaryOperand(opecode, operand);
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
		if ((unsigned int)*it >= 128) containsIlegal = true; // 2byte文字など

		if (containsAlphabet && it == label.begin()) isStartedWithAlphabet = true;
	}

	if (containsSymbol) throw _errLabelInvalidSymbol(label);
	if (containsIlegal) throw _errLabelIllegal(label);
	if (!isStartedWithAlphabet) throw _errLabelMustBeStartedAlphabet(label);

	if (this->LabelTable.Search(label) != -1) throw _errLabelDuplication(label);
	
validationOfOpecode:
	if (opecode.empty()) goto validationEnd;

	defines::OPECODE opecode_p = defines::ToOPECODE(opecode);

	if (opecode_p == defines::OPECODE::UNKNOWN) {
		if ((opecode != "TITLE") && (opecode != "ORG") && (opecode != "DC") && (opecode != "DS") && (opecode != "END")) {
			throw _errOpecodeInvalid(opecode);
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

	bool shouldBeSignedNumber = false;
	bool shouldBeHex = false;

	it = operand.begin();

	if (operand.length() > 2) {
		if ((*it == '0') && (*(it + 1) == 'x')) {
			shouldBeHex = true;
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
		if ((unsigned int)*it >= 128) containsIlegal = true; // 2byte文字など

		if (containsAlphabet && it == (operand.begin())) isStartedWithAlphabet = true;
		// 行頭の+/-は正常な記号として扱う
		if ((('-' == *it) || ('+' == *it)) && it == operand.begin()) {
			shouldBeSignedNumber = true;
			containsSymbol = false;
		}
	}

	bool isDec = false;
	bool isHex = false;
	bool isSignedNumber = false;
	bool isUnsignedNumber = false;

	// 16進数
	if (shouldBeHex && !(containsSymbol || containsUnhexAlphabet)) {
		isHex = true;
	}

	// 10進数
	if (containsNumber && !(containsAlphabet || containsSymbol)) {
		isDec = !isHex;
	}

	if (isHex || isDec) {
		isSignedNumber = shouldBeSignedNumber;
		isUnsignedNumber = !shouldBeSignedNumber;
	}

	if (shouldBeHex && containsUnhexAlphabet) throw _errOperandInvalidHex(operand);

	if (containsSymbol) throw _errOperandInvalidSymbol(operand);
	if (containsIlegal) throw _errOperandIllegal(operand);
	if (opecode_p != defines::OPECODE::UNKNOWN) {
		bool valid = false;
		valid |= defines::HasUnsignedHexNumber(opecode_p) && isHex && isUnsignedNumber;
		valid |= defines::HasName(opecode_p) && isStartedWithAlphabet;
		if (!valid) throw _errOperandInvalid(operand);
	}
	if ((opecode == "TITLE") && !isStartedWithAlphabet) throw _errOperandTitleInvalid(operand);
	if ((opecode == "ORG") && !(isHex && isUnsignedNumber)) throw _errOperandOrgInvalid(operand);
	if ((opecode == "DC") && !isDec && !(isHex && isUnsignedNumber)) throw _errOperandDcInvalid(operand);
	if ((opecode == "DS") && !(isDec && isUnsignedNumber)) throw _errOperandDsInvalid(operand);
validationEnd:
	return;
}

unsigned short Compiler::_ToAddrShort(std::string source, _AddrShortError* error) {
	if (source.find("0x") != 0 || source.find("+") == 0 || source.find("-") == 0) {
		if (error != NULL) *error = _AddrShortError::INVALID_VALUE;
		return 0;
	}

	char* endPtr;
	long result;
	result = std::strtol(source.c_str(), &endPtr, 16);

	if (error != NULL) {
		if (*endPtr != NULL) *error = _AddrShortError::INVALID_VALUE;

		if (!(0 <= result && result <= 0xFFF)) {
			*error = _AddrShortError::OUT_OF_MEMORY_RANGE;
		}
	}

	return (unsigned short)result;
}

unsigned short Compiler::_ToShort(std::string source) {
	return this->_ToShort(source, NULL);
}

unsigned short Compiler::_ToShort(std::string source, _ShortError* error) {
	bool isHex = false;
	bool isSigned = false;
	char* endPtr;
	long result;

	if (source.find("0x") == 0) isHex = true;
	if (source.find("+") == 0 || source.find("-") == 0) isSigned = true;

	if (isHex) {
		result = std::strtol(source.c_str(), &endPtr, 16);
	}
	else {
		result = std::strtol(source.c_str(), &endPtr, 10);
	}

	if (error != NULL) {
		if (*endPtr != NULL) {
			*error = _ShortError::INVALID_VALUE;
		}

		long min = isSigned ? SHRT_MIN : 0;
		long max = isSigned ? SHRT_MAX : USHRT_MAX;

		if (!(min <= result && result <= max)) {
			*error = isSigned ? _ShortError::OUT_OF_SIGNED_RANGE : _ShortError::OUT_OF_UNSIGNED_RANGE;
		}
	}

	return (unsigned short)result;
}

std::string _errOperandDsInvalid(std::string operand) {
	return "オペランド「" + operand + "」は10進数であるべきです。";
}

std::string _errOperandDcInvalid(std::string operand) {
	return "オペランド「" + operand + "」は16進数か符号付き10進数であるべきです。";
}

std::string _errOperandOrgInvalid(std::string operand) {
	return "オペランド「" + operand + "」は16進数であるべきです。";
}

std::string _errOperandTitleInvalid(std::string operand) {
	return "オペランド「" + operand + "」はアルファベットから始まっていなければいけません。";
}

std::string _errOperandInvalid(std::string operand) {
	return "オペランド「" + operand + "」はこの場所に不適切です。";
}

std::string _errOperandIllegal(std::string operand) {
	return "オペランド「" + operand + "」に無効な文字が含まれています。(日本語など)";
}

std::string _errOperandInvalidSymbol(std::string operand) {
	return "オペランド「" + operand + "」に記号が含まれています。";
}

std::string _errOperandInvalidHex(std::string operand) {
	return "オペランド「" + operand + "」は16進数として記述されていますが、無効な文字が含まれています。";
}

std::string _errOpecodeInvalid(std::string opecode) {
	return "命令「" + opecode + "」は存在しません。(大文字小文字を区別します）";
}

std::string _errLabelDuplication(std::string label) {
	return "ラベル「" + label + "」が複数回定義されています。";
}

std::string _errLabelMustBeStartedAlphabet(std::string label) {
	return "ラベル「" + label + "」はアルファベットから始まっていなければいけません。";
}

std::string _errLabelIllegal(std::string label) {
	return "ラベル「" + label + "」に無効な文字が含まれています。(日本語など)";
}

std::string _errLabelInvalidSymbol(std::string label) {
	return "ラベル「" + label + "」に記号が含まれています。";
}

std::string _errUnnecessaryOperand(std::string opecode, std::string operand) {
	return "命令" + opecode + "はオペランド「" + operand + "」を必要としません。";
}

std::string _errMissingOperand(std::string opecode) {
	return "命令「" + opecode + "」にはオペランドが必要です。";
}

std::string _errOperandUnexpected(std::string operand) {
	return "オペランド「" + operand + "」はこの場所には不適切です。";
}

std::string _errShortOutOfSignedRange(std::string operand) {
	return "オペランド「" + operand + "」は符号付き数の範囲外です";
}

std::string _errShortOutOfUnsignedRange(std::string operand) {
	return "オペランド「" + operand + "」は数値の範囲外です";
}

std::string _errShortInvalidValue(std::string operand) {
	return "オペランド「" + operand + "」は不正な値か、未定義のラベルです";
}

std::string _errShortOutOfMemoryRange(std::string operand) {
	return "オペランド「" + operand + "」はメモリ番地指定の範囲外です";
}

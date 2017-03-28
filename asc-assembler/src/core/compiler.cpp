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
	if (!this->_isFileLoaded) throw std::string("�t�@�C�����ݒ肳��Ă��܂���B");
	if (!this->_isBufferScanned) this->_Scan();


	if (!this->_Errors.str().empty()) {
		throw this->_Errors.str();
	}

	this->LabelTable.SetBaseAddress(this->_ORG);
	binary->SetORG(this->_ORG);

	std::stringstream log = std::stringstream();

	log << "[�ϊ�����]" << std::endl;
    log << std::setw(24) << std::left << "�A�h���X: �ϊ���" << "	" << "  �s: �ϊ��O" << std::endl;

	int index;

	std::list<Line>::iterator lit = this->_Lines.begin();

	for (this->LineNumber = 1; this->LineNumber - 1 < this->_Lines.size(); this->LineNumber++, lit++) {
		index = this->LineNumber -1;

		if (!(lit->opecode.empty())) {

			if (defines::ToOPECODE(lit->opecode) != defines::UNKNOWN) {
				// �ʏ햽�߂̏ꍇ
				short mnemonic = 0;

				mnemonic += defines::ToDecimal(lit->opecode) << 12;

				if (!(lit->operand.empty())) {
					// �����́A���O�����l����������������Ƃ������@���l���邱��!!
					if (this->LabelTable.Search(lit->operand) != -1) {
						mnemonic += this->LabelTable.Search(lit->operand);
					}
					else {
						bool hasError = false;
						mnemonic += this->_ToShort(lit->operand, &hasError);

						if (hasError) {
							std::stringstream error;
							error << "�G���[(" << this->LineNumber << "�s��): " << "�I�y�����h�u" << lit->operand << "�v�͕s���ȕ������܂܂�Ă��܂��B���x���̏ꍇ�A��`����Ă��Ȃ����x���ł��B" << std::endl;
							throw error.str();
						}
					}
				}

				*binary << mnemonic;

				log << "0x" << std::setw(4) << std::setfill('0') << std::hex << std::right << (binary->GetIndex()) << ": ";
				log << static_cast<std::bitset<16>>(mnemonic) << "	";
			}
			else {
				// ���z���߂̏ꍇ
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
		else { // ��i�����o�͂��Ȃ��ꍇ�B
			log << std::setw(24) << " " << "	";
		}

		// ���x���̕���������Ŕ��f�����邱�ƁI
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
	log << "[���x���e�[�u��]" << std::endl;
	log << "+----------+--------+" << std::endl;
	log << "|" << std::left << std::setw(10) << "���x����" << "|" << std::setw(6) << "�A�h���X" << "|" << std::endl;
	log << "+----------+--------+" << std::endl;
	for (; it != labels.end(); it++) {
		log << "|" << std::left << std::setw(10) << *it << "|" << "0x" << std::setw(6) << std::left << std::hex << this->LabelTable.Search(*it) << "|" << std::endl;
	}
	log << "+----------+--------+" << std::endl;

	return log.str();
}

void Compiler::_Scan() {
	if (!this->_isFileLoaded) throw std::string("�t�@�C�����ݒ肳��Ă��܂���B");

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
			case ' ':  // ��
			case '	': // �^�u
			case '�@': // 2byte��
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

			// �I�y�R�[�h�̎n�܂�
			if (isLabelWritten && !isOpecodeWritten && !isWhiteSpace) {
				isOpecode = true;
			}

			// �I�y�����h�̎n�܂�
			if (isOpecodeWritten && !isOperandWritten && !isWhiteSpace) {
				isOperand = true;
			}

			// ���x������n�܂�s
			if (isBeginOfLine && !isWhiteSpace) {
				isLabel = true;
			}
			// �I�y�R�[�h����n�܂�s
			if (isBeginOfLine && isWhiteSpace) {
				isLabelWritten = true;
			}

			// �I�y�����h�̌�ɓ�ڂ̃I�y�����h�������ꍇ
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
				this->_Errors << "�G���[(" << this->LineNumber << "�s��): " << message << std::endl;
				this->_Errors << "	" << std::dec << this->LineNumber << ": " << label << " " << opecode << " " << operand << " " << comment << std::endl;
			}
			if (!ilegal.empty()) {
				this->_Errors << "�G���[(" << this->LineNumber << "�s��): �I�y�����h�u" << ilegal << "�v�͂��̏ꏊ�ɕs�K�؂ł��B" << std::endl;
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
			this->_Errors << "�G���[(" << this->LineNumber << "�s��): " << message << std::endl;
			this->_Errors << "	" << std::dec << this->LineNumber << ": " << label << " " << opecode << " " << operand << " " << comment << std::endl;
		}
		if (!ilegal.empty()) {
			this->_Errors << "�G���[(" << this->LineNumber << "�s��): �I�y�����h�u" << ilegal << "�v�͂��̏ꏊ�ɕs�K�؂ł��B" << std::endl;
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

	if (!isTITLEWritten) this->_Errors << "�G���[: TITLE���߂��L�q����Ă��܂���B" << std::endl;
	if (!isORGWritten) this->_Warnings << "�x��  : ORG���߂��L�q����Ă��܂���B0x0000�Ԓn���v���O�����̎n�_�Ƃ��Ďg�p���܂��B" << std::endl;
	if (!isENDWritten) this->_Warnings << "�x��  : END���߂��L�q����Ă��܂���B" << std::endl;
}

void Compiler::_LineValidation(std::string label, std::string opecode, std::string operand, std::string comment) {

	if (opecode.empty() && !operand.empty()) {
		throw "�I�y�����h�u"+operand+"�v�͂��̏ꏊ�ɂ͕s�K�؂ł��B";
	}
	if (!opecode.empty() && operand.empty()) {
		if ((opecode != "HLT") && (opecode != "END") && (defines::ToOPECODE(opecode) != defines::UNKNOWN)) {
			throw "���߁u"+opecode+"�v�ɂ̓I�y�����h���K�v�ł��B";
		}
	}
	if (!opecode.empty() && !operand.empty()) {
		if ((opecode == "HLT") || (opecode == "END")) {
			throw "����"+opecode+"�̓I�y�����h�u"+operand+"�v��K�v�Ƃ��܂���B";
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
		if ((int)*it >= 128) containsIlegal = true; // 2byte�����Ȃ�

		if (containsAlphabet && it == label.begin()) isStartedWithAlphabet = true;
	}

	if (containsSymbol) throw "���x���u" + label + "�v�ɋL�����܂܂�Ă��܂��B";
	if (containsIlegal) throw "���x���u"+label+"�v�ɖ����ȕ������܂܂�Ă��܂��B(���{��Ȃ�)";
	if (!isStartedWithAlphabet) throw "���x���u" + label + "�v�̓A���t�@�x�b�g����n�܂��Ă��Ȃ���΂����܂���B";

	if (this->LabelTable.Search(label) != -1) throw "���x���u" + label + "�v���������`����Ă��܂��B";
	
validationOfOpecode:
	if (opecode.empty()) goto validationEnd;

	defines::OPECODE opecode_p = defines::ToOPECODE(opecode);

	if (opecode_p == defines::UNKNOWN) {
		if ((opecode != "TITLE") && (opecode != "ORG") && (opecode != "DC") && (opecode != "DS") && (opecode != "END")) {
			throw "���߁u" + opecode + "�v�͑��݂��܂���B(�啶������������ʂ��܂��j";
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
		if ((int)*it >= 128) containsIlegal = true; // 2byte�����Ȃ�

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

	if (isHex && containsUnhexAlphabet) throw "�I�y�����h�u" + operand + "�v��16�i���Ƃ��ċL�q����Ă��܂����A�����ȕ������܂܂�Ă��܂��B";

	if (containsSymbol) throw "�I�y�����h�u" + operand + "�v�ɋL�����܂܂�Ă��܂��B";
	if (containsIlegal) throw "�I�y�����h�u" + operand + "�v�ɖ����ȕ������܂܂�Ă��܂��B(���{��Ȃ�)";
	if (opecode_p != defines::UNKNOWN) {
		if (!defines::HasNumber(opecode_p) && isNumber) throw "�I�y�����h�u" + operand + "�v�͂��̏ꏊ�ɕs�K�؂ł��B";
		if (!defines::HasName(opecode_p) && isStartedWithAlphabet) throw "�I�y�����h�u" + operand + "�v�͂��̏ꏊ�ɕs�K�؂ł��B";
	}
	if ((opecode == "TITLE") && !isStartedWithAlphabet) throw "�I�y�����h�u" + operand + "�v�̓A���t�@�x�b�g����n�܂��Ă��Ȃ���΂����܂���B";
	if ((opecode == "ORG") && !isHex) throw "�I�y�����h�u" + operand + "�v��16�i���ł���ׂ��ł��B";
	if ((opecode == "DC") && !isDec) throw "�I�y�����h�u" + operand + "�v��10�i���ł���ׂ��ł��B";
	if ((opecode == "DS") && !isDec) throw "�I�y�����h�u" + operand + "�v��10�i���ł���ׂ��ł��B";
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
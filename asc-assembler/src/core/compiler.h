/*
 * core/compiler.h
 */

#ifndef CORE_COMPILER_H
#define CORE_COMPILER_H

#include <fstream>
#include <sstream>
#include <string>
#include <list>

#include "labels.h"
#include "binary.h"

typedef enum {
  UNKNOWN,
  RETURN,
  OPECODE,
  VIRCODE,
  LABEL,
  NAME,
  NUMBER,
  END
} TokenType;

typedef struct {
	std::string label;
	std::string opecode;
	std::string operand;
	std::string comment;
} Line;

class Compiler {

public:
  Compiler();
  ~Compiler();

  void SetStream(std::istream*);

  std::string Compile(Binary*);

  bool HasError() { return this->_Errors.tellp() > 0; };
  bool HasWarning() { return this->_Warnings.tellp() > 0; };

  int LineNumber;
  int CharNumber;

  Labels LabelTable;

private:
	void _Scan();
	void _LineValidation(std::string, std::string, std::string, std::string);
	short _ToShort(std::string);
	short _ToShort(std::string, bool*);

	std::string _Buffer;
	bool _isFileLoaded;
	bool _isBufferScanned;
	int _ORG;

	std::list<Line> _Lines;
	std::stringstream _Errors;
	std::stringstream _Warnings;
};

std::string _errOperandDsInvalid(std::string operand);
std::string _errOperandDcInvalid(std::string operand);
std::string _errOperandOrgInvalid(std::string operand);
std::string _errOperandTitleInvalid(std::string operand);
std::string _errOperandInvalid(std::string operand);
std::string _errOperandIllegal(std::string operand);
std::string _errOperandInvalidSymbol(std::string operand);
std::string _errOperandInvalidHex(std::string operand);
std::string _errOpecodeInvalid(std::string opecode);
std::string _errLabelDuplication(std::string label);
std::string _errLabelMustBeStartedAlphabet(std::string label);
std::string _errLabelIllegal(std::string label);
std::string _errLabelInvalidSymbol(std::string label);
std::string _errUnnecessaryOperand(std::string opecode, std::string operand);
std::string _errMissingOperand(std::string opecode);
std::string _errOperandUnexpected(std::string operand);

#endif // CORE_COMPILER_H

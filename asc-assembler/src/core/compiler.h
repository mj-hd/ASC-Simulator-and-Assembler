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

enum class _ShortError {
	NONE,
	OUT_OF_SIGNED_RANGE,
	OUT_OF_UNSIGNED_RANGE,
	INVALID_VALUE,
};

enum class _AddrShortError {
	NONE,
	OUT_OF_MEMORY_RANGE,
	INVALID_VALUE,
};

class Compiler {

public:
  Compiler();
  ~Compiler();

  void SetStream(std::istream*);

  std::string Compile(Binary*);

  bool HasError() { return this->_Errors.tellp() > 0; };
  bool HasWarning() { return this->_Warnings.tellp() > 0; };

  unsigned int LineNumber;
  unsigned int CharNumber;

  Labels LabelTable;

private:
	void _Scan();
	void _LineValidation(std::string, std::string, std::string, std::string);
	unsigned short _ToShort(std::string);
	unsigned short _ToShort(std::string, _ShortError*);
	unsigned short _ToAddrShort(std::string, _AddrShortError*);
	void _AppendAddrShortError(_AddrShortError, std::string);
	void _AppendShortError(_ShortError, std::string);

	std::string _Buffer;
	bool _isFileLoaded;
	bool _isBufferScanned;
	unsigned int _ORG;

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
std::string _errShortOutOfSignedRange(std::string operand);
std::string _errShortOutOfUnsignedRange(std::string operand);
std::string _errShortInvalidValue(std::string operand);
std::string _errShortOutOfMemoryRange(std::string operand);

#endif // CORE_COMPILER_H

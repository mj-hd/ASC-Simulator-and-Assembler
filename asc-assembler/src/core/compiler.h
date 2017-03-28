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

#endif // CORE_COMPILER_H

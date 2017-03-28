/*
 * main.cpp
 */

#include <iostream>
#include <fstream>
#include <cstdlib>
#include <iomanip>

#include "version.h"
#include "main.h"
#include "core/compiler.h"
#include "core/binary.h"

int main(int argc, char* argv[]) {

  std::string ifname;
  
#ifndef _DEBUG
  if (argc < 2) {
    std::cerr << "�p�����[�^�����Ȃ��Ƃ���K�v�ł��B" << std::endl;

    Help();

    return EXIT_FAILURE;

  } else
  if (argc > 2) {
    std::cerr << "�p�����[�^���������܂��B" << std::endl;

    return EXIT_FAILURE;
  }

  ifname = argv[1];

#else
  ifname = "Y:\\Downloads\\factorial.asm";

#endif

  std::ifstream istream(ifname, std::ios::in);
  if ( istream.fail() ) {
    std::cerr << "�t�@�C�������݂��܂���B" << std::endl;

    return EXIT_FAILURE;
  }


  Compiler compiler;
  Binary binary;
  std::string log;

  try {
    compiler.SetStream(&istream);
    log = compiler.Compile(&binary);
  }
  catch (std::string message) {
	  std::cerr << message << std::endl << "�G���[���������Ă��邽�߁A�A�Z���u���𒆎~���܂��B" << std::endl;;

    istream.close();

    return EXIT_FAILURE;
  }

  std::string ofname;
  getDirectoryFromPath(&ofname, ifname);

  ofname += binary.GetTitle() + ".asco";

  std::ofstream ostream(ofname, std::ios::binary | std::ios::trunc);
  if ( ostream.fail() ) {
    std::cerr << "�������ݗp�Ƀt�@�C�����J���܂���ł����B" << std::endl;
	std::cerr << "�t�@�C����: " << ofname << std::endl;

    return EXIT_FAILURE;
  }

  std::cout << "�v���O������: " << binary.GetTitle() << "	�t�@�C���T�C�Y:" << binary.GetSize() << " byte" << std::endl;
  std::cout << "ORG: 0x" << std::hex << binary.GetORG() << std::endl;
  std::cout << "--------------------------------------------------" << std::endl;
  std::cout << log << std::endl;

  binary.WriteToStream(&ostream);

  istream.close();
  ostream.close();

  return EXIT_SUCCESS;
}

void Help() {
  std::cout << "ASC Assembler " << VERSION_DISPLAY << std::endl;
  std::cout << "--------------------------------------------------" << std::endl;
  std::cout << "asc-assembler.exe <�t�@�C����>" << std::endl;
  std::cout << "          <�t�@�C����> : ascs�t�@�C���̃p�X" << std::endl;
  std::cout << std::endl;
}

void getDirectoryFromPath(std::string *out, std::string in) {

	std::string::iterator it;
	std::string result = "";
	bool hasStartedDirectory = false;

	it = in.end();
	do {
		it--;

		if (*it == '\\')
			hasStartedDirectory = true;

		if (hasStartedDirectory)
			result += *it;
	} while (it != in.begin());

	if (result == "") {
		*out = ".\\";
		return;
	}

	*out = "";

	it = result.end();
	do {
		it--;

		*out += *it;
	} while (it != result.begin());
}

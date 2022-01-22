#include <sstream>

#include "pch.h"
#include "CppUnitTest.h"

#include "../asc-assembler/src/core/compiler.h"
#include "../asc-assembler/src/core/binary.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace ascAssemblerTest
{
	TEST_CLASS(ascAssemblerTest)
	{
	public:

		TEST_METHOD(TestSuccess)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x100" << std::endl;
			stream << "  LD 0x000" << std::endl;
			stream << "  ST 0x001" << std::endl;
			stream << "  ADD 0x002" << std::endl;
			stream << "  SUB 0x003" << std::endl;
			stream << "  AND 0x004" << std::endl;
			stream << "  OR 0x005" << std::endl;
			stream << "  B 0x006" << std::endl;
			stream << "  BZ 0x007" << std::endl;
			stream << "  BN 0x008" << std::endl;
			stream << "  OPE1 0x009" << std::endl;
			stream << "  OPE2 0x00A" << std::endl;
			stream << "  OPE3 0x00B" << std::endl;
			stream << "  OPE4 0x00C" << std::endl;
			stream << "  OPE5 0x00D" << std::endl;
			stream << "  OPE6 0x00E" << std::endl;
			stream << "  HLT" << std::endl;
			stream << "  DC 1" << std::endl;
			stream << "  DC 2" << std::endl;
			stream << "  END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log =compiler.Compile(&binary).c_str();
			Logger::WriteMessage(log.c_str());

			Assert::AreEqual((short)0x1100, binary[0]);
			Assert::AreEqual((short)0x0000, binary[1]);
			Assert::AreEqual((short)0x1001, binary[2]);
			Assert::AreEqual((short)0x2002, binary[3]);
			Assert::AreEqual((short)0x3003, binary[4]);
			Assert::AreEqual((short)0x4004, binary[5]);
			Assert::AreEqual((short)0x5005, binary[6]);
			Assert::AreEqual((short)0x6006, binary[7]);
			Assert::AreEqual((short)0x7007, binary[8]);
			Assert::AreEqual((short)0x8008, binary[9]);
			Assert::AreEqual((short)0x9009, binary[10]);
			Assert::AreEqual((short)0xA00A, binary[11]);
			Assert::AreEqual((short)0xB00B, binary[12]);
			Assert::AreEqual((short)0xC00C, binary[13]);
			Assert::AreEqual((short)0xD00D, binary[14]);
			Assert::AreEqual((short)0xE00E, binary[15]);
			Assert::AreEqual((short)0xF000, binary[16]);
			Assert::AreEqual((short)0x0001, binary[17]);
			Assert::AreEqual((short)0x0002, binary[18]);
			Assert::AreEqual(log.find("エラー"), std::string::npos);
			Assert::AreEqual(log.find("警告"), std::string::npos);
		}

		TEST_METHOD(TestLabel)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x100" << std::endl;
			stream << "  LD Label0" << std::endl;
			stream << "  ST Label1" << std::endl;
			stream << "  ADD Label2" << std::endl;
			stream << "  SUB Label3" << std::endl;
			stream << "  AND Label4" << std::endl;
			stream << "  OR Label5" << std::endl;
			stream << "  B Label6" << std::endl;
			stream << "  BZ Label7" << std::endl;
			stream << "  BN Label8" << std::endl;
			stream << "  HLT" << std::endl;
			stream << "  DS 6" << std::endl;
			stream << "Label0  DC 0" << std::endl;
			stream << "Label1  DC 0" << std::endl;
			stream << "Label2  DC 0" << std::endl;
			stream << "Label3  DC 0" << std::endl;
			stream << "Label4  DC 0" << std::endl;
			stream << "Label5  DC 0" << std::endl;
			stream << "Label6  DC 0" << std::endl;
			stream << "Label7  DC 0" << std::endl;
			stream << "Label8  DC 0" << std::endl;
			stream << "  END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary).c_str();
			Logger::WriteMessage(log.c_str());

			Assert::AreEqual((short)0x1100, binary[0]);
			Assert::AreEqual((short)0x0110, binary[1]);
			Assert::AreEqual((short)0x1111, binary[2]);
			Assert::AreEqual((short)0x2112, binary[3]);
			Assert::AreEqual((short)0x3113, binary[4]);
			Assert::AreEqual((short)0x4114, binary[5]);
			Assert::AreEqual((short)0x5115, binary[6]);
			Assert::AreEqual((short)0x6116, binary[7]);
			Assert::AreEqual((short)0x7117, binary[8]);
			Assert::AreEqual((short)0x8118, binary[9]);
			Assert::AreEqual((short)0xF000, binary[10]);
			Assert::AreEqual(log.find("エラー"), std::string::npos);
			Assert::AreEqual(log.find("警告"), std::string::npos);
		}

		TEST_METHOD(TestTitleOrg)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0xABC" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log =compiler.Compile(&binary).c_str();
			Logger::WriteMessage(log.c_str());

			Assert::AreEqual(std::string("TEST"), binary.GetTitle());
			Assert::AreEqual(0xABC, binary.GetORG());
			Assert::AreEqual(log.find("エラー"), std::string::npos);
			Assert::AreEqual(log.find("警告"), std::string::npos);
		}

		TEST_METHOD(TestTitleValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		0TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOperandTitleInvalid("0TEST")), std::string::npos);
		}

		TEST_METHOD(TestOrgValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		-123" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOperandOrgInvalid("-123")), std::string::npos);
		}

		TEST_METHOD(TestDs)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	DS 1024" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary).c_str();
			Logger::WriteMessage(log.c_str());

			// サイズはHeaderSize(1) + DS(1024)になる
			Assert::AreEqual((int)((1 + 1024) * sizeof(short)), binary.GetSize());
			Assert::AreEqual(log.find("エラー"), std::string::npos);
			Assert::AreEqual(log.find("警告"), std::string::npos);
		}

		TEST_METHOD(TestDsValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	DS -1024" << std::endl;
			stream << "	DS +1024" << std::endl;
			stream << "	DS ABC" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOperandDsInvalid("-1024")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandDsInvalid("+1024")) , std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandDsInvalid("ABC")), std::string::npos);
		}

		TEST_METHOD(TestDc)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	DC 0xABCD" << std::endl;
			stream << "	DC 0xABC" << std::endl;
			stream << "	DC 10" << std::endl;
			stream << "	DC +10" << std::endl;
			stream << "	DC -10" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary).c_str();
			Logger::WriteMessage(log.c_str());

			Assert::AreEqual((short)0xABCD, binary[1]);
			Assert::AreEqual((short)0xABC, binary[2]);
			Assert::AreEqual((short)10, binary[3]);
			Assert::AreEqual((short)10, binary[4]);
			Assert::AreEqual((short)-10, binary[5]);
			Assert::AreEqual(log.find("エラー"), std::string::npos);
			Assert::AreEqual(log.find("警告"), std::string::npos);
		}

		TEST_METHOD(TestDcValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	DC 0xDEFG" << std::endl;
			stream << "	DC 0x-ABC" << std::endl;
			stream << "	DC -0xABC" << std::endl;
			stream << "	DC +0xABC" << std::endl;
			stream << "	DC +-0xABC" << std::endl;
			stream << "	DC +-10" << std::endl;
			stream << "	DC ABC" << std::endl;
			stream << "	DC 1-0" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOperandInvalidHex("0xDEFG")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalidSymbol("0x-ABC")) , std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandDcInvalid("-0xABC")) , std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandDcInvalid("+0xABC")) , std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalidSymbol("+-0xABC")) , std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalidSymbol("+-10")) , std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandDcInvalid("ABC")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalidSymbol("1-0")) , std::string::npos);
		}

		TEST_METHOD(TestOpecodeValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	UND 0xABC" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOpecodeInvalid("UND")), std::string::npos);
		}

		TEST_METHOD(TestLabelValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "0label		DS 1" << std::endl;
			stream << "label!		DS 1" << std::endl;
			stream << "label		DS 1" << std::endl;
			stream << "label		DS 1" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errLabelMustBeStartedAlphabet("0label")), std::string::npos);
			Assert::AreNotEqual(log.find(_errLabelInvalidSymbol("label!")), std::string::npos);
			Assert::AreNotEqual(log.find(_errLabelDuplication("label")), std::string::npos);
		}

		TEST_METHOD(TestUnnecessaryOperandValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "	HLT		0x001" << std::endl;
			stream << "	END 0x002" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errUnnecessaryOperand("HLT", "0x001")), std::string::npos);
			Assert::AreNotEqual(log.find(_errUnnecessaryOperand("END", "0x002")), std::string::npos);
		}

		TEST_METHOD(TestMissingOperandValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "  LD" << std::endl;
			stream << "  ST" << std::endl;
			stream << "  ADD" << std::endl;
			stream << "  SUB" << std::endl;
			stream << "  AND" << std::endl;
			stream << "  OR" << std::endl;
			stream << "  B" << std::endl;
			stream << "  BZ" << std::endl;
			stream << "  BN" << std::endl;
			stream << "  OPE1" << std::endl;
			stream << "  OPE2" << std::endl;
			stream << "  OPE3" << std::endl;
			stream << "  OPE4" << std::endl;
			stream << "  OPE5" << std::endl;
			stream << "  OPE6" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errMissingOperand("LD")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("ST")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("ADD")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("SUB")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("AND")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OR")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("B")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("BZ")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("BN")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OPE1")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OPE2")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OPE3")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OPE4")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OPE5")), std::string::npos);
			Assert::AreNotEqual(log.find(_errMissingOperand("OPE6")), std::string::npos);
		}

		TEST_METHOD(TestSignedOperandValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "  LD -0x001" << std::endl;
			stream << "  ST -0x002" << std::endl;
			stream << "  ADD -0x003" << std::endl;
			stream << "  SUB -0x004" << std::endl;
			stream << "  AND -0x005" << std::endl;
			stream << "  OR -0x006" << std::endl;
			stream << "  B -0x007" << std::endl;
			stream << "  BZ -0x008" << std::endl;
			stream << "  BN -0x009" << std::endl;
			stream << "  OPE1 -0x00A" << std::endl;
			stream << "  OPE2 -0x00B" << std::endl;
			stream << "  OPE3 -0x00C" << std::endl;
			stream << "  OPE4 -0x00D" << std::endl;
			stream << "  OPE5 -0x00E" << std::endl;
			stream << "  OPE6 -0x00F" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x001")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x002")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x003")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x004")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x005")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x006")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x007")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x008")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x009")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x00A")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x00B")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x00C")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x00D")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x00E")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("-0x00F")), std::string::npos);
		}

		TEST_METHOD(TestHexOperandValidation)
		{
			std::stringstream stream;
			stream << "	TITLE		TEST" << std::endl;
			stream << "	ORG		0x000" << std::endl;
			stream << "  LD 1" << std::endl;
			stream << "  ST 2" << std::endl;
			stream << "  ADD 3" << std::endl;
			stream << "  SUB 4" << std::endl;
			stream << "  AND 5" << std::endl;
			stream << "  OR 6" << std::endl;
			stream << "  B 7" << std::endl;
			stream << "  BZ 8" << std::endl;
			stream << "  BN 9" << std::endl;
			stream << "  OPE1 10" << std::endl;
			stream << "  OPE2 11" << std::endl;
			stream << "  OPE3 12" << std::endl;
			stream << "  OPE4 13" << std::endl;
			stream << "  OPE5 14" << std::endl;
			stream << "  OPE6 15" << std::endl;
			stream << "	END" << std::endl;

			Compiler compiler;
			Binary binary;

			compiler.SetStream(&stream);
			std::string log = compiler.Compile(&binary);
			Logger::WriteMessage(log.c_str());

			Assert::AreNotEqual(log.find(_errOperandInvalid("1")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("2")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("3")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("4")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("5")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("6")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("7")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("8")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("9")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("10")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("11")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("12")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("13")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("14")), std::string::npos);
			Assert::AreNotEqual(log.find(_errOperandInvalid("15")), std::string::npos);
		}
	};
}

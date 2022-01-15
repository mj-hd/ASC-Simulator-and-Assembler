/*
 * core/binary.cpp
 */

#include "binary.h"

#include <fstream>
#include <string>
#include <cstdlib>

/**
 * バイナリフォーマット
 * 0x0000: | headerSize 4bit | ORG 12bit |
 * ...
 * 0x000{headerSize}: | プログラム部 16bit |
 * ...
 */
Binary::Binary() {
  this->_Index = 0;
  this->_Capacity = 0;

  this->_Data = (short*)std::malloc(sizeof(short));
  if (this->_Data == NULL) throw "[Binary]バッファの確保に失敗しました。";

  this->_Data[0] = 0;
}

Binary::~Binary() {
  free(this->_Data);
}

short Binary::operator << (short mnemonic) {
  this->_Index += 1;

  if (this->_Capacity < this->_Index + 1) {
    this->_Capacity += 32;
    short* newData = (short*)std::realloc(this->_Data, this->_Capacity * sizeof(short));
    if (newData == NULL) throw "[Binary]バッファの確保に失敗しました。";
    this->_Data = newData;
  }

  this->_Data[this->_Index] = mnemonic;

  return mnemonic;
}

short Binary::operator[](unsigned short i) {
  if (i > this->_Index) {
    throw "[Binary]範囲外へのアクセスです";
  }

  return this->_Data[i];
}

void Binary::WriteToStream(std::ostream* stream) {
  stream->write( (char *)this->_Data, GetSize() );
}

int Binary::GetIndex() {
	return this->GetORG()+this->_Index-1;
}

int Binary::GetSize() {
  return sizeof(short) *(this->_Index +1);
}

int Binary::GetORG() {
  short headerSize = this->_Data[0] >> 12;

  if (headerSize < 1) return 0;

  return this->_Data[0] & 4095;
}
void Binary::SetORG(int org) {
  short headerSize = this->_Data[0] >> 12;

  if (headerSize < 1) headerSize = 1;
  
  this->_Data[0] = headerSize << 12;
  this->_Data[0] += org;
}


std::string Binary::GetTitle() {
  return this->_Title;
}
void Binary::SetTitle(std::string name) {
  this->_Title = name;
}

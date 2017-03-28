/*
 * core/binary.cpp
 */

#include "binary.h"

#include <fstream>
#include <string>
#include <cstdlib>

Binary::Binary() {
  this->_Index = 0;

  this->_Data = (short*)std::malloc(sizeof(short));
  if (this->_Data == NULL) throw "[Binary]バッファの確保に失敗しました。";

  this->_Data[0] = 0;
}

Binary::~Binary() {
  free(this->_Data);
}

short Binary::operator << (short mnemonic) {
  this->_Data = (short*)std::realloc(this->_Data, (++(this->_Index)+1) * sizeof(short));
  this->_Data[this->_Index] = mnemonic;

  return mnemonic;
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
  
  this->_Data[0] = 0;
  this->_Data[0] = headerSize;
  this->_Data[0] = this->_Data[0] << 12;
  this->_Data[0] += org;
}


std::string Binary::GetTitle() {
  return this->_Title;
}
void Binary::SetTitle(std::string name) {
  this->_Title = name;
}

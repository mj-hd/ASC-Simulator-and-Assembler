/*
 * core/binary.h
 */

#ifndef CORE_BINARY_H
#define CORE_BINARY_H

#include <fstream>
#include <string>

class Binary {
public:
  Binary();
  ~Binary();

  void WriteToStream(std::ostream*);
  
  unsigned int GetIndex();
  unsigned int GetSize();
  unsigned short GetORG();
  void SetORG(unsigned short);
  void SetTitle(std::string);
  std::string GetTitle();

  unsigned short operator << (unsigned short);
  unsigned short operator[](unsigned short);

private:
  unsigned short* _Data;
  unsigned int _Index;
  unsigned int _Capacity;
  unsigned int _HeaderSize;
  std::string _Title;
};

#endif

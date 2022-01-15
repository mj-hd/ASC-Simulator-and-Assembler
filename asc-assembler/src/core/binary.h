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
  
  int GetIndex();
  int GetSize();
  int GetORG();
  void SetORG(int);
  void SetTitle(std::string);
  std::string GetTitle();

  short operator << (short);
  short operator[](unsigned short);

private:
  short* _Data;
  int _Index;
  int _Capacity;
  std::string _Title;
};

#endif

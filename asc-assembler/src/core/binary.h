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
  void        SetTitle(std::string);
  std::string GetTitle();

  short operator << (short);

private:
  short* _Data;
  int _Index;
  std::string _Title;
};

#endif

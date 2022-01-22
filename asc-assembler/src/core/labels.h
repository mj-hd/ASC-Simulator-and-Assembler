/*
 * labels.h
 */

#ifndef CORE_LABELS_H
#define CORE_LABELS_H

#include <map>
#include <string>
#include <list>

class Labels {
public:
  Labels();
  ~Labels();

  unsigned short operator [] (std::string);
  std::string operator [] (unsigned short);

  std::list<std::string> GetNames();

  void SetBaseAddress(unsigned short);
  short Search(std::string);
  std::string Search(unsigned short);
  void Register(std::string, unsigned short);
  void Deregister(std::string);
  void Clear();

private:
  std::string _Chop(std::string);
  std::map<std::string, unsigned short> _Labels;
  unsigned short _BaseAddress;
};

#endif // CORE_LABELS_H

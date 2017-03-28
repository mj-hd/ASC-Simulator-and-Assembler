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

  short operator [] (std::string);
  std::string operator [] (short);

  std::list<std::string> GetNames();

  void SetBaseAddress(short);
  short Search(std::string);
  std::string Search(short);
  void Register(std::string, int);
  void Deregister(std::string);
  void Clear();

private:
  std::string _Chop(std::string);
  std::map<std::string, int> _Labels;
  short _BaseAddress;
};

#endif // CORE_LABELS_H

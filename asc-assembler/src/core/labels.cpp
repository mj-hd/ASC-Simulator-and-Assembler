/*
 * core/labels.cpp
 */

#include "labels.h"

#include <map>
#include <string>

Labels::Labels() {
}
Labels::~Labels() {
}

short Labels::operator [] (std::string label) {
  return Search(label);
}

std::string Labels::operator [] (short address) {
  return Search(address);
}

std::list<std::string> Labels::GetNames() {
	std::list<std::string> result;

	std::map<std::string, int>::iterator it = this->_Labels.begin();
	for (; it != this->_Labels.end(); it++) {
		result.push_back((*it).first);
	}

	return result;
}

void Labels::SetBaseAddress(short base) {
  this->_BaseAddress = base;
}
short Labels::Search(std::string label) {
  if (this->_Labels.count(label) < 1) return -1;
  return this->_Labels[label] + this->_BaseAddress;
}
std::string Labels::Search(short address) {

  std::map<std::string, int>::iterator it = this->_Labels.begin();
  for (; it != this->_Labels.end(); it++) {
    if ((*it).second == address+this->_BaseAddress) {
      return (*it).first;
    }
  }

  return "";
}

void Labels::Register(std::string label, int index) {
	if (Search(label) == -1) {
		this->_Labels.insert(this->_Labels.end(), std::pair<std::string, int>(_Chop(label), index));
	}
}

void Labels::Deregister(std::string label) {
  this->_Labels.erase(label);
}

void Labels::Clear() {
  this->_Labels.clear();
}

std::string Labels::_Chop(std::string raw) {
  if (raw[raw.length()-1] == ':') {
    return raw.substr(0, raw.length()-1);
  } else {
    return raw;
  }
}

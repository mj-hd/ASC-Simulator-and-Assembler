#pragma once

#include <string>
#include <unicode/ucsdet.h>
#include <unicode/ucnv.h>

std::string convertToSjis(std::string src) {
	auto error = U_ZERO_ERROR;
	auto detector = ucsdet_open(&error);

	if (U_FAILURE(error)) {
		return src;
	}

	ucsdet_setText(detector, src.c_str(), src.size(), &error);

	if (U_FAILURE(error)) {
		ucsdet_close(detector);
		return src;
	}

	auto match = ucsdet_detect(detector, &error);

	if (U_FAILURE(error)) {
		ucsdet_close(detector);
		return src;
	}

	auto name = ucsdet_getName(match, &error);

	if (U_FAILURE(error)) {
		ucsdet_close(detector);
		return src;
	}

	ucsdet_close(detector);

	// �댟�m�ɂ�镶��������h�����߁ASJIS��UTF8�̂ݑΉ�
	if (strcmp(name, "UTF-8") != 0) {
		return src;
	}
	auto toConverter = ucnv_open("MS932", &error);

	if (U_FAILURE(error)) {
		return src;
	}

	auto fromConverter = ucnv_open(name, &error);

	if (U_FAILURE(error)) {
		ucnv_close(toConverter);
		return src;
	}

	// �傫�����\��
	auto internalMaxLen = ucnv_getMaxCharSize(fromConverter) * src.size();
	auto internalBuffer = (UChar*)malloc(sizeof(UChar) * internalMaxLen);

	if (internalBuffer == nullptr) {
		ucnv_close(toConverter);
		ucnv_close(fromConverter);
		return src;
	}

	auto len = ucnv_toUChars(
		fromConverter,
		internalBuffer,
		internalMaxLen,
		src.c_str(),
		src.size(),
		&error
	);

	if (U_FAILURE(error)) {
		free(internalBuffer);
		ucnv_close(toConverter);
		ucnv_close(fromConverter);
		return src;
	}

	auto resultMaxLen = ucnv_getMaxCharSize(toConverter) * len;
	auto resultBuffer = (char*)malloc(sizeof(char) * resultMaxLen);

	if (resultBuffer == nullptr) {
		free(internalBuffer);
		ucnv_close(toConverter);
		ucnv_close(fromConverter);
		return src;
	}

	auto result = ucnv_fromUChars(
		toConverter,
		resultBuffer,
		resultMaxLen,
		internalBuffer,
		len,
		&error
	);

	if (U_FAILURE(error)) {
		free(internalBuffer);
		ucnv_close(toConverter);
		ucnv_close(fromConverter);
		return src;
	}

	auto dest = std::string(resultBuffer);

	free(internalBuffer);
	ucnv_close(toConverter);
	ucnv_close(fromConverter);

	return dest;
}

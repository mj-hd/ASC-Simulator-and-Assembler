; 符号付き数のテスト
	TITLE TEST5
	ORG 0x100

	LD Data1
	LD Data2
	LD Data3
	LD Data4
	LD Data5
	HLT

Data1	DC -0x8000
Data2	DC +0x7FFF
Data3	DC -0xFFFF
Data4	DC +1
Data5	DC -1
Data6	DC -0x0001
Data7	DC +0x0001
Data8	DC -32768
Data9	DC +32768
	END
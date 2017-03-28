; test4.asm
; 存在しないラベルへのアクセス

	TITLE TEST4
	ORG 0x0000

Data1	LD	Data2
	B	DATA1

Data2	HLT

	END
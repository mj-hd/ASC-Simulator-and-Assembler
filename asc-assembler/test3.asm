; TEST3
; END���Ȃ����e�X�g

		TITLE ADD
		ORG 0x10

Data1	DC 5  ;�f�[�^1
Data2	DC 10 ; �f�[�^2
A		DC 1
B		DC 2
C		DC 3

Main	LD Data1; LD Data1
		ADD Data2
		SUB A
		SUB B
		SUB C
		HLT
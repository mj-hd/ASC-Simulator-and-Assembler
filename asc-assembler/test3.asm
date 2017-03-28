; TEST3
; ENDを省いたテスト

		TITLE ADD
		ORG 0x10

Data1	DC 5  ;データ1
Data2	DC 10 ; データ2
A		DC 1
B		DC 2
C		DC 3

Main	LD Data1; LD Data1
		ADD Data2
		SUB A
		SUB B
		SUB C
		HLT
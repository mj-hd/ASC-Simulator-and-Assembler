; TEST1
; ���x��Result�̒l��30�ɂȂ�܂ŉ��Z���A��~

		TITLE TEST1
		ORG 0x10

		B Init

Zero	DC 0

One		DC 1
Thirty	DC 30
Result	DS 1

Init
		LD Zero ; R��������

Loop
		LD One ; R��1��ǂݍ���
		ADD Result ; Result��R�����Z����
		ST Result ; �v�Z���ʂ��i�[����
		LD Thirty
		SUB Result
		BN Continue
		B End

Continue
		B Loop

; for (R = 0; R < 30; R++);

End
		HLT
		END
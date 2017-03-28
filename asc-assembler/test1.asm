; TEST1
; ƒ‰ƒxƒ‹Result‚Ì’l‚ğ30‚É‚È‚é‚Ü‚Å‰ÁZ‚µA’â~

		TITLE TEST1
		ORG 0x10

		B Init

Zero	DC 0

One		DC 1
Thirty	DC 30
Result	DS 1

Init
		LD Zero ; R‚ğ‰Šú‰»

Loop
		LD One ; R‚É1‚ğ“Ç‚İ‚Ş
		ADD Result ; Result‚ÆR‚ğ‰ÁZ‚·‚é
		ST Result ; ŒvZŒ‹‰Ê‚ğŠi”[‚·‚é
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
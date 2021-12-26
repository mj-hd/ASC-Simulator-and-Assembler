@echo off
SET PATH=%~p0\assembler;%PATH%

asc-assembler.exe %1

echo;

PAUSE
exit
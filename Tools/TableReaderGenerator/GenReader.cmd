echo off
TableReaderGenerator.exe

xcopy *.cs ..\..\Src\Entitas.Data\DataReader\ /y/d 

del /f/q *.cs *.dsl

pause
exit /b %ec%
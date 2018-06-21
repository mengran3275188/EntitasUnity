echo off
TableReaderGenerator.exe

xcopy *.cs ..\..\Src\Entitas.Data\DataReader\ /y/d 

pause
exit /b %ec%
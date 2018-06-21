echo off
table_reader_generator.exe

xcopy *.cs ..\..\Src\Entitas.Data\DataReader\ /y/d 

pause
exit /b %ec%
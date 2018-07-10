echo off
TableReaderGenerator.exe

xcopy DataReader.cs ..\..\Src\Entitas.Data\DataReader\ /y/d 
xcopy FilePathDefine.cs ..\..\Src\Entitas.Data\DataReader\ /y/d 
xcopy AttributeConfigProvider.cs ..\..\Src\Entitas.Data\DataReader\ /y/d 

xcopy AttributeData.cs ..\..\Src\Entitas.Data\ /y/d 
xcopy AttributeEnum.cs ..\..\Src\Entitas.Data\ /y/d 

del /f/q *.cs *.dsl

pause
exit /b %ec%

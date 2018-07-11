echo off

set is_pause=True

set homedir=%~dp0
set workdir=%homedir%\Src
set plugindir=%homedir%\Resource\Unity\Assets\Plugins
set tabledir=%homedir%\Resource\Unity\Assets\StreamingAssets\Tables\

set xbuild=%homedir%\Tools\Msbuild\MSBuild.exe
set pdb2mdb=%homedir%\Tools\Xbuild\pdb2mdb.exe

set logdir=%workdir%\Buildlog

mkdir %logdir%

echo building UnityClient.sln ...
%xbuild% /m /nologo /noconsolelogger /property:Configuration=Debug ^
         /flp:LogFile=%logdir%\UnityClient.sln.log;Encoding=UTF-8 ^
		 /t:clean;rebuild ^
         %workdir%\UnityClient.sln
if NOT %ERRORLEVEL% EQU 0 (
  echo build failed, check %logdir%\UnityClient.sln.log.
  goto error_end
) else (
  echo done.
)

echo [client]: generate *mdb debug files for mono

pushd %workdir%\Bin
for /r %%i in (*.pdb) do (
  %pdb2mdb% %%~dpni.dll
)
popd

rem copy dll to unity3d's plugin directory
echo "update binaries"
xcopy %workdir%\Bin\*.dll %plugindir% /y /q
xcopy %workdir%\Bin\*.mdb %plugindir% /y /q

del /a /f %plugindir%\UnityEngine.dll
del /a /f %plugindir%\UnityEngine.UI.dll

if NOT %ERRORLEVEL% EQU 0 (
  echo copy failed, exclusive access error? check your running process and retry.
  goto error_end
) else (
  echo done.
)

rem copy tables to unity3d’s streaming directory
echo "update tables"
xcopy %homedir%\Resource\Tables %tabledir% /s /y /q

if NOT %ERRORLEVEL% EQU 0 (
  echo copy failed, exclusive access error? check your running process and retry.
  goto error_end
) else (
  echo done.
)

goto good_end

:error_end
set ec=1
goto end
:good_end
set ec=0
echo All Done, Good to Go.
:end
if %is_pause% EQU True (
  pause
  exit /b %ec%
)
echo off
cls
set "basepath=%2"
set "wpa=%basepath%Salesforce.SDK.Core.NuGet\build\wpa"
set "win=%basepath%Salesforce.SDK.Core.NuGet\build\win"
set "libwin=%basepath%Salesforce.SDK.Core.NuGet\lib\win"
set "libwpa=%basepath%Salesforce.SDK.Core.NuGet\lib\wpa"

if %1 == "release" (goto :release) else (goto :debug)

:release
set "mode=Release"
goto :copy

:debug
set "mode=Debug"
goto :copy

:copy
xcopy /y %basepath%Salesforce.SDK.Phone\bin\ARM\%mode%\Salesforce.SDK.* %wpa%\ARM
xcopy /y %basepath%Salesforce.SDK.Phone\bin\x86\%mode%\Salesforce.SDK.* %wpa%\x86
xcopy /y %basepath%Salesforce.SDK.Store\bin\ARM\%mode%\Salesforce.SDK.* %win%\ARM
xcopy /y %basepath%Salesforce.SDK.Store\bin\x86\%mode%\Salesforce.SDK.* %win%\x86
xcopy /y %basepath%Salesforce.SDK.Store\bin\x64\%mode%\Salesforce.SDK.* %win%\x86

xcopy /y %basepath%Salesforce.SDK.Phone\bin\ARM\%mode%\Salesforce.SDK.* %libwpa%
xcopy /y %basepath%Salesforce.SDK.Store\bin\ARM\%mode%\Salesforce.SDK.* %libwin%
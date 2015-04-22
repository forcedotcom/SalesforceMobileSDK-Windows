echo off
cls
set "basepath=%2"
set "wpa=%basepath%Salesforce.SDK.Core.NuGet\build\wpa"
set "win=%basepath%Salesforce.SDK.Core.NuGet\build\win"
set "libwin=%basepath%Salesforce.SDK.Core.NuGet\lib\win"
set "libwpa=%basepath%Salesforce.SDK.Core.NuGet\lib\wpa"

if "%1" == "Release" (goto :release) else (goto :debug)

:release
set "mode=Release"
goto :copy

:debug
set "mode=Debug"
goto :copy

:copy
echo %mode%
echo %1
xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\ARM\%mode%\Salesforce.SDK.* %wpa%\ARM
xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\x86\%mode%\Salesforce.SDK.* %wpa%\x86
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\ARM\%mode%\Salesforce.SDK.* %win%\ARM
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\x86\%mode%\Salesforce.SDK.* %win%\x86
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\x64\%mode%\Salesforce.SDK.* %win%\x64
xcopy /s /y %basepath%Salesforce.SDK.App\bin\ARM\%mode%\Salesforce.SDK.* %wpa%\ARM
xcopy /s /y %basepath%Salesforce.SDK.App\bin\x86\%mode%\Salesforce.SDK.* %wpa%\x86
xcopy /s /y %basepath%Salesforce.SDK.App\bin\ARM\%mode%\Salesforce.SDK.* %win%\ARM
xcopy /s /y %basepath%Salesforce.SDK.App\bin\x86\%mode%\Salesforce.SDK.* %win%\x86
xcopy /s /y %basepath%Salesforce.SDK.App\bin\x64\%mode%\Salesforce.SDK.* %win%\x64

xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\ARM\%mode%\Salesforce.SDK.* %libwpa%\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\ARM\%mode%\Salesforce.SDK.* %libwin%\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %libwpa%\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %libwin%\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %libwpa%\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %libwin%\Salesforce.SDK.App\

xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\ARM\%mode%\*.xbf %wpa%\ARM\Salesforce.SDK.Phone\
xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\x86\%mode%\*.xbf %wpa%\x86\Salesforce.SDK.Phone\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\ARM\%mode%\*.xbf %win%\ARM\Salesforce.SDK.Store\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\x86\%mode%\*.xbf %win%\x86\Salesforce.SDK.Store\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\x86\%mode%\*.xbf %win%\x64\Salesforce.SDK.Store\

xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\ARM\%mode%\*.xr.xml %wpa%\ARM\Salesforce.SDK.Phone\
xcopy /s /y %basepath%Salesforce.SDK.Phone\bin\x86\%mode%\*.xr.xml %wpa%\x86\Salesforce.SDK.Phone\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\ARM\%mode%\*.xr.xml %win%\ARM\Salesforce.SDK.Store\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\x86\%mode%\*.xr.xml %win%\x86\Salesforce.SDK.Store\
xcopy /s /y %basepath%Salesforce.SDK.Store\bin\x64\%mode%\*.xr.xml %win%\x64\Salesforce.SDK.Store\

xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %wpa%\ARM\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %wpa%\x86\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %win%\ARM\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %win%\x86\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xbf %win%\x64\Salesforce.SDK.App\

xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %wpa%\ARM\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %wpa%\x86\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %win%\ARM\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %win%\x86\Salesforce.SDK.App\
xcopy /s /y %basepath%Salesforce.SDK.App\obj\%mode%\*.xr.xml %win%\x64\Salesforce.SDK.App\
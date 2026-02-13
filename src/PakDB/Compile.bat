@if "%~1"=="" goto skip

@setlocal enableextensions
@pushd %~dp0
.\repak.exe pack "%~1" "%~1_P.pak"
@popd
@exit

:skip
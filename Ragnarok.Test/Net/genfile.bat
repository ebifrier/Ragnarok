@echo off

set PROTOGEN="%HOME%\..\bin\protogen.exe"

%PROTOGEN% -i:ProtoBufObject.proto -o:ProtoBufObject.cs

if not %ERRORLEVEL%==0 goto on_error

echo OK

goto on_end

:on_error
echo ÉGÉâÅ[Ç™î≠ê∂ÇµÇ‹ÇµÇΩÅB

:on_end
pause

@echo on
call "%VS110COMNTOOLS%vsvars32.bat"

msbuild.exe /ToolsVersion:4.0 "..\ReactiveMarrow\ReactiveMarrow\ReactiveMarrow.csproj" /p:configuration=Release

mkdir ..\Release

copy ..\ReactiveMarrow\ReactiveMarrow\bin\Release\ReactiveMarrow.dll ..\Release\ReactiveMarrow.dll
copy ..\ReactiveMarrow\ReactiveMarrow\bin\Release\ReactiveMarrow.xml ..\Release\ReactiveMarrow.xml

pause
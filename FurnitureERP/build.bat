@echo off
"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" FurnitureERP.csproj /p:Configuration=Debug /t:Build > build_output.txt 2>&1
echo Exit code: %ERRORLEVEL% >> build_output.txt

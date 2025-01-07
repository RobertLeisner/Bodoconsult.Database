@echo Off

set config=Release
set version=-Version 1.0.0

REM Build
"%programfiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" Bodoconsult.Database.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package
mkdir Build
REM call dotnet nuget pack "Bodoconsult.Database\Bodoconsult.Database.csproj" -symbols -o Build -p Configuration=%config% %version%
REM call dotnet nuget pack "Bodoconsult.Database.Sqlite\Bodoconsult.Database.Sqlite.csproj" -symbols -o Build -p Configuration=%config% %version%

pause
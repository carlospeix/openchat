
rmdir /S /Q .\Coverage\

rem https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
dotnet test --collect:"XPlat Code Coverage" --results-directory:Coverage -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.IncludeTestAssembly=true
rem --settings coverlet.runsettings

REM Movemos los archivos a .\Converage\
CD Coverage
FOR /D %%G in (*.) DO (
 ECHO %%G
 PUSHD %%G
 copy coverage.cobertura.xml ..\%%G.coverage.cobertura.xml
 POPD)
CD ..

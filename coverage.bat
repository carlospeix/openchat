
rem https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
dotnet test --collect:"XPlat Code Coverage" --results-directory:Coverage rem --settings coverlet.runsettings

rem https://github.com/danielpalme/ReportGenerator
reportgenerator "-reports:Coverage\coverage.cobertura.xml" "-targetdir:Coverage\html" -reporttypes:HTML;

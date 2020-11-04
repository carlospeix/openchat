
rmdir /S /Q .\Coverage\html

rem https://github.com/danielpalme/ReportGenerator
reportgenerator -reports:Coverage\*.coverage.cobertura.xml -targetdir:Coverage\html -reporttypes:HTML;

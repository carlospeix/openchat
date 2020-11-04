CD Coverage
FOR /D %%G in (*.) DO (
 ECHO %%G
 PUSHD %%G
 copy coverage.cobertura.xml ..\%%G.coverage.cobertura.xml
 POPD)
CD ..
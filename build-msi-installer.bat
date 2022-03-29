pushd %CD%\build
dotnet tool restore
dotnet script main.csx
popd
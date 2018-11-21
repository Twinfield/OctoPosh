set binaryVersion=%1

IF %1.==. set binaryVersion=0.0.0.1

powershell .\build.ps1 -Verbosity Diagnostic -BuildPowershellModule -BinaryVersion %binaryVersion%
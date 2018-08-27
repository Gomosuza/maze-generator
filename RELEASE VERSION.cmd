msbuild "MazeGenerator.sln" /t:Build /p:Configuration=Release /p:RestorePackages=true /nr:false

mkdir !Releases
echo f|xcopy bin\Release\MazeGenerator.exe !Releases\MazeGenerator.exe /y /i /s
echo f|xcopy bin\Release\settings.ini !Releases\settings.ini /y /i /s
set msbuild_VS15=C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe

"%msbuild_VS15%" "MazeGenerator.sln" /t:Build /p:Configuration=Release /p:RestorePackages=true /nr:false

mkdir !Releases
echo f|xcopy bin\Release\MazeGenerator.exe !Releases\MazeGenerator.exe /y /i /s
echo f|xcopy bin\Release\settings.ini !Releases\settings.ini /y /i /s
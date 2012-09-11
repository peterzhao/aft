@echo off

echo Note: Use of this script requires sqlcmd.exe and .NET Framework 4.0. sqlcmd.exe is part of the SQL Studio Manager Tools.

IF [%1]==[] (
	echo Please provide environment as first parameter
	goto :eof
	
)

%windir%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe SalsaImporter.build /t:Deploy /p:Env=%1


set current_folder=%cd%
for %%a in ("%current_folder%") do set folder_name=%%~nxa
echo Current folder name: %folder_name%
if "%folder_name%" neq "bin" cd PacketGenerator\\protoc-3.12.3-win64\\bin

protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../Assets/Scripts/Managers/Content/Network/Common/Packet"
XCOPY /Y ClientPacketManager.cs "../../../Assets/Scripts/Managers/Content/Network/Client/Packet"
XCOPY /Y ServerPacketManager.cs "../../../Assets/Scripts/Managers/Content/Network/Server/Packet"
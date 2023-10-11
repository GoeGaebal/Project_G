protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../Assets/Scripts/Managers/Content/Network/Common/Packet"
XCOPY /Y ClientPacketManager.cs "../../../Assets/Scripts/Managers/Content/Network/Client/Packet"
XCOPY /Y ServerPacketManager.cs "../../../Assets/Scripts/Managers/Content/Network/Server/Packet"
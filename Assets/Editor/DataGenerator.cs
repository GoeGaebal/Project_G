using UnityEngine;

#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.Tilemaps;
#endif

public class DataGenrator
{
#if UNITY_EDITOR

    // % (Ctrl), # (Shift), & (Alt)
    [MenuItem("Tools/GenerateData")]
    private static void GenerateMap()
    {
        Process.Start("DataBase\\ConvertExcelToJson.bat");
    }
    
    [MenuItem("Tools/GeneratePacket")]
    private static void GeneratePacket()
    {
        Process.Start("PacketGenerator\\protoc-3.12.3-win64\\bin\\GenProto.bat");
    }
#endif
}
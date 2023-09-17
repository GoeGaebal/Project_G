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
#endif
}
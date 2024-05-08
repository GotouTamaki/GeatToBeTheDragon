using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateHLSLFile
{
    [MenuItem("Assets/Create/Shader/HLSL File", false, 100)]
    public static void CreateNewHLSLFile()
    {
        var selectedAsset = Selection.activeObject;
        string selectedPath = AssetDatabase.GetAssetPath(selectedAsset);

        if (selectedAsset == null || string.IsNullOrEmpty(selectedPath))
        {
            selectedPath = "Assets";
        }

        string fullPath = Path.Combine(selectedPath, "NewHLSL.hlsl");
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        File.WriteAllText(fullPath, string.Empty);
        AssetDatabase.Refresh();
    }
}
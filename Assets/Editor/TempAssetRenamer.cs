using UnityEngine;
using UnityEditor;
using System.IO;

public class TempAssetRenamer : EditorWindow
{
    [MenuItem("Tools/Batch Rename Assets")]
    public static void ShowWindow()
    {
        GetWindow<TempAssetRenamer>("Batch Rename Assets");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Rename Assets"))
        {
            RenameAssets();
        }
    }

    private static void RenameAssets()
    {
        // The folder where your assets are located. You can specify the full path or a relative path.
        string folderPath = "Assets/ScriptableObject/Mob/"; // Change this to the folder where the assets are located.
        
        // Get all assets in the folder and subfolders.
        string[] assetPaths = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        int renamedCount = 0;

        foreach (string assetPath in assetPaths)
        {
            if (Path.GetExtension(assetPath) != ".meta") // Ignore .meta files
            {
                string fileName = Path.GetFileNameWithoutExtension(assetPath);

                if (fileName.Contains("[") || fileName.Contains("]"))
                {
                    string newFileName = fileName.Replace("[", "(").Replace("]", ")");

                    string newAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), newFileName + Path.GetExtension(assetPath));

                    AssetDatabase.RenameAsset(assetPath, newFileName);
                    renamedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Renamed {renamedCount} assets.");
    }
}
using System.IO;
using UnityEditor;
using UnityEngine;
using Aspose.Cells;


public class EditorHelper
{
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void CreateNewEditorAsset<T>(string _assetName) where T : ScriptableObject {

        // 
        string assetPath = @"Assets/Editor/Asset";
        string path = Path.Combine(assetPath, _assetName + ".asset");

        bool doCreate = true;
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists) {
            doCreate = EditorUtility.DisplayDialog(_assetName + " already exists.",
                                                   "Do you want to overwrite the old one?",
                                                   "Yes", "No");
        }
        if (doCreate) {

            // check if the asset is valid to create
            if (new DirectoryInfo(assetPath).Exists == false) {
                Debug.LogError("can't create asset, path not found");
                return;
            }
            if (string.IsNullOrEmpty(_assetName)) {
                Debug.LogError("can't create asset, the name is empty");
                return;
            }

            //
            T newAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newAsset, path);
            Selection.activeObject = newAsset;
        }
    }

    // ------------------------------------------------------------------ 
    // 
    // ------------------------------------------------------------------

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static Worksheet LoadExcelSheet(string _excelFilePath, string _sheetName)
    {
        Workbook book = new Workbook(_excelFilePath);
        foreach (Worksheet sheet in book.Worksheets)
        {
            if (sheet.Name == _sheetName)
            {
                return sheet;
            }
        }
        Debug.LogError("can't find the sheet you want " + _sheetName);
        return null;
    }
}

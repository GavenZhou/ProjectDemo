using System.IO;
using UnityEditor;
using UnityEngine;
using Aspose.Cells;


public class EditorHelper
{
    public static string profilePath = @"Assets\_ProcessAssets\_Profile";

    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    public static void CreateNewEditorProfile<T>(string _profileName) where T : ScriptableObject {

        // 
        string path = Path.Combine(profilePath, _profileName);

        bool doCreate = true;
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists) {
            doCreate = EditorUtility.DisplayDialog(_profileName + " already exists.",
                                                   "Do you want to overwrite the old one?",
                                                   "Yes", "No");
        }
        if (doCreate) {

            // check if the asset is valid to create
            if (new DirectoryInfo(profilePath).Exists == false) {
                Debug.LogError("can't create asset, path not found");
                return;
            }
            if (string.IsNullOrEmpty(_profileName)) {
                Debug.LogError("can't create asset, the name is empty");
                return;
            }

            //
            T newAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newAsset, path);
            Selection.activeObject = newAsset;
        }
    }

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

using System.IO;
using UnityEditor;
using UnityEngine;
using Aspose.Cells;

public enum TableValueType {

    INT,
    LONG,
    FLOAT,
    BOOL,
    STRING,
    
    CUSTOM,
};


public class EditorHelper
{
    public static string profileFolder = @"Assets\_ProcessAssets\_Profile";

    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    public static T CreateNewEditorProfile<T>(string _profileName) where T : ScriptableObject {

        // 
        string path = Path.Combine(profileFolder, _profileName);

        bool doCreate = true;
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists) {
            doCreate = EditorUtility.DisplayDialog(_profileName + " already exists.",
                                                   "Do you want to overwrite the old one?",
                                                   "Yes", "No");
        }
        if (doCreate) {

            // check if the asset is valid to create
            if (new DirectoryInfo(profileFolder).Exists == false) {
                Debug.LogError("can't create asset, path not found");
                return null;
            }
            if (string.IsNullOrEmpty(_profileName)) {
                Debug.LogError("can't create asset, the name is empty");
                return null;
            }

            //
            T newAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newAsset, path);
            Selection.activeObject = newAsset;
            return newAsset;
        }
        return null;
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

    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    public static object GetTableValue(string _valueType, string _valueString) {

        //Debug.Log(_valueType + " " + _valueString);

        if (string.Compare(TableValueType.INT.ToString(), _valueType, true) == 0) {

            int value = 0;
            _valueString.Trim();

            if (int.TryParse(_valueString, out value)) {
                return value;
            }
            return null;
        }
        else if (string.Compare(TableValueType.LONG.ToString(), _valueType, true) == 0) {

            long value = 0;
            _valueString.Trim();

            if (long.TryParse(_valueString, out value)) {
                return value;
            }
            return null;
        }
        else if (string.Compare(TableValueType.FLOAT.ToString(), _valueType, true) == 0) {

            float value = 0;
            _valueString.Trim();

            if (float.TryParse(_valueString, out value)) {
                return value;
            }
            return null;
        }
        else if (string.Compare(TableValueType.BOOL.ToString(), _valueType, true) == 0) {

            bool value = false;
            _valueString.Trim();

            if (bool.TryParse(_valueString, out value)) {
                return value;
            }
            return null;
        }
        else if (string.Compare(TableValueType.STRING.ToString(), _valueType, true) == 0) {
            return _valueString;
        }
        else if (string.Compare(TableValueType.CUSTOM.ToString(), _valueType, true) == 0) {
            return _valueString;
        }
        return null;
    }
}

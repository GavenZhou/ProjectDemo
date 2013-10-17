using UnityEngine;
using UnityEditor;
using Aspose.Cells;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

public class TableProcess : EditorWindow {

    static string assetName = "TableProcess.asset";
    
    int currentIndex = -1;
    SerializedObject serialized;
    SerializedProperty characterExcelProp;
    TableProcessProfile asset;


    [MenuItem("Helper/Table Process")]
    public static TableProcess NewWindow() {
        TableProcess newWindow = EditorWindow.GetWindow<TableProcess>();
        return newWindow;
    }

    [MenuItem("Assets/Create/Custom Assets/Table Process")]
    static void CreateAnimationTableAsset() {
        EditorHelper.CreateNewEditorProfile<TableProcessProfile>(assetName);
    }

    void OnEnable() {
        name = "Table Process";
        autoRepaintOnSceneChange = false;

        asset = AssetDatabase.LoadAssetAtPath(EditorHelper.profileFolder + "/" + assetName,
                                              typeof(TableProcessProfile)) as TableProcessProfile;
        if (asset == null) return;

        serialized = new SerializedObject(asset);
        characterExcelProp = serialized.FindProperty("characterExcel");
    }

    void OnGUI() {

        GUIStyle style = null;

        // if we don't have a profile
        if (asset == null) {
            GUILayout.Space(10);

            style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;
            GUILayout.Label("Please create a Table Asset!", style);
            return;
        }

        serialized.Update();

        //
        GUILayout.Space(10);

        // ======================================================== 
        // excel table 
        // ======================================================== 

        EditorGUILayout.PropertyField(characterExcelProp);

        int idx = -1;
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        // ======================================================== 
        // Phase 1 - Update Animation Events 
        // ======================================================== 

        ++idx;
        EditorGUILayout.BeginHorizontal();
        style.normal.textColor = (idx == currentIndex) ? Color.green : EditorStyles.label.normal.textColor;
        EditorGUILayout.LabelField(" Process Character ", style, GUILayout.Width(200));

        if (GUILayout.Button("Exec", GUILayout.Width(50))) {
            currentIndex = idx;
            ExportCharacterTable();
        }
        EditorGUILayout.EndHorizontal();

        serialized.ApplyModifiedProperties();
    }

    const int characterTableTitleRow = 1;
    const int characterTableValueTypeRow = 2;

    void ExportCharacterTable() {

        Dictionary<string, Dictionary<string, string>> excelData = new Dictionary<string, Dictionary<string, string>>();
        Worksheet sheet = EditorHelper.LoadExcelSheet(AssetDatabase.GetAssetPath(asset.characterExcel), "CharacterTable");

        // parse title 
        string[] header = new string[sheet.Cells.Columns.Count];
        string[] type = new string[sheet.Cells.Columns.Count];

        for (int i = 0; i < sheet.Cells.Columns.Count; ++i) {
            header[i] = sheet.Cells.Rows[characterTableTitleRow].GetCellOrNull(i) != null ? sheet.Cells.Rows[1].GetCellOrNull(i).StringValue : null;
            if (header[i] != null) header[i].Trim();

            type[i] = sheet.Cells.Rows[characterTableValueTypeRow].GetCellOrNull(i) != null ? sheet.Cells.Rows[2].GetCellOrNull(i).StringValue : null;
            if (type[i] != null) type[i].Trim();
        }

        // parse table content
        foreach (Row row in sheet.Cells.Rows) {
            if (row.Index <= characterTableValueTypeRow)
                continue;

            string key = row.GetCellOrNull(0) != null ? row.GetCellOrNull(0).StringValue : null;
            if (string.IsNullOrEmpty(key) == false) {
                if (excelData.ContainsKey(key) == false) {
                    excelData.Add(key, new Dictionary<string, string>());
                }

                for (int i = 0; i < sheet.Cells.Columns.Count; ++i) {
                    string value = row.GetCellOrNull(i) != null ? row.GetCellOrNull(i).StringValue : null;

                    if (string.IsNullOrEmpty(value) == false) {
                        if (excelData[key].ContainsKey(header[i])) {
                            Debug.LogError("Try to add duplicate key " + header[i] + " for animation " + key);
                            continue;
                        }
                        excelData[key].Add(header[i], value);
                    }
                }
            }
        }

        //foreach (string s in excelData.Keys) { 
        //    string str = "";
        //    Dictionary<string, string> dict = excelData[s];
        //    foreach (string k in dict.Keys) {
        //        str += ((dict[k]) + " ");
        //    }
        //    Debug.Log(str);
        //}

        // parse excelData
        Type t = typeof(CharacterData);
        CharacterTable table = EditorHelper.CreateNewEditorProfile<CharacterTable>("CharacterTable.asset");

        foreach (Dictionary<string, string> row in excelData.Values) {

            // 
            int index = 0;
            CharacterData c = new CharacterData();
            foreach (string title in row.Keys) {
                FieldInfo mInfo = t.GetField(header[index], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (mInfo != null) {
                    object param = EditorHelper.GetTableValue(type[index], row[title]);
                    if (param != null) {
                        t.InvokeMember(mInfo.Name, BindingFlags.SetField, null, c, new object[] { param });
                    }
                }
                index++;
            }
            table.lstCharacter.Add(c);
        }
    }
}

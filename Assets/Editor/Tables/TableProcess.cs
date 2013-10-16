using UnityEngine;
using UnityEditor;
using Aspose.Cells;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

        SortedDictionary<string, Dictionary<string, string>> excelData =
            new SortedDictionary<string, Dictionary<string, string>>();

        Worksheet sheet = EditorHelper.LoadExcelSheet(AssetDatabase.GetAssetPath(asset.characterExcel), "CharType");

        // parse title 
        string[] header = new string[sheet.Cells.Columns.Count];
        string[] type = new string[sheet.Cells.Columns.Count];

        for (int i = 0; i < sheet.Cells.Columns.Count; ++i) {
            header[i] = sheet.Cells.Rows[characterTableTitleRow].GetCellOrNull(i) != null ? sheet.Cells.Rows[1].GetCellOrNull(i).StringValue : null;
            type[i] = sheet.Cells.Rows[characterTableValueTypeRow].GetCellOrNull(i) != null ? sheet.Cells.Rows[2].GetCellOrNull(i).StringValue : null;
        }

        // parse table content
        foreach (Row row in sheet.Cells.Rows) {
            if (row.Index == 0)
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

        foreach (string s in excelData.Keys) { 
            string str = s + " ";
            Dictionary<string, string> dict = excelData[s];
            foreach (string k in dict.Keys) {
                str += ((dict[k]) + " ");
            }
            Debug.Log(str);
        }


        //// parse excelData
        //int index = 0;
        //List<AnimationEvent> animEvents = new List<AnimationEvent>();

        //foreach (string animName in excelData.Keys) {
        //    animEvents.Clear();
        //    float progress = (float)index / (float)excelData.Keys.Count;

        //    if (EditorUtility.DisplayCancelableProgressBar("Add Normal Animation Events",
        //                                                   "Processing " + animName,
        //                                                   progress))
        //    {
        //        break;
        //    }
        //    ++index;

        //    string path = Path.Combine(asset.animationClipsFolder, animName + ".anim");
        //    AnimationClip animClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
        //    if (animClip == null) {
        //        Debug.LogWarning("can't find anim clip " + path);
        //        continue;
        //    }

        //    Dictionary<string, string> srcEventInfos = excelData[animName];

        //    Regex regex = new Regex(@"(\w+)_(\d+)", RegexOptions.IgnoreCase);
        //    AnimationEvent animEvent = null;

        //    foreach (string title in srcEventInfos.Keys) {
        //        Match m = regex.Match(title);
        //        if (m.Success) {
        //            if (string.IsNullOrEmpty(srcEventInfos[title]))
        //                continue;

        //            //
        //            if (m.Groups[1].Value == "Keyframe") {
        //                animEvent = new AnimationEvent();
        //                animEvents.Add(animEvent);

        //                int keyframe = System.Convert.ToInt32(srcEventInfos[title]);
        //                animEvent.time = keyframe / 24.0f;//animClip.frameRate;
        //            }

        //            //
        //            if (m.Groups[1].Value == "FunctionName") {
        //                animEvent.functionName = srcEventInfos[title];
        //                animEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
        //            }

        //            //
        //            if (m.Groups[1].Value == "Parameter") {
        //                string param = srcEventInfos[title];
        //                if (string.IsNullOrEmpty(param) == false && string.Compare(param, "null", true) != 0) {
        //                    int intParam;
        //                    if (System.Int32.TryParse(param, out intParam)) {
        //                        animEvent.intParameter = intParam;
        //                    } else {
        //                        animEvent.stringParameter = param;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    AnimationUtility.SetAnimationEvents(animClip, animEvents.ToArray());
        //}
        EditorUtility.ClearProgressBar();
    }
}

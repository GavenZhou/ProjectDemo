
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Aspose.Cells;
using System.Text.RegularExpressions;
using System.IO;


public class AnimationTableWindow : EditorWindow
{
    static string assetPath = @"Assets\_ProcessAssets\_TableAssets";
    static string assetName = "AnimationTable.asset";
    
    
    ///////////////////////////////////////////////////////////////////////////////
    // Menu Item
    ///////////////////////////////////////////////////////////////////////////////

    [MenuItem("Helper/Animation Table Editor")]
    public static AnimationTableWindow NewWindow() {
        AnimationTableWindow newWindow = EditorWindow.GetWindow<AnimationTableWindow>();
        return newWindow;
    }

    [MenuItem("Assets/Create/Table Asset/Animation Table")]
    static void CreateAnimationTableAsset() {
        EditorHelper.CreateNewEditorAsset<AnimationTableAsset>("AnimationTable");
    }


    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////

    //
    int currentIndex = -1;
    AnimationTableAsset asset;
    SerializedObject serialized;
    SerializedProperty animationClipsProp;
    SerializedProperty animationTableExcelProp;


    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init() {

    }

    void OnEnable() {
        name = "Animation Table Editor";
        wantsMouseMove = false;
        autoRepaintOnSceneChange = false;

        asset = AssetDatabase.LoadAssetAtPath(assetPath + "/" + assetName,
                                              typeof(AnimationTableAsset)) as AnimationTableAsset;

        serialized = new SerializedObject(asset);
        animationClipsProp = serialized.FindProperty("animationClipsFolder");
        animationTableExcelProp = serialized.FindProperty("animationTableExcel");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange() {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI() {
        GUIStyle style = null;
        Rect rect = new Rect();

        // if we don't have a profile
        if (asset == null) {
            GUILayout.Space(10);

            style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;
            GUILayout.Label("Please create a Table Asset!", style);
            return;
        }

        //
        GUILayout.Space(10);

        serialized.Update();

        //
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(animationClipsProp);
        //if (GUILayout.Button("...", GUILayout.Width(30))) {
        //    string newPath = EditorUtility.SaveFolderPanel("Animation Clips",
        //                                                   animationClipsProp.stringValue,
        //                                                   Path.GetFullPath(Application.dataPath));
        //    if (newPath.Length != 0) {
        //        animationClipsProp.stringValue = newPath;
        //    }
        //}
        GUILayout.EndHorizontal();

        // ======================================================== 
        // excel table 
        // ======================================================== 

        EditorGUILayout.PropertyField(animationTableExcelProp);
        GUILayout.Space(20);

        int idx = -1;
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        // ======================================================== 
        // Phase 1 - Update Animation Events 
        // ======================================================== 

        ++idx;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        style.normal.textColor = (idx == currentIndex) ? Color.green : EditorStyles.label.normal.textColor;
        EditorGUILayout.LabelField(" [" + (idx + 1).ToString("00") + "]", style, GUILayout.Width(40));
        rect = GUILayoutUtility.GetLastRect();
        GUI.Box(rect, "");

        EditorGUILayout.LabelField(" Update Animation Events", style, GUILayout.Width(300));

        if (GUILayout.Button("Exec", GUILayout.Width(50))) {
            currentIndex = idx;
            UpdateAnimationEvents();
        }
        EditorGUILayout.EndHorizontal();

        serialized.ApplyModifiedProperties();
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateAnimationEvents() {
        SortedDictionary<string, Dictionary<string, string>> excelData =
            new SortedDictionary<string, Dictionary<string, string>>();

        Worksheet sheet = EditorHelper.LoadExcelSheet(AssetDatabase.GetAssetPath(asset.animationTableExcel),
                                                      "AnimationEvents");

        // parse title 
        string[] header = new string[sheet.Cells.Columns.Count];
        for (int i = 0; i < sheet.Cells.Columns.Count; ++i) {
            header[i] = sheet.Cells.Rows[0].GetCellOrNull(i) != null ? sheet.Cells.Rows[0].GetCellOrNull(i).StringValue : null;
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


        // parse excelData
        int index = 0;
        List<AnimationEvent> animEvents = new List<AnimationEvent>();

        foreach (string animName in excelData.Keys) {
            animEvents.Clear();
            float progress = (float)index / (float)excelData.Keys.Count;

            if (EditorUtility.DisplayCancelableProgressBar("Add Normal Animation Events",
                                                           "Processing " + animName,
                                                           progress))
            {
                break;
            }
            ++index;

            string path = Path.Combine(asset.animationClipsFolder, animName + ".anim");
            AnimationClip animClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
            if (animClip == null) {
                Debug.LogWarning("can't find anim clip " + path);
                continue;
            }

            Dictionary<string, string> srcEventInfos = excelData[animName];

            Regex regex = new Regex(@"(\w+)_(\d+)", RegexOptions.IgnoreCase);
            AnimationEvent animEvent = null;

            foreach (string title in srcEventInfos.Keys) {
                Match m = regex.Match(title);
                if (m.Success) {
                    if (string.IsNullOrEmpty(srcEventInfos[title]))
                        continue;

                    //
                    if (m.Groups[1].Value == "Keyframe") {
                        animEvent = new AnimationEvent();
                        animEvents.Add(animEvent);

                        int keyframe = System.Convert.ToInt32(srcEventInfos[title]);
                        animEvent.time = keyframe / 24.0f;//animClip.frameRate;
                    }

                    //
                    if (m.Groups[1].Value == "FunctionName") {
                        animEvent.functionName = srcEventInfos[title];
                        animEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
                    }

                    //
                    if (m.Groups[1].Value == "Parameter") {
                        string param = srcEventInfos[title];
                        if (string.IsNullOrEmpty(param) == false && string.Compare(param, "null", true) != 0) {
                            int intParam;
                            if (System.Int32.TryParse(param, out intParam)) {
                                animEvent.intParameter = intParam;
                            } else {
                                animEvent.stringParameter = param;
                            }
                        }
                    }
                }
            }
            AnimationUtility.SetAnimationEvents(animClip, animEvents.ToArray());
        }
        EditorUtility.ClearProgressBar();
    }
}


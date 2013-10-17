
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Aspose.Cells;
using System.Text.RegularExpressions;
using System.IO;


public class AnimationProcess : EditorWindow
{
    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////    

    static string assetName = "AnimationProcess.asset";
    const string duplicatePostfix = "";
    const string processAssets = "_ProcessAssets";
    const string animationFolder = "_Clips";


    ///////////////////////////////////////////////////////////////////////////////
    // Menu Item
    ///////////////////////////////////////////////////////////////////////////////

    [MenuItem("Helper/Animation Process")]
    public static AnimationProcess NewWindow() {
        AnimationProcess newWindow = EditorWindow.GetWindow<AnimationProcess>();
        return newWindow;
    }

    [MenuItem("Assets/Create/Custom Assets/Animation Process")]
    static void CreateAnimationTableAsset() {
        EditorHelper.CreateNewEditorProfile<AnimationProcessProfile>(assetName);
    }

    [MenuItem("Assets/Helper/Copy Animation Clips")]
    static void CopyCurvesToDuplicate() {

        // "Assets/_ProcessAssets/_Clips"
        string folder = "Assets/" + processAssets + "/" + animationFolder;
        if (Directory.Exists(folder) == false) {
            AssetDatabase.CreateFolder("Assets/" + processAssets, animationFolder);
        }

        // Get selected object
        AnimationClip clipToCopy = null;
        AnimationClip newAnimClip = null;
        Object[] imported = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        for (int i = 0; i < imported.Length; ++i) {
            UnityEngine.Object obj = imported[i];

            if (EditorUtility.DisplayCancelableProgressBar("Copy Animation Clips",
                                                            "Processing " + obj.name,
                                                            (float)i / (float)imported.Length)) {
                break;
            }

            //
            clipToCopy = obj as AnimationClip;
            if (clipToCopy) {
                string animName = clipToCopy.name + ".anim";
                newAnimClip = CopyClip(Path.Combine(folder, animName), clipToCopy);
            }

            //
            GameObject go = obj as GameObject;
            if (go && go.animation) {
                foreach (AnimationState state in go.animation) {
                    clipToCopy = state.clip;
                    if (clipToCopy) {
                        string animName = clipToCopy.name + ".anim";
                        newAnimClip = CopyClip(Path.Combine(folder, animName), clipToCopy);
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        Selection.activeObject = newAnimClip;
        EditorGUIUtility.PingObject(newAnimClip);
    }

    static AnimationClip CopyClip(string _path, AnimationClip _clipToCopy) {

        AnimationClip newClip = new AnimationClip();
        newClip.name = _clipToCopy.name + duplicatePostfix;
        AssetDatabase.CreateAsset(newClip, _path);
        AssetDatabase.Refresh();

        AnimationClipCurveData[] srcCurveData = AnimationUtility.GetAllCurves(_clipToCopy, true);

        for (int i = 0; i < srcCurveData.Length; ++i) {
            AnimationUtility.SetEditorCurve(newClip,
                                            srcCurveData[i].path,
                                            srcCurveData[i].type,
                                            srcCurveData[i].propertyName,
                                            srcCurveData[i].curve);
        }
        EditorUtility.SetDirty(newClip);
        Debug.Log("Copying curves into " + _clipToCopy.name + " is done");

        return newClip;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////

    //
    int currentIndex = -1;
    AnimationProcessProfile asset;
    SerializedObject serialized;
    SerializedProperty animationClipsProp;
    SerializedProperty animationEventExcelProp;


    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    void OnEnable() {
        name = "Animation Process";
        autoRepaintOnSceneChange = false;

        asset = AssetDatabase.LoadAssetAtPath(EditorHelper.profileFolder + "/" + assetName,
                                              typeof(AnimationProcessProfile)) as AnimationProcessProfile;
        if (asset == null) return;

        serialized = new SerializedObject(asset);
        animationClipsProp = serialized.FindProperty("animationClipsFolder");
        animationEventExcelProp = serialized.FindProperty("animationEventExcel");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

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

        //
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(animationClipsProp);
        GUILayout.EndHorizontal();

        // ======================================================== 
        // excel table 
        // ======================================================== 

        EditorGUILayout.PropertyField(animationEventExcelProp);
        GUILayout.Space(10);

        int idx = -1;
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        // ======================================================== 
        // Phase 1 - Update Animation Events 
        // ======================================================== 

        ++idx;
        EditorGUILayout.BeginHorizontal();
        style.normal.textColor = (idx == currentIndex) ? Color.green : EditorStyles.label.normal.textColor;
        EditorGUILayout.LabelField(" Process Animation Events", style, GUILayout.Width(200));

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

        Worksheet sheet = EditorHelper.LoadExcelSheet(AssetDatabase.GetAssetPath(asset.animationEventExcel),
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
                                                           progress)) {
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


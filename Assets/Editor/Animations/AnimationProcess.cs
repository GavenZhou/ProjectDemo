
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

    public static string assetName = "AnimationProcess.asset";

    
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

    [MenuItem("Assets/Helper/Copy Animation Clip")]
    static void CopyCurvesToDuplicate() {
        // Get selected AnimationClip
        Object[] imported = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Unfiltered);
        if (imported.Length == 0) {
            Debug.LogWarning("Either no objects were selected or the objects selected were not AnimationClips.");
            return;
        }

        //If necessary, create the animations folder.
        if (Directory.Exists("Assets/" + processAssets) == false) {
            AssetDatabase.CreateFolder("Assets", processAssets);
        }
        if (Directory.Exists("Assets/" + processAssets + "/" + animationFolder) == false) {
            AssetDatabase.CreateFolder("Assets/" + processAssets, animationFolder);
        }

        string path = "Assets/" + processAssets + "/" + animationFolder;

        foreach (AnimationClip clip in imported) {

            string importedPath = AssetDatabase.GetAssetPath(clip);

            //If the animation came from an FBX, then use the FBX name as a subfolder to contain the animations.
            string copyPath;
            if (importedPath.Contains(".fbx")) {
                //With subfolder.
                string folder = importedPath.Substring(importedPath.LastIndexOf("/") + 1, importedPath.LastIndexOf(".") - importedPath.LastIndexOf("/") - 1);
                if (!Directory.Exists(path + "/" + folder)) {
                    AssetDatabase.CreateFolder(path, folder);
                }
                copyPath = path + "/" + folder + "/" + clip.name + duplicatePostfix + ".anim";
            } else {
                //No Subfolder
                copyPath = path + "/" + clip.name + duplicatePostfix + ".anim";
            }
            Debug.Log("CopyPath: " + copyPath);

            CopyClip(importedPath, copyPath);

            AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
            if (copy == null) {
                Debug.Log("No copy found at " + copyPath);
                return;
            }
            // Copy curves from imported to copy
            AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(clip, true);
            for (int i = 0; i < curveDatas.Length; i++) {
                AnimationUtility.SetEditorCurve(
                    copy,
                    curveDatas[i].path,
                    curveDatas[i].type,
                    curveDatas[i].propertyName,
                    curveDatas[i].curve
                );
            }
            Debug.Log("Copying curves into " + copy.name + " is done");
        }
    }

    static void CopyClip(string importedPath, string copyPath) {

        AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
        AnimationClip newClip = new AnimationClip();
        newClip.name = src.name + duplicatePostfix;
        AssetDatabase.CreateAsset(newClip, copyPath);
        AssetDatabase.Refresh();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////

    //
    int currentIndex = -1;
    AnimationProcessProfile asset;
    SerializedObject serialized;
    SerializedProperty animationClipsProp;
    SerializedProperty animationTableExcelProp;


    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    void OnEnable() {
        name = "Animation Process";
        wantsMouseMove = false;
        autoRepaintOnSceneChange = false;

        asset = AssetDatabase.LoadAssetAtPath(EditorHelper.profilePath + "/" + assetName,
                                              typeof(AnimationProcessProfile)) as AnimationProcessProfile;

        serialized = new SerializedObject(asset);
        animationClipsProp = serialized.FindProperty("animationClipsFolder");
        animationTableExcelProp = serialized.FindProperty("animationTableExcel");
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

        //
        GUILayout.Space(10);

        serialized.Update();

        //
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(animationClipsProp);
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

    const string duplicatePostfix = "Edit";
    const string processAssets = "_ProcessAssets";
    const string animationFolder = "_Clips";
}


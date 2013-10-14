using UnityEditor;
using UnityEngine;

using System.IO;
using System.Collections;

public class AnimationCopyHelper
{
    const string duplicatePostfix = "Edit";
    const string processAssets = "_ProcessAssets";
    const string animationFolder = "_Clips";

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
}
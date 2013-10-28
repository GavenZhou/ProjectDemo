// ======================================================================================
// File         : CombineChildrenPrime.cs
// Author       : Joey
// Last Change  : 08/07/2012 | 14:14:13 PM | Tuesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//[ExecuteInEditMode()]
[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildrenPrime : MonoBehaviour
{
    public int frameToWait = 0;
    public bool combineOnStart = false;
    public bool destroyAfterOptimized = false;

    public bool generateTriangleStrips = true;
    public bool castShadow = true;
    public bool receiveShadow = true;
    public bool keepLayer = true;
    public bool addMeshColliderAfter = false;

    bool combined = false;

    void Start() {
        if (combineOnStart && !combined) {
            StartCoroutine(CombineLate());
        }
    }

    IEnumerator CombineLate() {

        //
        if (frameToWait == 0) {
            frameToWait = UnityEngine.Random.Range(0, 20);
        }
        for (int i = 0; i < frameToWait; i++) {
            yield return 0;
        }

        Combine();
    }

    [ContextMenu("Combine All Childs In Editor")]
    public void CombineAllChilds() {
        CombineChildrenPrime[] c = gameObject.GetComponentsInChildren<CombineChildrenPrime>();
        int count = c.Length;
        for (int i = 0; i < count; i++) {
            if (c[i] != this) {
                c[i].Combine();
            }
        }
        combineOnStart = enabled = false;
    }

    [ContextMenu("Revert Combine")]
    public void RevertCombine() {

        if (!combined) {
            Debug.LogError("The object have not combined yet!");
            return;
        }
        combined = false;

        Component[] filters = GetComponentsInChildren(typeof(MeshFilter));
        foreach (Component m in filters) {
            if (m.gameObject.name == "Combined mesh" && m.renderer.enabled == true) {
                GameObject.DestroyImmediate(m.gameObject);
                continue;
            }
            if (m.renderer != null && m.renderer.enabled == false) {
                m.renderer.enabled = true;
            }
        }
    }

    [ContextMenu("Combine In Editor")]
    public void Combine() {

        if (combined) {
            Debug.LogError("The object has already combined!");
            return;
        }
        combined = true;

        // 
        bool _destroyImmediate = false;

        Component[] filters = GetComponentsInChildren(typeof(MeshFilter));
        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        Hashtable materialToMesh = new Hashtable();

        for (int i = 0; i < filters.Length; i++) {

            MeshFilter filter = (MeshFilter)filters[i];
            Renderer curRenderer = filters[i].renderer;
            MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
            instance.mesh = filter.sharedMesh;
            if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
                instance.transform = myTransform * filter.transform.localToWorldMatrix;

                Material[] materials = curRenderer.sharedMaterials;
                for (int m = 0; m < materials.Length; m++) {
                    instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                    ArrayList objects = (ArrayList)materialToMesh[materials[m]];
                    if (objects != null) { objects.Add(instance); } else {
                        objects = new ArrayList();
                        objects.Add(instance);
                        materialToMesh.Add(materials[m], objects);
                    }
                }
                if (Application.isPlaying && destroyAfterOptimized && combineOnStart) {
                    if (_destroyImmediate)
                        DestroyImmediate(curRenderer.gameObject);
                    else
                        Destroy(curRenderer.gameObject);
                } else if (destroyAfterOptimized) {
                    DestroyImmediate(curRenderer.gameObject);
                } else {
                    curRenderer.enabled = false;
                }
            }
        }

        GameObject parent = new GameObject(gameObject.name + "(Combine)");
        parent.transform.localScale = transform.localScale;
        parent.transform.localRotation = transform.rotation;
        parent.transform.localPosition = transform.position;

        foreach (DictionaryEntry de in materialToMesh) {

            ArrayList elements = (ArrayList)de.Value;
            MeshCombineUtility.MeshInstance[] instances
                = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

            GameObject go = new GameObject("Combined mesh");
            if (keepLayer) {
                go.layer = gameObject.layer;
            }
            go.transform.parent = parent.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent(typeof(MeshFilter));
            go.AddComponent("MeshRenderer");
            go.renderer.material = (Material)de.Key;

            MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
            if (Application.isPlaying) {
                filter.mesh = MeshCombineUtility.Combine(instances);
            } else {
                filter.sharedMesh = MeshCombineUtility.Combine(instances);
            }
            go.renderer.castShadows = castShadow;
            go.renderer.receiveShadows = receiveShadow;

            if (addMeshColliderAfter) {
                MeshCollider meshCollider = go.AddComponent<MeshCollider>();
                MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                if (meshFilter) {
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                }
            }
        }
    }
}

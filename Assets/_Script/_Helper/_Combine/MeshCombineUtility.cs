
using UnityEngine;
using System.Collections;

public class MeshCombineUtility
{
    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
    }

    public static Mesh Combine(MeshInstance[] _combines) {
        Mesh mesh = new Mesh();
        mesh.name = "Combined Mesh";

        CombineInstance[] combineInsts = new CombineInstance[_combines.Length];
        int i = 0;
        while (i < _combines.Length) {
            combineInsts[i].mesh = _combines[i].mesh;
            combineInsts[i].transform = _combines[i].transform;
            ++i;
        }
        mesh.CombineMeshes(combineInsts);
        return mesh;
    }
}

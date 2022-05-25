using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace iffnsStuff.iffnsBaseSystemForUnity.Tools
{
    public static class ObjExporter
    {
        //triangleIndexOffset : Used for adding multiple meshes to 1 Obj file

        public enum UpDirection
        {
            Y,
            Z
        }

        public static List<string> GetObjLines(string meshName, Mesh mesh, int triangleIndexOffset)
        {
            return GetObjLines(meshName: meshName, vertices: mesh.vertices, uvs: mesh.uv, triangles: mesh.triangles, triangleIndexOffset: triangleIndexOffset, upDirection: UpDirection.Y);
        }

        public static List<string> GetObjLines(string meshName, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, int triangleIndexOffset, UpDirection upDirection)
        {
            List<int> usedTriangles = new List<int>();

            foreach (int index in triangles)
            {
                usedTriangles.Add(index + triangleIndexOffset);
            }

            List<string> returnList = new List<string>();

            returnList.Add("o " + meshName);

            //Vertices
            switch (upDirection)
            {
                case UpDirection.Y:
                    foreach (Vector3 vertex in vertices)
                    {
                        returnList.Add("v " + -vertex.x + " " + vertex.y + " " + vertex.z);
                    }
                    break;
                case UpDirection.Z:
                    foreach (Vector3 vertex in vertices)
                    {
                        returnList.Add("v " + -vertex.x + " " + vertex.z + " " + vertex.y);
                    }
                    break;
                default:
                    break;
            }

            //UVs
            foreach (Vector2 uv in uvs)
            {
                returnList.Add("vt " + uv.x + " " + uv.y + " ");
            }

            //Faces
            if (usedTriangles.Count != 0)
            {
                for (int i = 0; i < usedTriangles.Count - 1; i += 3)
                {
                    //First index in OBJ is 1 for some reason
                    int t1 = usedTriangles[i] + 1;
                    int t2 = usedTriangles[i + 1] + 1;
                    int t3 = usedTriangles[i + 2] + 1;

                    returnList.Add("f " + t1 + "/" + t1 + " " + t3 + "/" + t3 + " " + t2 + "/" + t2);
                }
            }

            return returnList;
        }

        public static List<string> GetObjLines(string meshName, Vector3[] vertices, Vector2[] uvs, int[] triangles, int triangleIndexOffset, UpDirection upDirection)
        {
            return(GetObjLines(meshName, vertices.ToList(), uvs.ToList(), triangles.ToList(), triangleIndexOffset: triangleIndexOffset, upDirection: upDirection));

        }
    }
}
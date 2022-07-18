using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;

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
                        returnList.Add(GetVertexStringFromVector3(-vertex.x, vertex.y, vertex.z));
                    }
                    break;
                case UpDirection.Z:
                    foreach (Vector3 vertex in vertices)
                    {
                        returnList.Add(GetVertexStringFromVector3(-vertex.x, vertex.z, vertex.y));
                    }
                    break;
                default:
                    break;
            }

            //UVs
            foreach (Vector2 uv in uvs)
            {
                returnList.Add(GetUVStringFromVector2(uv.x, uv.y));
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

                    returnList.Add(GetTriangleStringFromTrianglePoints(t1, t3, t2));

                }
            }

            return returnList;
        }

        static string GetVertexStringFromVector3(float x, float y, float z)
        {
            string xString = x.ToString("0.#############################", CultureInfo.InvariantCulture);
            string yString = y.ToString("0.#############################", CultureInfo.InvariantCulture);
            string zString = z.ToString("0.#############################", CultureInfo.InvariantCulture);

            return $"v {xString} {yString} {zString}";
        }

        static string GetUVStringFromVector2(float x, float y)
        {
            string xString = x.ToString("0.#############################", CultureInfo.InvariantCulture);
            string yString = y.ToString("0.#############################", CultureInfo.InvariantCulture);

            return $"vt {xString} {yString}";
        }

        public static string GetTriangleStringFromTrianglePoints(int t1, int t2, int t3)
        {
            string t1String = t1.ToString(CultureInfo.InvariantCulture);
            string t2String = t2.ToString(CultureInfo.InvariantCulture);
            string t3String = t3.ToString(CultureInfo.InvariantCulture);

            return $"f {t1String}/{t1String} {t2String}/{t2String} {t3String}/{t3String}"; //ToDo: Check int to string conversion
        }

        public static List<string> GetObjLines(string meshName, Vector3[] vertices, Vector2[] uvs, int[] triangles, int triangleIndexOffset, UpDirection upDirection)
        {
            return(GetObjLines(meshName, vertices.ToList(), uvs.ToList(), triangles.ToList(), triangleIndexOffset: triangleIndexOffset, upDirection: upDirection));

        }
    }
}
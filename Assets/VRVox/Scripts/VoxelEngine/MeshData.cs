// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	public class MeshData
	{
		public List<Vector3> vertices = new List<Vector3>();
		public List<Color> colors = new List<Color>();
		public List<int> triangles = new List<int>();
		public List<Vector2> uv = new List<Vector2>();

		public List<Vector3> colVertices = new List<Vector3>(); // Collider Verts
		public List<int> colTriangles = new List<int>(); // Collider Tris

		public bool UseRenderDataForCol;

		public MeshData()
		{
		}

		public void AddVertex(Vector3 vertex, Color color)
		{
			vertices.Add(vertex);

			colors.Add(color);

			if (UseRenderDataForCol)
			{
				colVertices.Add(vertex);
			}

		}

		public void AddQuadTriangles()
		{
			triangles.Add(vertices.Count - 4);
			triangles.Add(vertices.Count - 3);
			triangles.Add(vertices.Count - 2);

			triangles.Add(vertices.Count - 4);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);

			if (UseRenderDataForCol)
			{
				colTriangles.Add(colVertices.Count - 4);
				colTriangles.Add(colVertices.Count - 3);
				colTriangles.Add(colVertices.Count - 2);
				colTriangles.Add(colVertices.Count - 4);
				colTriangles.Add(colVertices.Count - 2);
				colTriangles.Add(colVertices.Count - 1);
			}
		}

		public void AddTriangle(int tri)
		{
			triangles.Add(tri);

			if (UseRenderDataForCol)
			{
				colTriangles.Add(tri - (vertices.Count - colVertices.Count));
			}
		}
	}
}

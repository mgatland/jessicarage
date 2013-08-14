using UnityEngine;
using System.Collections;

public class Chernikhov : MonoBehaviour {
	
	private int cubeIndex;
	private int faceIndex;
	
	void CreateFace(Vector3 position, int xSize, int ySize, float xOff, float yOff, float zOff, float xRot, float yRot, float zRot, Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uv) {
		int o4 = (faceIndex + cubeIndex * 6) * 4;
		Vector3 offset = new Vector3(xOff, yOff, zOff) + position;
		vertices[0+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(0,0,0) + offset;
		vertices[1+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(xSize,0,0) + offset;
		vertices[2+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(0,ySize,0) + offset;
		vertices[3+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(xSize,ySize,0) + offset;
		
		int o6 = (faceIndex + cubeIndex * 6) * 6;
		triangles[0+o6] = 0+o4;
		triangles[1+o6] = 2+o4;
		triangles[2+o6] = 1+o4;
		triangles[3+o6] = 2+o4;
		triangles[4+o6] = 3+o4;
		triangles[5+o6] = 1+o4;
		
		for (int i = 0; i < 4; i++) {
			normals[i+o4] = Quaternion.Euler(xRot, yRot, zRot) * -Vector3.forward;
		}
		
		uv[0+o4] = new Vector2(0*xSize, 0*ySize);
		uv[1+o4] = new Vector2(1*xSize, 0*ySize);
		uv[2+o4] = new Vector2(0*xSize, 1*ySize);
		uv[3+o4] = new Vector2(1*xSize, 1*ySize);
		faceIndex++;
	}
	
	void CreateCuboid(Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uv,Vector3 size, Vector3 position) {
		int xSize = (int)size.x;
		int ySize = (int)size.y;
		int zSize = (int)size.z;

		//sides
		CreateFace(position, xSize, ySize, 0,0,0, 0, 0, 0, vertices, triangles, normals, uv);
		CreateFace(position, zSize, ySize, 0,0,zSize, 0, 90, 0, vertices, triangles, normals, uv);
		CreateFace(position, xSize, ySize, xSize,0,zSize, 0, 180, 0, vertices, triangles, normals, uv);
		CreateFace(position, zSize, ySize, xSize,0,0, 0, 270, 0, vertices, triangles, normals, uv);
		//top and bottom
		CreateFace(position, xSize, zSize, 0,ySize,0, 90, 0, 0, vertices, triangles, normals, uv);
		CreateFace(position, xSize, zSize, 0,0,zSize, -90, 0, 0, vertices, triangles, normals, uv);
	
		cubeIndex++;
		faceIndex = 0;
	}
	
	// Use this for initialization
	void Start () {
		faceIndex = 0;
		cubeIndex = 0;
		int numberOfCubes = 5;
		int facesPerCube = 6;
		
		Vector3[] vertices = new Vector3[4*facesPerCube*numberOfCubes];
		int[] triangles = new int[6*facesPerCube*numberOfCubes];
		Vector3[] normals = new Vector3[4*facesPerCube*numberOfCubes];
		Vector2[] uv = new Vector2[4*facesPerCube*numberOfCubes];
		
		{
			Vector3 size = new Vector3(20, 10, 20);
			Vector3 position = new Vector3(-5,-12,-26);
			CreateCuboid(vertices, triangles, normals, uv, size, position);
		}
		
		for (int i = 0; i < 4; i++) {
			Vector3 size = new Vector3(60, 1, 6);
			Vector3 position = new Vector3(-42,-2 + i * 9,-20);
			CreateCuboid(vertices, triangles, normals, uv, size, position);
		}
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
	}
	
	// Update is called once per frame
	void Update () {
		Start ();
	}
}

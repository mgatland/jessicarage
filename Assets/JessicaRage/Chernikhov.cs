using UnityEngine;
using System.Collections;

public class Chernikhov : MonoBehaviour {
	
	void CreateFace(int xSize, int ySize, float xOff, float yOff, float zOff, float xRot, float yRot, float zRot, Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uv, int faceIndex) {
		int o4 = faceIndex * 4;
		Vector3 offset = new Vector3(xOff, yOff, zOff);
		vertices[0+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(0,0,0) + offset;
		vertices[1+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(xSize,0,0) + offset;
		vertices[2+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(0,ySize,0) + offset;
		vertices[3+o4] = Quaternion.Euler(xRot, yRot, zRot) * new Vector3(xSize,ySize,0) + offset;
		
		int o6 = faceIndex * 6;
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

	}
	
	void CreateCuboid(Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uv, int cubeIndex) {
		int xSize = 20;
		int ySize = 15;
		int zSize = 10;
		
			
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		
		//sides
		CreateFace(xSize, ySize, 0,0,0, 0, 0, 0, vertices, triangles, normals, uv, 0 + cubeIndex*6);
		CreateFace(zSize, ySize, 0,0,zSize, 0, 90, 0, vertices, triangles, normals, uv, 1 + cubeIndex*6);
		CreateFace(xSize, ySize, xSize,0,zSize, 0, 180, 0, vertices, triangles, normals, uv, 2 + cubeIndex*6);
		CreateFace(zSize, ySize, xSize,0,0, 0, 270, 0, vertices, triangles, normals, uv, 3 + cubeIndex*6);
		//top and bottom
		CreateFace(xSize, zSize, 0,ySize,0, 90, 0, 0, vertices, triangles, normals, uv, 4 + cubeIndex*6);
		CreateFace(xSize, zSize, 0,0,zSize, -90, 0, 0, vertices, triangles, normals, uv, 5 + cubeIndex*6);
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
	}
	
	// Use this for initialization
	void Start () {
		int numberOfCubes = 1;
		int facesPerCube = 6;
		
		Vector3[] vertices = new Vector3[4*facesPerCube*numberOfCubes];
		int[] triangles = new int[6*facesPerCube*numberOfCubes];
		Vector3[] normals = new Vector3[4*facesPerCube*numberOfCubes];
		Vector2[] uv = new Vector2[4*facesPerCube*numberOfCubes];
		
		CreateCuboid(vertices, triangles, normals, uv, 0);
	}
	
	// Update is called once per frame
	void Update () {
		Start ();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor; //for adding a menu item

[ExecuteInEditMode]
public class Chernikhov : MonoBehaviour {
	
	public GameObject template;
	public GameObject clonesContainer;
	
	public Material red;
	public Material lightGrey;
	public Material brown;
	public Material darkGrey;
	public Material black;
	public Material brightred;
	public Material blue;
	public Material test;
	
	private bool isTesting = false;
	
	private int cubeIndex;
	private int faceIndex;
	
	void CreateFace(Vector3 position, float xSize, float ySize, float xOff, float yOff, float zOff, float xRot, float yRot, float zRot, Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uv, Vector2[] uv2) {
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
		
		//divide the texture into 6 faces for light maps
		//0, 1 and 2 across the top
		//3, 4 and 5 across the middle (the bottom strip is unused)
		float width = 0.3333f;
		float xPos = (faceIndex % 3) * width;
		float yPos = faceIndex >= 3 ? width : 0f;
		uv2[0+o4] = new Vector2(0+xPos, 0+yPos);
		uv2[1+o4] = new Vector2(width+xPos, 0+yPos);
		uv2[2+o4] = new Vector2(0+xPos, width+yPos);
		uv2[3+o4] = new Vector2(width+yPos, width+yPos);
		
		
		faceIndex++;
	}
	
	void CreateCuboid(Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uv,Vector2[] uv2, Vector3 size, Vector3 position) {
		float xSize = size.x;
		float ySize = size.y;
		float zSize = size.z;

		//sides
		CreateFace(position, xSize, ySize, 0,0,0, 0, 0, 0, vertices, triangles, normals, uv, uv2);
		CreateFace(position, zSize, ySize, 0,0,zSize, 0, 90, 0, vertices, triangles, normals, uv, uv2);
		CreateFace(position, xSize, ySize, xSize,0,zSize, 0, 180, 0, vertices, triangles, normals, uv, uv2);
		CreateFace(position, zSize, ySize, xSize,0,0, 0, 270, 0, vertices, triangles, normals, uv, uv2);
		//top and bottom
		CreateFace(position, xSize, zSize, 0,ySize,0, 90, 0, 0, vertices, triangles, normals, uv, uv2);
		CreateFace(position, xSize, zSize, 0,0,zSize, -90, 0, 0, vertices, triangles, normals, uv, uv2);

		cubeIndex++;
		faceIndex = 0;
	}
	
	private class CubeDefinition {
		public Vector3 size;
		public Vector3 position;
		public CubeDefinition(Vector3 size, Vector3 position) {
			this.size = size;
			this.position = position;
			this.position.x -= this.size.x / 2f;
			this.position.y -= this.size.y / 2f;
			this.position.z -= this.size.z / 2f;
		}
		
		public CubeDefinition(Vector3 size, Vector3 position, bool translate) {
			this.size = size;
			this.position = position;
			if (translate) {
				this.position.x -= this.size.x / 2f;
				this.position.y -= this.size.y / 2f;
				this.position.z -= this.size.z / 2f;
			} else {
				this.position /= 2;
				this.size /= 2;
			}
		}
	}
	
	void createThing(CubeDefinition cube, Material material) {
		List<CubeDefinition> cubes = new List<CubeDefinition>();
		cubes.Add (cube);
		createThing (cubes, material);
	}
	
	void createThing(List<CubeDefinition> cubes, Material material) {
		
		if (Application.isEditor && !Application.isPlaying && isTesting) material = test;
		
		faceIndex = 0;
		cubeIndex = 0;
		int numberOfCubes = cubes.Count;
		int facesPerCube = 6;
		
		Vector3[] vertices = new Vector3[4*facesPerCube*numberOfCubes];
		int[] triangles = new int[6*facesPerCube*numberOfCubes];
		Vector3[] normals = new Vector3[4*facesPerCube*numberOfCubes];
		Vector2[] uv = new Vector2[4*facesPerCube*numberOfCubes];
		Vector2[] uv2 = new Vector2[4*facesPerCube*numberOfCubes];
		
		for (int i = 0; i < cubes.Count; i++) {
			CreateCuboid(vertices, triangles, normals, uv, uv2,
				cubes[i].size, cubes[i].position);
		}
		
		GameObject clone = Instantiate(template, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		clone.tag = "clone";
		clone.transform.parent = clonesContainer.transform;
		clone.renderer.material = material;
		MeshFilter meshFilter = clone.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.uv2 = uv2;
		
		MeshCollider meshCollider = clone.GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}

	void createTower1 () {
		List<CubeDefinition> cubes = new List<CubeDefinition>();
		//base
		{
			Vector3 size = new Vector3(20, 10, 20);
			Vector3 position = new Vector3(-1,-8,-20);
			cubes.Add(new CubeDefinition(size, position));
		}
		
		//brown walkways
		for (int i = 0; i < 4; i++) {
			Vector3 size = new Vector3(60, 1, 6);
			Vector3 position = new Vector3(-16,-2 + i * 9,-20);
			cubes.Add(new CubeDefinition(size, position));
		}
		createThing(cubes, brown);
		
		cubes.Clear();
		//dark grey walkways
		for (int i = 0; i < 4; i++) {
			Vector3 size = new Vector3(6, 1, 50);
			Vector3 position = new Vector3(-13,6 + i * 9,-23);
			cubes.Add(new CubeDefinition(size, position));
		}
		createThing (cubes, darkGrey);
		
		cubes.Clear();
		//light grey walkways
		for (int i = 0; i < 4; i++) {
			Vector3 size = new Vector3(6, 1, 50);
			Vector3 position = new Vector3(-21,6 + i * 9,-23);
			cubes.Add(new CubeDefinition(size, position));
		}
		createThing (cubes, lightGrey);
		
		//Small Red Pillars
		cubes.Clear();
		{
			//end of red walkway
			Vector3 size = new Vector3(1, 26, 1);
			Vector3 position = new Vector3(13, 11.5f,-17.6f);
			cubes.Add(new CubeDefinition(size, position));
			
			position = new Vector3(13, 11.5f, -22.4f);
			cubes.Add(new CubeDefinition(size, position));
			
			//center
			size = new Vector3(1, 53, 1);
			position = new Vector3(-9.5f, 18, -16.5f);
			cubes.Add (new CubeDefinition(size, position));
			
			position = new Vector3(-9.5f, 18, -23.5f);
			cubes.Add (new CubeDefinition(size, position));
		}
		createThing (cubes, red);
		
		{
			cubes.Clear ();
			//red wall
			addCube (cubes, v3(1, 70, 20), v3(-17.0f, 6, -35));
			createThing (cubes, red);
			
			//grey wall
			cubes.Clear ();
			addCube (cubes, v3(1, 70, 20), v3(-17.0f, 6, -6));
			createThing (cubes, lightGrey);
			
			//signpost
			cubes.Clear ();
			addCube (cubes, v3(1, 7, 15), v3(-8.5f, 45, -16.5f));
			createThing (cubes, lightGrey);
			
			//concrete base
			cubes.Clear ();
			addCube(cubes, v3 (100, 20, 60), v3 (1, -24, -13));
			createThing(cubes, lightGrey);
		}
	}
	
	void addCube(List<CubeDefinition> cubes, Vector3 size, Vector3 pos) {
		cubes.Add (new CubeDefinition(size, pos));
	}
	
	Vector3 v3 (float x, float y, float z) {
		return new Vector3(x,y,z);
	}
	
	CubeDefinition cd(Vector3 size, Vector3 pos) {
		return new CubeDefinition(size, pos, false);	
	}
	
	//--------
	void CreateB2() {
		Vector3 offset = v3 (25, 0, -400);
		var pillarSize = v3 (0.9f, 18, 1);
		//vertical pillars
		createThing(cd (pillarSize, offset + v3 (0.05f, 1, 0)), brightred);
		createThing(cd (pillarSize, offset + v3 (0.05f, 1, 2)), brightred);
		createThing(cd (pillarSize, offset + v3 (0.05f, 1, 4)), brightred);
		createThing(cd (pillarSize, offset + v3 (0.05f, 1, 12)), brightred);
		createThing(cd (pillarSize, offset + v3 (0.05f, 1, 14)), brightred);
		createThing(cd (pillarSize, offset + v3 (0.05f, 1, 22)), brightred);
		
		//horizontal beams
		createThing(cd (v3 (1,1,23), offset + v3 (0, 0, 0)), brightred);
		createThing(cd (v3 (1,1,22), offset + v3 (0, 3, 1)), brightred);
		createThing(cd (v3 (1,1,22), offset + v3 (0, 6, 1)), brightred);
		createThing(cd (v3 (1,1,23), offset + v3 (0, 19, 0)), brightred);
	}
	
	void CreateBlueWall() {
		Vector3 offset = v3 (8, 0, -315);
		createXWall(offset, 28f, 9f, 0.5f, 1.5f, 1.5f, blue);
	}
	
	void createXWall(Vector3 offset, float length, float height, float thickness, float xGap, float yGap, Material material) {
		
		float beamThickness = thickness - 0.01f;
		
		//trim width and height to have no excess
		float xRemainder = (length - thickness) % (thickness + xGap);
		length -= xRemainder;
		float yRemainder = (height - beamThickness) % (beamThickness + yGap);
		height -= yRemainder;
		
		Vector3 pillarSize = v3 (thickness, height, thickness);
		float x = 0;
		while (x < length) {
			createThing(cd (pillarSize, offset + v3 (-x, 0f, 0f)), material);
			x += thickness + xGap;
		}
		
		Vector3 beam = v3 (length - thickness*2, beamThickness, beamThickness);
		float y = 0;
		while (y < height) {
			createThing(cd (beam, offset + v3 (-beam.x, y, 0.005f)), material);
			y += beamThickness + yGap;
		}

	}
	//--------------------------
	
	void CreateBuilding1() {
		Vector3 offset = v3 (25, 0, -350);
		B1Side(offset, v3 (0,0,0));
		B1Side(offset, v3 (-16,0,0));
		B1Top(offset);
	}
	
	void B1Side(Vector3 offset, Vector3 offset2) {
		var pillarSize = v3 (0.9f, 18, 1);
		//vertical pillars
		createThing(cd (pillarSize, offset + offset2 + v3 (0.05f, 1, 0)), black);
		createThing(cd (pillarSize, offset + offset2 + v3 (0.05f, 1, 10)), black);
		createThing(cd (pillarSize, offset + offset2 + v3 (0.05f, 1, 25)), black);
		createThing(cd (pillarSize, offset + offset2 + v3 (0.05f, 1, 35)), black);
		
		//horizontal beams
		createThing(cd (v3 (1,1,46), offset + offset2 + v3 (0, 0, 0)), red); //long on ground
		createThing(cd (v3 (1,1,34), offset + offset2 + v3 (0, 3, 1)), red);
		createThing(cd (v3 (1,1,34), offset + offset2 + v3 (0, 6, 1)), red);
		createThing(cd (v3 (1,1,56), offset + offset2 + v3 (0, 19, -10)), black); //long on top
	}
	
	void B1Top(Vector3 offset) {
		//in the same direction as the walls
		for (int i = 0; i < 5; i++) {
			createThing ( cd(v3(0.9f,0.9f,48), offset + v3 (-i*3 + 0.05f, 19+0.05f, -9)), black);	
		}
		//crossways
		for (int i = 0; i < 8; i++) {
			createThing ( cd(v3(15,1,1), offset + v3 (-15, 19, -10 + i * 7)), red);	
		}
	}
	
	//--------------------------
	
	// Use this for initialization
	void Start () {
		// Z means back-left in the drawing
		// -X is back-right
		// Y is downwards
		
		createTower1();
		CreateBuilding1();  //bottom left one with a roof. 
		CreateB2(); //red wall above >z of it
		CreateBlueWall(); //blue wall heading in negative x-direction
		
		//CreateRedWall
		Vector3 redWallPos = v3 (-30, 0, -400);
		createXWall(redWallPos, 128f, 80f, 3f, 6f, 6f, brightred);
	}
	
	// (ctrl-g on Windows, cmd-g on OS X).
	[MenuItem ("MyMenu/Regenerate Geometry %g")]
	static void DoSomethingWithAShortcutKey () {
		Debug.Log ("Delete existing clones and generate new ones...");
		GameObject[] clones = GameObject.FindGameObjectsWithTag("clone");
		for (int i = 0; i < clones.Length; i++) {
			DestroyImmediate (clones[i]);
		}
		((Chernikhov)GameObject.FindObjectOfType(typeof(Chernikhov))).Start();
	}
	
	void Update () {
	}
}

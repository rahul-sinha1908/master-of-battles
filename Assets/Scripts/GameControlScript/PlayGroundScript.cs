using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class PlayGroundScript : MonoBehaviour {

	private Mesh mesh;
	private MeshCollider meshCollider;
	private Vector3[] vertices;
	private Vector3 planeOffset;
	private void Awake()
	{
		GetComponent<MeshFilter>().mesh=mesh=new Mesh();
		meshCollider=GetComponent<MeshCollider>();
		mesh.name="My Play Ground";
		generateVertices();
		generateTriangles();

		meshCollider.sharedMesh=mesh;
	}
	private void generateVertices(){
		int xSize=GameContants.sizeOfBoardX,ySize=GameContants.sizeOfBoardY;
		xSize+=1;
		ySize+=1;
		vertices = new Vector3[xSize*ySize];
		planeOffset=new Vector3(-xSize/2*GameContants.boxSize,0,-ySize/2*GameContants.boxSize);
		
		for (int i = 0, y = 0; y < ySize; y++) {
			for (int x = 0; x < xSize; x++, i++) {
				vertices[i] = new Vector3(x*GameContants.boxSize,0 ,y*GameContants.boxSize)+planeOffset;
			}
		}
		mesh.vertices=vertices;
	}
	private void generateTriangles(){
		int xSize=GameContants.sizeOfBoardX,ySize=GameContants.sizeOfBoardY;
		int[] triangles = new int[xSize*ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			//break;
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles=triangles;
		mesh.RecalculateNormals();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnDrawGizmos () {
		if(vertices==null)
			return;
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}
}

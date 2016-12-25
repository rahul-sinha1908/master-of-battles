using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class PlayGroundScript : MonoBehaviour {

	private Mesh mesh;
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;
	private Vector3[] vertices;
	private Vector3 planeOffset;
	private void Awake()
	{
		GetComponent<MeshFilter>().mesh=mesh=new Mesh();
		meshCollider=GetComponent<MeshCollider>();
		meshRenderer=GetComponent<MeshRenderer>();
		// Color c=meshRenderer.material.color;
		// c.a=0.2f;
		// meshRenderer.material.color=c;
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
		planeOffset=new Vector3(-xSize/2*GameContants.boxSize,0.1f,-ySize/2*GameContants.boxSize);
		
		for (int i = 0, y = 0; y < ySize; y++) {
			for (int x = 0; x < xSize; x++, i++) {
				vertices[i] = new Vector3(x*GameContants.boxSize,0 ,y*GameContants.boxSize)+planeOffset;
			}
		}
		xSize--;ySize--;
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				//uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				float x1=x%2,y1=y%2;
				if(x1==0)
					x1=0.05f;
				else
					x1=0.95f;
				if(y1==0)
					y1=0.05f;
				else
					y1=0.95f;

				uv[i] = new Vector2(x1, y1);
				tangents[i] = tangent;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;
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

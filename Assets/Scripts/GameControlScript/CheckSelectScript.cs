using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class CheckSelectScript : MonoBehaviour {
	private Mesh mesh;
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;
	private List<Vector3> vertices;
	private List<Vector2> uv;
	private List<int> triangles;
	private Vector3 planeOffset;
	private Vector3 heightOffset=new Vector3(0,0.11f,0);
	private void Awake()
	{
		GetComponent<MeshFilter>().mesh=mesh=new Mesh();
		meshRenderer=GetComponent<MeshRenderer>();
		mesh.name="Selected Play Ground";
		vertices=new List<Vector3>();
		uv=new List<Vector2>();
		triangles=new List<int>();
		//generateVertices();
		//generateTriangles();
	}

	private void generateVerticesAndTriangle(int x, int y){
		x=x-GameContants.sizeOfBoardX/2;
		y=y-GameContants.sizeOfBoardY/2;
		x*=GameContants.boxSize;
		y*=GameContants.boxSize;
		vertices.Add(new Vector3(x,0,y)+heightOffset);
		uv.Add(new Vector2(0,0));
		vertices.Add(new Vector3(x+GameContants.boxSize,0,y)+heightOffset);
		uv.Add(new Vector2(1,0));
		vertices.Add(new Vector3(x,0,y+GameContants.boxSize)+heightOffset);
		uv.Add(new Vector2(0,1));
		vertices.Add(new Vector3(x+GameContants.boxSize,0,y+GameContants.boxSize)+heightOffset);
		uv.Add(new Vector2(1,1));
		int vi=vertices.Count-4;
		for(int k=0;k<6;k++)
			triangles.Add(0);
		int ti=triangles.Count-6;
		triangles[ti] = vi;
		triangles[ti + 3] = triangles[ti + 2] = vi + 1;
		triangles[ti + 4] = triangles[ti + 1] = vi + 2;
		triangles[ti + 5] = vi + 3;
		Debug.Log(vertices.ToArray().Length+" : "+uv.ToArray().Length+" : "+triangles.ToArray().Length);
		// for(int i=0;i<vertices.Count;i++)
		// 	Debug.Log(vertices[i]);
		// for(int i=0;i<triangles.Count;i++)
		// 	Debug.Log(triangles[i]);
		mesh.triangles=null;
		mesh.uv=null;
		mesh.vertices=null;
		mesh.vertices = vertices.ToArray();
		mesh.uv = uv.ToArray();
		mesh.triangles=triangles.ToArray();
		mesh.RecalculateNormals();
		//mesh.tangents = tangents;
	}

	public void showSelectedTiles(List<Point> points, int colorInd){
		vertices.Clear();
		triangles.Clear();
		uv.Clear();

		for(int i=0;i<points.Count;i++){
			generateVerticesAndTriangle(points[i].x,points[i].y);
		}
	}
	public void showSelectedTiles(Point point, int colorInd){
		vertices.Clear();
		triangles.Clear();
		uv.Clear();
		generateVerticesAndTriangle(point.x,point.y);
	}
	public void addSelectedTiles(List<Point> points, int colorInd){
		vertices.Clear();
		triangles.Clear();
		uv.Clear();

		for(int i=0;i<points.Count;i++){
			generateVerticesAndTriangle(points[i].x,points[i].y);
		}
	}
	public void addSelectedTiles(Point point, int colorInd){
		generateVerticesAndTriangle(point.x,point.y);
	}

	// Update is called once per frame
	void Update () {
		
	}
}

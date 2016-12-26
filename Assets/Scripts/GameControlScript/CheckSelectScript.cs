using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class CheckSelectScript : MonoBehaviour {

	public struct Reference{
		public Point p;
		public int colorInd;
	};
	List<Reference> listRef=new List<Reference>();
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

	private void generateVerticesAndTriangle(int x, int y, int colorInd){
		x=x-GameContants.sizeOfBoardX/2;
		y=y-GameContants.sizeOfBoardY/2;
		x*=GameContants.boxSize;
		y*=GameContants.boxSize;
		vertices.Add(new Vector3(x,0,y)+heightOffset);
		vertices.Add(new Vector3(x+GameContants.boxSize,0,y)+heightOffset);
		vertices.Add(new Vector3(x,0,y+GameContants.boxSize)+heightOffset);
		vertices.Add(new Vector3(x+GameContants.boxSize,0,y+GameContants.boxSize)+heightOffset);
		
		if(colorInd==BoardConstants.Select){
			uv.Add(new Vector2(0,0.5f));
			uv.Add(new Vector2(0.5f,0.5f));
			uv.Add(new Vector2(0,1f));
			uv.Add(new Vector2(0.5f,1f));
		}else if(colorInd==BoardConstants.Move){
			uv.Add(new Vector2(0.5f,0.5f));
			uv.Add(new Vector2(1f,0.5f));
			uv.Add(new Vector2(0.5f,1f));
			uv.Add(new Vector2(1f,1f));
		}

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
	}

	private void refreshMesh(){
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
		listRef.Clear();

		for(int i=0;i<points.Count;i++){
			int k=checkExisting(points[i], colorInd);
			if(k==-1)
				generateVerticesAndTriangle(points[i].x,points[i].y, colorInd);
		}
		refreshMesh();
	}
	public void showSelectedTiles(Point point, int colorInd){
		List<Point> l=new List<Point>();
		l.Add(point);
		showSelectedTiles(l,colorInd);
	}
	public void addSelectedTiles(List<Point> points, int colorInd){

		for(int i=0;i<points.Count;i++){
			int k=checkExisting(points[i], colorInd);
			Debug.Log("K = "+k);
			if(k==-1)
				generateVerticesAndTriangle(points[i].x,points[i].y, colorInd);
			else{
				removeExisting(k);
			}
		}
		refreshMesh();
	}
	public void addSelectedTiles(Point point, int colorInd){
		List<Point> l=new List<Point>();
		l.Add(point);
		addSelectedTiles(l,colorInd);
	}

	private int checkExisting(Point p, int colorInd){
		for(int i=0;i<listRef.Count;i++){
			if(listRef[i].p.x==p.x && listRef[i].p.y==p.y)
				return i;
		}
		Reference r;
		r.p=p;
		r.colorInd=colorInd;
		listRef.Add(r);
		return -1;
	}
	private void removeExisting(int i){
		List<Point> list=new List<Point>();
		List<int> col=new List<int>();
		for(int k=0;k<listRef.Count;k++){
			if(k!=i){
				list.Add(listRef[k].p);
				col.Add(listRef[k].colorInd);
			}
		}
		listRef.Clear();
		triangles.Clear();
		uv.Clear();
		vertices.Clear();
		for(i=0;i<list.Count;i++){
			addSelectedTiles(list[i],col[i]);
		}
			
	}
	// Update is called once per frame
	void Update () {
		
	}
}

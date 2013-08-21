using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewTimedTrailRenderer : MonoBehaviour
{
	public bool emit = true;
	public float emitTime = 0.00f;
	public Material material;
	
	public float lifeTime = 1.00f;
	
	public Color[] colors;
	public float[] sizes;
	
	public float uvLengthScale = 0.01f;
	public bool higherQualityUVs = true;
	
	public float maxRebuildTime = 0.015f;
	public bool autoDestruct = false;
	
	private List<Point> pointList;
	private GameObject o;
	private float lastRebuildTime = 0.00f;

	private Transform myTransform;
	private Camera mainCamera;
	
	private Vector3[] newVertices;
	private Vector2[] newUV;
	private int[] newTriangles;
	private Color[] newColors;
	
	private float tempPointTime;
	private Color tempPointColor;
	private float tempPointSize;
	
	private Mesh mesh;

	public class Point
	{
		public float timeCreated = 0.00f;
		public Vector3 position;
		public bool lineBreak = false;
		
		public Vector3 lineDirection = Vector3.zero;
		public Vector3 perpendicularVector = Vector3.zero;
	}
	
	private void Awake()
	{
		mainCamera = Camera.main;
		myTransform = transform;
		pointList = new List<Point>();
		
		o = new GameObject("Trail-" + name);
		o.transform.parent = null;
		o.transform.position = Vector3.zero;
		o.transform.rotation = Quaternion.identity;
		o.transform.localScale = Vector3.one;
		o.AddComponent(typeof(MeshFilter));
		o.AddComponent(typeof(MeshRenderer));
		o.renderer.material = material;
		mesh = (o.GetComponent(typeof(MeshFilter)) as MeshFilter).mesh;
	}
	
	void OnEnable()
	{
		if (o == null)
		{
			o = new GameObject("Trail");
			o.transform.parent = null;
			o.transform.position = Vector3.zero;
			o.transform.rotation = Quaternion.identity;
			o.transform.localScale = Vector3.one;
			o.AddComponent(typeof(MeshFilter));
			o.AddComponent(typeof(MeshRenderer));
			o.renderer.material = material;
			mesh = (o.GetComponent(typeof(MeshFilter)) as MeshFilter).mesh;
		}
	}

	void OnDisable ()
	{
		Destroy(o);   
	}

	private void LateUpdate()
	{
		//Se deve emitir apenas por um tempo
		if(emit && emitTime != 0)
		{
			emitTime -= Time.deltaTime;
			if(emitTime < 0)
				emit = false;
		}
  
		//Se nao esta emitindo e todos os pontos ja foram removidos, e estiver setado para autodestruir
		if(!emit && pointList.Count == 0 && autoDestruct)
		{
			Destroy(o);
			Destroy(gameObject);
		}
  
		
		//Se deve emitir, cria um ponto na posicao atual
		if(emit)
		{
			if (Time.time - lastRebuildTime > maxRebuildTime)
			{
				Point p = new Point();
				p.position = myTransform.position;
				p.timeCreated = Time.time;
				pointList.Add(p);
			}
		}
		
		
		//Se existe algum ponto, precisa dar rebuild
		bool rebuild = false;
		if(pointList.Count > 1)
		{
			if(Time.time - lastRebuildTime > maxRebuildTime)
				rebuild = true;
		}


		if(rebuild)
		{
			lastRebuildTime = Time.time;
			
			//Verifica na lista de pontos quais ja passaram do lifetime
			for (int p = 0; p < pointList.Count; p++)
			{
				if (Time.time - pointList[p].timeCreated > lifeTime)
				{
					pointList.Remove(pointList[p]);
					p--;
				}
			}
			
			
			if (pointList.Count > 1)
			{
				newVertices = new Vector3[pointList.Count * 2];
				newUV = new Vector2[pointList.Count * 2];
				newTriangles = new int[(pointList.Count - 1) * 6];
				newColors = new Color[pointList.Count * 2];
				
				float curDistance = 0.00f;
				
				for (int p = 0; p < pointList.Count; p++)
				{
					//Calcula quanto % da vida desse ponto ja passou
					tempPointTime = (Time.time - pointList[p].timeCreated) / lifeTime;
					
					
					//Calcula a cor que esse ponto deve ter
					if (colors != null && colors.Length > 0)
					{
						if (tempPointTime < 1)
						{
							float colorLerpTime = tempPointTime * (colors.Length - 1);
							int firstColorIndex = Mathf.FloorToInt(colorLerpTime);
							colorLerpTime -= firstColorIndex;
							tempPointColor = Color.Lerp(colors[firstColorIndex], colors[firstColorIndex + 1], colorLerpTime);
						}
						else
							tempPointColor = colors[colors.Length - 1];
					}
					else
						tempPointColor = Color.Lerp(Color.white, Color.clear, tempPointTime);
					
					
					//Calcula o tamanho que esse ponto deve ter
					if (sizes != null && sizes.Length > 0)
					{
						if (tempPointTime < 1)
						{
							float sizeLerpTime = tempPointTime * (sizes.Length - 1);
							int firstSizeIndex = Mathf.FloorToInt(sizeLerpTime);
							sizeLerpTime -= firstSizeIndex;
							tempPointSize = Mathf.Lerp(sizes[firstSizeIndex], sizes[firstSizeIndex + 1], sizeLerpTime);
						}
						else
							tempPointSize = sizes[sizes.Length - 1];
					}
					else
						tempPointSize = 1;
					
					
					//Acha a direcao entre este ponto e o proximo (ou se for o primeiro, entre o proximo e esse)
					if (pointList[p].lineDirection == Vector3.zero)
					{
						if (p == 0)
							pointList[p].lineDirection = pointList[p].position - pointList[p + 1].position;
						else
							pointList[p].lineDirection = pointList[p - 1].position - pointList[p].position;
					}
					
					
					//Acha o vetor perpendicular para posicionar os vertices deste ponto
					if (pointList[p].perpendicularVector == Vector3.zero)
						pointList[p].perpendicularVector = Vector3.Cross(pointList[p].lineDirection, mainCamera.transform.position - pointList[p].position).normalized;
					
					
					//Diz a posicao dos vertices relativos a esse ponto
					newVertices[p * 2] = pointList[p].position + (pointList[p].perpendicularVector * (tempPointSize * 0.5f));
					newVertices[(p * 2) + 1] = pointList[p].position + (-pointList[p].perpendicularVector * (tempPointSize * 0.5f));
					
					
					//Diz a cor que estara nos vertices relativos a esse ponto
					newColors[p * 2] = newColors[(p * 2) + 1] = tempPointColor;
					
				
					//Diz como o UV deve ser nos vertices relativos a esse ponto
					newUV[p * 2] = new Vector2(curDistance * uvLengthScale, 0);
					newUV[(p * 2) + 1] = new Vector2(curDistance * uvLengthScale, 1);
					
					
					//Cria os triangulos
					if(p > 0)// && !pointList[p - 1].lineBreak)
					{
						if(higherQualityUVs)
							curDistance += (pointList[p].position - pointList[p - 1].position).magnitude;
						else
							curDistance += (pointList[p].position - pointList[p - 1].position).sqrMagnitude;
						
						newTriangles[(p - 1) * 6] = (p * 2) - 2;
						newTriangles[((p - 1) * 6) + 1] = (p * 2) - 1;
						newTriangles[((p - 1) * 6) + 2] = p * 2;
						
						newTriangles[((p - 1) * 6) + 3] = (p * 2) + 1;
						newTriangles[((p - 1) * 6) + 4] = p * 2;
						newTriangles[((p - 1) * 6) + 5] = (p * 2) - 1;
					}
				}
				
				mesh.Clear();
				mesh.vertices = newVertices;
				mesh.colors = newColors;
				mesh.uv = newUV;
				mesh.triangles = newTriangles;
			}
		}
		else
		{
			if (mesh.vertices.Length > 0)
				mesh.Clear();
		}
	}
}
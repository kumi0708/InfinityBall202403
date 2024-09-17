using OpenCvSharp;
using OpenCvSharp.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

[System.Serializable]
public class CameraTransform
{
	public Vector3 position;
	public Vector3 scale;
}
public class CountorFinder : WebCamera
{
	[SerializeField] private FlipMode ImageFlip;
	[SerializeField,Range(0,300)] private float Threshold = 96.0f;
	[SerializeField] private bool ShowPrecessingImae = true;
	[SerializeField] private float CureAccuracy = 10f; 
	[SerializeField] private float MinArea = 5000f;
	//[SerializeField] private PolygonCollider2D PolygonCollider;
	private Mat image;
	private Mat processImage = new Mat();
	private Point[][] contours;
	private HierarchyIndex[] hierarchy;
	private Vector2[] vectorList;


	private GameObject polygonCollider_;

	// èâä˙ílÇ™ê›íË
	private CameraTransform[] Transforms ={
		new CameraTransform() { position = new Vector3(-102f, 59f),scale = new Vector3(0.107f, -0.107f) }, // 1920 1080
		new CameraTransform() { position = new Vector3(-102.64f,58.73f),scale = new Vector3(0.1605f, -0.1606f) }, // 1280Å~720
		new CameraTransform() { position = new Vector3(-102.65f, 58.75f),scale = new Vector3(0.32127f, -0.241f) }, // 1920 /2 1080 /2
	};

	private int CameraTransformIndex = 1;
	bool ClearFlag_ = false;
	[SerializeField]
	float ClearTime_ = 5.0f;
	float ClearTimer_ = 0.0f;

	protected override void Awake()
	{
		polygonCollider_ = new GameObject("polygonCollider_");
		polygonCollider_.transform.localPosition = Transforms[CameraTransformIndex].position;
		polygonCollider_.transform.localScale = Transforms[CameraTransformIndex].scale;

		polygonCollider_.AddComponent<PolygonCollider2D>();
		base.Awake();
	}
	
	private void OnDestroy()
	{
		Destroy(polygonCollider_);
	}
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ClearTimer_ = ClearTime_;
			ClearFlag_ = true;
			Surface.GetComponent<RawImage>().texture = null;

		}
		if (ClearTimer_ > 0)
		{
			ClearTimer_ -= Time.deltaTime;
		}
		else
		{
			ClearFlag_ = false;
		}
	}

	protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
	{
		if (ClearFlag_)
		{
			polygonCollider_.GetComponent<PolygonCollider2D>().pathCount = 0;
			return false;
		}
		image = OpenCvSharp.Unity.TextureToMat(input);

		Cv2.Flip(image,image, ImageFlip);
		Cv2.CvtColor(image, processImage,ColorConversionCodes.BGR2GRAY);
		Cv2.Threshold(processImage,processImage,Threshold,255,ThresholdTypes.Binary);
		Cv2.FindContours(processImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple,null);

		polygonCollider_.GetComponent<PolygonCollider2D>().pathCount = 0;
		foreach (var contour in contours)
		{
			var area = Cv2.ContourArea(contour);

			if (area > MinArea)
			{
				var points = Cv2.ApproxPolyDP(contour, CureAccuracy, true);
				drawContour(processImage,new Scalar(127, 127, 127),2,points);

				polygonCollider_.GetComponent<PolygonCollider2D>().pathCount++;
				polygonCollider_.GetComponent<PolygonCollider2D>().SetPath(polygonCollider_.GetComponent<PolygonCollider2D>().pathCount-1, toVectpr2(points));
			}
		}

		if (output == null)
		{
			output = OpenCvSharp.Unity.MatToTexture(ShowPrecessingImae ? processImage : image);
		}
		else
		{
			OpenCvSharp.Unity.MatToTexture(ShowPrecessingImae ? processImage : image, output);
		}
		return true;
	}

	private Vector2 [] toVectpr2(Point[] points)
	{

		vectorList  = new Vector2[points.Length];
		for (var i = 0; i < points.Length; i++)
		{
			vectorList[i] = new Vector2(points[i].X, points[i].Y);
		}
		return vectorList;
	}

	private Vector2[] toVector2(Point[] points, int width, int height)
	{
		float Mx = (float)Screen.width / (float)width;
		float My = (float)Screen.height / (float)height;
		int _x = 0;
		int _y = 0;
		vectorList = new Vector2[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			_x = Convert.ToInt32(points[i].X * Mx);
			_y = Convert.ToInt32((height - points[i].Y) * My);
			Vector3 p = new Vector3(_x, _y, 0);
			p = Camera.main.ScreenToWorldPoint(p);
			vectorList[i] = new Vector2(p.x, p.y);
		}
		return vectorList;
	}

	private void drawContour( Mat image, Scalar Color ,int ThsicKress, Point[] points)
	{
		for (var i = 1; i < points.Length; i++)
		{
			Cv2.Line(image, points[i - 1], points[i], Color, ThsicKress);
		}
		Cv2.Line(image, points[points.Length - 1], points[0], Color, ThsicKress);
	}
}

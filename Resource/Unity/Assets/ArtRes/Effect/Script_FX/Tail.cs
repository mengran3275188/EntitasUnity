using UnityEngine;
using System.Collections;

public class Tail : MonoBehaviour {

    private int savedIndex;

    private int pointIndex; 

    //public bool FourPointsInterpolate = false;
    // Material - Particle Shader with "Tint Color" property

    public Material material;

    private bool _start = false;

    // Emit

    public bool emit

    {

        get { return Emit; }

        set { Emit = value; }

    }

    private bool Emit = false;

    private bool emittingDone = false;

    private Vector3 _lastPosition = new Vector3();

    //Minimum velocity (script terminates)

    //public float minVel = 10;


    // Facing

    private bool faceCamera = false;


    // Lifetime of each segment

    public float lifetime = 1;

    private float lifeTimeRatio = 1;
    public float[] StartEndTime;
    private float nextStateChangeTime = 0;
    private int _startEndTimeCount = 0;
	public float LastTime = 2.0f;
    public float UpAdd = 0.0f;
    private float _lastTimer = 0.0f;
    private int _maxRenderPoint = 30;
    private float fadeOutRatio;

    // Colors

    public Color[] colors;


    // Widths

    public float[] widths;


    // Optimization

    //public float pointDistance = 0.5f;

    private float pointSqrDistance = 0;

    public int segmentsPerPoint = 4;

    public int uvSegments = 0;

    private float tRatio;
    // Print Output

    //public bool printResults = false;

    //public bool printSavedPoints = false;

    //public bool printSegmentPoints = false;


    // Objects

    private GameObject trail = null;

    private Mesh mesh = null;

    private Material trailMaterial = null;

    private Color _startColor;


    // Points

    private Vector3[] saved;

    private Vector3[] savedUp;

    private Vector3[] savedForward;

    private int savedCnt = 0;

    private Vector3[] points;

    private Vector3[] SecondPoints;

    private Vector3[] pointsUp;

    private int pointCnt = 0;


    // Segment Appearance Normalization

    private int displayCnt = 0;

    private float lastPointCreationTime = 0;

    private float averageCreationTime = 0;

    private float averageInsertionTime = 0;

    private float elapsedInsertionTime = 0;
    // Initialization

    private bool initialized = false;


    private void Start()

    {
        if (StartEndTime.Length > 0)
        {
            nextStateChangeTime = StartEndTime[0];
        }

        // Data Inititialization

        saved = new Vector3[128];

        savedUp = new Vector3[saved.Length];

        savedForward = new Vector3[saved.Length];

        points = new Vector3[saved.Length * segmentsPerPoint];

        SecondPoints = new Vector3[saved.Length * segmentsPerPoint];

        pointsUp = new Vector3[points.Length];

        tRatio = 1f / (segmentsPerPoint);

        //pointSqrDistance = pointDistance * pointDistance;


        // Create the mesh object

        trail = new GameObject("Trail");

        GameObject.Destroy(trail, LastTime + lifetime - 0.2f);

        trail.transform.position = Vector3.zero;

        trail.transform.rotation = Quaternion.identity;

        trail.transform.localScale = Vector3.one;

        MeshFilter meshFilter = (MeshFilter) trail.AddComponent(typeof(MeshFilter));

        mesh = meshFilter.mesh;

        trail.AddComponent(typeof(MeshRenderer));

        trailMaterial = new Material(material);

        fadeOutRatio = trailMaterial.GetColor("_TintColor").a;

        trail.GetComponent<Renderer>().material = trailMaterial;

        _lastPosition = transform.position;

        _startColor = trailMaterial.GetColor("_TintColor");

    }



    private void printPoints()

    {

        if(savedCnt == 0)

            return;

        string s = "Saved Points at time " + Time.time + ":\n";

        for(int i = 0; i < savedCnt; i++)

            s += "Index: " + i + "\tPos: " + saved[i] + "\n";

        print(s);

    }


    private void printAllPoints()

    {

        if(pointCnt == 0)

            return;

        string s = "Points at time " + Time.time + ":\n";

        for(int i = 0; i < pointCnt; i++)

            s += "Index: " + i + "\tPos: " + points[i] + "\n";

        print(s);

    }

    private void findCoordinates(int index)

    {

        Vector3 P1 = saved[index - 1];

        Vector3 P2 = saved[index];

        int pointIndex = (index - 1) * segmentsPerPoint;

        for (int i = pointIndex; i < pointIndex + segmentsPerPoint; i++)
        {

            float t = (i - pointIndex) * tRatio;

            float t2 = t * t;

            float t3 = t2 * t;

            float blend1 = 2 * t3 - 3 * t2 + 1;

            float blend2 = 3 * t2 - 2 * t3;

            float blend3 = t3 - 2 * t2 + t;

            float blend4 = t3 - t2;

            int pntInd = i;

            points[pntInd] = blend1 * P1 + blend2 * P2 + blend3 * (P2 - P1).magnitude * savedForward[index - 1] + blend4 * blend3 * (P2 - P1).magnitude * savedForward[index];

            pointsUp[pntInd] = Vector3.Lerp(savedUp[index - 1], savedUp[index], t);

        }

        pointCnt = pointIndex +segmentsPerPoint;

        points[pointCnt] = saved[index];

        pointsUp[pointCnt] = savedUp[index];

        pointCnt += 1;

    }

    private void findCoordinates4Points(int index)
    {

        if (index == 0 || index > savedCnt - 2)
        {
            return;
        }

        Vector3 P0 = saved[index - 1];

        Vector3 P1 = saved[index];

        Vector3 P2 = saved[index + 1];

        Vector3 P3 = saved[index + 2];

        Vector3 T1 = 0.5f * (P2 - P0);

        Vector3 T2 = 0.5f * (P3 - P1);

        int pointIndex = index * segmentsPerPoint;

        for (int i = pointIndex; i < pointIndex + segmentsPerPoint; i++)
        {

            float t = (i - pointIndex) * tRatio;

            float t2 = t * t;

            float t3 = t2 * t;

            float blend1 = 2 * t3 - 3 * t2 + 1;

            float blend2 = 3 * t2 - 2 * t3;

            float blend3 = t3 - 2 * t2 + t;

            float blend4 = t3 - t2;

            int pntInd = i - segmentsPerPoint;

            points[pntInd] = blend1 * P1 + blend2 * P2 + blend3 * T1 + blend4 * T2;

            pointsUp[pntInd] = Vector3.Lerp(savedUp[index], savedUp[index + 1], t);

        }

        pointCnt = pointIndex;
    }

    private void findCoordinatesEx(int index)
    {

        Vector3 P0;

        Vector3 P1;

        Vector3 P2;

        Vector3 P3;

        Vector3 T1;

        Vector3 T2;

        Vector3 SecP0;

        Vector3 SecP1;

        Vector3 SecP2;

        Vector3 SecP3;

        Vector3 SecT1;

        Vector3 SecT2;

        float width = 1.0f;

        if (index == 1)
        {
            P0 = saved[index - 1];

            P1 = P0;

            P2 = saved[index];

            P3 = P2;

            T1 = 0.5f * (P2 - P0);

            T2 = 0.5f * (P3 - P1);

            SecP0 = saved[index - 1] - width * savedUp[index - 1];

            SecP1 = SecP0;

            SecP2 = saved[index] - width * savedUp[index];

            SecP3 = SecP2;

            SecT1 = 0.5f * (SecP2 - SecP0);

            SecT2 = 0.5f * (SecP3 - SecP1);
        }
        else
        {
            P0 = saved[index - 2];

            P1 = saved[index - 1];

            P2 = saved[index];

            P3 = saved[index];

            T1 = 0.5f * (P2 - P0);

            T2 = 0.5f * (P3 - P1);

            SecP0 = saved[index - 2] - width * savedUp[index - 2];

            SecP1 = saved[index - 1] - width * savedUp[index - 1];

            SecP2 = saved[index] - width * savedUp[index];

            SecP3 = SecP2;

            SecT1 = 0.5f * (SecP2 - SecP0);

            SecT2 = 0.5f * (SecP3 - SecP1);
        }



        int pointIndex = (index - 1) * segmentsPerPoint;

        for (int i = pointIndex; i < pointIndex + segmentsPerPoint; i++)
        {

            float t = (i - pointIndex) * tRatio;

            float t2 = t * t;

            float t3 = t2 * t;

            float blend1 = 2 * t3 - 3 * t2 + 1;

            float blend2 = 3 * t2 - 2 * t3;

            float blend3 = t3 - 2 * t2 + t;

            float blend4 = t3 - t2;

            int pntInd = i;

            //float add = (P2 - P1).magnitude;

            //points[pntInd] = blend1*P1 + blend2*P2 + blend3*add*savedForward[index - 1] +blend4 * add * savedForward[index];

            //SecondPoints[pntInd] = blend1*SecP1 + blend2*SecP2 + blend3*add*savedForward[index - 1] +blend4 * add * savedForward[index];

            points[pntInd] = blend1 * P1 + blend2 * P2 + blend3 * T1 + blend4 * T2;

            SecondPoints[pntInd] = blend1 * SecP1 + blend2 * SecP2 + blend3 * SecT1 + blend4 * SecT2;

            pointsUp[pntInd] = Vector3.Lerp(savedUp[index - 1], savedUp[index], t);

        }

        pointCnt = pointIndex + segmentsPerPoint;

        points[pointCnt] = saved[index];

        SecondPoints[pointCnt] = saved[index] - width*savedUp[index];

        pointsUp[pointCnt] = savedUp[index];

        pointCnt += 1;

        if (index < 3 || index > savedCnt  )
        {

            return;
        }

        P0 = saved[index - 3];

        P1 = saved[index - 2];

        P2 = saved[index - 1];

        P3 = saved[index];

        T1 = 0.5f * (P2 - P0);

        T2 = 0.5f * (P3 - P1);

        SecP0 = saved[index - 3] - width*savedUp[index - 3];

        SecP1 = saved[index - 2] - width * savedUp[index - 2];

        SecP2 = saved[index - 1] - width * savedUp[index - 1];

        SecP3 = saved[index] - width * savedUp[index];

        SecT1 = 0.5f * (SecP2 - SecP0);

        SecT2 = 0.5f * (SecP3 - SecP1);

        pointIndex = (index - 2) * segmentsPerPoint;

        for (int i = pointIndex; i < pointIndex + segmentsPerPoint; i++)
        {

            float t = (i - pointIndex) * tRatio;

            float t2 = t * t;

            float t3 = t2 * t;

            float blend1 = 2 * t3 - 3 * t2 + 1;

            float blend2 = 3 * t2 - 2 * t3;

            float blend3 = t3 - 2 * t2 + t;

            float blend4 = t3 - t2;

            int pntInd = i;

            points[pntInd] = blend1 * P1 + blend2 * P2 + blend3 * T1 + blend4 * T2;

            SecondPoints[pntInd] = blend1 * SecP1 + blend2 * SecP2 + blend3 * SecT1 + blend4 * SecT2;

        }

    }

    private void LateUpdate()

    {
        _lastTimer += Time.deltaTime;
        if (StartEndTime.Length >0 && _startEndTimeCount < StartEndTime.Length)
        {
            if (_lastTimer > nextStateChangeTime)
            {
                emit = !emit;
                trailMaterial.SetColor("_TintColor", _startColor);
                _startEndTimeCount++;
                if (_startEndTimeCount < StartEndTime.Length)
                {
                    nextStateChangeTime += StartEndTime[_startEndTimeCount];
                }
                if (emit)
                {
                    Reset();
                }
            }
        }
        if (_lastTimer > LastTime)
        {
            Destroy(trail);
            Destroy(gameObject);
        }
		
        _lastPosition = transform.position;
			
        if (savedCnt > saved.Length-1)
        {
            return;
        }

        Vector3 position = transform.position;

        // Wait till the object is active (update called) and emitting

        if( ! initialized && Emit)

        {

            //// Place the first point behind this as a starter projected point

            //saved[savedCnt] = position + transform.forward * UpAdd - transform.up * pointDistance;

            //savedUp[savedCnt] =  transform.forward;

            //savedCnt++;

            // Place the second point at the current position

            saved[savedCnt] = position + transform.forward * UpAdd;

            savedUp[savedCnt] = transform.forward;

            savedForward[savedCnt] = transform.up;

            savedCnt++;

            // Begin tracking the saved point creation time

            lastPointCreationTime = Time.time;

            initialized = true;

        }


        //if(printSavedPoints)

        //    printPoints();

        //if(printSegmentPoints)

        //    printAllPoints();


        if(Emit)

        {


            // Do we save a new point?

            //if( (saved[savedCnt-1] - position).sqrMagnitude > pointSqrDistance)

            //{

                saved[savedCnt] = position + transform.forward * UpAdd;

                savedUp[savedCnt] = transform.forward;

                savedForward[savedCnt] = transform.up;

                savedCnt++;
	
	            // Calc the average point display time
	
                //if(averageCreationTime == 0)
	
                //    averageCreationTime = Time.time - lastPointCreationTime;
	
                //else
	
                //{
	
                //    float elapsedTime = Time.time - lastPointCreationTime;
	
                //    averageCreationTime = (averageCreationTime + elapsedTime) * 0.5f;
	
                //}
	
                //averageInsertionTime = averageCreationTime * tRatio;
	
                //lastPointCreationTime = Time.time;
	
	            // Calc the last saved segment coordinates

                //if (!FourPointsInterpolate)
                //{
                //    if (savedCnt > 1)
                //        findCoordinates(savedCnt - 1);
                //}
                //else
                //{
                //    if (savedCnt > 3)
                //        findCoordinates4Points(savedCnt - 3);
                //}

            if(savedCnt > 1)
                findCoordinatesEx(savedCnt - 1);
        }

        // Do we fade it out?

        if( ! Emit && displayCnt == pointCnt)

        {
            Color color = trailMaterial.GetColor("_TintColor");

            if (color.a > 0)
            {

                color.a -= fadeOutRatio*lifeTimeRatio*Time.deltaTime;

                trailMaterial.SetColor("_TintColor", color);
            }

            else
            {

                //if(printResults)

                //    print("Trail effect ending with a segment count of: " + pointCnt);

            }

            return;

        }

        displayCnt = pointCnt;

        // Do we render this?

        if(displayCnt < 2)

        {

            trail.GetComponent<Renderer>().enabled = false;

            return;

        }

        trail.GetComponent<Renderer>().enabled = true;         

        // Common data

        lifeTimeRatio = 1f / lifetime;

        Color[] meshColors;

        int startIndex = Mathf.Max(0, displayCnt - _maxRenderPoint);

        // Rebuild the mesh

        Vector3[] vertices = new Vector3[displayCnt * 2];

        Vector2[] uvs = new Vector2[displayCnt * 2];

        int[] triangles = new int[(displayCnt-1) * 6];

        meshColors = new Color[displayCnt * 2];


        float pointRatio = 1f / _maxRenderPoint;

        //Vector3 cameraPos = Camera.main.transform.position;

        //Vector3 pointDir = Camera.main.transform.up;

        for (int i = 0; i < displayCnt - startIndex; i++)

        {

            Vector3 point = points[i + startIndex];

            Vector3 SecPoint = SecondPoints[i + startIndex];

            float ratio = i * pointRatio;

            float uvRatio = i * 1.0f / Mathf.Min(displayCnt, _maxRenderPoint);

            // Color

            Color color;

            if(colors.Length == 0)

                color = Color.Lerp(Color.clear, Color.white, ratio);

            else if(colors.Length == 1)

                color = Color.Lerp(Color.clear, colors[0], ratio);

            else if(colors.Length == 2)

                color = Color.Lerp(colors[1], colors[0], ratio);

            else

            {

                float colorRatio = colors.Length - 1 - ratio * (colors.Length-1);

                if(colorRatio == colors.Length-1)

                    color = colors[colors.Length-1];

                else

                {

                    int min = (int) Mathf.Floor(colorRatio);

                    float lerp = colorRatio - min;

                    color = Color.Lerp(colors[min+0], colors[min+1], lerp);

                }

            }

            meshColors[i * 2] = color;

            meshColors[(i * 2) + 1] = color;


            // Width

            float width;

            if(widths.Length == 0)

                width = 1;

            else if(widths.Length == 1)

                width = widths[0];

            else if(widths.Length == 2)

                width = Mathf.Lerp(widths[1], widths[0], ratio);

            else

            {

                float widthRatio = widths.Length - 1 - ratio * (widths.Length-1);

                if(widthRatio == widths.Length-1)

                    width = widths[widths.Length-1];

                else

                {

                    int min = (int) Mathf.Floor(widthRatio);

                    float lerp = widthRatio - min;

                    width = Mathf.Lerp(widths[min+0], widths[min+1], lerp);

                }

            }

            // Vertices

//            if(faceCamera)
//
//            {
//
//                Vector3 from = i == displayCnt-1 ?  points[i-1]   : point;
//
//                Vector3 to = i == displayCnt-1 ?    point    : points[i+1];
//
//                //pointDir = to - from;
//
//                Vector3 vectorToCamera = cameraPos - point;
//
//                Vector3 perpendicular = Vector3.Cross(pointDir, vectorToCamera).normalized;
//
//                vertices[i * 2] = point + perpendicular * width;
//
//                vertices[i*2 + 1] = point;// -perpendicular * width * 0.5f;
//
//            }
//
//            else

            {

                vertices[i*2] = point;// +pointsUp[i] * width;

                vertices[i * 2 + 1] = point - (point - SecPoint) * width;

            }


            // UVs

            uvs[i * 2 + 0] = new Vector2(Mathf.Max(0.05f,Mathf.Min(0.95f, uvRatio)), 0.05f);

            uvs[i * 2 + 1] = new Vector2(Mathf.Max(0.05f,Mathf.Min(0.95f, uvRatio)), 0.95f + uvSegments);

            if(i > 0)

            {

                // Triangles

                int triIndex = (i - 1) * 6;

                int vertIndex = i * 2;

                triangles[triIndex+0] = vertIndex - 2;

                triangles[triIndex+1] = vertIndex - 1;

                triangles[triIndex+2] = vertIndex - 0;


                triangles[triIndex+3] = vertIndex + 0;

                triangles[triIndex+4] = vertIndex - 1;

                triangles[triIndex+5] = vertIndex + 1;

            }

        }

        trail.transform.position = Vector3.zero;

        trail.transform.rotation = Quaternion.identity;

        mesh.Clear();

        mesh.vertices = vertices;

        mesh.colors = meshColors;

        mesh.uv = uvs;

        mesh.triangles = triangles;

    }

    private void Reset()
    {
        saved.Initialize();

        savedUp.Initialize();

        savedForward.Initialize();

        savedCnt = 0;

        points.Initialize();

        SecondPoints.Initialize();

        pointsUp.Initialize();

        initialized = false;

        if (mesh != null)
            mesh.Clear();
    }
}



using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/FreeCamera")]
public class FreeCamera : MonoBehaviour
{
    private Vector3 oldMousePos;
    private Vector3 newMosuePos;
    private Camera bindCamera;

    [SerializeField]
    private bool isKeyBoardActive = true;
    [SerializeField]
    private float minimumY = 0.2f;
    [SerializeField]
    private float keyBoardMoveSpeed = 0.2f;//键盘控制相机移动的速度
    [SerializeField]
    private float rotSpeed = 0.05f;//鼠标右键控制相机旋转的速度    
    [SerializeField]
    private float mouseMoveSpeed = 0.5f;//鼠标中键控制相机移动的速度
    [SerializeField]
    private float zoomSpeed = 200.0f;//鼠标中键控制相机缩放的速度

    private Transform followedObject;
    //private Vector3 followedPosition;
    private float distance = 5;
    private Vector3 centerOffset = Vector3.zero;

    public static FreeCamera Instance;
    void Awake()
    {
        Instance = this;
        this.bindCamera = GetComponent<Camera>();
    }

    void Start()
    {
        oldMousePos = newMosuePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isKeyBoardActive)
        {
            MoveCameraKeyBoard();
        }
        ZoomCamera();
        SuperViewMouse();

        oldMousePos = Input.mousePosition;
    }

    void LateUpdate()
    {
        if (followedObject)
        {
            Vector3 targetPositon = this.followedObject.transform.position + this.centerOffset + transform.forward * -distance;
            targetPositon.y = Mathf.Max(targetPositon.y, this.minimumY);
            this.transform.position = Vector3.Lerp(this.transform.position, targetPositon, Time.unscaledDeltaTime * 15);
            //this.transform.position = targetPositon;
        }
    }

    private void MoveCameraKeyBoard()
    {
        if (/*Input.GetKey(KeyCode.A) ||*/ Input.GetKey(KeyCode.LeftArrow))//(Input.GetAxis("Horizontal")<0)
        {
            transform.Translate(new Vector3(-keyBoardMoveSpeed, 0, 0), Space.Self);
        }
        if (/*Input.GetKey(KeyCode.D) ||*/ Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(keyBoardMoveSpeed, 0, 0), Space.Self);
        }
        if (/*Input.GetKey(KeyCode.W) ||*/ Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, keyBoardMoveSpeed), Space.Self);
        }
        if (/*Input.GetKey(KeyCode.S) ||*/ Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -keyBoardMoveSpeed), Space.Self);
        }
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    transform.Translate(new Vector3(0, keyBoardMoveSpeed, 0), Space.World);
        //}
        //if (Input.GetKey(KeyCode.E))
        //{
        //    if (transform.transform.position.y - keyBoardMoveSpeed >= this.minimumY)
        //        transform.Translate(new Vector3(0, -keyBoardMoveSpeed, 0), Space.World);
        //}
    }

    private void ZoomCamera()
    {
        float offset = Input.GetAxis("Mouse ScrollWheel");
        if (offset != 0)
        {
            offset *= zoomSpeed;
            //currentCamera.transform.position = currentCamera.transform.position + currentCamera.transform.forward * offset;//localPosition
            this.distance -= offset;
            //if ((transform.position + Vector3.forward * offset).y >= minimumY)
            transform.Translate(Vector3.forward * offset, Space.Self); //
        }
    }

    private void SuperViewMouse()
    {
        if (Input.GetMouseButton(1))
        {
            newMosuePos = Input.mousePosition;
            Vector3 dis = newMosuePos - oldMousePos;
            float angleX = dis.x * rotSpeed;//* Time.deltaTime
            float angleY = dis.y * rotSpeed;//* Time.deltaTime
            transform.Rotate(new Vector3(-angleY, 0, 0), Space.Self);
            transform.Rotate(new Vector3(0, angleX, 0), Space.World);
        }

        if (Input.GetMouseButton(2))
        {
            newMosuePos = Input.mousePosition;
            Vector3 dis = newMosuePos - oldMousePos;
            float angleX = dis.x * mouseMoveSpeed;//* Time.deltaTime
            float angleY = dis.y * mouseMoveSpeed;//* Time.deltaTime
            transform.Translate(new Vector3(-angleX, 0, 0), Space.Self);
            if (transform.position.y - angleY >= minimumY)
                transform.Translate(new Vector3(0, -angleY, 0), Space.Self);
        }
    }

    public void FocusOn(Transform obj, bool includeEffects = false)
    {
        //1、Mark
        this.followedObject = obj;
        //this.followedPosition = obj.position;
        //2、计算Bounds
        var renderers = obj.GetComponentsInChildren<Renderer>();
        //Vector3 lookAtPoint = Vector3.zero;
        Bounds bounds = new Bounds();
        bool initBounds = false;
        foreach (Renderer r in renderers)
        {
            if (!((r is TrailRenderer) || (r is ParticleSystemRenderer))) //|| (r is ParticleRenderer) 
            {
                if (!initBounds)
                {
                    initBounds = true;
                    bounds = r.bounds;
                    //lookAtPoint = r.bounds.center;
                }
                else
                {
                    bounds.Encapsulate(r.bounds);
                }
            }
        }
        //3、计算distance
        float max = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        distance = max * 2f / Mathf.Tan(Mathf.Deg2Rad * bindCamera.fieldOfView / 2);
        //4、计算center offset
        this.centerOffset = bounds.center - this.followedObject.position;
        //transform.position = transform.forward * -distance + lookAtPoint;//
        //5、记录顶点
        this.CreateBoundBox(obj.gameObject);
    }

    public void UnFocus()
    {
        this.followedObject = null;
    }


    private Vector3[] boundsVertex;
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void OnRenderObject()
    {
        if (this.followedObject != null)
        {
            if (this.boundsVertex.Length < 8)
                return;
            if (!lineMaterial)
                CreateLineMaterial();
            lineMaterial.SetPass(0);// Apply the line material

            GL.PushMatrix();
            GL.MultMatrix(this.followedObject.transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            GL.Color(Color.green);

            GL.Vertex(this.boundsVertex[0]);
            GL.Vertex(this.boundsVertex[1]);
            GL.Vertex(this.boundsVertex[1]);
            GL.Vertex(this.boundsVertex[2]);
            GL.Vertex(this.boundsVertex[2]);
            GL.Vertex(this.boundsVertex[3]);
            GL.Vertex(this.boundsVertex[3]);
            GL.Vertex(this.boundsVertex[0]);

            GL.Vertex(this.boundsVertex[4]);
            GL.Vertex(this.boundsVertex[5]);
            GL.Vertex(this.boundsVertex[5]);
            GL.Vertex(this.boundsVertex[6]);
            GL.Vertex(this.boundsVertex[6]);
            GL.Vertex(this.boundsVertex[7]);
            GL.Vertex(this.boundsVertex[7]);
            GL.Vertex(this.boundsVertex[4]);

            GL.Vertex(this.boundsVertex[0]);
            GL.Vertex(this.boundsVertex[4]);
            GL.Vertex(this.boundsVertex[1]);
            GL.Vertex(this.boundsVertex[5]);
            GL.Vertex(this.boundsVertex[2]);
            GL.Vertex(this.boundsVertex[6]);
            GL.Vertex(this.boundsVertex[3]);
            GL.Vertex(this.boundsVertex[7]);

            GL.End();
            GL.PopMatrix();
        }
    }

    private void CreateBoundBox(GameObject obj)
    {
        Bounds bounds;
        MeshFilter filter = obj.GetComponent<MeshFilter>();
        if (filter != null)
            bounds = filter.mesh.bounds;
        else
            throw new UnityException("Can't find the MeshFilter in " + obj.ToString());

        bounds.Expand(0.02f);
        if (this.boundsVertex == null)
            this.boundsVertex = new Vector3[8];
        this.boundsVertex[0] = bounds.extents;
        this.boundsVertex[1] = new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
        this.boundsVertex[2] = new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
        this.boundsVertex[3] = new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
        this.boundsVertex[4] = new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
        this.boundsVertex[5] = new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
        this.boundsVertex[6] = -bounds.extents;
        this.boundsVertex[7] = new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

        for (int i = 0; i < this.boundsVertex.Length; i++)
        {
            this.boundsVertex[i] += bounds.center;
        }
    }
}
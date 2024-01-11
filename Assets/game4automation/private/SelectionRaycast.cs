
using System.Collections.Generic;
using NaughtyAttributes;
using RuntimeInspectorNamespace;
using UnityEngine.Rendering;

using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace game4automation
{
#pragma warning disable 0108
    [System.Serializable]
    public class Game4AutomationEventSelected: UnityEvent<GameObject, bool>
    {
    }

    [System.Serializable]
    public class Game4AutomationEventHovered: UnityEvent<GameObject, bool>
    {
    }
    
    [System.Serializable]
    public class GameAutomationEventBlockRotation: UnityEvent<bool>
    {
    }
    
    


    //! Selection Raycast for selecting objects during runtime
    public class SelectionRaycast : Game4AutomationBehavior
    {
        public bool AlwaysOn = false; //!<if true the selection is always on even if button is turned off in game4automation controller
        public bool IsActive = true; //!<selection by raycast is active
        public bool EnableToch = true; //!<allow touch input
        public bool AutomaticallyAddColliders = true;
        public bool ChangeMaterialOnHover = true; //!<change material on hover
        [ShowIf("ChangeMaterialOnHover")]public Material HighlightMaterial; //!<the highlight materiials
        public bool ChangeMaterialOnSelect = true; //!<change material on select
        [ShowIf("ChangeMaterialOnSelect")]public Material SelectMaterial;//!<the select material
        [ReorderableList] public List<string> SelectionLayer; //!<the layers that can be selected
        public string UILayer = "UI"; //!<the layer for the UI - selection will be disabled if UI is touched
        public string ContextToolbarLayer = "g4a ContextToolbar"; //!<the layer for the toolbar - selection will be kept active if toolbar is touched
        [ReadOnly] public bool ObjectIsTouched;
        [ReadOnly] public GameObject TouchedObject;
        [ReadOnly] public bool ObjectIsSelected;
        [ReadOnly] public GameObject SelectedObject;//!<the selected object
        [ReadOnly] public Vector3 SelectedPosition; //!<the selected object hit point position
        [ReadOnly] public bool ObjectIsHovered;
        [ReadOnly] public GameObject HoveredObject;//!<the hovered object
        [ReadOnly] public Vector3 HoveredPosition;//!<the hovered object hit point position
        [ReadOnly] public bool DoubleSelect; 
        [ReadOnly] public bool OnUI; 
        [ReadOnly] public bool OnContextToolbar; //!<true if the object was double clicked
        public bool PingHoverObject; //!<true if the hovered object should be pinged in the hierarchy
        public bool SelectHoverObject; //!<true if the hovered object should be selected in the hierarchy
        public bool PingSelectObject; //!<true if the selected object should be pinged in the hierarchy
        public bool SelectSelectObject; //!<true if the selected object should be selected in the hierarchy
        public bool AutoCenterSelectedObject; //!<true if the selected object (its selection point) is automatically centered in the scene view
        public bool ZoomDoubleClickedObject=true; //!<true if the selected object (its selection point) is automatically centered in the scene view when double clicking on it
        public bool FocusDoubleClickedObject=true; //!<true if the selected object (its selection point) is automatically centered in the scene view when double clicking on it
        public bool OpenRuntimeINspector;
        public bool ShowSelectedIcon;

        [Foldout("Events")] public Game4AutomationEventSelected
            EventSelected; //!<  Unity event which is called for MU enter and exit. On enter it passes MU and true. On exit it passes MU and false.
        [Foldout("Events")] public Game4AutomationEventHovered
            EventHovered; //!<  Unity event which is called for MU enter and exit. On enter it passes MU and true. On exit it passes MU and false.
        
        [Foldout("Events")] public GameAutomationEventBlockRotation
            EventBlockRotation; //!<  Unity event which is called when rotation should be blocked

        public RuntimeInspector RuntimeInspector;
        public GameObject SelectedIcon;

        private Vector3 Hitpoint;
        private Vector3 distancehitpoint;
        private RaycastHit GObject;
        private SceneMouseNavigation navigate;
        private int layermask;
        private int uilayer;
        private int toolbarlayer;
        private Camera camera;
        private List<ObjectSelection> selections = new List<ObjectSelection>();
        private List<ObjectSelection> hovers = new List<ObjectSelection>();
        private bool isactivebefore = false;
        private GameObject selectedicon;
        private Material lineMaterial;
        private bool isDrawing;
        private bool inittouch = false;
        
        
        public void SelectObject(GameObject obj)
        {
            SelectedObject = obj;
            ObjectIsSelected = true;
            HighlightSelectObject(true, SelectedObject);
            OnSelected();
            var selectable = obj.GetComponent<ISelectable>();
            if (selectable != null)
                selectable.OnSelected();

            // On Touch block rotation and register it as touched
            if (Input.touchCount ==1  && TouchedObject != obj)
            {
                inittouch = true;
            }
            //Debug.Log("Select "+ obj.name);
        }

        public void DeSelectObject(GameObject obj)
        {
            // Debug.Log("DeSelect "+ obj.name);
            HighlightSelectObject(false, SelectedObject);
            SelectedObject = null;
            ObjectIsSelected = false;
            OnDeSeselected();
            var selectable = obj.GetComponent<ISelectable>();
            if (selectable != null)
                selectable.OnDeselected();
        }

        public void DeSelectObject()
        {
            if (SelectedObject != null)
              DeSelectObject(SelectedObject);
        }
        
        public void DeSelectIfNotThis(GameObject obj)
        {
            if (SelectedObject != null && SelectedObject != obj)
                DeSelectObject(SelectedObject);
        }
        
        public Vector3 GetHitpoint()
        {
            return SelectedObject.transform.position - distancehitpoint;
        }

        public void ShowCenterIcon(bool show)
        {
            if (show)
            {
                if (SelectedIcon != null && ShowSelectedIcon)
                {
                    if (selectedicon == null)
                        selectedicon = Instantiate(SelectedIcon, Hitpoint, Quaternion.identity);
                    selectedicon.transform.position = GetHitpoint();
                }
            }
            else
            {
                if (selectedicon != null)
                    DestroyImmediate(selectedicon);
            }
        }

        //Returns 'true' if we touched or hovering on Unity UI element.
        public bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }
 
 
        //Returns 'true' if we touched or hovering on Unity UI element.
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == uilayer)
                    return true;
            }
            return false;
        }
        
        public bool IsPointerOverToolbar()
        {
            return IsPointerOverToolbar(GetEventSystemRaycastResults());
        }
        
        private bool IsPointerOverToolbar(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == toolbarlayer)
                    return true;
            }
            return false;
        }
 
 
        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

        
        private void HighlightHoverObject(bool highlight, GameObject currObj)
        {
            var meshrenderer = currObj.GetComponentInChildren<MeshRenderer>();
            if (highlight)
            {
                HoveredPosition = Hitpoint;
                if (ChangeMaterialOnHover)
                {
                    var mu = currObj.GetComponent<MU>();
                    if (mu != null)
                    {
                        var meshes = mu.GetComponentsInChildren<MeshRenderer>();
                        foreach (var mesh in meshes)
                        {
                            var sel = mesh.gameObject.AddComponent<ObjectSelection>();
                            sel.SetNewMaterial(HighlightMaterial);
                            hovers.Add(sel);
                        }
                    }
                    else
                    {
                        var sel = currObj.AddComponent<ObjectSelection>();
                        sel.SetNewMaterial(HighlightMaterial);
                        hovers.Add(sel);
                    }
                }

#if UNITY_EDITOR
                if (PingHoverObject)
                    EditorGUIUtility.PingObject((currObj));
                if (HoveredObject)
                    if (SelectHoverObject)
                        Selection.objects = new[] {currObj};
#endif
            }
            else
            {
                HoveredPosition = Vector3.zero;
                if (ChangeMaterialOnHover)
                {
                    foreach (var hover in hovers)
                    {
                        hover.ResetMaterial();
                    }
                    hovers.Clear();
                }
            
            }
        }

        private void HighlightSelectObject(bool highlight, GameObject currObj)
        {
            MU mu = null;
            if (currObj != null)
                mu = currObj.GetComponent<MU>();
           
            if (highlight)
            {

#if UNITY_EDITOR
                if (PingSelectObject)
                    EditorGUIUtility.PingObject((currObj));
                if (SelectSelectObject)
                    Selection.objects = new[] {currObj};
#endif
                if (!ChangeMaterialOnSelect)
                    return;

                if (mu != null)
                {
                    var meshes = mu.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mesh in meshes)
                    {
                        var sel = mesh.gameObject.AddComponent<ObjectSelection>();
                        sel.SetNewMaterial(SelectMaterial);
                        selections.Add(sel);
                    }
                }
                else
                {
                    var sel = currObj.AddComponent<ObjectSelection>();
                    sel.SetNewMaterial(SelectMaterial);
                    selections.Add(sel);
                }

            

            }
            else
            {
                if (!ChangeMaterialOnSelect)
                    return;

                foreach (var sel in selections)
                {
                    sel.ResetMaterial();
                }
                selections.Clear();
            }
        }

        void OnSelected()
        {
            Debug.Log("Selected " + SelectedObject.name);
            SelectedPosition = Hitpoint;
            distancehitpoint = SelectedObject.transform.position - Hitpoint;
            // Set an Icon at Hitpoint
        
            if (EventSelected != null) EventSelected.Invoke(SelectedObject,true);
            if (OpenRuntimeINspector)
            {
                // enable gameobject runtimeinspector
                if (RuntimeInspector != null && Game4AutomationController.RuntimeInspectorEnabled)
                {
                    RuntimeInspector.gameObject.SetActive(true);
                    RuntimeInspector.Inspect(SelectedObject);
                }
            }

        }

        void OnDeSeselected()
        {
         
            SelectedPosition = Vector3.zero;
     
            if (EventSelected != null) EventSelected.Invoke(SelectedObject,false);
            if (OpenRuntimeINspector)
            {
                if (RuntimeInspector != null && Game4AutomationController.RuntimeInspectorEnabled)
                {
                    RuntimeInspector.StopInspect();
                    RuntimeInspector.gameObject.SetActive(false);
                }
            }
            ShowCenterIcon(false);
               
        }
        
        
        void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

        public void CheckPointerBackground()
        {
            OnUI = IsPointerOverUIElement();
            OnContextToolbar = IsPointerOverToolbar();
        }
        
        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            OnPostRender();
        }
       
        private void OnPostRender()
        {
            if (!isDrawing && Application.isPlaying)
            {
                Color color = Color.white;
                BoxCollider boxcollider = null;
                if (ObjectIsHovered)
                {
                    color = Color.yellow;
                    boxcollider = HoveredObject.GetComponent<BoxCollider>();
                }
                if (ObjectIsSelected)
                { 
                    color = Color.green;
                    boxcollider = SelectedObject.GetComponent<BoxCollider>();
                }
                if (boxcollider != null)
                {
                    isDrawing = true;
                    lineMaterial.SetPass(0);
                    GL.PushMatrix();
                    //GL.MultMatrix(transform.localToWorldMatrix);
                    GL.Begin(GL.LINES);
                    GL.Color(color);

                    Vector3 min = boxcollider.bounds.min;
                    Vector3 max = boxcollider.bounds.max;

                    // Draw the lines of the box
                    GL.Vertex3(min.x, min.y, min.z);
                    GL.Vertex3(max.x, min.y, min.z);
                    
                    GL.Vertex3(max.x, min.y, min.z);
                    GL.Vertex3(max.x, min.y, max.z);
                    
                    GL.Vertex3(max.x, min.y, max.z);
                    GL.Vertex3(min.x, min.y, max.z);
                    
                    GL.Vertex3(min.x, min.y, max.z);
                    GL.Vertex3(min.x, min.y, min.z);
                    
                    GL.Vertex3(min.x, max.y, min.z);
                    GL.Vertex3(max.x, max.y, min.z);
                    
                    GL.Vertex3(max.x, max.y, min.z);
                    GL.Vertex3(max.x, max.y, max.z);
                    
                    GL.Vertex3(max.x, max.y, max.z);
                    GL.Vertex3(min.x, max.y, max.z);
                    
                    GL.Vertex3(min.x, max.y, max.z);
                    GL.Vertex3(min.x, max.y, min.z);
                    
                    GL.Vertex3(min.x, min.y, min.z);
                    GL.Vertex3(min.x, min.y, max.z);

                    GL.Vertex3(max.x, min.y, min.z);
                    GL.Vertex3(max.x, min.y, max.z);

                    GL.Vertex3(max.x, max.y, min.z);
                    GL.Vertex3(max.x, max.y, max.z);

                    GL.Vertex3(min.x, max.y, min.z);
                    GL.Vertex3(min.x, max.y, max.z);
                    
                    GL.End();
                    GL.PopMatrix();
                }

                isDrawing = false;
            }
        }

        new void Awake()
        {
            base.Awake();

            if (!Game4AutomationController.ObjectSelectionEnabled && !AlwaysOn)
            {
                return;
            }
        
            
            camera = GetComponent<Camera>();
            // get all meshrenderers
            var meshrenderers = FindObjectsOfType<MeshRenderer>();

            if (AutomaticallyAddColliders)
            {
                foreach (var comp in meshrenderers)
                {
                    if (comp.gameObject.GetComponent<Collider>() == null)
                    {
                        // get mesh from gameobject
                        try
                        {
                            var collider = comp.gameObject.AddComponent<MeshCollider>();
                            collider.convex = true;
                        }
                        catch
                        {
                        }
                        comp.gameObject.layer = LayerMask.NameToLayer(SelectionLayer[0]);
                    }
                    else
                    {
                        var collider = comp.GetComponent<Collider>();
                        // check if collider is on default layer
                        if (collider.gameObject.layer == 0)
                        {
                            // set layer to selection layer
                            collider.gameObject.layer = LayerMask.NameToLayer(SelectionLayer[0]);
                        }
                    }
                }
            }
         
            navigate = gameObject.GetComponent<SceneMouseNavigation>();
            base.Awake();
            uilayer = LayerMask.NameToLayer(UILayer);
            toolbarlayer = LayerMask.NameToLayer(ContextToolbarLayer);
            layermask = LayerMask.GetMask(SelectionLayer.ToArray());
            
        }
        
        void Start()
        {
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
            isDrawing= false;
        }
        
        // Update is called once per frame
        void Update()
        {
 
            DoubleSelect = false;
            // turn component off if isactive = false
            if ((!IsActive || !Game4AutomationController.ObjectSelectionEnabled) && !AlwaysOn)
            {
                // hide selected and hovered objects which have been selected before
                if (isactivebefore && !IsActive)
                {
                    if (SelectedObject != null)
                    {
                        HighlightSelectObject(false,SelectedObject);
                        DeSelectObject(SelectedObject);
                    }

                    if (HoveredObject != null)
                    {
                        HighlightHoverObject(false,HoveredObject);
                        HoveredObject = null;
                        ObjectIsHovered= false;
                    }
                }
                isactivebefore = false;
                return;
            }

            isactivebefore = true;
            OnUI = IsPointerOverUIElement();
            OnContextToolbar = IsPointerOverToolbar();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var target =
                camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                    camera.nearClipPlane));
            GameObject mouseoverobj = null;

            bool hit = false;
            bool touchhit = false;
            bool touched = false;
            
            if (EnableToch)
            {
                // check if touch is active
                if (Input.touchCount ==1)
                {
                    touched = true;
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began || inittouch)
                    {
                        Ray touchray = Camera.main.ScreenPointToRay(touch.position);
                        if ((Physics.Raycast(touchray, out GObject, Mathf.Infinity, layermask) && !OnUI) || inittouch)
                        {
                            hit = true;
                            touchhit = true;
                            Hitpoint = GObject.point;
                            ObjectIsTouched = true;
                            if (!inittouch)
                                TouchedObject = GObject.transform.gameObject;
                            else
                            {
                                touchhit = false;
                                TouchedObject = SelectedObject;
                            }
                              
                            if (EventBlockRotation != null) EventBlockRotation.Invoke(true);
                        }
                        else
                        {
                            if (!OnContextToolbar)
                            {
                                // no hit but new touch and selected object is not null -> deselect
                                if (EventBlockRotation != null) EventBlockRotation.Invoke(false);
                                ObjectIsTouched = false;
                                if (TouchedObject != null && SelectedObject == TouchedObject)
                                {
                                    if (EventSelected != null) EventSelected.Invoke(TouchedObject, false);
                                    DeSelectObject(TouchedObject);
                                }
                            }
                        }
                    }
                }
            }

            // only do raycast from mouse if no touchhit
            if (!touched && ObjectIsTouched == false)
            {
                if (Physics.Raycast(ray, out GObject, Mathf.Infinity, layermask) && !OnUI)
                {
                    hit = true;
                    Hitpoint = GObject.point;
                }
            }


            if (hit && !inittouch)
            {
                GameObject NewHoveredObject = null;

                if (!inittouch)
                    NewHoveredObject = GObject.transform.gameObject;
                else
                {
                    NewHoveredObject = SelectedObject;
                    inittouch = false;
                }

            // check if selectable in standard true
                var selectable = true;

                // only if gameobject is not in game4automationcontroller
                if (NewHoveredObject.transform.parent != null)
                {
                    if (NewHoveredObject.transform.parent == Game4AutomationController.gameObject.transform)
                        selectable = false;
                }

                if (NewHoveredObject.GetComponentInChildren<MeshRenderer>() == null)
                    selectable = false;

                /// only if it is not already hovered or not the selected object
                mouseoverobj = NewHoveredObject;
                if (!(NewHoveredObject != HoveredObject && NewHoveredObject != SelectedObject))
                {
                    selectable = false;
                }

                if (selectable)
                {
                    // Selected object is changing, unhighlight old, highlicht new
                    if (HoveredObject != null)
                        HighlightHoverObject(false, HoveredObject);
                    HighlightHoverObject(true, NewHoveredObject);
                    HoveredObject = NewHoveredObject;
                    ObjectIsHovered = true;
                    if (EventHovered!=null) EventHovered.Invoke(HoveredObject,true);
                }
            }
            else
            {
                if (!touched && !OnContextToolbar)
                {
                    Hitpoint = Vector3.zero;
                    // No raycast is hitting - deselect selected object
                    if (HoveredObject != null)
                    {
                        HighlightHoverObject(false, HoveredObject);
                        if (EventHovered!=null) EventHovered.Invoke(HoveredObject,false);
                    }
                    HoveredObject = null;
                    ObjectIsHovered = false;
                }
            }
            

            if ((Input.GetMouseButtonDown(0) || touchhit) && !OnUI)
            {
                if (HoveredObject != null)
                {
                    if (SelectedObject != HoveredObject)
                    {
                        if (SelectedObject != null)
                        {
                            DeSelectObject(SelectedObject);
                        }
                           
                        if (HoveredObject != null)
                            HighlightHoverObject(false, HoveredObject);
                        SelectObject(HoveredObject);
                        if (EventHovered != null) EventHovered.Invoke(HoveredObject,false);
                        HoveredObject = null;
                        ObjectIsHovered = false;
                    }
                }
                else
                {
                    if (mouseoverobj == SelectedObject && mouseoverobj != null)
                    {
                        OnSelected();
                        DoubleSelect = true;
                      
                    }
                    else
                    {
                        if (SelectedObject != null && !OnContextToolbar)
                        {
                            if (EventSelected!= null) EventSelected.Invoke(SelectedObject,false);
                            DeSelectObject(SelectedObject);

                        }
                    }
                   
                }
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SelectedObject != null)
                {
                    if (EventSelected != null)
                        EventSelected.Invoke(SelectedObject,false);
                    DeSelectObject(SelectedObject);
                    HoveredObject = null;
                    ObjectIsHovered = false;
                }
            }
            
            inittouch = false;
        }
    }
}
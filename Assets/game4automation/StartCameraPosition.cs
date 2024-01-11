#if CINEMACHINE
using Cinemachine;
#endif

using UnityEngine;

namespace game4automation
{


    public class StartCameraPosition : MonoBehaviour
    {
        [Header("Actions")] public CameraPos cameraposition;

#if CINEMACHINE
    public CinemachineVirtualCamera cinemachinecam;
#endif
        private GenericButton _button;
        private SceneMouseNavigation _nav;



        // Start is called before the first frame update
        void Awake()
        {
            _button = GetComponent<GenericButton>();
            if(GameObject.Find("/game4automation/Main Camera"))
                _nav = GameObject.Find("/game4automation/Main Camera").GetComponent<SceneMouseNavigation>();
            _button.EventOnClick.AddListener(OnClick);
        }


        public void SetCameraPosition()
        {
            _nav.SetNewCameraPosition(cameraposition.TargetPos, cameraposition.CameraDistance,
                cameraposition.CameraRot);

        }

        void OnClick(GenericButton button)
        {
            if (cameraposition != null && _nav!= null)
                SetCameraPosition();
#if CINEMACHINE
        if (cinemachinecam != null)
        {
            _nav.ActivateCinemachineCam(cinemachinecam);
        }
#endif
        }
        
    }
}

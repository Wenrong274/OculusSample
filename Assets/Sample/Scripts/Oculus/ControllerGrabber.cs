using UnityEngine;

namespace hyhy.Oculus
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabberWeapon : MonoBehaviour
    {
        [SerializeField] private OVRInput.Controller m_controller;
        [SerializeField] private float grabBeginValue = 0.55f;
        [SerializeField] private float grabEndValue = 0.35f;
        [SerializeField] private float defaultMaxDistance = 12f;
        [SerializeField] private LayerMask rayLayerMask;
        [SerializeField] private LayerMask gripRayLayerMask;
        [SerializeField] private Transform m_gripTransform = null;
        [SerializeField] private Transform FingertGrab;
        [SerializeField] private Pointer OculusPointer;

        private float m_prevFlex;
        private GameInput gameInput;
        private IRaycastFeature m_raycastObj;
        private IGrabberFeature m_grabbedObject;
        private IPressFeature m_pressObject;

        public Transform GripTransform => FingertGrab;
        public IGrabberFeature GrabbedObject => m_grabbedObject;

        private void Start()
        {
            gameInput = new GameInput(this, m_controller);
        }

        private void Update()
        {
            Ray ray = new Ray(m_gripTransform.position, m_gripTransform.forward);
            RaycastHit hit;
            float RayDistance = 0.6f;
            LayerMask layerMask = GrabbedObject != null ? gripRayLayerMask : rayLayerMask;
            bool IsRay = false;
            if (m_grabbedObject == null && m_pressObject == null)
            {
                if (Physics.Raycast(ray, out hit, defaultMaxDistance, layerMask))
                {
                    var item = hit.collider.GetComponent<IRaycastFeature>();
                    if (item == null)
                    {
                        m_raycastObj?.OnRaycast(false, gameInput);
                        m_raycastObj = null;
                    }
                    else if (item != null && m_raycastObj != item)
                    {
                        bool IsGrabber = false;
                        if (item.Type == VRItemType.Grabber)
                        {
                            IsGrabber = item.Self.GetComponent<IGrabberFeature>().IsGrabber;
                        }

                        if (!IsGrabber || GrabbedObject != null)
                        {
                            m_raycastObj?.OnRaycast(false, gameInput);
                            m_raycastObj = item;
                            m_raycastObj.OnRaycast(true, gameInput);
                        }
                    }

                    if (m_raycastObj != null)
                    {
                        IsRay = true;
                    }
                    RayDistance = hit.distance;
                }
                else
                {
                    m_raycastObj?.OnRaycast(false, gameInput);
                    m_raycastObj = null;
                }
            }
            CheckRaycastObject();
            SetPointer(RayDistance, IsRay);
        }

        private void CheckRaycastObject()
        {
            if (m_raycastObj == null)
            {
                return;
            }

            var status = m_raycastObj.Type;

            switch (status)
            {
                case VRItemType.Trigger:
                    CheckTrigger();
                    break;
                case VRItemType.Grabber:
                    CheckGrabber();
                    break;
                case VRItemType.Press:
                    CheckPress();
                    break;
                default:
                    break;
            }
        }

        private void CheckTrigger()
        {
            float prevFlex = m_prevFlex;
            var Axis1D = OVRInput.Axis1D.PrimaryIndexTrigger;
            m_prevFlex = OVRInput.Get(Axis1D, m_controller);
            var Trigger = m_raycastObj.Self.GetComponent<ITriggerFeature>();
            m_grabbedObject = null;

            if ((m_prevFlex >= grabBeginValue) && (prevFlex < grabBeginValue))
            {
                Trigger.OnTrigger(true, gameInput);
            }
            else if ((m_prevFlex <= grabEndValue) && (prevFlex > grabEndValue))
            {
                Trigger?.OnTrigger(false, gameInput);
            }
        }

        private void CheckGrabber()
        {
            float prevFlex = m_prevFlex;
            var Axis1D = OVRInput.Axis1D.PrimaryIndexTrigger;
            m_prevFlex = OVRInput.Get(Axis1D, m_controller);
            if ((m_prevFlex >= grabBeginValue) && (prevFlex < grabBeginValue))
            {
                if (m_grabbedObject != m_raycastObj)
                {
                    m_grabbedObject?.OnTrigger(false, gameInput);
                    m_grabbedObject = m_raycastObj.Self.GetComponent<IGrabberFeature>();
                    m_grabbedObject?.OnTrigger(true, gameInput);
                }
            }
            else if ((m_prevFlex <= grabEndValue) && (prevFlex > grabEndValue))
            {
                m_grabbedObject?.OnTrigger(false, gameInput);
                m_grabbedObject = null;
            }
        }

        private void CheckPress()
        {
            if (m_pressObject == null && m_raycastObj != null)
            {
                IPressFeature target = m_raycastObj.Self.GetComponent<IPressFeature>();
                m_pressObject = target;
            }


            if (m_pressObject.PressType == PressType.AB)
            {
                CheckPressAB(m_pressObject);
            }
            else if (m_pressObject.PressType == PressType.PrimaryTrigger)
            {
                CheckPressPrimary(m_pressObject);
            }
        }

        private void CheckPressAB(IPressFeature target)
        {
            bool BOne = OVRInput.Get(OVRInput.Button.One, m_controller);
            bool BTwo = OVRInput.Get(OVRInput.Button.Two, m_controller);
            bool TouchOne = BOne || BTwo;

            target.OnTouchOne(gameInput, TouchOne, 1);
            if (TouchOne)
                m_pressObject = target;
            else
            {
                m_pressObject?.OnTouchOne(gameInput, false, 0);
                m_pressObject = null;
            }
        }

        private void CheckPressPrimary(IPressFeature target)
        {
            var Axis1D = OVRInput.Axis1D.PrimaryIndexTrigger;
            float Flex = OVRInput.Get(Axis1D, m_controller);
            target.OnTouchOne(gameInput, false, Flex);
            if (Flex > 0)
            {
                m_pressObject = target;
            }
            else
            {
                m_pressObject?.OnTouchOne(gameInput, false, 0);
                m_pressObject = null;
            }
        }

        private void SetPointer(float distance, bool IsRay)
        {
            if (OculusPointer == null)
                return;
            Color color = IsRay ? Color.cyan : Color.white;
            if (m_grabbedObject != null)
                OculusPointer.RayPointer(0, color);
            else
                OculusPointer.RayPointer(distance, color);
        }
    }
}

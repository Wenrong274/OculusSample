using UnityEngine;

namespace hyhy.Oculus
{
    public class OculusDrag : MonoBehaviour, IGrabberFeature
    {
        public virtual VRItemType Type => VRItemType.Grabber;
        public GameObject Self => gameObject;
        public bool IsGrabber => isGrabber;

        public EventBool OnHitEvent;

        public EventBool OffPressEvent;
        public EventFloat PressHandleEvent;

        [SerializeField] private Transform beltEnd;
        private bool isGrabber;
        [SerializeField] private Transform rootBelt;
        private float BeltValue = 0;

        Vector3 initPos;

        private void Start()
        {
            initPos = transform.position;
            OnHitEvent?.Invoke(false);
        }

        private void Update()
        {

        }

        public void OnRaycast(bool On, GameInput gameInput)
        {
            OnHitEvent?.Invoke(On);
        }

        public void OnTrigger(bool On, GameInput gameInput)
        {
            isGrabber = On;

            if (On)
            {
                transform.SetParent(gameInput.Grabber.GripTransform);
            }
            else
            {
                transform.SetParent(rootBelt);
                transform.localPosition = Vector3.zero;
                OffPressEvent.Invoke(BeltValue >= 0.95f);
            }
        }
    }
}

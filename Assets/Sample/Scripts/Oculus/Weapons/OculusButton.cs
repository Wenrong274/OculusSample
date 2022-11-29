using UnityEngine;
using UnityEngine.Events;

namespace hyhy.Oculus
{
    public class OculusButton : MonoBehaviour, ITriggerFeature
    {
        public virtual VRItemType Type => VRItemType.Trigger;
        public GameObject Self => gameObject;

        public EventBool OnHitEvent;
        public UnityEvent OnClickEvent;
        public UnityEvent OffClickEvent;

        private void Start()
        {
            OnHitEvent?.Invoke(false);
        }

        public virtual void OnRaycast(bool On, GameInput gameInput)
        {
            OnHitEvent?.Invoke(On);
        }

        public void OnTrigger(bool On, GameInput gameInput)
        {
            if (On)
                OnClickEvent?.Invoke();
            else
                OffClickEvent?.Invoke();
        }
    }
}

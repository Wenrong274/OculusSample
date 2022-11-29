using UnityEngine;
using UnityEngine.Events;

namespace hyhy.Oculus
{
    public class OculusPressButton : MonoBehaviour, IPressFeature
    {
        public UnityEvent OnClickEvent;

        [SerializeField] private float duration = 3f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color OnPressColor = new Color(1, 0, 0, 1);
        [SerializeField] private Color OffPressColor = new Color(1, 1, 1, 1);

        public VRItemType Type => VRItemType.Press;
        public GameObject Self => gameObject;
        public PressType PressType => PressType.AB;
        public EventBool OnHitEvent;

        private bool isPress;
        private float durTime;

        private void Start()
        {
            OnHitEvent?.Invoke(false);
        }

        public void OnRaycast(bool On, GameInput gameInput)
        {
            OnHitEvent?.Invoke(On);
        }

        public void OnTouchOne(GameInput gameInput, bool IsPress, float PressVale)
        {
            isPress = IsPress;

            if (IsPress && durTime <= 0)
            {
                durTime = duration;
            }
            else if (!IsPress)
            {
                durTime = 0;
                spriteRenderer.color = OffPressColor;
            }
        }


        private void Update()
        {
            if (isPress)
            {
                durTime -= Time.deltaTime;
                float v = 1 - durTime / duration;
                spriteRenderer.color = Color.Lerp(OffPressColor, OnPressColor, v);
                if (durTime <= 0)
                    OnClickEvent?.Invoke();
            }
        }
    }
}

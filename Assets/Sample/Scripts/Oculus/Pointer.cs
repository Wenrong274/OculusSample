using UnityEngine;

namespace hyhy.Oculus
{
    [RequireComponent(typeof(LineRenderer))]
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private float defaultLength = 5.0f;
        [SerializeField] private float startOffset = 0.05f;
        [SerializeField] private bool isRightPointer;
        [SerializeField] private GrabberWeapon grabber;

        public LineRenderer lineRenderer = null;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void RayPointer(float distance, Color color)
        {
            var grabberTra = grabber.GripTransform;
            Vector3 endPosition = grabberTra.position + (grabberTra.forward * distance);
            Vector3 startPosition = grabberTra.position + (grabberTra.forward * startOffset) +
                (grabberTra.up * -0.01f);
            if (isRightPointer)
            {
                startPosition += grabberTra.right * 0.01f;
            }
            else
            {
                startPosition -= grabberTra.right * 0.01f;
            }
            if (lineRenderer.gameObject.activeSelf)
            {
                lineRenderer.positionCount = 2;
                //lineRenderer.material.color = color;
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, endPosition);
            }
        }

        public void RayPointerBezierCurve(Vector3 point_end, Color color)
        {
            var grabberTra = grabber.GripTransform;
            lineRenderer.material.color = color;
            Vector3 startPosition = grabberTra.position + (grabberTra.forward * startOffset) +
                (grabberTra.up * -0.01f);
            if (isRightPointer)
            {
                startPosition += grabberTra.right * 0.01f;
            }
            else
            {
                startPosition -= grabberTra.right * 0.01f;
            }
            DrawQuadraticBezierCurve(startPosition, point_end);
        }

        void DrawQuadraticBezierCurve(Vector3 point_start, Vector3 point_end)
        {
            Vector3 point_mid = (point_start - point_end) * 0.5f + point_end;
            point_mid.y += 1.0f;
            lineRenderer.positionCount = 100;
            float t = 0f;
            Vector3 B;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                B = (1 - t) * (1 - t) * point_start + 2 * (1 - t) * t * point_mid + t * t * point_end;
                lineRenderer.SetPosition(i, B);
                t += (1 / (float)lineRenderer.positionCount);
            }
        }
    }
}

using UnityEngine;

namespace hyhy.Oculus
{
    public enum VRItemType
    {
        NONE = -1,
        Trigger,
        Grabber,
        Press,
    }

    public enum PressType
    {
        AB,
        PrimaryTrigger,
    }

    public interface IRaycastFeature
    {
        VRItemType Type { get; }
        GameObject Self { get; }
        void OnRaycast(bool On, GameInput gameInput);
    }

    public interface ITriggerFeature : IRaycastFeature
    {
        void OnTrigger(bool On, GameInput gameInput);
    }

    public interface IPressFeature : IRaycastFeature
    {
        PressType PressType { get; }
        void OnTouchOne(GameInput gameInput, bool IsPress, float PressValue);
    }

    public interface IGrabberFeature : IRaycastFeature
    {
        bool IsGrabber { get; }
        void OnTrigger(bool On, GameInput gameInput);
    }

    public class GameInput
    {
        public GrabberWeapon Grabber;
        public OVRInput.Controller controller;

        public GameInput(GrabberWeapon grabber, OVRInput.Controller controller)
        {
            Grabber = grabber;
            this.controller = controller;
        }
    }

    [System.Serializable]
    public class EventFloat : UnityEngine.Events.UnityEvent<float> { }
    [System.Serializable]
    public class EventBool : UnityEngine.Events.UnityEvent<bool> { }
}

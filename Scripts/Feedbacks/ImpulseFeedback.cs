using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.Feedbacks
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class ImpulseFeedback : Feedback
    {
        private CinemachineImpulseSource impulseSource;
        private void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
        public override void CreateFeedback()
        {
            impulseSource.GenerateImpulse();
        }
    }
}

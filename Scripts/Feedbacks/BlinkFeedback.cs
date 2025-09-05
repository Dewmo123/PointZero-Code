using UnityEngine;

namespace Scripts.Feedbacks
{
    public class BlinkFeedback : Feedback
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float blinkDuration = 0.15f;
        [SerializeField] private float blinkIntensity = 0.2f;

        private readonly int _blinkHash = Shader.PropertyToID("_BlinkValue");
        public override async void CreateFeedback()
        {
            meshRenderer.material.SetFloat(_blinkHash, blinkIntensity);
            await Awaitable.WaitForSecondsAsync(blinkDuration);
            FinishFeedback();
        }

        public override void FinishFeedback()
        {
            if (meshRenderer != null)
                meshRenderer.material.SetFloat(_blinkHash, 0f);
        }
    }
}

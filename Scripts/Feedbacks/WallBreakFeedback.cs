using Scripts.Feedbacks;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Feedbacks
{
    public class WallBreakFeedback : Feedback
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private float duration;
        public override async void CreateFeedback()
        {
            transform.parent = null;
            effect.Play();
            await Awaitable.WaitForSecondsAsync(duration);
            effect.Stop();
            Destroy(this);
        }
    }
}

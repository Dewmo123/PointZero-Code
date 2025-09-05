using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Scripts.Feedbacks
{
    public class BloodScreenFeedback : Feedback
    {
        [SerializeField] private VolumeProfile volume;
        [SerializeField] private float destinationVal;
        [SerializeField] private float destinationTime;
        private Vignette vignette;


        private void Awake()
        {
            volume.TryGet(out vignette);
        }
        public override void CreateFeedback()
        {
            StartCoroutine(InterpVignette());
        }

        private IEnumerator InterpVignette()
        {
            float currentTime = 0;
            while (true)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= destinationTime/2)
                {
                    currentTime = 0;
                    break;
                }
                vignette.intensity.Interp(vignette.intensity.value, destinationVal, currentTime / destinationTime / 2);
                yield return null;
            }
            while (true)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= destinationTime / 2)
                {
                    currentTime = 0;
                    break;
                }
                vignette.intensity.Interp(vignette.intensity.value, 0, currentTime / destinationTime / 2);
                yield return null;
            }
        }
    }
}

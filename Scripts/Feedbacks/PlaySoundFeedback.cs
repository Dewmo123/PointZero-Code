using DewmoLib.ObjectPool.RunTime;
using Scripts.Core.Sound;
using UnityEngine;

namespace Scripts.Feedbacks
{
    public class PlaySoundFeedback : Feedback
    {

        [SerializeField] private SoundSO sound;
        [SerializeField] private PoolItemSO spItem;
        [SerializeField] private Transform playTrm;
        [SerializeField] private bool isFollow;
        [SerializeField] private PoolManagerSO poolManager;
        public override void CreateFeedback()
        {
            var sp = poolManager.Pop(spItem) as SoundPlayer;
            if (isFollow)
                sp.PlaySound(sound, playTrm);
            else
                sp.PlaySound(sound, playTrm.position);
        }
    }
}

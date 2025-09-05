using EPOOutline;
using Scripts.Entities;
using UnityEngine;

namespace Scripts.GameSystem.CheckPlayer
{
    public class BombArea : Area, IFindable
    {
        [SerializeField] private Outlinable outline;

        public int sightCount { get; set; }=0;

        public void Escape()
        {
            outline.enabled = false;
        }

        public void Founded()
        {
            outline.enabled = true;
        }
    }
}

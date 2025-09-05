using Scripts.Entities;

namespace Assets.Scripts.Combat
{
    public interface IHittable
    {
        int index { get; }
        int Health { get; set; }
        void SetDead(NetworkEntity attacker);
        void Hit();
    }
}

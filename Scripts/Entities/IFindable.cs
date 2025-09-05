namespace Scripts.Entities
{
    public interface IFindable
    {
        int sightCount { get; set; }
        void Founded();
        void Escape();
    }
}

namespace SeedGrowthModel.Models
{
    public class Seed
    {
        public string Id { get; }
        public bool IsWatered { get; private set; }
        public bool IsSprouted { get; private set; }

        public Seed(string id)
        {
            Id = id;
        }

        public virtual void Water()
        {
            IsWatered = true;
        }

        public virtual void TrySprout(double temperature)
        {
            if (IsWatered && temperature > 15)
            {
                IsSprouted = true;
            }
        }
    }
}
namespace SeedGrowthModelss.Models
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

        public void Water()
        {
            IsWatered = true;
        }

        public void TrySprout(double temperature)
        {
            if (IsWatered && temperature > 15)
            {
                IsSprouted = true;
            }
        }
    }
}
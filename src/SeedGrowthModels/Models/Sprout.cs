namespace SeedGrowthModel.Models
{
    public class Sprout
    {
        public double Height { get; private set; }
        public bool HasLeaves { get; private set; }

        public void Grow(double growthAmount)
        {
            Height += growthAmount;

            if (Height > 5)
            {
                HasLeaves = true;
            }
        }
    }
}
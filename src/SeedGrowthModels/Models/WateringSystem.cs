namespace SeedGrowthModel.Models
{
    public class WateringSystem
    {
        public double WaterLevel { get; private set; }

        public void Fill(double amount)
        {
            WaterLevel += amount;
        }

        public void Water(Seed seed, Soil soil)
        {
            if (WaterLevel < 0.3)
                return;

            seed.Water();
            soil.AbsorbWater(0.3);
            WaterLevel -= 0.3;
        }
    }
}
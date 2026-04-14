namespace SeedGrowthModel.Models
{
    public class Gardener
    {
        public int WateringActionsCount { get; private set; }

        public void WaterSeed(Seed seed, Soil soil)
        {
            seed.Water();
            soil.AbsorbWater(0.3);
            WateringActionsCount++;
        }
    }
}
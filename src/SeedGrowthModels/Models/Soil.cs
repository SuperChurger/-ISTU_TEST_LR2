namespace SeedGrowthModel.Models
{
    public class Soil
    {
        public double Moisture { get; private set; }

        public virtual void AbsorbWater(double amount)
        {
            Moisture += amount;
        }

        public virtual bool IsMoistEnough()
        {
            return Moisture > 0.5;
        }
    }
}
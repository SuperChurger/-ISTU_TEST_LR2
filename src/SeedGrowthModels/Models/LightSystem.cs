namespace SeedGrowthModel.Models
{
    public class LightSystem
    {
        public double LightLevel { get; private set; }
        public bool IsOn { get; private set; }

        public void TurnOn(double level)
        {
            IsOn = true;
            LightLevel = level;
        }

        public void TurnOff()
        {
            IsOn = false;
            LightLevel = 0;
        }

        public bool IsEnoughLight()
        {
            return IsOn && LightLevel > 5;
        }
    }
}
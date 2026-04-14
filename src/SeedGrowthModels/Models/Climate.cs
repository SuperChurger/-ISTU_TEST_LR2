using SeedGrowthModel.Abstractions;

namespace SeedGrowthModel.Models
{
    public class Climate : IClimate
    {
        public double CurrentTemperature { get; private set; }
        public double CurrentLightLevel { get; private set; }

        public Climate(double initialTemperature = 20.0, double initialLightLevel = 10.0)
        {
            CurrentTemperature = initialTemperature;
            CurrentLightLevel = initialLightLevel;
        }

        public double GetTemperature() => CurrentTemperature;
        public double GetLightLevel() => CurrentLightLevel;

        public void SetTemperature(double value) => CurrentTemperature = value;
        public void SetLightLevel(double value) => CurrentLightLevel = value;
    }
}
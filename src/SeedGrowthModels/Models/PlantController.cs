using System.Collections.Generic;
using System.Linq;
using SeedGrowthModel.Abstractions;

namespace SeedGrowthModel.Models
{
    public class PlantController
    {
        private readonly IClimate _climate;
        private readonly List<Seed> _processedSeeds = new();
        private readonly List<Sprout> _sprouts = new();

        public PlantController(IClimate climate)
        {
            _climate = climate;
        }

        public void ProcessGrowth(Seed seed, LightSystem light)
        {
            var temperature = _climate.GetTemperature();
            if (light.IsEnoughLight())
            {
                seed.TrySprout(temperature);
            }
            _processedSeeds.Add(seed);
        }

        public void GrowSprout(Sprout sprout, double growthAmount)
        {
            sprout.Grow(growthAmount);
            _sprouts.Add(sprout);
        }

        // Накопление: получить все обработанные семена
        public IReadOnlyList<Seed> GetAllProcessedSeeds() => _processedSeeds.AsReadOnly();

        // Выборка по условию: семена, которые проросли
        public IEnumerable<Seed> GetSproutedSeeds() => _processedSeeds.Where(s => s.IsSprouted);

        // Выборка по условию: ростки с листьями
        public IEnumerable<Sprout> GetSproutsWithLeaves() => _sprouts.Where(s => s.HasLeaves);
    }
}
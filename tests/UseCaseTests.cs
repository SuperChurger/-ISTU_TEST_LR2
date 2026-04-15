using Xunit;
using Moq;
using SeedGrowthModel.Models;
using SeedGrowthModel.Abstractions;
using System.Linq;

namespace SeedGrowthModel.Tests
{
    public class UseCaseTests
    {
        // Use Case 1: Контроллер накапливает все обработанные сценарии (полив + свет + температура)
        [Fact]
        public void UseCase_ControllerAccumulatesProcessedSeeds()
        {
            var mockClimate = new Mock<IClimate>();
            mockClimate.Setup(c => c.GetTemperature()).Returns(20);
            var controller = new PlantController(mockClimate.Object);
            var light = new LightSystem();
            light.TurnOn(10);

            var seedA = new Seed("A");
            seedA.Water();
            var seedB = new Seed("B");
            seedB.Water();

            controller.ProcessGrowth(seedA, light);
            controller.ProcessGrowth(seedB, light);

            var allSeeds = controller.GetAllProcessedSeeds();
            Assert.Equal(2, allSeeds.Count);
            Assert.Contains(seedA, allSeeds);
            Assert.Contains(seedB, allSeeds);
        }

        // Use Case 2: Выборка проросших семян (условие IsSprouted == true)
        [Fact]
        public void UseCase_FilterSproutedSeeds()
        {
            var mockClimate = new Mock<IClimate>();
            mockClimate.Setup(c => c.GetTemperature()).Returns(25);
            var controller = new PlantController(mockClimate.Object);
            var light = new LightSystem();
            light.TurnOn(10);

            var seedOk = new Seed("ok");
            seedOk.Water();
            var seedNoWater = new Seed("noWater");
            // seedNoWater не поливаем
            var seedCold = new Seed("cold");
            seedCold.Water();

            // изменим температуру для cold - не прорастёт
            mockClimate.Setup(c => c.GetTemperature()).Returns(10);
            controller.ProcessGrowth(seedCold, light);
            mockClimate.Setup(c => c.GetTemperature()).Returns(25);
            controller.ProcessGrowth(seedOk, light);
            controller.ProcessGrowth(seedNoWater, light);

            var sprouted = controller.GetSproutedSeeds().ToList();
            Assert.Single(sprouted);
            Assert.Equal("ok", sprouted[0].Id);
        }

        // Use Case 3: Выборка ростков, у которых уже есть листья
        [Fact]
        public void UseCase_FilterSproutsWithLeaves()
        {
            var controller = new PlantController(Mock.Of<IClimate>());
            var smallSprout = new Sprout();
            var bigSprout = new Sprout();

            controller.GrowSprout(smallSprout, 3); // без листьев
            controller.GrowSprout(bigSprout, 8);   // с листьями

            var withLeaves = controller.GetSproutsWithLeaves().ToList();
            Assert.Single(withLeaves);
            Assert.Equal(bigSprout, withLeaves[0]);
        }

        // Use Case 4: Контроллер сохраняет порядок обработанных семян (накопление как журнал событий)
        [Fact]
        public void UseCase_ControllerPreservesProcessingOrder()
        {
            var climate = new Mock<IClimate>();
            climate.Setup(c => c.GetTemperature()).Returns(25);
            var controller = new PlantController(climate.Object);

            var light = new LightSystem();
            light.TurnOn(10);

            var a = new Seed("A"); a.Water();
            var b = new Seed("B"); b.Water();
            var c = new Seed("C"); c.Water();

            controller.ProcessGrowth(a, light);
            controller.ProcessGrowth(b, light);
            controller.ProcessGrowth(c, light);

            var all = controller.GetAllProcessedSeeds();
            Assert.Equal(new[] { "A", "B", "C" }, all.Select(s => s.Id).ToArray());
        }

        // Use Case 5: Обработка при недостатке света (накопление без прорастания)
        [Fact]
        public void UseCase_ProcessGrowth_WhenLightNotEnough_AccumulatesButDoesNotSprout()
        {
            var climate = new Mock<IClimate>();
            climate.Setup(c => c.GetTemperature()).Returns(30);
            var controller = new PlantController(climate.Object);

            var light = new LightSystem(); // off => not enough light
            var seed = new Seed("s1");
            seed.Water();

            controller.ProcessGrowth(seed, light);

            Assert.Single(controller.GetAllProcessedSeeds());
            Assert.Contains(seed, controller.GetAllProcessedSeeds());
            Assert.False(seed.IsSprouted);
        }

        // Use Case 6: Выборка проросших семян после разных температур (выборка по условию)
        [Fact]
        public void UseCase_FilterSproutedSeeds_AfterDifferentTemperatureRuns()
        {
            var climate = new Mock<IClimate>();
            var controller = new PlantController(climate.Object);

            var light = new LightSystem();
            light.TurnOn(10);

            var warm = new Seed("warm"); warm.Water();
            var cold = new Seed("cold"); cold.Water();

            climate.Setup(c => c.GetTemperature()).Returns(25);
            controller.ProcessGrowth(warm, light);

            climate.Setup(c => c.GetTemperature()).Returns(10);
            controller.ProcessGrowth(cold, light);

            var sprouted = controller.GetSproutedSeeds().Select(s => s.Id).ToList();
            Assert.Single(sprouted);
            Assert.Contains("warm", sprouted);
        }
    }
}
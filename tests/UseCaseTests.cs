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
    }
}
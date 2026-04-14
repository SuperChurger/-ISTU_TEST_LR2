using Xunit;
using Moq;
using System.Linq;
using SeedGrowthModel.Models;
using SeedGrowthModel.Abstractions;

namespace SeedGrowthModel.Tests
{
    public class ScenarioTests
    {
        // Сценарий 1: Садовник вручную поливает семя и землю
        [Fact]
        public void Scenario_GardenerWatersSeedAndSoil()
        {
            var seed = new Seed("s1");
            var soil = new Soil();
            var gardener = new Gardener();

            gardener.WaterSeed(seed, soil);

            Assert.True(seed.IsWatered);
            Assert.True(soil.Moisture > 0);
            Assert.Equal(1, gardener.WateringActionsCount);
        }

        // Сценарий 2: Автоматическая система полива (WateringSystem) заполняется и поливает
        [Fact]
        public void Scenario_WateringSystemFillsAndWaters()
        {
            var seed = new Seed("s1");
            var soil = new Soil();
            var ws = new WateringSystem();
            ws.Fill(1.0);

            ws.Water(seed, soil);

            Assert.True(seed.IsWatered);
            Assert.True(soil.Moisture > 0);
            Assert.Equal(0.7, ws.WaterLevel, 2); // было 1.0, потрачено 0.3
        }

        // Сценарий 3: Проращивание семени при хорошем свете и температуре (через контроллер)
        [Fact]
        public void Scenario_SeedSproutsViaControllerWithGoodConditions()
        {
            var mockClimate = new Mock<IClimate>();
            mockClimate.Setup(c => c.GetTemperature()).Returns(22);
            var controller = new PlantController(mockClimate.Object);
            var seed = new Seed("s1");
            seed.Water();
            var light = new LightSystem();
            light.TurnOn(10);

            controller.ProcessGrowth(seed, light);

            Assert.True(seed.IsSprouted);
            Assert.Contains(seed, controller.GetAllProcessedSeeds());
        }

        // Сценарий 4: Росток растёт и появляются листья
        [Fact]
        public void Scenario_SproutGrowsAndGetsLeaves()
        {
            var sprout = new Sprout();
            var controller = new PlantController(Mock.Of<IClimate>());

            controller.GrowSprout(sprout, 6); // высота >5

            Assert.Equal(6, sprout.Height);
            Assert.True(sprout.HasLeaves);
            Assert.Contains(sprout, controller.GetSproutsWithLeaves());
        }

        // Сценарий 5: Контроллер обрабатывает несколько семян и позволяет выбрать проросшие
        [Fact]
        public void Scenario_ControllerAccumulatesSeedsAndFiltersSprouted()
        {
            var mockClimate = new Mock<IClimate>();
            mockClimate.Setup(c => c.GetTemperature()).Returns(22);
            var controller = new PlantController(mockClimate.Object);
            var light = new LightSystem();
            light.TurnOn(10);

            var seed1 = new Seed("s1");
            seed1.Water();
            var seed2 = new Seed("s2");
            seed2.Water();
            var seed3 = new Seed("s3"); // не полит

            controller.ProcessGrowth(seed1, light); // хорошие условия -> sprout
            controller.ProcessGrowth(seed2, light); // sprout
            controller.ProcessGrowth(seed3, light); // не полито -> не sprout

            var sprouted = controller.GetSproutedSeeds().ToList();
            Assert.Equal(2, sprouted.Count);
            Assert.Contains(seed1, sprouted);
            Assert.Contains(seed2, sprouted);
            Assert.DoesNotContain(seed3, sprouted);
        }
    }
}
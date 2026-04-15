using Xunit;
using Moq;
using SeedGrowthModel.Models;
using SeedGrowthModel.Abstractions;

namespace SeedGrowthModel.Tests
{
    public class MethodTests
    {
        // ----- Seed -----
        [Fact]
        public void Seed_Water_SetsIsWatered()
        {
            var seed = new Seed("s1");
            seed.Water();
            Assert.True(seed.IsWatered);
        }

        [Fact]
        public void Seed_TrySprout_WhenWateredAndWarm_Sprouts()
        {
            var seed = new Seed("s1");
            seed.Water();
            seed.TrySprout(20);
            Assert.True(seed.IsSprouted);
        }

        [Fact]
        public void Seed_TrySprout_WhenNotWatered_NoSprout()
        {
            var seed = new Seed("s1");
            seed.TrySprout(20);
            Assert.False(seed.IsSprouted);
        }

        [Fact]
        public void Seed_TrySprout_TemperatureExactly15_DoesNotSprout()
        {
            var seed = new Seed("s1");
            seed.Water();

            seed.TrySprout(15);

            Assert.False(seed.IsSprouted);
        }

        [Fact]
        public void Seed_TrySprout_TemperatureJustAbove15_Sprouts()
        {
            var seed = new Seed("s1");
            seed.Water();

            seed.TrySprout(15.0001);

            Assert.True(seed.IsSprouted);
        }

        // ----- Soil -----
        [Fact]
        public void Soil_AbsorbWater_IncreasesMoisture()
        {
            var soil = new Soil();
            soil.AbsorbWater(0.7);
            Assert.Equal(0.7, soil.Moisture);
        }

        [Fact]
        public void Soil_IsMoistEnough_ReturnsTrueWhenMoistureAboveThreshold()
        {
            var soil = new Soil();
            soil.AbsorbWater(0.6);
            Assert.True(soil.IsMoistEnough());
        }

        [Fact]
        public void Soil_IsMoistEnough_WhenExactlyPointFive_ReturnsFalse()
        {
            var soil = new Soil();
            soil.AbsorbWater(0.5);

            Assert.False(soil.IsMoistEnough());
        }

        [Fact]
        public void Soil_IsMoistEnough_WhenJustAbovePointFive_ReturnsTrue()
        {
            var soil = new Soil();
            soil.AbsorbWater(0.50001);

            Assert.True(soil.IsMoistEnough());
        }

        // ----- Sprout -----
        [Fact]
        public void Sprout_Grow_IncreasesHeight()
        {
            var sprout = new Sprout();
            sprout.Grow(4);
            Assert.Equal(4, sprout.Height);
        }

        // ----- WateringSystem (с моками зависимостей) -----
        [Fact]
        public void WateringSystem_Water_WhenEnoughWater_CallsSeedAndSoilMethods()
        {
            var mockSeed = new Mock<Seed>("s1");
            var mockSoil = new Mock<Soil>();
            var ws = new WateringSystem();
            ws.Fill(1.0);

            ws.Water(mockSeed.Object, mockSoil.Object);

            mockSeed.Verify(s => s.Water(), Times.Once);
            mockSoil.Verify(s => s.AbsorbWater(It.IsAny<double>()), Times.Once);
        }

        [Fact]
        public void WateringSystem_Water_WhenNotEnoughWater_DoesNothing()
        {
            var mockSeed = new Mock<Seed>("s1");
            var mockSoil = new Mock<Soil>();
            var ws = new WateringSystem();

            ws.Water(mockSeed.Object, mockSoil.Object);

            mockSeed.Verify(s => s.Water(), Times.Never);
            mockSoil.Verify(s => s.AbsorbWater(It.IsAny<double>()), Times.Never);
        }

        [Fact]
        public void WateringSystem_Water_WhenWaterLevelExactlyPoint3_WatersAndBecomesZero()
        {
            var ws = new WateringSystem();
            ws.Fill(0.3);
            var seed = new Seed("s1");
            var soil = new Soil();

            ws.Water(seed, soil);

            Assert.True(seed.IsWatered);
            Assert.Equal(0.3, soil.Moisture, 10);
            Assert.Equal(0, ws.WaterLevel, 10);
        }

        // ----- Gardener (с моками) -----
        [Fact]
        public void Gardener_WaterSeed_CallsSeedAndSoilMethods()
        {
            var mockSeed = new Mock<Seed>("s1");
            var mockSoil = new Mock<Soil>();
            var gardener = new Gardener();

            gardener.WaterSeed(mockSeed.Object, mockSoil.Object);

            mockSeed.Verify(s => s.Water(), Times.Once);
            mockSoil.Verify(s => s.AbsorbWater(0.3), Times.Once);
            Assert.Equal(1, gardener.WateringActionsCount);
        }

        // ----- LightSystem -----
        [Fact]
        public void LightSystem_TurnOn_SetsIsOnAndLevel()
        {
            var light = new LightSystem();
            light.TurnOn(8);
            Assert.True(light.IsOn);
            Assert.Equal(8, light.LightLevel);
        }

        [Fact]
        public void LightSystem_TurnOff_SetsIsOffAndLevelZero()
        {
            var light = new LightSystem();
            light.TurnOn(8);
            light.TurnOff();
            Assert.False(light.IsOn);
            Assert.Equal(0, light.LightLevel);
        }

        [Fact]
        public void LightSystem_IsEnoughLight_LevelExactly5_ReturnsFalse()
        {
            var light = new LightSystem();
            light.TurnOn(5);

            Assert.False(light.IsEnoughLight());
        }

        [Fact]
        public void LightSystem_IsEnoughLight_LevelJustAbove5_ReturnsTrue()
        {
            var light = new LightSystem();
            light.TurnOn(5.0001);

            Assert.True(light.IsEnoughLight());
        }

        // ----- Climate -----
        [Fact]
        public void Climate_SetTemperature_ChangesValue()
        {
            var climate = new Climate(10);
            climate.SetTemperature(25);
            Assert.Equal(25, climate.GetTemperature());
        }

        // ----- PlantController (с моком IClimate) -----
        [Fact]
        public void PlantController_ProcessGrowth_UsesClimate()
        {
            var mockClimate = new Mock<IClimate>();
            mockClimate.Setup(c => c.GetTemperature()).Returns(22);
            var controller = new PlantController(mockClimate.Object);
            var seed = new Seed("s1");
            seed.Water();
            var light = new LightSystem();
            light.TurnOn(10);

            controller.ProcessGrowth(seed, light);

            mockClimate.Verify(c => c.GetTemperature(), Times.Once);
        }

        [Fact]
        public void PlantController_ProcessGrowth_CallsClimateEveryTime()
        {
            var climate = new Mock<IClimate>();
            climate.Setup(c => c.GetTemperature()).Returns(20);
            var controller = new PlantController(climate.Object);

            var light = new LightSystem();
            light.TurnOn(10);

            var seed1 = new Seed("s1");
            var seed2 = new Seed("s2");
            seed1.Water();
            seed2.Water();

            controller.ProcessGrowth(seed1, light);
            controller.ProcessGrowth(seed2, light);

            climate.Verify(c => c.GetTemperature(), Times.Exactly(2));
        }
    }
}
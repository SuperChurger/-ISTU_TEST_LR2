using Xunit;
using SeedGrowthModel.Models;

namespace SeedGrowthModel.Tests
{
    public class PropertyTests
    {
        [Fact]
        public void Seed_InitialProperties_AreDefault()
        {
            var seed = new Seed("s1");
            Assert.Equal("s1", seed.Id);
            Assert.False(seed.IsWatered);
            Assert.False(seed.IsSprouted);
        }

        [Fact]
        public void Seed_AfterWater_IsWateredTrue()
        {
            var seed = new Seed("s1");
            seed.Water();
            Assert.True(seed.IsWatered);
        }

        [Fact]
        public void Soil_InitialMoisture_Zero()
        {
            var soil = new Soil();
            Assert.Equal(0, soil.Moisture);
        }

        [Fact]
        public void Soil_AfterAbsorbWater_MoistureIncreases()
        {
            var soil = new Soil();
            soil.AbsorbWater(1.2);
            Assert.Equal(1.2, soil.Moisture);
        }

        [Fact]
        public void Sprout_InitialProperties_ZeroHeightNoLeaves()
        {
            var sprout = new Sprout();
            Assert.Equal(0, sprout.Height);
            Assert.False(sprout.HasLeaves);
        }

        [Fact]
        public void WateringSystem_InitialWaterLevel_Zero()
        {
            var ws = new WateringSystem();
            Assert.Equal(0, ws.WaterLevel);
        }

        [Fact]
        public void Gardener_InitialActionsCount_Zero()
        {
            var gardener = new Gardener();
            Assert.Equal(0, gardener.WateringActionsCount);
        }

        [Fact]
        public void LightSystem_InitialState_OffAndZeroLevel()
        {
            var light = new LightSystem();
            Assert.False(light.IsOn);
            Assert.Equal(0, light.LightLevel);
        }

        [Fact]
        public void Climate_InitialTemperature_SetByConstructor()
        {
            var climate = new Climate(25.5);
            Assert.Equal(25.5, climate.GetTemperature());
        }
    }
}
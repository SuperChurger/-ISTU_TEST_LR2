using Xunit;
using SeedGrowthModel.Models;

namespace SeedGrowthModel.Tests
{
    public class StateTests
    {
        // Последовательность 1: Dry → Watered → Sprouted (успешное прорастание)
        [Fact]
        public void SeedState_DryToWateredToSprouted()
        {
            var seed = new Seed("s1");
            Assert.False(seed.IsWatered);   // Dry
            seed.Water();
            Assert.True(seed.IsWatered);    // Watered
            Assert.False(seed.IsSprouted);
            seed.TrySprout(20);
            Assert.True(seed.IsSprouted);   // Sprouted
        }

        // Последовательность 2: Dry → Watered → (попытка с низкой температурой) → остаётся Watered
        [Fact]
        public void SeedState_DryToWatered_StaysWateredWhenCold()
        {
            var seed = new Seed("s1");
            seed.Water();
            seed.TrySprout(10);
            Assert.True(seed.IsWatered);
            Assert.False(seed.IsSprouted);
        }

        // Последовательность 3: Dry → (попытка прорасти без полива) → остаётся Dry
        [Fact]
        public void SeedState_Dry_NoSproutWithoutWater()
        {
            var seed = new Seed("s1");
            seed.TrySprout(20);
            Assert.False(seed.IsWatered);
            Assert.False(seed.IsSprouted);
        }

        // Последовательность 4: Watered → Sprouted → повторный вызов TrySprout не меняет состояния
        [Fact]
        public void SeedState_FromSprouted_RemainsSprouted()
        {
            var seed = new Seed("s1");
            seed.Water();
            seed.TrySprout(20);
            Assert.True(seed.IsSprouted);
            seed.TrySprout(5); // даже при плохих условиях
            Assert.True(seed.IsSprouted);
        }

        // Последовательность 5: Dry → Watered → Sprouted (проверка, что после прорастания Watered остаётся true)
        [Fact]
        public void SeedState_SproutedRetainsWateredFlag()
        {
            var seed = new Seed("s1");
            seed.Water();
            seed.TrySprout(20);
            Assert.True(seed.IsWatered);
            Assert.True(seed.IsSprouted);
        }

        // Последовательность 6: WateringSystem Empty → Filled → Watered (уровень воды уменьшается)
        [Fact]
        public void WateringSystemState_EmptyToFilledToWatered_DecreasesWaterLevel()
        {
            var ws = new WateringSystem();
            Assert.Equal(0, ws.WaterLevel);

            ws.Fill(1.0);
            Assert.True(ws.WaterLevel > 0);

            var seed = new Seed("s1");
            var soil = new Soil();
            ws.Water(seed, soil);

            Assert.True(seed.IsWatered);
            Assert.True(soil.Moisture > 0);
            Assert.Equal(0.7, ws.WaterLevel, 10);
        }

        // Последовательность 7: WateringSystem Filled → Watered → Watered → Insufficient (останавливается)
        [Fact]
        public void WateringSystemState_FilledToInsufficientWater_StopsWatering()
        {
            var ws = new WateringSystem();
            ws.Fill(0.6);

            var seed1 = new Seed("s1");
            var seed2 = new Seed("s2");
            var seed3 = new Seed("s3");
            var soil = new Soil();

            ws.Water(seed1, soil); // 0.6 -> 0.3
            ws.Water(seed2, soil); // 0.3 -> 0.0
            ws.Water(seed3, soil); // 0.0 stays 0.0, no watering

            Assert.True(seed1.IsWatered);
            Assert.True(seed2.IsWatered);
            Assert.False(seed3.IsWatered);
            Assert.Equal(0, ws.WaterLevel, 10);
        }

        // Последовательность 8: LightSystem Off → On(Enough) → Off
        [Fact]
        public void LightSystemState_OffToOnEnough_ToOff()
        {
            var light = new LightSystem();
            Assert.False(light.IsOn);
            Assert.False(light.IsEnoughLight());

            light.TurnOn(10);
            Assert.True(light.IsOn);
            Assert.True(light.IsEnoughLight());

            light.TurnOff();
            Assert.False(light.IsOn);
            Assert.False(light.IsEnoughLight());
            Assert.Equal(0, light.LightLevel);
        }

        // Последовательность 9: Sprout NoLeaves → Leaves (пересечение порога)
        [Fact]
        public void SproutState_NoLeavesToLeaves_WhenCrossingThreshold()
        {
            var sprout = new Sprout();
            Assert.False(sprout.HasLeaves);

            sprout.Grow(5);
            Assert.False(sprout.HasLeaves);

            sprout.Grow(0.01);
            Assert.True(sprout.HasLeaves);
        }

        // Последовательность 10: Soil Dry → MoistEnough после нескольких впитываний
        [Fact]
        public void SoilState_DryToMoistEnough_AfterMultipleAbsorbs()
        {
            var soil = new Soil();
            Assert.False(soil.IsMoistEnough());

            soil.AbsorbWater(0.2);
            Assert.False(soil.IsMoistEnough());

            soil.AbsorbWater(0.4);
            Assert.True(soil.IsMoistEnough());
        }
    }
}
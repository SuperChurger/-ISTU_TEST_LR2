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
    }
}
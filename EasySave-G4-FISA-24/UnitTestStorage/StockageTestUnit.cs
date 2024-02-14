using Stockage.Load;
using Stockage.Save;

namespace UnitTestStorage
{
    public class StockageTestUnit
    {
        [Fact]
        public void TestSerialisation()
        {
            string lPath = Environment.CurrentDirectory;
            ISauve lSauveCollection = new SauveCollection(lPath);
            List<int> lData = new List<int>() { 1, 2, 3, 3, 3, 4, 6 };
            lSauveCollection.Sauver(lData, "int");

            ICharge chargerCollection = new ChargerCollection(null);
            string lPath2 = Path.Combine(lPath, "int.json");
            List<int> lLoadedData = chargerCollection.Charger<List<int>>(lPath2, true);

            Assert.Equal(lLoadedData, lData);
            Assert.Equal(lLoadedData[0], lData[0]);
            Assert.Equal(lLoadedData.Last(), lData.Last());
        }
    }
}
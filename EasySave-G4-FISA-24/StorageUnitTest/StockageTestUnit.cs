using Stockage;

namespace StorageUnitTest
{
    public class StockageTestUnit
    {
        [Fact]
        public void TestSerialisation()
        {
            string path = Environment.CurrentDirectory;
            SauveCollection sauveCollection = new SauveCollection(path);
            List<int> col = new List<int>() { 1, 2, 3, 3, 3, 4, 6 };
            sauveCollection.Sauver(col, "int");

            ChargerCollection chargerCollection = new ChargerCollection();
            List<int> s = chargerCollection.Charger<List<int>>(Path.Combine(path,"int"));

            Assert.Equal(s, col);
            Assert.Equal(s[0], col[0]);
            Assert.Equal(s.Last(), col.Last());
        }
    }
}
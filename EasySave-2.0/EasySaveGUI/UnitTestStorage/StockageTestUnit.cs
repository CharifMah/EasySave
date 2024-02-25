using Stockage.Load;
using Stockage.Save;

namespace UnitTestStorage
{
    public class StockageTestUnit
    {
        [Fact]
        public void TestSerialization()
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

        [Fact]
        public void TestExtensionSerialization()
        {
            string lPath = Environment.CurrentDirectory;
            ISauve lSauveCollection = new SauveCollection(lPath);
            List<int> lData = new List<int> { 1, 2, 3, 3, 3, 4, 6 };

            // Chemins possibles pour les fichiers sauvegard�s
            string lPathJson = Path.Combine(lPath, "int.json");
            string lPathXml = Path.Combine(lPath, "int.xml");

            // Initialise de la variable pour stocker le chemin du fichier charg�
            string? loadedFilePath = null;

            // Sauvegarde les donn�es, Sauver d�termine le format
            lSauveCollection.Sauver(lData, "int", false, "json");
            Assert.True(File.Exists(lPathJson));

            // V�rifie l'existence et le format du fichier
            if (File.Exists(lPathJson))
            {
                loadedFilePath = lPathJson;
                Assert.True(loadedFilePath.EndsWith(".json"), "Le fichier doit �tre au format JSON.");
                Assert.False(loadedFilePath.EndsWith(".xml"));
                File.Delete(lPathJson);
            }

             Assert.False(File.Exists(lPathXml));

            // Sauvegarde les donn�es, Sauver d�termine le format
            lSauveCollection.Sauver(lData, "int", false, "xml");
            Assert.True(File.Exists(lPathXml));

            if (File.Exists(lPathXml))
            {
                loadedFilePath = lPathXml;
                Assert.True(loadedFilePath.EndsWith(".xml"), "Le fichier doit �tre au format XML.");
                Assert.False(loadedFilePath.EndsWith(".json"));
                File.Delete(lPathXml);
            }
        }

    }
}
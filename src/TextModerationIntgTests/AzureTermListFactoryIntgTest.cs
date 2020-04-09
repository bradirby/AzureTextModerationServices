//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using TextModeration;
using NUnit.Framework;

namespace TextModerationIntgTests
{
 
    [Category("Integration")]
    public class AzureTermListFactoryIntgTest
    {
        private AzureTermListFactory sut { get; set; }
        private AzureTermListApi api { get; set; }

        private IConfigurationProvider ConfigProvider { get; set; }

        private AzureTermList Local { get; set; }

        private string LocalListName => "AzureTermListApiIntgTest";
        private string LocalListName2 => "AzureTermList2ApiIntgTest";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var cfg = new ConfigurationProvider();
            cfg.LoadNamedConfiguration("INTGTEST");
            ConfigProvider = cfg;

            api = new AzureTermListApi(ConfigProvider);
        }

        [SetUp]
        public void Setup()
        {
            sut = new AzureTermListFactory(api);
        }

        [Test]
        [Explicit]
        public void RunToCleanupTestLists()
        {
            var resultLst = sut.GetAllExistingAsync().Result;
            foreach (var azureTermList in resultLst)
            {
                if (azureTermList.Name.ToLower().EndsWith("intgtest"))
                    azureTermList.Delete();
            }
        }

        [Test]
        public void CreateNewAsync_WhenCalled_CreatesList()
        {
            var result = sut.CreateNewAsync(LocalListName, "test term list for the intg tests").Result;
            Assert.AreNotEqual(0, result.ListID);

            result = sut.CreateNewAsync(LocalListName2, "test term list for the intg tests").Result;
            Assert.AreNotEqual(0, result.ListID);

            var resultLst = sut.GetAllExistingAsync().Result;
            Assert.AreEqual(2, resultLst.Count);

        }

     

    }
}

//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System;
using System.Threading.Tasks;
using TextModeration;
using NUnit.Framework;

namespace TextModerationIntgTests
{
 
    [Category("Integration")]
    public class AzureTermListIntgTest
    {
        private AzureTermListFactory Factory { get; set; }

        private AzureTermList sut { get; set; }
        private string LocalListName => "AzureTermListApiIntgTest";


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var cfg = new ConfigurationProvider();
            cfg.LoadNamedConfiguration("INTGTEST");

            var api = new AzureTermListApi(cfg);
            Factory = new AzureTermListFactory(api);
            var lst = Factory.GetAllExistingAsync().Result;
            foreach (var item in lst)
            {
                if (item.Name == LocalListName) sut = item;
            }

            if (sut == null) sut = Factory.CreateNewAsync(LocalListName, "list for intg tests").Result;
        }

        [Test]
        [Explicit]
        public void RunToCleanUp()
        {
            sut.Delete();
        }


        [Test]
        public void AddTerm_WhenCalled_AddsTerm()
        {
            int numTerms = 0;
            var beforeResult = sut.GetAllTerms().Result;
            if (beforeResult != null) numTerms = beforeResult.Count;
            var newTerm = "hithere" + Guid.NewGuid().ToString();

            Task.Run(async() => await sut.AddTerm(newTerm)).Wait();

            var afterResult = sut.GetAllTerms(true).Result;
            Assert.IsTrue(afterResult.Contains(newTerm));
            Assert.AreEqual(numTerms + 1, afterResult.Count);
        }


    }
}

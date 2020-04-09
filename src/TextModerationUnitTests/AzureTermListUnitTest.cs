//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextModeration;
using Moq;
using NUnit.Framework;

namespace TextModerationUnitTests
{
 
    public class AzureTermListUnitTest
    {
        private Mock<IAzureTermListApi> MockApi { get; set; }

        private AzureTermList sut { get; set; }
        private string LocalListName => "AzureTermListApiIntgTest";


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            MockApi = new Mock<IAzureTermListApi>();
            sut = new AzureTermList(null, MockApi.Object);
        }


        [Test]
        public void AddTerm_AddingDuplicateWithCacheEnabled_DoesNotThrow()
        {
            var termCache = new List<string>();
            termCache.Add("hello");
            MockApi.Setup(x => x.GetAllTermsInTermListAsStringsAsync(sut.ListID.ToString(), sut.Language))
                .Returns(Task.FromResult(termCache));

            sut.EnableTermCache();

            sut.AddTerm("hello").Wait();
        }

        [Test]
        public void AddTerm_AddingDuplicateWithCacheDisabled_Throws()
        {
            var termCache = new List<string>();
            var term = "hello";
            termCache.Add(term);
            MockApi.Setup(x => x.GetAllTermsInTermListAsStringsAsync(sut.ListID.ToString(), sut.Language))
                .Returns(Task.FromResult(termCache));
            MockApi.Setup(x => x.AddTermToTermListAsync(sut.ListID.ToString(), term, sut.Language))
                .Throws<Exception>();
            sut.DisableTermCache();

            Assert.Throws<AggregateException>(() =>  sut.AddTerm("hello").Wait());
        }

    }
}

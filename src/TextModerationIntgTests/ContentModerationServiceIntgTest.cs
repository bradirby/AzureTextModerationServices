//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System.Linq;
using AzureContentModeration;
using TextModeration;
using NUnit.Framework;

namespace TextModerationIntgTests
{
 
    [Category("Integration")]
    [Explicit]
    public class ContentModerationServiceIntgTest
    {
        private ContentModerationService sut { get; set; }
        private IAzureTextModerationAPI AzureTextApi { get; set; }
        private IAzureTermListApi AzureTermListApi { get; set; }
        private IConfigurationProvider ConfigProvider { get; set; }
        private AzureTermList BadWordlist {
            get
            {
                if (_badWordList == null) _badWordList = SetupBadWordList();
                return _badWordList;
            }
        }

        private AzureTermList _badWordList;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var cfg = new ConfigurationProvider();
            cfg.LoadNamedConfiguration("INTGTEST");
            ConfigProvider = cfg;

            AzureTextApi = new AzureTextModerationApi(ConfigProvider);
            AzureTermListApi = new AzureTermListApi(ConfigProvider);
        }

        [Test]
        [Explicit]
        public void CleanupTestWordList()
        {
            BadWordlist.Delete().Wait();
        }

        private AzureTermList SetupBadWordList()
        {
            var fact = new AzureTermListFactory(AzureTermListApi);
            var allLists = fact.GetAllExistingAsync().Result;
            var bwLst = allLists.FirstOrDefault(x => x.Name == "ContentModerationServiceIntgTest");
            if (bwLst == null)
            {
                bwLst = fact.CreateNewAsync("ContentModerationServiceIntgTest", "list for intg tests").Result;
                bwLst.AddTerm("BadWord1").Wait();
                bwLst.AddTerm("BadWord2").Wait();
            }

            return bwLst;
        }

        [SetUp]
        public void Setup()
        {
            sut = new ContentModerationService(AzureTextApi);
        }

        [Test]
        public void ModerateBlogPost_ValidText_DoesNotReportBadWords()
        {
            var blog = new BlogPost {BlogText = "validate this text"};
            var modResultTask = sut.ModerateBlogPostAsync(blog);
            var modResult = modResultTask.Result;
            Assert.IsFalse(modResult.HasBadWords);
            Assert.IsFalse(modResult.HasPII);
            Assert.IsFalse(modResult.HasWordsInCustomList);
        }

        [Test]
        public void ModerateBlogPost_HasFuck_ReportsBadWords()
        {
            var blog = new BlogPost {BlogText = "validate this fucking text"};
            var modResultTask = sut.ModerateBlogPostAsync(blog);
            var modResult = modResultTask.Result;
            Assert.IsTrue(modResult.HasBadWords);
            Assert.IsFalse(modResult.HasPII);
            Assert.IsFalse(modResult.HasWordsInCustomList);
        }

        [Test]
        public void ModerateBlogPost_HasCrap_DoesNotReportBadWords()
        {
            var blog = new BlogPost {BlogText = "validate this crap text"};
            var modResultTask = sut.ModerateBlogPostAsync(blog);
            var modResult = modResultTask.Result;
            Assert.IsFalse(modResult.HasBadWords);
            Assert.IsFalse(modResult.HasPII);
            Assert.IsFalse(modResult.HasWordsInCustomList);
        }

        [Test]
        public void ModerateBlogPost_HasEmail_ReportsPII()
        {
            var blog = new BlogPost {BlogText = "validate this email emailaddr@microsoft.com"};
            var modResultTask = sut.ModerateBlogPostAsync(blog);
            var modResult = modResultTask.Result;
            Assert.IsFalse(modResult.HasBadWords);
            Assert.IsTrue(modResult.HasPII);
            Assert.IsFalse(modResult.HasWordsInCustomList);
        }

        
        [Test]
        public void ModerateBlogPost_WithIgnorePIIOption_StillChecksPII()
        {
            var blog = new BlogPost {BlogText = "validate this fucking email text emailaddr@microsoft.com"};
            var options = new ContentModerationServiceOptions(true, false);
            var modResultTask = sut.ModerateBlogPostAsync(blog, options);
            var modResult = modResultTask.Result;
        
            Assert.IsTrue(modResult.HasBadWords);

            //Azure seems to ignore the option to turn off PII checking
            Assert.IsTrue(modResult.HasPII);
            Assert.IsFalse(modResult.HasWordsInCustomList);
        }

        [Test]
        public void ModerateBlogPost_HasBothFuckAndEmail_ReportsResultsCorrectly()
        {
            var blog = new BlogPost {BlogText = "validate this fucking email emailaddr@microsoft.com"};
            var modResultTask = sut.ModerateBlogPostAsync(blog);
            var modResult = modResultTask.Result;
            Assert.IsTrue(modResult.HasBadWords);
            Assert.IsTrue(modResult.HasPII);
            Assert.IsFalse(modResult.HasWordsInCustomList);
        }

    

        [Test]
        public void ModerateBlogPost_WordsFromCustomList_SetsProperflag()
        {
            var options = new ContentModerationServiceOptions(true, true, BadWordlist);
            var blog = new BlogPost {BlogText = "validate this BadWord1 email with BadWord2 custom bad words "};
            var modResult = sut.ModerateBlogPostAsync(blog, options).Result;
            Assert.IsFalse(modResult.HasBadWords);
            Assert.IsFalse(modResult.HasPII);
            Assert.IsTrue(modResult.HasWordsInCustomList);
        }
    }
}

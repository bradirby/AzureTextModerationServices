//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices


using System;
using System.Threading.Tasks;
using AzureContentModeration;
using TextModeration;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Moq;
using NUnit.Framework;

namespace TextModerationUnitTests
{
 
    public class ContentModerationServiceUnitTest
    {
        private ContentModerationService sut { get; set; }
        private Mock<IAzureTextModerationAPI> MockAzureTextApi { get; set; }


        [SetUp]
        public void Setup()
        {
            MockAzureTextApi = new Mock<IAzureTextModerationAPI>();
            sut = new ContentModerationService(MockAzureTextApi.Object);
        }

        [Test]
        public void ModerateBlogPost_NullBlogPost_Throws()
        {
            var resultTask = sut.ModerateBlogPostAsync(null);
            BlogPostModerationResult result;
            Assert.Throws<AggregateException>(() => result = resultTask.Result);
        }

        [Test]
        public void ModerateBlogPost_NoBlogText_DoesNotThrow()
        {
            var blog = new BlogPost {BlogText = ""};
            var azureRetVal = new Screen();

            MockAzureTextApi.Setup(x => x.ModerateTextAsync(blog.BlogText, It.IsAny<IAzureTextModerationOptions>()))
                .Returns(Task.FromResult(azureRetVal));


            var retval = sut.ModerateBlogPostAsync(blog);
        }
    }
}

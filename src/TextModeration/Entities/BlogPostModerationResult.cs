//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace TextModeration
{
    /// <summary>
    /// Sample moderation results with just the properties one might care about
    /// when allowing others to write a blog post.  This should be customized for your own purposes.
    /// </summary>
    public class BlogPostModerationResult
    {
        public bool HasPII { get; set; }
        public bool HasBadWords { get; set;}
        public bool HasWordsInCustomList { get; set; }
        public Screen FullModerationResults { get; }

        public BlogPostModerationResult(Screen moderationResults)
        {
            FullModerationResults = moderationResults;
        }
    }
}

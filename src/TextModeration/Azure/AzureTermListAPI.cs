//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace TextModeration
{
    //https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/term-lists-quickstart-dotnet

    public class AzureTermListApi : IAzureTermListApi
    {
        private IConfigurationProvider ConfigProvider { get; }

        public AzureTermListApi(IConfigurationProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

        private ContentModeratorClient GetNewClient()
        {
            // Create and initialize an instance of the Content Moderator API wrapper.
            var client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(ConfigProvider.AzureContentModerationSubscriptionKey));

            client.Endpoint = ConfigProvider.AzureContentModerationEndpoint;
            return client;
        }

        /// <summary>
        /// Refresh the search index for the indicated term list.
        /// </summary>
        public async Task RefreshSearchIndexAsync ( string listID, string language)
        {
            using var client = GetNewClient();
            await client.ListManagementTermLists.RefreshIndexMethodAsync(listID, language);
        }

        /// <summary>
        /// Creates a new term list.
        /// </summary>
        public async Task<TermList> CreateTermListAsync (string listName, string listDesc)
        {
            TermList newTermList;
            Body body = new Body(listName, listDesc);
            using (var client = GetNewClient())
            {
                newTermList = await client.ListManagementTermLists.CreateAsync("application/json", body);
            }

            if (false == newTermList.Id.HasValue)
            {
                throw new Exception("TermList.Id value missing.");
            }
            return newTermList;
        }

        /// <summary>
        /// Update the information for the indicated term list.
        /// </summary>
        public async Task UpdateTermListDescriptionAsync (string listID, string name, string description )
        {
            Body body = new Body(name, description);
            using var client = GetNewClient();
            await client.ListManagementTermLists.UpdateAsync(listID, "application/json", body);
        }

        public Task<List<string>> GetAllTermsInTermListAsStringsAsync(string listID, string language)
        {
            var lst = GetAllTermsInTermListAsync(listID, language).Result;
            var termCache = new List<string>();
            foreach (var term in lst.Data.Terms)
            {
                termCache.Add(term.Term);
            }

            return Task.FromResult(termCache);
        }

        /// <summary>
        /// Add a term to the indicated term list.
        /// </summary>
        public async Task AddTermToTermListAsync (string listID, string term, string language)
        {
            using var client = GetNewClient();
            await client.ListManagementTerm.AddTermAsync(listID, term, language);
        }


        /// <summary>
        /// Get all terms in the indicated term list.
        /// </summary>
        public async Task<Terms> GetAllTermsInTermListAsync(string listID, string language)
        {
            using var client = GetNewClient();
            return await client.ListManagementTerm.GetAllTermsAsync(listID,language);
        }

        /// <summary>
        /// Delete a term from the indicated term list.
        /// </summary>
        public async Task DeleteTermAsync ( string listID, string term, string language)
        {
            using var client = GetNewClient();
            await client.ListManagementTerm.DeleteTermAsync(listID, term, language);
        }

        /// <summary>
        /// Delete all terms from the indicated term list.
        /// </summary>
        public async Task DeleteAllTermsAsync ( string listID, string language)
        {
            using var client = GetNewClient();
            await client.ListManagementTerm.DeleteAllTermsAsync(listID, language);
        }

        /// <summary>
        /// Delete the indicated term list.
        /// </summary>
        public async Task DeleteTermListAsync ( string listID)
        {
            using var client = GetNewClient();
            await client.ListManagementTermLists.DeleteAsync(listID);
        }

        /// <summary>
        /// Delete the indicated term list.
        /// </summary>
        public async Task<IList<TermList>> GetAllTermListsAsync ( )
        {
            using var client = GetNewClient();
            return await client.ListManagementTermLists.GetAllTermListsAsync();
        }
      
    }

    public interface IAzureTermListApi
    {
        Task<IList<TermList>> GetAllTermListsAsync();
        Task<TermList> CreateTermListAsync (string listName, string listDesc);
        Task RefreshSearchIndexAsync(string listID, string language);
        Task DeleteTermListAsync(string listID);
        Task UpdateTermListDescriptionAsync(string listID, string name, string description);
        Task<Terms> GetAllTermsInTermListAsync(string listID, string language);
        Task<List<string>> GetAllTermsInTermListAsStringsAsync(string listID, string language);
        Task AddTermToTermListAsync(string listID, string term, string language);
        Task DeleteTermAsync(string listID, string term, string language);
        Task DeleteAllTermsAsync(string listID, string language);
    }
}

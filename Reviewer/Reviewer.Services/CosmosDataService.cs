using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer.SharedModels;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq.Expressions;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace Reviewer.Services
{
    public class CosmosDataService : IDataService
    {
        string databaseName = "BuildReviewer";
        string businessCollectionName = "Businesses";
        string reviewCollectionName = "Reviews";

        DocumentClient client;

        public async Task Initialize()
        {
            if (client != null)
                return;

            client = new DocumentClient(new Uri(APIKeys.CosmosEndpointUrl), APIKeys.CosmosAuthKey);

            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });

            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseName),
                new DocumentCollection { Id = businessCollectionName },
                new RequestOptions { OfferThroughput = 400 }
            );

            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseName),
                new DocumentCollection { Id = reviewCollectionName },
                new RequestOptions { OfferThroughput = 400 }
            );
        }

        public async Task<List<Business>> GetBusinesses()
        {
            await Initialize();

            var businesses = new List<Business>();

            var businessQuery = client.CreateDocumentQuery<Business>(
                UriFactory.CreateDocumentCollectionUri(databaseName, businessCollectionName),
                new FeedOptions { MaxItemCount = -1 }).AsDocumentQuery();

            while (businessQuery.HasMoreResults)
            {
                var queryResults = await businessQuery.ExecuteNextAsync<Business>();

                businesses.AddRange(queryResults);
            }

            return businesses;
        }

        public async Task<List<Review>> GetReviewsForBusiness(string businessId)
        {
            await Initialize();

            var reviews = new List<Review>();

            var reviewQuery = client.CreateDocumentQuery<Review>(
                UriFactory.CreateDocumentCollectionUri(databaseName, reviewCollectionName),
                new FeedOptions { MaxItemCount = -1 })
                                    .Where(r => r.BusinessId == businessId)
                                    .AsDocumentQuery();

            while (reviewQuery.HasMoreResults)
            {
                var queryResults = await reviewQuery.ExecuteNextAsync<Review>();

                reviews.AddRange(queryResults);
            }

            return reviews;
        }

        public async Task<List<Review>> GetReviewsByAuthor(string authorId)
        {
            await Initialize();

            var reviews = new List<Review>();

            var reviewQuery = client.CreateDocumentQuery<Review>(
                UriFactory.CreateDocumentCollectionUri(databaseName, reviewCollectionName),
                new FeedOptions { MaxItemCount = -1 })
                                    .Where(r => r.AuthorId == authorId)
                                    .AsDocumentQuery();

            while (reviewQuery.HasMoreResults)
            {
                var queryResults = await reviewQuery.ExecuteNextAsync<Review>();

                reviews.AddRange(queryResults);
            }

            return reviews;
        }

        public async Task InsertReview(Review review)
        {
            await Initialize();

            await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseName, reviewCollectionName),
                review);
        }

        public async Task UpdateReview(Review review)
        {
            await Initialize();

            var reviewUri = UriFactory.CreateDocumentUri(databaseName, reviewCollectionName, review.Id);

            await client.ReplaceDocumentAsync(reviewUri, review);
        }
    }
}

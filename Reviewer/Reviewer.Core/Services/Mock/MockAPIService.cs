using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public class MockAPIService : IAPIService
    {
        List<string> authors = new List<string> { "Fred", "Wilma", "Barney", "Betty" };
        List<string> text = new List<string> { "Good", "Meh", "Delicious", "Best thing ever!", "Never go there!" };

        public MockAPIService()
        {
        }

        public async Task<List<Review>> GetReviewsForBusiness(string businessId)
        {
            var r = new Random();

            var review1 = new Review
            {
                Author = authors[r.Next(0, authors.Count)],
                BusinessId = businessId,
                Date = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                Id = Guid.NewGuid().ToString(),
                Photos = new List<string>(),
                Rating = r.Next(1, 5),
                ReviewText = text[r.Next(0, text.Count)]
            };

            var review2 = new Review
            {
                Author = authors[r.Next(0, authors.Count)],
                BusinessId = businessId,
                Date = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                Id = Guid.NewGuid().ToString(),
                Photos = new List<string>(),
                Rating = r.Next(1, 5),
                ReviewText = text[r.Next(0, text.Count)]
            };


            var review3 = new Review
            {
                Author = authors[r.Next(0, authors.Count)],
                BusinessId = businessId,
                Date = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                Id = Guid.NewGuid().ToString(),
                Photos = new List<string>(),
                Rating = r.Next(1, 5),
                ReviewText = text[r.Next(0, text.Count)]
            };

            return await Task.FromResult(new List<Review> { review1, review2, review3 });
        }

        public Task InsertReview(Review review, string token)
        {
            return Task.CompletedTask;
        }

        public Task UpdateReview(Review review, string token)
        {
            return Task.CompletedTask;
        }

        public Task<List<Review>> GetReviewsForAuthor(string authorId, string token)
        {
            return Task.FromResult(new List<Review>());
        }

        public Task<string> GetContainerWriteSasToken()
        {
            return Task.FromResult("");
        }

        public Task WritePhotoInfoToQueue(string businessId, string photoUrl)
        {
            return Task.CompletedTask;
        }
    }
}

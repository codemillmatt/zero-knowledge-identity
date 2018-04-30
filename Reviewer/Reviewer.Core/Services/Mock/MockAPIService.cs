using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Reviewer.Services;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public class MockAPIService : IAPIService
    {
        HttpClient webClient = new HttpClient();


        public async Task<List<Review>> GetReviewsForBusiness(string businessId)
        {
            var reviewJson = await webClient.GetStringAsync($"{APIKeys.WebAPIUrl}review/business/{businessId}");

            return JsonConvert.DeserializeObject<List<Review>>(reviewJson);
        }

        public async Task InsertReview(Review review, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{APIKeys.NoSecurityWebAPIInsertReview}");

            request.Content = new StringContent(JsonConvert.SerializeObject(review), Encoding.UTF8, "application/json");

            await webClient.SendAsync(request);
        }

        public async Task UpdateReview(Review review, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{APIKeys.NoSecurityWebAPIUpdateReview}");

            request.Content = new StringContent(JsonConvert.SerializeObject(review), Encoding.UTF8, "application/json");

            await webClient.SendAsync(request);
        }

        public async Task<List<Review>> GetReviewsForAuthor(string authorId, string token)
        {
            authorId = Guid.Empty.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{APIKeys.NoSecurityWebAPIGetReviewsForAuthor}{authorId}");

            var response = await webClient.SendAsync(request);

            var reviewJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Review>>(reviewJson);
        }

        public async Task<string> GetContainerWriteSasToken()
        {
            var spr = new StoragePermissionRequest { Permission = "Write" };

            var request = new HttpRequestMessage(HttpMethod.Post, APIKeys.NoSecuritySasUrl);

            var content = new StringContent(JsonConvert.SerializeObject(spr), Encoding.UTF8, "application/json");

            request.Content = content;

            var response = await webClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task WritePhotoInfoToQueue(string reviewId, string photoUrl)
        {
            var queueInfo = new { reviewId, photoUrl };

            var request = new HttpRequestMessage(HttpMethod.Post, APIKeys.NoSecurityWriteToQueueUrl);

            var content = new StringContent(JsonConvert.SerializeObject(queueInfo), Encoding.UTF8, "application/json");

            request.Content = content;

            await webClient.SendAsync(request);
        }
    }
}

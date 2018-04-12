using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reviewer.Services;
using Reviewer.SharedModels;

namespace Reviewer.WebAPI.Controllers
{
    [Produces("application/json")]
    public class ReviewController : Controller
    {
        static CosmosDataService cosmosService = new CosmosDataService();

        [HttpGet("/Review/Business/{id}")]
        public async Task<IEnumerable<Review>> ReviewsForBusiness(string id)
        {
            return await cosmosService.GetReviewsForBusiness(id);
        }

        [HttpGet("/Review/Author/{id}")]
        public async Task<IEnumerable<Review>> ReviewsForAuthor(string id)
        {
            return await cosmosService.GetReviewsByAuthor(id);
        }

        [HttpPost("/Review")]
        public async Task InsertReview([FromBody]Review review)
        {
            await cosmosService.InsertReview(review);
        }

        [HttpPut("/Review")]
        public async Task UpdateReview([FromBody]Review review)
        {
            await cosmosService.UpdateReview(review);
        }
    }

}
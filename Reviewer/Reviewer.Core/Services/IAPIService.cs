using System;
using System.Collections.Generic;
using Reviewer.SharedModels;
using System.Threading.Tasks;
namespace Reviewer.Core
{
    public interface IAPIService
    {
        Task<List<Review>> GetReviewsForBusiness(string businessId);
    }
}

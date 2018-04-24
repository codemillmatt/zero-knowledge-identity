using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace Reviewer.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class WritePhotoInfoQueue
    {
        [FunctionName("WritePhotoInfoQueue")]
        [return: Queue("review-photos")]
        public static QueueMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]QueueMessage req, TraceWriter log)
        {
            log.Info("Starting the queue trigger");
            log.Info($"Review ID: {req.reviewId}, Photo Url: {req.photoUrl}");

            return req;
        }

        
    }

    
}

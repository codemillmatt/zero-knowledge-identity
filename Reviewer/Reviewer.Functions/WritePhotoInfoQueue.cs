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
        public static QueueMsg Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]QueueMsg req, TraceWriter log)
        {

            log.Info("Starting the queue trigger");
            log.Info($"Review ID: {req.reviewId}, Photo Url: {req.photoUrl}");

            // Get request body

            // Set name to query string or body data
            //name = name ?? data?.name;

            //return name == null
            //? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //: req.CreateResponse(HttpStatusCode.OK, "Hello " + name);

            //return req.CreateErrorResponse

            return req;
        }

        //[FunctionName("WritePhotoInfoCosmos")]
        //public static void Run([QueueTrigger("review-photos")]string myQueueItem,
        //    [DocumentDB("build", "reviews", ConnectionStringSetting = "Reviews_Cosmos", Id = "/id")]out dynamic document,
        //    TraceWriter log)
        //{
        //    log.Info($"C# Queue trigger function processed: {myQueueItem}");

        //    try
        //    {
        //        var jsonified = JsonConvert.DeserializeObject<PhotoInfo>(myQueueItem);

        //        if (jsonified != null)
        //            document = jsonified;
        //        else
        //            document = null;
        //    }
        //    catch (JsonReaderException ex)
        //    {
        //        log.Error($"*** Error: {ex.Message}");
        //        document = null;
        //    }
        //}:w

    }

    public class QueueMsg
    {
        public string reviewId { get; set; }
        public string photoUrl { get; set; }
    }
}

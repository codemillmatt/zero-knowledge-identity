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
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
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
}

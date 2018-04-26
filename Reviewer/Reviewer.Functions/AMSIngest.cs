using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Reviewer.Functions.Helpers;
using Reviewer.Functions.Models;

namespace Reviewer.Functions
{
    public static class AMSIngest
    {
        static readonly string AADTenantDomain = Environment.GetEnvironmentVariable("AMSAADTenantDomain");
        static readonly string RESTAPIEndpoint = Environment.GetEnvironmentVariable("AMSRESTAPIEndpoint");
        static readonly string mediaServicesClientId = Environment.GetEnvironmentVariable("AMSClientId");
        static readonly string mediaServicesClientSecret = Environment.GetEnvironmentVariable("AMSClientSecret");

        static readonly string storageAccountName = Environment.GetEnvironmentVariable("MediaServicesStorageAccountName");
        static readonly string storageAccountKey = Environment.GetEnvironmentVariable("MediaServicesStorageAccountKey");

        private static CloudMediaContext context = null;
        private static CloudStorageAccount destinationStorageAccount = null;

        [FunctionName("AMSIngest")]
        [return: Queue("review-videos", Connection ="AzureWebJobsStorage")]
        public static async Task<VideoQueueMessage> Run([BlobTrigger("review-photos/{fileName}.mp4", Connection = "AzureWebJobsStorage")]CloudBlockBlob inputBlob,
            string fileName, TraceWriter log)
        {
            try
            {
                fileName = $"{fileName}.mp4";

                log.Info($"C# Blob trigger function Processed blob\n Name:{fileName}");

                if (!inputBlob.Metadata.ContainsKey("reviewId"))
                {
                    log.Warning("No review id metadata!");
                    await inputBlob.DeleteIfExistsAsync();
                    return null;
                }

                string reviewId = inputBlob.Metadata["reviewId"];
                string assetName = inputBlob.Name;

                AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(AADTenantDomain,
                               new AzureAdClientSymmetricKey(mediaServicesClientId, mediaServicesClientSecret),
                               AzureEnvironments.AzureCloudEnvironment);

                AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                context = new CloudMediaContext(new Uri(RESTAPIEndpoint), tokenProvider);

                StorageCredentials mediaServicesCredentials = new StorageCredentials(storageAccountName, storageAccountKey);

                // 1. Copy BLOB into Input Asset
                //IAsset newAsset = await CreateAssetFromBlobAsync(inputBlob, fileName, log);
                IAsset thumbAsset = await CreateAssetFromBlobAsync(inputBlob, $"thumb-{fileName}", log);

                log.Info("Deleting the source asset from the input container");
                await inputBlob.DeleteIfExistsAsync();

                // 2. Create an encoding job

                // Declare a new encoding job with the Standard encoder
                IJob job = context.Jobs.Create("Build Reviewer - Function Kickoff");

                // Get a media processor reference, and pass to it the name of the 
                // processor to use for the specific task.
                IMediaProcessor processor = GetLatestMediaProcessorByName("Media Encoder Standard");

                // Create a task with the encoding details, using the Adaptive Streaming System Preset.
                //ITask task = job.Tasks.AddNew("Encode with Adaptive Streaming",
                //    processor,
                //    "Content Adaptive Multiple Bitrate MP4",//"Adaptive Streaming",
                //    TaskOptions.None);


                // Set the Task Priority
                //task.Priority = 100;

                // Specify the input asset to be encoded.
                //task.InputAssets.Add(newAsset);

                // Add an output asset to contain the results of the job. 
                // This output is specified as AssetCreationOptions.None, which 
                // means the output asset is not encrypted. 
                //task.OutputAssets.AddNew(fileName, AssetCreationOptions.None);

                log.Info("Trying to create the thumbnail blob");

                string homePath = Environment.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.Process);
                string presetPath = "";
                log.Info($"Home path: {homePath}");

                if (string.IsNullOrEmpty(homePath))
                {
                    presetPath = @"../Presets/thumbnail.json";
                }
                else
                {
                    presetPath = Path.Combine(homePath, @"site\wwwroot\Presets\thumbnail.json");
                }

                log.Info($"Preset path: {presetPath}");

                string thumbnailPreset = File.ReadAllText(presetPath);

                thumbnailPreset = VideoEncodingPresetGenerator.Thumbnail().ToJson();
                log.Info($"Start of preset: {thumbnailPreset.Substring(0, 10)}");


                ITask thumbnailTask = job.Tasks.AddNew("Thumbnail encode", processor, thumbnailPreset, TaskOptions.None);
                thumbnailTask.Priority = 100;
                if (thumbAsset == null)
                {
                    log.Error("Thumbnail asset is null!!!");
                }
                thumbnailTask.InputAssets.Add(thumbAsset);
                var outAssetName = $"thumbname-{fileName}";

                thumbnailTask.OutputAssets.AddNew(outAssetName, AssetCreationOptions.None);

                log.Info("Thumbnail task created");

                job.Submit();
                log.Info("Job Submitted");

                // 3. Monitor the job
                while (true)
                {
                    job.Refresh();

                    // Refresh every 5 seconds
                    await Task.Delay(5000);

                    log.Info($"Job: {job.Id}    State: {job.State.ToString()}");

                    if (job.State == JobState.Error || job.State == JobState.Finished || job.State == JobState.Canceled)
                        break;
                }

                if (job.State == JobState.Finished)
                {
                    log.Info($"Job {job.Id} is complete.");

                    return new VideoQueueMessage { reviewId = reviewId, assetName = outAssetName};
                }
                else if (job.State == JobState.Error)
                {
                    log.Error("Job Failed with Error. ");
                    throw new Exception("Job failed encoding .");
                }
            } 
            catch (Exception ex)
            {
                log.Error($"An exception occurred: {ex.Message}");
            }

            // Write to a queue
            log.Info("All done!!");
            return null;
        }

        public static async Task<IAsset> CreateAssetFromBlobAsync(CloudBlockBlob blob, string assetName, TraceWriter log)
        {
            //Get a reference to the storage account that is associated with the Media Services account. 
            StorageCredentials mediaServicesStorageCredentials =
                new StorageCredentials(storageAccountName, storageAccountKey);

            destinationStorageAccount = new CloudStorageAccount(mediaServicesStorageCredentials, false);

            // Create a new asset. 
            var asset = context.Assets.Create(blob.Name, AssetCreationOptions.None);

            log.Info($"Created new asset {asset.Name}");

            IAccessPolicy writePolicy = context.AccessPolicies.Create("writePolicy",
                TimeSpan.FromHours(4), AccessPermissions.Write);

            ILocator destinationLocator = context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);

            CloudBlobClient destBlobStorage = destinationStorageAccount.CreateCloudBlobClient();

            // Get the destination asset container reference
            string destinationContainerName = (new Uri(destinationLocator.Path)).Segments[1];

            log.Info($"Destination container name: {destinationContainerName}");

            CloudBlobContainer assetContainer = destBlobStorage.GetContainerReference(destinationContainerName);

            try
            {
                assetContainer.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                log.Error("ERROR:" + ex.Message);
            }

            log.Info("Created asset.");

            // Get hold of the destination blob
            CloudBlockBlob destinationBlob = assetContainer.GetBlockBlobReference(blob.Name);

            // Copy Blob
            try
            {
                using (var stream = await blob.OpenReadAsync())
                {
                    await destinationBlob.UploadFromStreamAsync(stream);
                }

                log.Info("Copy Complete.");

                var assetFile = asset.AssetFiles.Create(blob.Name);
                assetFile.ContentFileSize = blob.Properties.Length;

                //assetFile.MimeType = "video/mp4";

                assetFile.IsPrimary = true;
                assetFile.Update();
                asset.Update();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Info(ex.StackTrace);
                log.Info("Copy Failed.");

                throw;
            }

            destinationLocator.Delete();
            writePolicy.Delete();

            return asset;
        }

        private static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        
    }
}

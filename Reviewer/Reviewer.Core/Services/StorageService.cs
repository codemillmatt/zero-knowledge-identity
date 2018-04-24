using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using MonkeyCache.FileStore;

namespace Reviewer.Core
{
    public class StorageService : IStorageService
    {
        enum StoragePermissionType
        {
            List,
            Read,
            Write
        }

        public Task<Uri> UploadBlob(Stream blobContent, UploadProgress progressUpdater)
        {
            throw new NotImplementedException();
        }

        #region Helpers

        static async Task<StorageCredentials> ObtainStorageCredentials(StoragePermissionType permissionType)
        {
            var cacheKey = permissionType.ToString();

            if (Barrel.Current.Exists(cacheKey) && !Barrel.Current.IsExpired(cacheKey))
                return new StorageCredentials(Barrel.Current.Get<string>(cacheKey));

            string storageToken = null;
            switch (permissionType)
            {
                case StoragePermissionType.List:
                    storageToken = await FunctionService.GetContainerListSasToken().ConfigureAwait(false);
                    break;
                case StoragePermissionType.Read:
                    storageToken = await FunctionService.GetContainerReadSASToken().ConfigureAwait(false);
                    break;
                case StoragePermissionType.Write:
                    storageToken = await FunctionService.GetContainerWriteSasToken().ConfigureAwait(false);
                    break;
            }

            return storageToken == null ? null : StuffCredentialsInBarrel(storageToken, cacheKey);
        }

        static TimeSpan GetExpirationSpan(string tokenQueryString)
        {
            // We'll need to parse the token query string
            // easiest way is to make it ino a URI and parse it with URI.ParseQueryString

            var fakeTokenUri = new Uri($"http://localhost{tokenQueryString}");
            var queryStringParts = fakeTokenUri.ParseQueryString();

            var endDateString = queryStringParts["se"];

            // Expire one minute before we really need to
            var endTimeSpan = DateTimeOffset.Parse(endDateString) - DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1);

            return endTimeSpan;
        }

        static StorageCredentials StuffCredentialsInBarrel(string storageToken, string cacheKey)
        {
            var credentials = new StorageCredentials(storageToken);
            var expireIn = GetExpirationSpan(storageToken);

            Barrel.Current.Add<string>(cacheKey, storageToken, expireIn);

            return credentials;
        }

        #endregion
    }
}

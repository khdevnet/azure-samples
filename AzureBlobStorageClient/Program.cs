using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageClient
{
    class Program
    {
        private static string sourceConnectionString = "";
        private static string destinationConnectionString = "";

        static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();
            sourceConnectionString = configuration.GetValue<string>("storage:source");
            destinationConnectionString = configuration.GetValue<string>("storage:destination");

            var listNames = new[] { "https://enzofilesdevturkey.blob.core.windows.net/inspection/a20190902141903384.jpg" };
         var dd =   Path.GetExtension("https://enzofilesdevturkey.blob.core.windows.net/inspection/a20190902141903384.jpg");
            await CreateCarDetails();
        //    await CopyDocuments(listNames);

            Console.WriteLine("Hello World!");
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                        .AddJsonFile("appsettings.json", false)
                        .Build();
        }

        private static async Task CopyDocuments(IEnumerable<string> listUrls)
        {
            var container = await GetContainer(sourceConnectionString, "cars");
            var destContainer = await GetContainer(destinationConnectionString, "cars");

            var sourceBlobClient = CreateClient(sourceConnectionString);
            var destBlobClient = CreateClient(destinationConnectionString);
            CloudBlockBlob[] sourceBlobs = listUrls.Select(name => new CloudBlockBlob(new Uri(name), sourceBlobClient)).ToArray();

            var dests = await CopyBlobsAsync(sourceBlobs, destContainer);
        }

        private static async Task CreateCarDetails()
        {
            var destContainer = await GetContainer(destinationConnectionString, "cars");

            var destBlob = destContainer.GetBlockBlobReference("id1-car-details");
            var json = JsonConvert.SerializeObject(new { name = "anton" });
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            destBlob.Properties.ContentType = "application/json";
            await destBlob.UploadFromByteArrayAsync(jsonBytes, 0, jsonBytes.Length);

            //var dests = await CopyBlobsAsync(sourceBlobs, destContainer);
        }

        private static async Task<CloudBlobContainer> GetContainer(string externalStorageConnection, string containerName)
        {
            CloudBlobClient blobClient = CreateClient(externalStorageConnection);
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            //TODO: Validate permissions
            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            return container;
        }

        private static CloudBlobClient CreateClient(string externalStorageConnection)
        {
            var storageAccount = CloudStorageAccount.Parse(externalStorageConnection);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient;
        }

        private static Task<ICloudBlob[]> CopyBlobsAsync(
            IEnumerable<CloudBlockBlob> sourceBlobRefs,
            CloudBlobContainer destContainer)
        {
            var destinationBlobs = sourceBlobRefs.Select(sb => CopyBlobAsync(destContainer, sb));
            return Task.WhenAll(destinationBlobs);
        }

        private static async Task<ICloudBlob> CopyBlobAsync(CloudBlobContainer destContainer, CloudBlockBlob sourceBlobRef)
        {
            // var directory = destContainer.GetDirectoryReference("/1my-car/documents");
            CloudBlockBlob destBlob = destContainer.GetBlockBlobReference("1my-car/documents" + sourceBlobRef.Name);
            sourceBlobRef.Properties.ContentType = "image/png";
            await sourceBlobRef.SetPropertiesAsync();
            await destBlob.StartCopyAsync(sourceBlobRef);
            ICloudBlob destBlobRef = await destContainer.GetBlobReferenceFromServerAsync("1my-car/documents" + sourceBlobRef.Name);
            while (destBlobRef.CopyState.Status == CopyStatus.Pending)
            {
                await Task.Delay(200);
                destBlobRef = await destContainer.GetBlobReferenceFromServerAsync("1my-car/documents/" + sourceBlobRef.Name);
            }
            return destBlobRef;
        }
    }
}

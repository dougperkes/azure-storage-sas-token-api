using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Web.Http;

namespace SASTokenAPI.Controllers
{
    /// <summary>
    /// Simple Data Transfer Object for the JSON response
    /// </summary>
    public class FileResponse
    {
        public string FileSasUri { get; set; }
        public string FileName { get; set; }
    }

    public class FileController : ApiController
    {
        // GET api/values/5
        public FileResponse Post(string jobId)
        {
            string randomFileName = $"{jobId}-{Faker.Company.CatchPhrase().Replace(" ", "-")}.txt";

            CloudBlobContainer container = GetBlobContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(randomFileName);
            CreateRandomFile(blockBlob);

            var fileSasUri = GetBlobSasUri(container, randomFileName);
            var response = new FileResponse
            {
                FileName = randomFileName,
                FileSasUri = fileSasUri
            };
            return response;
        }

        private static void CreateRandomFile(CloudBlockBlob blockBlob)
        {
            var randomText = String.Join(Environment.NewLine + Environment.NewLine, Faker.Lorem.Paragraphs(5));
            blockBlob.UploadText(randomText);
        }

        private CloudBlobContainer GetBlobContainer()
        {
            // Retrieve storage account from connection string.
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("demo-files");

            // Create the container if it doesn't already exist.
            // By default, the new container is private, meaning that you must specify your storage access key to download blobs from this container.
            container.CreateIfNotExists();
            return container;
        }

        /// <summary>
        /// Returns a URI containing a SAS for the blob.
        /// </summary>
        /// <param name="container">A reference to the container.</param>
        /// <param name="blobName">A string containing the name of the blob.</param>
        /// <param name="policyName">A string containing the name of the stored access policy. If null, an ad-hoc SAS is created.</param>
        /// <returns>A string containing the URI for the blob, with the SAS token appended.</returns>
        static string GetBlobSasUri(CloudBlobContainer container, string blobName, string policyName = null)
        {
            string sasBlobToken;

            //Get a reference to a blob within the container.
            //Note that the blob may not exist yet, but a SAS can still be created for it.
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            if (policyName == null)
            {
                // Create a new access policy and define its constraints.
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and 
                // to construct a shared access policy that is saved to the container's shared access policies. 
                SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
                {
                    // Set start time to five minutes before now to avoid clock skew.
                    SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Read // | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
                };

                //Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);
            }
            else
            {
                //Generate the shared access signature on the blob. In this case, all of the constraints for the
                //shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }


    }
}

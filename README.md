# azure-storage-sas-token-api

Simple demonstration of returning a shared access token for a single file in Azure blob storage. The sample consists of a web api that generates a dummy file in Azure blob storage, creates a shared access token granting read-only access to the file, and returns a simple JSON document containing the file name and full SAS URI. The sample also includes a sample client application which invokes that API and downloads the file.

**Sample JSON Response**

```JSON
{
    "FileSasUri": "https://sampleblobaccount.blob.core.windows.net/demo-files/asdfa-Universal-heuristic-adapter.txt?sv=2017-04-17&sr=b&sig=YtGVocRtasczA%2BKs16Oi5NULDdPCPh3qa0%3D&st=2017-09-06T17%3A20%3A37Z&se=2017-09-07T17%3A25%3A37Z&sp=r",
    "FileName": "asdfa-Universal-heuristic-adapter.txt"
}
```

## Running this sample
1. Create a general purpose storage account in Azure. See https://docs.microsoft.com/en-us/azure/storage/common/storage-create-storage-account. 
1. Retrieve the connection string for the storage account. See https://docs.microsoft.com/en-us/azure/storage/common/storage-create-storage-account#manage-your-storage-account. The connection string must be in the format DefaultEndpointsProtocol=https;AccountName=[your account name];AccountKey=[your account key]
1. Update the web.config with your storage connection string.
1. Open the .sln file in Visual Studio. 
    1. In Solution Explorer, right click on the solution name, and click Properties.
    1. Under Startup Project, select **Multiple startup projects**, and set the action to **Start** for both SASTokenAPI and SASTokenClient.
    1. Click OK to save the settings changes.
1. Press F5 to test the project.
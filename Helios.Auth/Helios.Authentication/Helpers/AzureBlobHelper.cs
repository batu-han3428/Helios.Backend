using Azure.Storage.Blobs;

namespace Helios.Authentication.Helpers
{
    public interface IFileStorageHelper
    {
        Task CreateFolderIfNotExist(string path);
        Task<bool> HasFolderExists(string path);
        Task<BlobClient> GetBlob(string path);
        Task<bool> Upload(string path, Stream stream, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> UploadAndOverWrite(string path, Stream stream, CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> ReadIFormFileAsync(IFormFile file);
        Task RemoveFile(string path);
    }
    public class AzureBlobHelper : IFileStorageHelper
    {
        public AzureBlobHelper(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        private readonly IConfiguration Configuration;
        public async Task<bool> HasFolderExists(string path)
        {
            var container = new BlobContainerClient(Configuration["Azure:FileShareConnection"], Configuration["Azure:ShareName"]);
            if (await container.ExistsAsync())
            {
                var ppp = path.TrimStart('/');
                BlobClient blobClient2 = container.GetBlobClient(ppp);
                return await blobClient2.ExistsAsync();
            }
            return false;
        }
        public async Task<BlobClient> GetBlob(string path)
        {
            try
            {
                var container = new BlobContainerClient(Configuration["Azure:FileShareConnection"], Configuration["Azure:ShareName"]);

                if (await container.ExistsAsync())
                {
                    BlobClient blobClient = container.GetBlobClient(path);
                    if (await blobClient.ExistsAsync())
                        return blobClient;
                }
                return null;
            }
            catch (Exception e)
            {

                throw;
            }   
        }

        public async Task CreateFolderIfNotExist(string path)
        {
            var container = new BlobContainerClient(Configuration["Azure:FileShareConnection"], Configuration["Azure:ShareName"]);
            if (await container.ExistsAsync())
            {
                var ppp = path.TrimStart('/');
                BlobClient blobClient2 = container.GetBlobClient(ppp);
                if (!await blobClient2.ExistsAsync())
                {
                    using (var ms = new MemoryStream())
                    {
                        await blobClient2.UploadAsync(ms, true);
                    }
                }

            }
        }
        public async Task<bool> Upload(string path, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            var container = new BlobContainerClient(Configuration["Azure:FileShareConnection"], Configuration["Azure:ShareName"]);
            if (await container.ExistsAsync())
            {
                try
                {
                    var result = await container.UploadBlobAsync(path.TrimStart('/'), stream);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> UploadAndOverWrite(string path, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            var container = new BlobContainerClient(Configuration["Azure:FileShareConnection"], Configuration["Azure:ShareName"]);


            if (await container.ExistsAsync())
            {
                try
                {
                    //var result = await container.UploadBlobAsync(path, stream);
                    BlobServiceClient blobServiceClient = new BlobServiceClient(Configuration["Azure:FileShareConnection"]);

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container.Name);
                    BlobClient blobClient = containerClient.GetBlobClient(path);

                    blobClient.Upload(stream, overwrite: true);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<byte[]> ReadIFormFileAsync(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task RemoveFile(string path)
        {
            var container = new BlobContainerClient(Configuration["Azure:FileShareConnection"], Configuration["Azure:ShareName"]);
            if (await container.ExistsAsync())
            {
                var ppp = path.TrimStart('/');
                BlobClient blobClient = container.GetBlobClient(ppp + "/");
                if (await blobClient.ExistsAsync())
                {
                    using (var ms = new MemoryStream())
                    {
                        await blobClient.DeleteIfExistsAsync();
                    }
                }
            }
        }
    }
}

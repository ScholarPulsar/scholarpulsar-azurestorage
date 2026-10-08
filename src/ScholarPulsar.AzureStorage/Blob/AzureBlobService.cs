using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScholarPulsar.AzureStorage.Blob
{
    public class AzureBlobService
    {
        private string storageConnectionString;
        public AzureBlobService(string storageConnectionString)
        {
            this.storageConnectionString = storageConnectionString;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public bool FileExist(string filePath)
        {
            return FileExist(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public bool FileExist(string containerName, string fileName)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            return blobClient.Exists();
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public async Task<bool> FileExistAsync(string filePath)
        {
            return await FileExistAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<bool> FileExistAsync(string containerName, string fileName)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            return await blobClient.ExistsAsync();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public virtual string UploadFile(string containerName, string fileName, Stream fileStream)
        {
            return UploadFile(containerName, fileName, fileStream, new BlobUploadParameters()
            {
                ContainerAccessType = PublicAccessType.None,
                Overwrite = true
            });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="byteArray">文件数据</param>
        /// <returns></returns>
        public virtual string UploadFile(string containerName, string fileName, byte[] byteArray)
        {
            return UploadFile(containerName, fileName, byteArray, new BlobUploadParameters()
            {
                ContainerAccessType = PublicAccessType.None,
                Overwrite = true
            });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public string UploadFile(string containerName, string fileName, Stream fileStream, BlobUploadParameters options)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
            if (!blobContainerClient.Exists())
            {
                blobContainerClient.Create(options.ContainerAccessType);
            }

            BlobClient blob = blobContainerClient.GetBlobClient(fileName);
            if (options.Overwrite || !blob.Exists())
            {

                blob.Upload(fileStream, options.Overwrite);
                if (string.IsNullOrEmpty(options.ContentType))
                {
                    options.ContentType = MimeMapping.MimeUtility.GetMimeMapping(fileName);
                }

                BlobProperties properties = blob.GetProperties();
                BlobHttpHeaders headers = new BlobHttpHeaders
                {
                    // Set the MIME ContentType every time the properties 
                    // are updated or the field will be cleared
                    ContentType = options.ContentType,

                    // Populate remaining headers with 
                    // the pre-existing properties
                    CacheControl = properties.CacheControl,
                    ContentDisposition = properties.ContentDisposition,
                    ContentEncoding = properties.ContentEncoding,
                    ContentHash = properties.ContentHash
                };
                blob.SetHttpHeaders(headers);
            }



            return blob.Uri.ToString();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="byteArray">文件数据</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public string UploadFile(string containerName, string fileName, byte[] byteArray, BlobUploadParameters options)
        {
            using (var stream = new MemoryStream(byteArray))
            {
                return UploadFile(containerName, fileName, stream, options);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public string UploadFile(string filePath, Stream fileStream)
        {
            return UploadFile(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), fileStream);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public string UploadFile(string filePath, Stream fileStream, BlobUploadParameters options)
        {
            return UploadFile(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), fileStream, options);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="byteArray">文件数据</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public string UploadFile(string filePath, byte[] byteArray, BlobUploadParameters options)
        {
            return UploadFile(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), byteArray, options);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="byteArray">文件数据</param>
        /// <returns></returns>
        public string UploadFile(string filePath, byte[] byteArray)
        {
            return UploadFile(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), byteArray);
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public virtual async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            return await UploadFileAsync(containerName, fileName, fileStream, new BlobUploadParameters()
            {
                ContainerAccessType = PublicAccessType.None,
                Overwrite = true
            });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="byteArray">文件数据</param>
        /// <returns></returns>
        public virtual async Task<string> UploadFileAsync(string containerName, string fileName, byte[] byteArray)
        {
            return await UploadFileAsync(containerName, fileName, byteArray, new BlobUploadParameters()
            {
                ContainerAccessType = PublicAccessType.None,
                Overwrite = true
            });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream, BlobUploadParameters options)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
            if (!await blobContainerClient.ExistsAsync())
            {
                await blobContainerClient.CreateAsync(options.ContainerAccessType);
            }

            BlobClient blob = blobContainerClient.GetBlobClient(fileName);
            if (options.Overwrite || !await blob.ExistsAsync())
            {
                await blob.UploadAsync(fileStream, options.Overwrite);
                if (string.IsNullOrEmpty(options.ContentType))
                {
                    options.ContentType = MimeMapping.MimeUtility.GetMimeMapping(fileName);
                }

                BlobProperties properties = await blob.GetPropertiesAsync();
                BlobHttpHeaders headers = new BlobHttpHeaders
                {
                    // Set the MIME ContentType every time the properties 
                    // are updated or the field will be cleared
                    ContentType = options.ContentType,

                    // Populate remaining headers with 
                    // the pre-existing properties
                    CacheControl = properties.CacheControl,
                    ContentDisposition = properties.ContentDisposition,
                    ContentEncoding = properties.ContentEncoding,
                    ContentHash = properties.ContentHash
                };
                await blob.SetHttpHeadersAsync(headers);
            }



            return blob.Uri.ToString();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <param name="byteArray">文件数据</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(string containerName, string fileName, byte[] byteArray, BlobUploadParameters options)
        {
            using (var stream = new MemoryStream(byteArray))
            {
                return await UploadFileAsync(containerName, fileName, stream, options);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(string filePath, Stream fileStream)
        {
            return await UploadFileAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), fileStream);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(string filePath, Stream fileStream, BlobUploadParameters options)
        {
            return await UploadFileAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), fileStream, options);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="byteArray">文件数据</param>
        /// <param name="options">上传选项</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(string filePath, byte[] byteArray, BlobUploadParameters options)
        {
            return await UploadFileAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), byteArray, options);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="byteArray">文件数据</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(string filePath, byte[] byteArray)
        {
            return await UploadFileAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), byteArray);
        }

        /// <summary>
        /// 复制Blob
        /// </summary>
        /// <param name="sourceBlob">源Blob</param>
        /// <param name="newFileName">目的文件名</param>
        /// <returns></returns>
        public bool CopyBlob(BlobClient sourceBlob, string newFileName)
        {
            bool success = false;


            BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();

            // Specifying -1 for the lease interval creates an infinite lease.
            lease.Acquire(TimeSpan.FromSeconds(-1));

            var newBlobClient = new BlobClient(storageConnectionString, sourceBlob.BlobContainerName, newFileName);
            newBlobClient.StartCopyFromUri(sourceBlob.Uri);
            try
            {
                while (true)
                {
                    // Get the destination blob's properties and display the copy status.
                    BlobProperties destProperties = newBlobClient.GetProperties();
                    if (destProperties.CopyStatus != CopyStatus.Pending)
                    {
                        success = true;
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch { }
            finally
            {
                // Update the source blob's properties.
                BlobProperties sourceProperties = sourceBlob.GetProperties();

                if (sourceProperties.LeaseState == LeaseState.Leased)
                {
                    // Break the lease on the source blob.
                    lease.Break();
                }
            }
            return success;
        }

        /// <summary>
        /// 复制Blob
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">源文件名</param>
        /// <param name="newFileName">新文件名</param>
        /// <returns></returns>
        public bool CopyBlob(string containerName, string fileName, string newFileName)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, newFileName);
            return CopyBlob(blobClient, newFileName);
        }

        /// <summary>
        /// 复制Blob
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">源文件名</param>
        /// <param name="newFileName">新文件名</param>
        /// <returns></returns>
        public async Task<bool> CopyBlobAsync(string containerName, string fileName, string newFileName)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, newFileName);
            return await CopyBlobAsync(blobClient, newFileName);
        }

        /// <summary>
        /// 复制Blob
        /// </summary>
        /// <param name="sourceBlob">源Blob</param>
        /// <param name="newFileName">新文件名</param>
        /// <returns></returns>
        public async Task<bool> CopyBlobAsync(BlobClient sourceBlob, string newFileName)
        {
            bool success = false;


            BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();

            // Specifying -1 for the lease interval creates an infinite lease.
            await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

            var newBlobClient = new BlobClient(storageConnectionString, sourceBlob.BlobContainerName, newFileName);
            await newBlobClient.StartCopyFromUriAsync(sourceBlob.Uri);
            try
            {
                while (true)
                {
                    // Get the destination blob's properties and display the copy status.
                    BlobProperties destProperties = await newBlobClient.GetPropertiesAsync();
                    if (destProperties.CopyStatus != CopyStatus.Pending)
                    {
                        success = true;
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch { }
            finally
            {
                // Update the source blob's properties.
                BlobProperties sourceProperties = await sourceBlob.GetPropertiesAsync();

                if (sourceProperties.LeaseState == LeaseState.Leased)
                {
                    // Break the lease on the source blob.
                    await lease.BreakAsync();
                }
            }
            return success;
        }

        /// <summary>
        /// 获取容器里面所有文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <returns></returns>
        public List<string> GetFiles(string containerName)
        {
            List<string> files = new List<string>();

            BlobContainerClient container = new BlobContainerClient(storageConnectionString, containerName);

            if (container.Exists())
            {
                foreach (BlobItem blob in container.GetBlobs())
                {
                    if (blob.Properties.BlobType == BlobType.Block)
                    {
                        files.Add(Flurl.Url.Combine(
                        container.Uri.AbsoluteUri,
                        blob.Name));
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// 获取容器里面所有文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <returns></returns>
        public async Task<List<string>> GetFilesAsync(string containerName)
        {
            List<string> files = new List<string>();

            BlobContainerClient container = new BlobContainerClient(storageConnectionString, containerName);

            if (await container.ExistsAsync())
            {
                var enumerator = container.GetBlobsAsync().GetAsyncEnumerator();

                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        BlobItem blob = enumerator.Current;

                        if (blob.Properties.BlobType == BlobType.Block)
                        {
                            files.Add(Flurl.Url.Combine(
                                container.Uri.AbsoluteUri,
                                blob.Name));
                        }
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }
            }
            return files;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        public void DeleteFile(string containerName, string fileName)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            blobClient.DeleteIfExists();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">路径</param>
        public void DeleteFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                DeleteFile(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            }
        }

        /// <summary>
        /// 删除容器内所有文件
        /// </summary>
        /// <param name="containerName">容器</param>
        public void DeleteFiles(string containerName)
        {
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, containerName);
            container.DeleteIfExists();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        public async Task DeleteFileAsync(string containerName, string fileName)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public bool DeleteFiles(IEnumerable<string> filePaths)
        {
            var service = new BlobServiceClient(storageConnectionString);
            var batch = service.GetBlobBatchClient();

            var uris = new List<Uri>();
            foreach (var file in filePaths)
            {
                var blobClient = new BlobClient(storageConnectionString, Path.GetDirectoryName(file), Path.GetFileName(file));
                if (blobClient.Exists())
                { 
                    uris.Add(blobClient.Uri);
                }
            }
            var result = false;
            if (uris.Count > 0)
            {
                try
                {
                    batch.DeleteBlobs(uris, DeleteSnapshotsOption.IncludeSnapshots);
                    result = true;
                }
                catch (AggregateException)
                { 
                    result = false;
                }
            }
            return result;
            
        }

        public async Task<bool> DeleteFilesAsync(IEnumerable<string> filePaths)
        {
            var service = new BlobServiceClient(storageConnectionString);
            var batch = service.GetBlobBatchClient();

            var uris = new List<Uri>();
            foreach (var file in filePaths)
            {
                var blobClient = new BlobClient(storageConnectionString, Path.GetDirectoryName(file), Path.GetFileName(file));
                if (await blobClient.ExistsAsync())
                {
                    uris.Add(blobClient.Uri);
                }
            }
            var result = false;
            if (uris.Count > 0)
            {
                try
                {
                    await batch.DeleteBlobsAsync(uris, DeleteSnapshotsOption.IncludeSnapshots);
                    result = true;
                }
                catch (AggregateException)
                {
                   
                }
            }
            return result;

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">路径</param>
        public async Task DeleteFileAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                await DeleteFileAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            }
        }

        /// <summary>
        /// 删除容器内所有文件
        /// </summary>
        /// <param name="containerName">容器</param>
        public async Task DeleteFilesAsync(string containerName)
        {
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, containerName);
            await container.DeleteIfExistsAsync();
        }

        /// <summary>
        /// 下载文件到文件流
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileStream">文件流</param>
        public void DownloadFileToStream(string containerName, string fileName, Stream fileStream)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            if (blobClient.Exists())
            {
                blobClient.DownloadTo(fileStream);
            }
        }

        /// <summary>
        /// 下载文件到文件流
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileStream">文件流</param>
        public void DownloadFileToStream(string filePath, Stream fileStream)
        {
            DownloadFileToStream(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), fileStream);
        }

        /// <summary>
        /// 下载到文件到字节数组
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public byte[] DownloadFileToByteArray(string containerName, string fileName)
        {
            using (var stream = new MemoryStream())
            {
                DownloadFileToStream(containerName, fileName, stream);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return stream.ToArray();

            }

        }

        /// <summary>
        /// 下载到文件到字节数组
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public byte[] DownloadFileToByteArray(string filePath)
        {
            return DownloadFileToByteArray(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));

        }

        /// <summary>
        /// 下载文件到文件流
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileStream">文件流</param>
        public async Task DownloadFileToStreamAsync(string containerName, string fileName, Stream fileStream)
        {
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            if (await blobClient.ExistsAsync())
            {
                await blobClient.DownloadToAsync(fileStream);
            }
        }

        /// <summary>
        /// 下载文件到文件流
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileStream">文件流</param>
        public async Task DownloadFileToStreamAsync(string filePath, Stream fileStream)
        {
            await DownloadFileToStreamAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), fileStream);
        }

        /// <summary>
        /// 下载到文件到字节数组
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFileToByteArrayAsync(string containerName, string fileName)
        {
            using (var stream = new MemoryStream())
            {
                await DownloadFileToStreamAsync(containerName, fileName, stream);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return stream.ToArray();

            }

        }

        /// <summary>
        /// 下载到文件到字节数组
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFileToByteArrayAsync(string filePath)
        {
            return await DownloadFileToByteArrayAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));

        }

        /// <summary>
        /// 获取文件链接
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public string GetFileUri(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return GetFileUri(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取文件链接
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public string GetFileUri(string containerName, string fileName)
        {
            var fileUri = string.Empty;
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            if (blobClient.Exists())
            {
                var accessPolicy = blobClient.GetParentBlobContainerClient().GetAccessPolicy().Value;
                if (accessPolicy.BlobPublicAccess == PublicAccessType.Blob || accessPolicy.BlobPublicAccess == PublicAccessType.BlobContainer)
                {
                    fileUri = blobClient.Uri.AbsoluteUri;
                }
                else
                {
                    fileUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(5)).AbsoluteUri;
                }
            }
            return fileUri;
        }

        /// <summary>
        /// 获取文件链接
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public async Task<string> GetFileUriAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return await GetFileUriAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取文件链接
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<string> GetFileUriAsync(string containerName, string fileName)
        {
            var fileUri = string.Empty;
            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            if (await blobClient.ExistsAsync())
            {
                var accessPolicy = (await blobClient.GetParentBlobContainerClient().GetAccessPolicyAsync()).Value;
                if (accessPolicy.BlobPublicAccess == PublicAccessType.Blob || accessPolicy.BlobPublicAccess == PublicAccessType.BlobContainer)
                {
                    fileUri = blobClient.Uri.AbsoluteUri;
                }
                else
                {
                    fileUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(5)).AbsoluteUri;
                }
            }
            return fileUri;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public long GetFileSize(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return GetFileSize(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public long GetFileSize(string containerName, string fileName)
        {
            long fileSzie = 0;

            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            if (blobClient.Exists())
            {
                BlobProperties blobProperties = blobClient.GetProperties();
                fileSzie = blobProperties.ContentLength;
            }
            return fileSzie;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public async Task<long> GetFileSizeAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return await GetFileSizeAsync(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="containerName">容器</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<long> GetFileSizeAsync(string containerName, string fileName)
        {
            long fileSzie = 0;

            var blobClient = new BlobClient(storageConnectionString, containerName, fileName);
            if (await blobClient.ExistsAsync())
            {
                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                fileSzie = blobProperties.ContentLength;
            }
            return fileSzie;
        }

    }
}

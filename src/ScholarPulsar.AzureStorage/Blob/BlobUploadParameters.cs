using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScholarPulsar.AzureStorage.Blob
{
    /// <summary>
    /// Blob上传选项
    /// </summary>
    public class BlobUploadParameters
    {
        /// <summary>
        /// 权限
        /// </summary>
        public PublicAccessType ContainerAccessType { get; set; }

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Content类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 访问类型
        /// </summary>
        public AccessTier? AccessTier { get; set; }
    }
}

using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScholarPulsar.AzureStorage.Table
{
    public class AzureTableService
    {
        private string storageConnectionString;
        private TableServiceClient serviceClient;
        public AzureTableService(string storageConnectionString)
        {
            this.storageConnectionString = storageConnectionString;
            //serviceClient = new TableServiceClient(storageConnectionString);
        }

        /// <summary>
        /// 插入Table实体
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        public void InsertTableEntity(string tableName, ITableEntity entity)
        {

            var client = new TableClient(storageConnectionString, tableName);
            client.CreateIfNotExists();
            client.AddEntity(entity);
        }

        /// <summary>
        /// 插入Table实体
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        public async Task InsertTableEntityAsync(string tableName, ITableEntity entity)
        {
            var client = new TableClient(storageConnectionString, tableName);
            await client.CreateIfNotExistsAsync();
            await client.AddEntityAsync(entity);
        }

        public async Task<T> GetTableEntity<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity
        {
            var client = new TableClient(storageConnectionString, tableName);
            var response = await client.GetEntityAsync<T>(partitionKey, rowKey);
            return response.Value;
        }

        public void InsertTableEntity(AzureTableModel table)
        {

            var client = new TableClient(storageConnectionString, table.TableName);
            client.CreateIfNotExists();
            var entity = new TableEntity(table.Data);
            entity.PartitionKey = table.PartitionKey;
            entity.RowKey = table.RowKey;
            client.AddEntity(entity);
        }

        public async Task InsertTableEntityAsync(AzureTableModel table)
        {
            var client = new TableClient(storageConnectionString, table.TableName);
            await client.CreateIfNotExistsAsync();
            var entity = new TableEntity(table.Data);
            entity.PartitionKey = table.PartitionKey;
            entity.RowKey = table.RowKey;
            await client.AddEntityAsync(entity);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScholarPulsar.AzureStorage.Table
{
    public class AzureTableModel
    {
        public AzureTableModel() { }

        public AzureTableModel(string tableName, string partitionKey, string rowKey) 
        { 
            this.TableName = tableName;
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
        public string TableName { get; set; }
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}

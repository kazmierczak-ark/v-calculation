using VCalculateApi.Interfaces;
using InfluxDB.Client;
using System;
using System.Threading.Tasks;

namespace VCalculateApi.Services
{
    public class StorageClient : IStorageClient
    {
        const string bucket = "v_timeseries";
        const string retentionPolicy = "autogen";

        private readonly string host = "localhost";
        private readonly int port = 8086; 

        public StorageClient()
        {
            host = Environment.GetEnvironmentVariable("DB_HOST")==null ? host : Environment.GetEnvironmentVariable("DB_HOST");
            port = Environment.GetEnvironmentVariable("DB_PORT")==null ? port : Convert.ToInt32(Environment.GetEnvironmentVariable("DB_PORT"));            
        }

        public async Task<(float sum, float avg)> RetrieveData(string name, int? since, int? to)
        {
            using (var client = InfluxDBClientFactory.CreateV1($"http://{host}:{port}", null, null, bucket, retentionPolicy))
            {
                var query = $"from(bucket: \"{bucket}/{retentionPolicy}\")";
                query = string.Concat(query, $" |> range(start: time(v: {(since != null ? toNanoSecond(since.Value) : 0)}))");
                if (to == null)
                {
                    query = string.Concat(query, $" |> range(start: time(v: {(since != null ? toNanoSecond(since.Value) : 0)}))");                
                }
                else
                {
                    query = string.Concat(query, $" |> range(start: time(v: {(since != null ? toNanoSecond(since.Value) : 0)}), stop: time(v: {toNanoSecond(to.Value)+1}))");
                }
                query = string.Concat(query, $" |> filter(fn: (r) => r._measurement == \"{name}\")");
                var querySum = string.Concat(query, $" |> sum()");
                var queryAvg = string.Concat(query, $" |> mean()");

                var dataSum = await client.GetQueryApi().QueryAsync(querySum);  
                var dataAvg = await client.GetQueryApi().QueryAsync(queryAvg);

                if (dataSum.Count > 0 && dataAvg.Count > 0)
                {
                    return (sum: Convert.ToSingle(dataSum[0].Records[0].GetValue()), avg: Convert.ToSingle(dataAvg[0].Records[0].GetValue())); 
                }
                else 
                {
                    return (sum: 0, avg: 0); 
                }    
            }                
        }

        private long toNanoSecond(int time) => ((long)time)*1000000000;
    }
}
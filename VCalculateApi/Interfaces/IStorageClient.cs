using System.Threading.Tasks;

namespace VCalculateApi.Interfaces
{
    public interface IStorageClient
    {
        Task<(int sum, float avg)> RetrieveData(string name, int? since, int? to);
    }
}
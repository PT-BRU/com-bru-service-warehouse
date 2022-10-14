using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces
{
    public interface ICreateable
    {
        Task<int> Create(object model);
    }
}

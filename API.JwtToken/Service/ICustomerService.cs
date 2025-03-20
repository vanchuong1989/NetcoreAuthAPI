using API.JwtToken.Helper;
using API.JwtToken.Modal;
using API.JwtToken.Repos.Models;

namespace API.JwtToken.Service
{
    public interface ICustomerService
    {
        //List<TblCustomer> GetAll();
        Task<List<CustomerModal>> GetAll();
        Task<CustomerModal> GetByCode(string code);
        Task<APIResponse> Remove(string code);
        Task<APIResponse> Create(CustomerModal data);
        Task<APIResponse> Update(CustomerModal data, string code);
    }
}

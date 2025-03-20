using API.JwtToken.Helper;
using API.JwtToken.Modal;
using API.JwtToken.Repos;
using API.JwtToken.Repos.Models;
using API.JwtToken.Service;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.JwtToken.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly dbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;

        public CustomerService(dbContext dbContext, IMapper mapper, ILogger<CustomerService> logger)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<APIResponse> Create(CustomerModal data)
        {
            APIResponse response = new APIResponse();
            try
            {
                this.logger.LogInformation("Create Begins ...");
                TblCustomer _customer = this.mapper.Map<CustomerModal, TblCustomer>(data);
                await this.dbContext.TblCustomers.AddAsync(_customer);
                await this.dbContext.SaveChangesAsync();
                response.ResponseCode = 201; //data was created
                response.Result = data.Code;

            }
            catch (Exception ex)
            {

                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
                this.logger.LogError(ex.Message, ex);
            }

            return response;
        }

        public async Task<List<CustomerModal>> GetAll()
        {
            List<CustomerModal> _response = new List<CustomerModal>();
            var _data = await this.dbContext.TblCustomers.ToListAsync();
            if (_data!=null)
            {
                _response = this.mapper.Map<List<TblCustomer>, List<CustomerModal>>(_data);
            }
            return _response;
        }

        public  async Task<CustomerModal> GetByCode(string code)
        {
            CustomerModal _response = new CustomerModal();
            var _customer = await this.dbContext.TblCustomers.FindAsync(code);
            if (_customer != null)
            {
                _response = this.mapper.Map<TblCustomer, CustomerModal>(_customer);
            }
   

            return _response;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
               
                var _customer = await this.dbContext.TblCustomers.FindAsync(code);
                if (_customer !=null)
                {
                    this.dbContext.TblCustomers.Remove(_customer);
                    await this.dbContext.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Data not found";
                }

            }
            catch (Exception ex)
            {

                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<APIResponse> Update(CustomerModal data, string code)
        {
            APIResponse response = new APIResponse();
            try
            {

                var _customer = await this.dbContext.TblCustomers.FindAsync(code);
                if (_customer != null)
                {
                    _customer.Name = data.Name;
                    _customer.Email = data.Email;
                    _customer.Phone = data.Phone;
                    _customer.IsActive = data.IsActive;
                    _customer.Creditlimit = data.Creditlimit;
                    await this.dbContext.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Data not found";
                }

            }
            catch (Exception ex)
            {

                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        
    }
}

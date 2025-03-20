using API.JwtToken.Modal;
using API.JwtToken.Repos.Models;
using AutoMapper;

namespace API.JwtToken.Helper
{
    public class AutoMapperHandler:Profile
    {
        public AutoMapperHandler()
        {
            //create a Mapper
            CreateMap<TblCustomer, CustomerModal>()
                .ForMember(item=>item.StatusName, 
                            opt=>opt.MapFrom(item => (item.IsActive==true && item.IsActive.Value) ? "Active":"InActive")).ReverseMap();
        }
    }
}

using AutoMapper;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model;

namespace WebBanHang.Infrastructre.Models
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration() 
        {
            CreateMap<UserRegisterModel, User>().AfterMap((src, dest) => 
            { 
                dest.CreateAt = DateTime.Now;
                dest.UserName = src.Email.Split("@")[0];
            });
            CreateMap<CreateRequest, User>().AfterMap((src, dest) =>
            {
                dest.CreateAt = DateTime.Now;
                dest.UserName = src.Email.Split("@")[0];
                dest.RoleID = src.Role switch
                {
                    "Admin" => 1,
                    "Employee" => 2,
                    "Customer" => 3,
                    _ => throw new ArgumentOutOfRangeException("None of role is match with system role"),
                };
            });
            CreateMap<User, AccountRespone>().ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.RoleID.ToString())).AfterMap((src, dest) =>
                {
                    if (dest.Role == "1") dest.Role = "Admin";
                    else if (dest.Role == "2") dest.Role = "Employee";
                    else dest.Role = "Customer";
                });
        }
    }
}

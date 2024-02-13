using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Infrastructre.Models
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration() 
        {
            CreateMap<UserRegisterModel, Customer>().AfterMap((src, dest) => dest.CreateAt = DateTime.Now);
        }
    }
}

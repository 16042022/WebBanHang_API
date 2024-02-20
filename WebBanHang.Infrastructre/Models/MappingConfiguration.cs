using AutoMapper;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model.Account;
using WebBanHang.Domain.Model.Cart;

namespace WebBanHang.Infrastructre.Models
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration() 
        {
            CreateMap<UserRegisterModel, Users>().AfterMap((src, dest) => 
            { 
                dest.CreateAt = DateTime.Now;
                dest.UserName = src.Email.Split("@")[0];
                dest.Status = "Active";
            });
            CreateMap<CreateRequest, Users>().AfterMap((src, dest) =>
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
                dest.Status = "Active";
            });
            CreateMap<EditAccountRequest, Users>().AfterMap((src, dest) =>
            {
                dest.UpdateAt = DateTime.Now;
                dest.UserName = src.UserName ??= dest.UserName;
                dest.FirstName = src.FirstName ??= dest.FirstName;
                dest.LastName = src.LastName??= dest.LastName;
                dest.Password = src.Password ??= dest.Password;
                dest.Email = src.Email ??= dest.Email;
                dest.RoleID = src.Role switch
                {
                    "Admin" => 1,
                    "Employee" => 2,
                    "Customer" => 3,
                    _ => dest.RoleID,
                };
                dest.Status = "Modified";
            });
            CreateMap<Users, AccountRespone>().ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.RoleID.ToString())).AfterMap((src, dest) =>
                {
                    if (dest.Role == "1") dest.Role = "Admin";
                    else if (dest.Role == "2") dest.Role = "Employee";
                    else dest.Role = "Customer";
                });
            CreateMap<Product, ProductDtos>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDescription))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock));
            CreateMap<AddingProductRequest, Product>().AfterMap((src, dest) =>
            {
                dest.ProductName = src.Name;
                dest.ProductDescription = src.Description;
                dest.Price = src.Price; dest.Discount = src.Discount;
                dest.Stock = src.Stock;
                dest.StatusID = src.Status switch
                {
                    "Available" => 1,
                    "Out_of_stock" => 2,
                    _ => throw new ArgumentOutOfRangeException("Out of range product status")
                };
                dest.CategoryID = src.Category switch
                {
                    "Dry_food" => 1,
                    "Watery_food" => 2,
                    "Refined_food" => 3,
                    "Processed_food" => 4,
                    "Freeze_food" => 5,
                    _=> throw new ArgumentOutOfRangeException("Out of range product category")
                };
            });

        }
    }
}

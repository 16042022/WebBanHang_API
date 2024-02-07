using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Users_Admin;
using WebBanHang.Infrastructre.Models;
using WebBanHang.Infrastructre.Security;

namespace WebBanHang.Infrastructre.User_Admin
{
    public class CustomerInfor : IRepository<Customer>
    {
        //private DataSet customerSet;
        //private DataRow UpdateRow;
        private AppDbContext dbContext;
        private Customer? tempCus;
        private int batch = 0;

        private static bool IsValidValue(Customer customer)
        {
            ValidationContext context = new(customer);
            List<ValidationResult> results = new();
            bool Valid = Validator.TryValidateObject(customer, context, results);
            return Valid;
        }

        public CustomerInfor(AppDbContext context)
        {
            // customerSet = new DataSet();
            // UpdateRow = customerSet.Tables[0].NewRow();
            dbContext = context;
        }

        public async Task Add(Customer entity)
        {
            if (entity != null & IsValidValue(entity!))
            {
                if (!dbContext.Customers.Any(x => x.Email == entity!.Email))
                {
                    try
                    {
                        entity!.Password = PasswordManagement.HashPassword(entity.Password);
                        dbContext.Add(entity!);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new AggregateException(ex);
                    }
                }
                else throw new InvalidOperationException("This user have same email as stored user, check again");
            }
            else throw new InvalidDataException("Input is not valid");
        }

        public async Task AddRange(IEnumerable<Customer> entities)
        {
            int count = entities.Count();
            if (count == 0) throw new InvalidDataException("List entity have no element");
            else if (count == 1) await Add(entities.ToList()[0]);
            else
            {
                foreach (Customer entity in entities)
                {
                    if (entity != null && IsValidValue(entity)) 
                    {
                        if (!dbContext.Customers.Any(x => x.Email == entity!.Email))
                        {
                            try
                            {
                                entity.Password = PasswordManagement.HashPassword(entity!.Password);
                                dbContext.Customers.Add(entity);
                                ++batch;
                                if (batch == count)
                                {
                                    await dbContext.SaveChangesAsync();
                                    batch = 0;
                                }
                            } catch (Exception ex)
                            {
                                throw new AggregateException(ex);
                            }
                        }
                        else throw new InvalidOperationException("This customer have same email as stored customer, check again");
                    }
                    else throw new InvalidDataException("Input is not valid");
                }
            }
        }

        public async Task Delete(int id)
        {
            await dbContext.Customers.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task DeleteRange(IEnumerable<Customer> entities)
        {
            foreach (Customer entity in entities)
            {
                await Delete(entity.Id);
            }
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetById(int id)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task<bool> IsPassChangedIllegal(Customer customer)
        {
            // Retrive the equivalent customer:
            Customer checkCus = await GetByName(customer.Email);
            tempCus = checkCus;
            if (customer.Password == null) return true; // Trong cap nhat ko co thong tin Password
            else return PasswordManagement.IsValidPassword(customer.Password, checkCus.Password);
        }

        public async Task Update(Customer entity)
        {
            // This method won't update user's password => Perform a pwd check before update
            if (entity != null && await IsPassChangedIllegal(entity))
            {
                // Truy vet ra Id cua kh
                int cusID = tempCus!.Id;
                entity.Id = cusID;
                dbContext.Customers.Update(entity);
                await dbContext.SaveChangesAsync();
            }
            else throw new InvalidDataException("Is this customer empty or their password is illegal changed?");
        }

        public async Task UpdateRange(IEnumerable<Customer> entities)
        {
            int count = entities.Count();
            foreach (Customer entity in entities)
            {
                if (entity != null && !await IsPassChangedIllegal(entity))
                {
                    // Truy vet ra Id cua kh
                    int cusID = tempCus!.Id;
                    entity.Id = cusID;
                    dbContext.Customers.Update(entity);
                    if (batch == count)
                    {
                        await dbContext.SaveChangesAsync();
                        batch = 0;
                    }
                    ++batch;
                }
                else
                {
                    batch = -1; break;
                }
            }
            if (batch < 0) throw new InvalidDataException("Is this customer empty or their password is illegal changed?");
        }

        // Tim theo email
        public async Task<Customer> GetByName(string email)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain;
using WebBanHang.Domain.Entities;
using WebBanHang.Infrastructre.Models;
using WebBanHang.Infrastructre.Security;

namespace WebBanHang.Infrastructre.User_Admin
{
    public class UserInfor : IRepository<User>
    {
        private AppDbContext dbContext;
        private int batch = 0;
        private User? tempCus;

        private static bool IsValidValue(User acc)
        {
            ValidationContext context = new(acc);
            List<ValidationResult> results = new();
            bool Valid = Validator.TryValidateObject(acc, context, results);
            return Valid;
        }

        public UserInfor(AppDbContext connect)
        {
            this.dbContext = connect;
        }

        public async Task Add(User entity)
        {
            if (entity != null && IsValidValue(entity))
            {
                try
                {
                    entity.Password = PasswordManagement.HashPassword(entity.Password);
                    dbContext.user.Add(entity);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new AggregateException(ex);
                }
            }
            else throw new InvalidDataException("Invalid input");
        }

        public async Task AddRange(IEnumerable<User> entities)
        {
            int count = entities.Count();
            if (count == 0) throw new InvalidDataException("List entity have no element");
            else if (count == 1) await Add(entities.ToList()[0]);
            else
            {
                foreach (User entity in entities)
                {
                    if (entity != null && IsValidValue(entity))
                    {
                        try
                        {
                            entity.Password = PasswordManagement.HashPassword(entity!.Password);
                            dbContext.user.Add(entity);
                            ++batch;
                            if (batch == count)
                            {
                                await dbContext.SaveChangesAsync();
                                batch = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new AggregateException(ex);
                        }
                    }
                    else throw new InvalidDataException("Input is not valid");
                }
            }
        }

        public async Task Delete(int id)
        {
            await dbContext.user.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task DeleteRange(IEnumerable<User> entities)
        {
            foreach (User entity in entities)
            {
                await Delete(entity.Id);
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await dbContext.user.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await dbContext.user.FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task<bool> IsPassChangedIllegal(User customer)
        {
            // Retrive the equivalent customer:
            User checkCus = await GetByName(customer.UserName);
            tempCus = checkCus;
            if (customer.Password == null) return true;
            else return PasswordManagement.IsValidPassword(customer.Password, checkCus.Password);
        }

        public async Task Update(User entity)
        {
            // This method won't update user's password => Perform a pwd check before update
            if (entity != null && await IsPassChangedIllegal(entity))
            {
                // Truy vet ra Id cua kh
                int cusID = tempCus!.Id;
                entity.Id = cusID;
                dbContext.user.Update(entity);
                await dbContext.SaveChangesAsync();
            }
            else throw new InvalidDataException("Is this user empty or their password is illegal changed?");
        }

        public async Task UpdateRange(IEnumerable<User> entities)
        {
            int count = entities.Count();
            foreach (User entity in entities)
            {
                if (entity != null && !await IsPassChangedIllegal(entity))
                {
                    // Truy vet ra Id cua kh
                    int cusID = tempCus!.Id;
                    entity.Id = cusID;
                    dbContext.user.Update(entity);
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
            if (batch < 0) throw new InvalidDataException("Is this user empty or their password is illegal changed?");
        }

        public async Task<User> GetByName(string name)
        {
            return await dbContext.user.FirstOrDefaultAsync(x => x.UserName == name);
        }
    }
}

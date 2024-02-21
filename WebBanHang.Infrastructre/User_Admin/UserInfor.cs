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
    public class UserInfor : IRepository<Users>
    {
        private AppDbContext dbContext;
        private DbSet<Users>? _entity;
        private int batch = 0;

        private static bool IsValidValue(Users acc)
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

        public async Task Add(Users entity)
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

        public async Task AddRange(IEnumerable<Users> entities)
        {
            int count = entities.Count();
            if (count == 0) throw new InvalidDataException("List entity have no element");
            else if (count == 1) await Add(entities.ToList()[0]);
            else
            {
                foreach (Users entity in entities)
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

        public async Task DeleteRange(IEnumerable<Users> entities)
        {
            foreach (Users entity in entities)
            {
                await Delete(entity.Id);
            }
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            return await dbContext.Set<Users>().ToListAsync();
        }

        public async Task<Users> GetById(int id)
        {
            return await dbContext.user.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Users entity)
        {
            if (entity != null)
            {
                dbContext.user.Update(entity);
                await dbContext.SaveChangesAsync();
            }
            else throw new AggregateException("No valid user to update");
        }

        public async Task UpdateRange(IEnumerable<Users> entities)
        {
            int count = entities.Count();
            foreach (Users entity in entities)
            {
                if (entity != null)
                {
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

        public async Task<Users> GetByName(string name)
        {
            return await dbContext.user.FirstOrDefaultAsync(x => x.UserName == name);
        }

        #region Helpers
        private DbSet<Users> Entities
        {
            get
            {
                _entity ??= dbContext.Set<Users>();

                return _entity;
            }
        }

        public DbSet<Users> DbSet()
        {
            return Entities;
        }
        #endregion
    }
}

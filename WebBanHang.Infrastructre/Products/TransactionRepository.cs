using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain;
using WebBanHang.Domain.Common;
using WebBanHang.Infrastructre.Models;

namespace WebBanHang.Infrastructre.Products
{
    public class TransactionRepository<T> : IRepository<T> where T : BaseEntity
    {
        private AppDbContext dbContext;
        private DbSet<T>? _entity;
        private int batch = 0;

        public TransactionRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual async Task Add(T entity)
        {
            Entities.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task AddRange(IEnumerable<T> entities)
        {
            int cnt = entities.Count();
            if (cnt == 1)
            {
                Entities.AddRange(entities);
                await dbContext.SaveChangesAsync();
            }
            foreach (T entity in entities)
            {
                Entities.Add(entity);
                if (batch == cnt)
                {
                    await dbContext.SaveChangesAsync();
                    batch = 0;
                }
                ++batch;
            }
        }

        public virtual async Task Delete(int id)
        {
            await Entities.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public virtual async Task DeleteRange(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                await Entities.Where(x => x.Id == entity.Id).ExecuteDeleteAsync();
            }
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.Id == id);
        }

        // This method must be overloading before using
        public virtual Task<T> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public virtual async Task Update(T entity)
        {
            Entities.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<T> entities)
        {
            int count = entities.Count();
            if (count == 1)
            {
                Entities.UpdateRange(entities);
                await dbContext.SaveChangesAsync();
            }
            foreach (T entity in entities)
            {
                Entities.Update(entity);
                if (batch == count)
                {
                    await dbContext.SaveChangesAsync();
                    batch = 0;
                }
                ++batch;
            }
        }

        #region Helpers
        private DbSet<T> Entities
        {
            get
            {
                _entity ??= dbContext.Set<T>();

                return _entity;
            }
        }

        public DbSet<T> DbSet()
        {
            return Entities;
        }
        #endregion
    }
}

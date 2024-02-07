using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain
{
    public interface IRepository<T> where T: class
    {
        Task<T> GetById(int id);
        Task<T> GetByName(string name);
        Task<IEnumerable<T>> GetAll();
        Task Add (T entity);
        Task Update (T entity);
        Task Delete (int id);
        Task AddRange (IEnumerable<T> entities);
        Task UpdateRange (IEnumerable<T> entities);
        Task DeleteRange (IEnumerable<T> entities);
    }
}

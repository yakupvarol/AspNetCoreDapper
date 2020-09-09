using AspNetCoreDapper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDapper.Data
{
    public interface IDapperUserDAL
    {
        Task<IEnumerable<QueryGetAllDTO>> GetAllAsync();
        Task<UserDTO> GetIDAsync(int ID);
        Task<IEnumerable<UserDTO>> FindAsync(UserListRequestDTO src);
        Task<QueryGetAllDTO> QueryOneToOneAsync(int UserId);
        Task<IEnumerable<GroupOneToOneDTO>> QueryManyToOneAsync(int Id);
        Task<UserGroupDTO> QueryOneToManyAsync(int Id);
        Task<IEnumerable<UserOneDTO>> QuerySlapperManyToOneAsync();
        Task<UserOneDTO> QuerySlapperOneToOneAsync(int Id);
        Task<UserGroupDTO> QuerySlapperOneToManyAsync(int Id);
        Task<IEnumerable<UserGroupDTO>> QuerySlapperManyToManyAsync();
        Task<long> CountAsync();
        Task<long> InsertAsync(UserDTO dt);
        Task<bool> InsertMultipleAsync(IList<UserDTO> dt);
        Task<bool> InsertOneToOneAsync(GroupOneToOneDTO dt);
        Task<bool> InsertOneToManyAsync(UserGroupDTO dt);
        Task<bool> InsertManyToManyAsync(IList<UserGroupDTO> dt);

        Task<bool> UpdateAsync(UserDTO dt);
        Task<bool> UpdateMultipleAsync(IList<UserDTO> dt);
        Task<bool> UpdateOneToOneAsync(GroupOneToOneDTO dt);
        Task<bool> UpdateOneToManyAsync(UserGroupDTO dt);
        Task<bool> UpdateToManyAsync(IList<UserGroupDTO> dt);

        Task<bool> DeleteAsync(UserDTO dt);
        Task<bool> DeleteMultipleAsync(IList<UserDTO> dt);
        Task<bool> DeleteOneToOneAsync(GroupOneToOneDTO dt);
        Task<bool> DeleteOneToManyAsync(UserGroupDTO dt);
        Task<bool> DeleteToManyAsync(IList<UserGroupDTO> dt);
    }
}

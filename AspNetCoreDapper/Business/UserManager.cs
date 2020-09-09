using AspNetCoreDapper.Data;
using AspNetCoreDapper.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreDapper.Business
{
    public class UserManager : IUserService
    {
        private IDapperUserDAL _dapperUserDAL;

        public UserManager(IDapperUserDAL dapperUserDAL)
        {
            _dapperUserDAL = dapperUserDAL;
        }

        public async Task<long> CountAsync()
        {
            return await _dapperUserDAL.CountAsync();
        }

        public async Task<UserDTO> GetIDAsync(int ID)
        {
            return await _dapperUserDAL.GetIDAsync(ID);
        }

        public async Task<IEnumerable<UserDTO>> FindAsync(UserListRequestDTO src)
        {
            return await _dapperUserDAL.FindAsync(src);
        }

        public async Task<long> InsertAsync(UserDTO dt)
        {
            return await _dapperUserDAL.InsertAsync(dt);
        }

        public async Task<bool> InsertMultipleAsync(IList<UserDTO> dt)
        {
            return await _dapperUserDAL.InsertMultipleAsync(dt);
        }

        public async Task<bool> InsertOneToOneAsync(GroupOneToOneDTO dt)
        {
            return await _dapperUserDAL.InsertOneToOneAsync(dt);
        }

        public async Task<bool> InsertOneToManyAsync(UserGroupDTO dt)
        {
            return await _dapperUserDAL.InsertOneToManyAsync(dt);
        }

        public async Task<bool> InsertManyToManyAsync(IList<UserGroupDTO> dt)
        {
            return await _dapperUserDAL.InsertManyToManyAsync(dt);
        }

        public async Task<IEnumerable<QueryGetAllDTO>> GetAllAsync()
        {
            return await _dapperUserDAL.GetAllAsync();
        }

        public async Task<QueryGetAllDTO> QueryOneToOneAsync(int UserId)
        {
            return await _dapperUserDAL.QueryOneToOneAsync(UserId);
        }

        public async Task<UserGroupDTO> QueryOneToManyAsync(int Id)
        {
            return await _dapperUserDAL.QueryOneToManyAsync(Id);
        }

        public async Task<IEnumerable<GroupOneToOneDTO>> QueryManyToOneAsync(int Id)
        {
            return await _dapperUserDAL.QueryManyToOneAsync(Id);
        }

        public async Task<IEnumerable<UserOneDTO>> QuerySlapperManyToOneAsync()
        {
            return await _dapperUserDAL.QuerySlapperManyToOneAsync();
        }

        public async Task<UserOneDTO> QuerySlapperOneToOneAsync(int Id)
        {
            return await _dapperUserDAL.QuerySlapperOneToOneAsync(Id);
        }

        public async Task<UserGroupDTO> QuerySlapperOneToManyAsync(int Id)
        {
            return await _dapperUserDAL.QuerySlapperOneToManyAsync(Id);
        }

        public async Task<IEnumerable<UserGroupDTO>> QuerySlapperManyToManyAsync()
        {
            return await _dapperUserDAL.QuerySlapperManyToManyAsync();
        }

        public async Task<bool> UpdateAsync(UserDTO dt)
        {
            return await _dapperUserDAL.UpdateAsync(dt);
        }

        public Task<bool> UpdateMultipleAsync(IList<UserDTO> dt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOneToOneAsync(GroupOneToOneDTO dt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOneToManyAsync(UserGroupDTO dt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateToManyAsync(IList<UserGroupDTO> dt)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(UserDTO dt)
        {
            return await _dapperUserDAL.DeleteAsync(dt);
        }

        public Task<bool> DeleteMultipleAsync(IList<UserDTO> dt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOneToOneAsync(GroupOneToOneDTO dt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOneToManyAsync(UserGroupDTO dt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteToManyAsync(IList<UserGroupDTO> dt)
        {
            throw new NotImplementedException();
        }
    }
}

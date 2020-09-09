using AspNetCoreDapper.Dapper;
using AspNetCoreDapper.DTO;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace AspNetCoreDapper.Data
{
    public class DapperUserDAL : IDapperUserDAL
    {
        private IDBConnection _connection;

        public DapperUserDAL(IDBConnection dbConnection)
        {
            _connection = dbConnection;
        }

        public async Task<Int64> CountAsync()
        {
            return await _connection.DB.ExecuteScalarAsync<Int64>("SELECT COUNT(ID) FROM USERS");
        }

        public async Task<IEnumerable<QueryGetAllDTO>> GetAllAsync()
        {
            //View
            //return await _connection.DB.QueryAsync<QueryGetAllDTO>("SELECT * FROM V_USERGROUP");
            return await _connection.DB.QueryAsync<QueryGetAllDTO>("SELECT ID,GROUPID,FIRST_NAME,LAST_NAME,PHONE,(SELECT NAME FROM USER_GROUPS WHERE ID=GROUPID) AS GROUPNAME FROM USERS");
        }

        public async Task<UserDTO> GetIDAsync(int ID)
        {
            // parameters 1
            var parameters1 = new { @UseriD = ID };

            // parameters 2
            var dictionary = new Dictionary<string, object>
            {
                { "@ID", ID }
            };
            var parameters2 = new DynamicParameters(dictionary);

            // parameters 3
            DynamicParameters parameters3 = new DynamicParameters();
            parameters3.Add("@ID", ID);

            return await _connection.DB.QueryFirstAsync<UserDTO>("SELECT ID  as USERiD, * FROM USERS WITH(NOLOCK) WHERE ID=@UseriD", parameters1);
        }

        public async Task<IEnumerable<UserDTO>> FindAsync(UserListRequestDTO src)
        {
            var SQL = "SELECT * FROM USERS WHERE ID>0 ";
            if (src.Groupid > 0)
            { SQL += " AND GROUPID=@Groupid"; }
            if (!string.IsNullOrEmpty(src.FirstName))
            { SQL += " AND FIRST_NAME=@FirstName"; }
            return await _connection.DB.QueryAsync<UserDTO>(SQL, src);
        }

        public async Task<QueryMulitpleOther> QueryMulitpleOther()
        {
            var SQL = @"SELECT * FROM USER_GROUPS; SELECT * FROM USERS";
            var results = _connection.DB.QueryMultiple(SQL);
            var userGroup = await results.ReadAsync<GroupOneToOneDTO>();
            var user = await results.ReadAsync<UserOneDTO>();

            return new QueryMulitpleOther { USER_GROUPS = userGroup, USERS = user };
        }

        public async Task<QueryGetAllDTO> QueryOneToOneAsync(int UserID)
        {
            //1
            var SQL = "SELECT R.ID AS GROUPiD ,R.NAME,E.GROUPID, E.FIRST_NAME FROM dbo.USER_GROUPS R INNER JOIN dbo.USERS E ON E.GROUPID = R.ID ORDER BY R.ID, E.GROUPID";
            var d = _connection.DB.Query<GroupOneToOneDTO, UserOneDTO, GroupOneToOneDTO>(SQL,
            map: (usergroup, user) =>
            {
                usergroup.USERS = user;
                return usergroup;
            },
            splitOn: "GROUPID");

            //2
            SQL = "SELECT USER_GROUPS.ID AS GROUPiD, USER_GROUPS.NAME AS GROUPNAME, USERS.ID, USERS.FIRST_NAME, USERS.LAST_NAME, USERS.PHONE FROM USER_GROUPS INNER JOIN USERS ON USER_GROUPS.ID = USERS.GROUPID WHERE USERS.ID = @UserID";
            return await _connection.DB.QueryFirstAsync<QueryGetAllDTO>(SQL, new { @UserID = UserID });
        }

        public async Task<UserGroupDTO> QueryOneToManyAsync(int Id)
        {
            string SQL = @"SELECT ID as GroupiD, NAME FROM USER_GROUPS WHERE ID=@id; SELECT ID as USERiD, * FROM USERS WHERE GROUPID=@id";
            var rs = await _connection.DB.QueryMultipleAsync(SQL, new { @id = Id });

            var userGroup = await rs.ReadSingleAsync<UserGroupDTO>();
            userGroup.USERS = await rs.ReadAsync<UserDTO>();
            return userGroup;
        }

        public async Task<IEnumerable<GroupOneToOneDTO>> QueryManyToOneAsync(int Id)
        {
            var SQL = "SELECT USER_GROUPS.ID, USER_GROUPS.NAME AS NAME, USERS.GROUPID, USERS.ID , USERS.USERTYPEID, USERS.EMAIL, USERS.FIRST_NAME AS FIRST_NAME, USERS.LAST_NAME FROM USER_GROUPS INNER JOIN USERS ON USER_GROUPS.ID = USERS.GROUPID WHERE USER_GROUPS.ID=@GroupId ";
            return await _connection.DB.QueryAsync<GroupOneToOneDTO, UserOneDTO, GroupOneToOneDTO>(SQL, (os, o) => { os.USERS = o; return os; }, splitOn: "GROUPID", param: new { @GroupId = Id });
        }

        public async Task<UserOneDTO> QuerySlapperOneToOneAsync(int Id)
        {
            var dt = await _connection.DB.QueryAsync<dynamic>(
                  "SELECT R.ID as USER_GROUPS_GROUPiD, R.NAME as USER_GROUPS_NAME,E.FIRST_NAME as FiRST_NAME,E.ID AS USERiD  " +
                  "FROM USER_GROUPS R " +
                  "INNER JOIN USERS E " +
                  "ON E.GROUPID = R.ID WHERE E.ID=@Id", new { @Id = Id });
            Slapper.AutoMapper.Configuration.AddIdentifier(typeof(UserOneDTO), "USERiD");
            return Slapper.AutoMapper.MapDynamic<UserOneDTO>(dt).FirstOrDefault();
        }

        public async Task<IEnumerable<UserOneDTO>> QuerySlapperManyToOneAsync()
        {
            var dt = await _connection.DB.QueryAsync<dynamic>(
                   "SELECT R.ID as USER_GROUPS_GROUPİD, R.NAME as USER_GROUPS_NAME,E.FIRST_NAME as FiRST_NAME ,E.ID AS USERiD  " +
                   "FROM USER_GROUPS R " +
                   "INNER JOIN USERS E " +
                   "ON E.GROUPID = R.ID");
            Slapper.AutoMapper.Configuration.AddIdentifier(typeof(UserOneDTO), "USERiD");
            return Slapper.AutoMapper.MapDynamic<UserOneDTO>(dt);
        }

        public async Task<UserGroupDTO> QuerySlapperOneToManyAsync(int id)
        {
            var dt = await _connection.DB.QueryAsync<dynamic>(
                  "SELECT R.ID as GROUPID, R.NAME,E.FIRST_NAME AS USERS_FiRST_NAME ,E.ID AS USERS_USERiD  " +
                  "FROM USER_GROUPS R " +
                  "INNER JOIN USERS E " +
                  "ON E.GROUPID = R.ID WHERE R.ID=@Id", new { @Id=id });
            Slapper.AutoMapper.Configuration.AddIdentifier(typeof(UserGroupDTO), "GROUPiD");
            return Slapper.AutoMapper.MapDynamic<UserGroupDTO>(dt).FirstOrDefault();
        }

        public async Task<IEnumerable<UserGroupDTO>> QuerySlapperManyToManyAsync()
        {
            var dt = await _connection.DB.QueryAsync<dynamic>(
                  "SELECT R.ID as GROUPiD, R.NAME,E.FIRST_NAME AS USERS_FiRST_NAME,E.ID AS USERS_USERID  " +
                  "FROM USER_GROUPS R " +
                  "INNER JOIN USERS E " +
                  "ON E.GROUPID = R.ID ");
            Slapper.AutoMapper.Configuration.AddIdentifier(typeof(UserGroupDTO), "GROUPID");
            return Slapper.AutoMapper.MapDynamic<UserGroupDTO>(dt).AsList();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<long> InsertAsync(UserDTO dt)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                var ID = await _connection.DB.ExecuteScalarAsync<long>(@"INSERT USERS(USERTYPEID,GROUPID,FIRST_NAME,LAST_NAME,EMAIL,PASSWORD,PHONE) values(@USERTYPEiD,@GROUPiD,@FiRST_NAME,@LAST_NAME,@EMAiL,@PASSWORD,@PHONE); select SCOPE_IDENTITY();", dt).ConfigureAwait(false);
                transactionScope.Complete();
                return ID;
            }
        }

        public async Task<bool> InsertMultipleAsync(IList<UserDTO> dt)
        {
            IDbTransaction transaction = null;
            bool result = false;
            try
            {
                transaction = _connection.DB.BeginTransaction();
                int rows = await _connection.DB.ExecuteAsync(@"INSERT USERS(USERTYPEID,GROUPID,FIRST_NAME,LAST_NAME,EMAIL,PASSWORD,PHONE) values(@USERTYPEiD,@GROUPiD,@FiRST_NAME,@LAST_NAME,@EMAiL,@PASSWORD,@PHONE); select SCOPE_IDENTITY();", dt, transaction);
                transaction.Commit();
                result = true;
            }
            catch
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> InsertOneToOneAsync(GroupOneToOneDTO dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    dt.USERS.GROUPiD = await _connection.DB.ExecuteScalarAsync<int>(@"INSERT USER_GROUPS(NAME) values(@NAME) select SCOPE_IDENTITY()", dt, transaction);
                    await _connection.DB.ExecuteAsync(@"INSERT USERS(USERTYPEID,GROUPID,FIRST_NAME,LAST_NAME,EMAIL,PASSWORD,PHONE) values(@USERTYPEiD,@GROUPiD,@FiRST_NAME,@LAST_NAME,@EMAiL,@PASSWORD,@PHONE); select SCOPE_IDENTITY();", dt.USERS, transaction);
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> InsertOneToManyAsync(UserGroupDTO dt)
        {
            bool result = false;
            try
            {
                var GROUPID = await _connection.DB.ExecuteScalarAsync<int>(@"INSERT USER_GROUPS(NAME) values(@NAME) select SCOPE_IDENTITY()", dt);
                foreach (var user in dt.USERS)
                {
                    user.GROUPiD = GROUPID;
                    await _connection.DB.ExecuteAsync(@"INSERT USERS(USERTYPEID,GROUPID,FIRST_NAME,LAST_NAME,EMAIL,PASSWORD,PHONE) values(@USERTYPEiD,@GROUPiD,@FiRST_NAME,@LAST_NAME,@EMAiL,@PASSWORD,@PHONE); select SCOPE_IDENTITY();", user);
                }
                result = true;
            }
            catch { }
            return result;
        }

        public async Task<bool> InsertManyToManyAsync(IList<UserGroupDTO> dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    foreach (var userGroup in dt)
                    {
                        var GROUPID = await _connection.DB.ExecuteScalarAsync<int>(@"INSERT USER_GROUPS(NAME) values(@NAME) select SCOPE_IDENTITY()", userGroup, transaction);
                        foreach (var user in userGroup.USERS)
                        {
                            user.GROUPiD = GROUPID;
                            await _connection.DB.ExecuteAsync(@"INSERT USERS(USERTYPEID,GROUPID,FIRST_NAME,LAST_NAME,EMAIL,PASSWORD,PHONE) values(@USERTYPEiD,@GROUPiD,@FiRST_NAME,@LAST_NAME,@EMAiL,@PASSWORD,@PHONE); select SCOPE_IDENTITY();", user, transaction);
                        }
                    }
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(UserDTO dt)
        {
            bool result = false;
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var ID = await _connection.DB.ExecuteAsync(@"UPDATE USERS SET FIRST_NAME= @FiRST_NAME WHERE ID = @UseriD", dt);
                transactionScope.Complete();
                result = true;
            }
            return result;
        }

        public async Task<bool> UpdateMultipleAsync(IList<UserDTO> dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    await _connection.DB.ExecuteAsync(@"UPDATE USERS SET FIRST_NAME= @FiRST_NAME WHERE ID = @UseriD", dt, transaction);
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> UpdateOneToOneAsync(GroupOneToOneDTO dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    await _connection.DB.ExecuteAsync(@"UPDATE USER_GROUPS SET NAME= @NAME WHERE ID = @GROUPiD", dt,transaction);
                    await _connection.DB.ExecuteAsync(@"UPDATE USERS SET FIRST_NAME= @FiRST_NAME WHERE GROUPID = @GROUPiD AND ID = @UseriD", dt.USERS, transaction);
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> UpdateOneToManyAsync(UserGroupDTO dt)
        {
            bool result = false;
            try
            {
                await _connection.DB.ExecuteAsync(@"UPDATE USER_GROUPS SET NAME= @NAME WHERE ID = @GROUPiD", dt);
                foreach (var user in dt.USERS)
                {
                    user.GROUPiD = dt.GROUPiD;
                    await _connection.DB.ExecuteAsync(@"UPDATE USERS SET FIRST_NAME= @FiRST_NAME WHERE GROUPID = @GROUPiD AND ID = @UseriD", user);
                }
                result = true;
            }
            catch { }
            return result;
        }

        public async Task<bool> UpdateToManyAsync(IList<UserGroupDTO> dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    foreach (var userGroup in dt)
                    {
                        await _connection.DB.ExecuteAsync(@"UPDATE USER_GROUPS SET NAME= @NAME WHERE ID = @GROUPiD", dt, transaction);
                        foreach (var user in userGroup.USERS)
                        {
                            user.GROUPiD = userGroup.GROUPiD;
                            await _connection.DB.ExecuteAsync(@"UPDATE USERS SET FIRST_NAME= @FiRST_NAME WHERE GROUPID = @GROUPiD AND ID = @UseriD", user, transaction);
                        }
                    }
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(UserDTO dt)
        {
            bool result = false;
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var ID = await _connection.DB.ExecuteAsync(@"DELETE FROM USERS WHERE ID = @UseriD", dt);
                transactionScope.Complete();
                result = true;
            }
            return result;
        }

        public async Task<bool> DeleteMultipleAsync(IList<UserDTO> dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    await _connection.DB.ExecuteAsync(@"DELETE FROM USERS WHERE ID = @UseriD", dt, transaction);
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> DeleteOneToOneAsync(GroupOneToOneDTO dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    await _connection.DB.ExecuteAsync(@"DELETE FROM USER_GROUPS WHERE ID = @GROUPiD", dt, transaction);
                    await _connection.DB.ExecuteAsync(@"DELETE FROM USERS WHERE GROUPID = @GROUPiD AND ID = @UseriD", dt.USERS, transaction);
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> DeleteOneToManyAsync(UserGroupDTO dt)
        {
            bool result = false;
            try
            {
                await _connection.DB.ExecuteAsync(@"DELETE FROM USER_GROUPS WHERE ID = @GROUPiD", dt);
                foreach (var user in dt.USERS)
                {
                    user.GROUPiD = dt.GROUPiD;
                    await _connection.DB.ExecuteAsync(@"DELETE FROM USERS WHERE GROUPID = @GROUPiD AND ID = @UseriD", user);
                }
                result = true;
            }
            catch { }
            return result;
        }

        public async Task<bool> DeleteToManyAsync(IList<UserGroupDTO> dt)
        {
            bool result = false;
            using (var transaction = _connection.DB.BeginTransaction())
            {
                try
                {
                    foreach (var userGroup in dt)
                    {
                        await _connection.DB.ExecuteAsync(@"DELETE FROM USER_GROUPS WHERE ID = @GROUPiD", dt, transaction);
                        foreach (var user in userGroup.USERS)
                        {
                            user.GROUPiD = userGroup.GROUPiD;
                            await _connection.DB.ExecuteAsync(@"DELETE FROM USERS WHERE GROUPID = @GROUPiD AND ID = @UseriD", user, transaction);
                        }
                    }
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return result;
        }


    }

    // Not
}               /*
                var lookup = new Dictionary<int, Shop>()

                    conn.Query<Shop, Account, Shop>(@"
                    SELECT s.*, a.*
                    FROM Shop s
                    INNER JOIN Account a ON s.ShopId = a.ShopId                    
                    ", (s, a) => {
                         Shop shop;
                         if (!lookup.TryGetValue(s.Id, out shop)) {
                             lookup.Add(s.Id, shop = s);
                         }
                         if (shop.Accounts == null) 
                             shop.Accounts = new List<Account>();
                         shop.Accounts.Add(a);
                         return shop;
                     },
                     ).AsQueryable();
                     */



                    /*
                    var lookup = new Dictionary<int, OrderDetail>();
                    var lookup2 = new Dictionary<int, OrderLine>();
                    connection.Query<OrderDetail, OrderLine, OrderLineSize, OrderDetail>(@"
							SELECT o.*, ol.*, ols.*
							FROM orders_mstr o
							INNER JOIN order_lines ol ON o.id = ol.order_id
							INNER JOIN order_line_size_relations ols ON ol.id = ols.order_line_id           
							", (o, ol, ols) =>
					{
						OrderDetail orderDetail;
						if (!lookup.TryGetValue(o.id, out orderDetail))
						{
							lookup.Add(o.id, orderDetail = o);
						}
						OrderLine orderLine;
						if (!lookup2.TryGetValue(ol.id, out orderLine))
						{
							lookup2.Add(ol.id, orderLine = ol);
							orderDetail.OrderLines.Add(orderLine);
						}
						orderLine.OrderLineSizes.Add(ols);
						return orderDetail;
					}).AsQueryable();

                    var resultList = lookup.Values.ToList();
                    */
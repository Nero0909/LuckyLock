using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Locks.Entities;
using Locks.Repository.Entities;
using Locks.Repository.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Locks.Repository
{
    public class LocksRepository : ILocksRepository
    {
        private readonly IConfiguration _config;

        public LocksRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        private IDbConnection Connection => new SqliteConnection(_config.GetConnectionString("Locks.db"));

        public async Task<Lock> TryCreateAsync(Lock @lock, string userId)
        {
            try
            {
                var dbEntity = ConvertToDb(@lock, userId);
                using (var db = Connection)
                {
                    await db.ExecuteAsync(
                        "INSERT INTO Locks " +
                        $"({nameof(LockDbEntity.Id)}, {nameof(LockDbEntity.CreatedBy)}, {nameof(LockDbEntity.CreatedDate)}, " +
                        $"{nameof(LockDbEntity.ModifiedBy)}, {nameof(LockDbEntity.ModifiedDate)}, {nameof(LockDbEntity.Name)}, " +
                        $"{nameof(LockDbEntity.UniqueNumber)}, {nameof(LockDbEntity.State)}) " +
                        "VALUES " +
                        $"(@{nameof(LockDbEntity.Id)}, @{nameof(LockDbEntity.CreatedBy)}, @{nameof(LockDbEntity.CreatedDate)}, " +
                        $"@{nameof(LockDbEntity.ModifiedBy)}, @{nameof(LockDbEntity.ModifiedDate)}, @{nameof(LockDbEntity.Name)}, " +
                        $"@{nameof(LockDbEntity.UniqueNumber)}, @{nameof(LockDbEntity.State)}) ",
                        dbEntity
                    ).ConfigureAwait(false);
                }

                return ConvertFromDb(dbEntity);
            }
            catch (SqliteException e)
            {
                if (e.IsUniqueConstraintViolation())
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid lockId, string userId)
        {
            using (var db = Connection)
            {
                var count = await db.ExecuteAsync(
                        "DELETE FROM Locks " +
                        $"WHERE {nameof(LockDbEntity.Id)} = @{nameof(LockDbEntity.Id)} " +
                        $"AND {nameof(LockDbEntity.CreatedBy)} = @{nameof(LockDbEntity.CreatedBy)}",
                        new {Id = lockId, CreatedBy = userId})
                    .ConfigureAwait(false);

                return count == 1;
            }
        }

        public async Task<Lock> UpdateAsync(Lock @lock, string userId)
        {
            var dbEntity = ConvertToDb(@lock, userId);
            using (var db = Connection)
            {
                await db.ExecuteAsync(
                    "UPDATE Locks SET " +
                    $"{nameof(LockDbEntity.ModifiedBy)}=@{nameof(LockDbEntity.ModifiedBy)}, " +
                    $"{nameof(LockDbEntity.ModifiedDate)}=@{nameof(LockDbEntity.ModifiedDate)}, " +
                    $"{nameof(LockDbEntity.Name)}=@{nameof(LockDbEntity.Name)}, " +
                    $"{nameof(LockDbEntity.UniqueNumber)}=@{nameof(LockDbEntity.UniqueNumber)}, " +
                    $"{nameof(LockDbEntity.State)}=@{nameof(LockDbEntity.State)} " +
                    $"WHERE {nameof(LockDbEntity.Id)}=@{nameof(LockDbEntity.Id)} ",
                    dbEntity
                ).ConfigureAwait(false);
            }

            return ConvertFromDb(dbEntity);
        }


        public async Task<Lock> GetByIdAsync(Guid id, string userId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryFirstOrDefaultAsync<LockDbEntity>(
                        "SELECT * FROM Locks " +
                        $"WHERE {nameof(LockDbEntity.Id)} = @{nameof(LockDbEntity.Id)} " +
                        $"AND {nameof(LockDbEntity.CreatedBy)} = @{nameof(LockDbEntity.CreatedBy)}",
                        new {Id = id, CreatedBy = userId})
                    .ConfigureAwait(false);

                return result != null ? ConvertFromDb(result) : null;
            }
        }

        public async Task<IEnumerable<Lock>> GetByUserAsync(string userId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryAsync<LockDbEntity>(
                    "SELECT * FROM Locks " +
                    $"WHERE {nameof(LockDbEntity.CreatedBy)} = @{nameof(LockDbEntity.CreatedBy)}",
                    new {CreatedBy = userId}).ConfigureAwait(false);

                return result.Select(ConvertFromDb).ToArray();
            }
        }

        private LockDbEntity ConvertToDb(Lock @lock, string userId)
        {
            return new LockDbEntity
            {
                Id = @lock.Id,
                CreatedBy = userId,
                CreatedDate = @lock.CreatedDate,
                ModifiedBy = userId,
                ModifiedDate = DateTime.UtcNow,
                Name = @lock.Name,
                UniqueNumber = @lock.UniqueNumber,
                State = @lock.State.ToString()
            };
        }

        private Lock ConvertFromDb(LockDbEntity @lock)
        {
            return new Lock
            {
                Id = @lock.Id,
                Name = @lock.Name,
                UniqueNumber = @lock.UniqueNumber,
                CreatedDate = @lock.CreatedDate,
                CreatedBy = @lock.CreatedBy,
                State = Enum.Parse<LockState>(@lock.State)
            };
        }
    }
}

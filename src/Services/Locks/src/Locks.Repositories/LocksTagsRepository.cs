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
    public class LocksTagsRepository : ILocksTagsRepository
    {
        private readonly IConfiguration _config;

        public LocksTagsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        private IDbConnection Connection => new SqliteConnection(_config.GetConnectionString("Locks.db"));

        public async Task<IEnumerable<Guid>> GetLinkedLocksByTagIdAsync(Guid tagId, string userId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryAsync<Guid>(
                        $"SELECT {nameof(LockTagDbEntity.LockId)} FROM LockTags " +
                        $"WHERE {nameof(LockTagDbEntity.TagId)} = @{nameof(LockTagDbEntity.TagId)} " +
                        $"AND {nameof(LockDbEntity.CreatedBy)} = @{nameof(LockDbEntity.CreatedBy)}",
                        new { TagId = tagId, CreatedBy = userId})
                    .ConfigureAwait(false);

                return result.ToArray();
            }
        }

        public async Task<bool> CheckLinkedLocksExistence(Guid tagId, string userId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryFirstOrDefaultAsync<Guid?>(
                        $"SELECT {nameof(LockTagDbEntity.LockId)} FROM LockTags " +
                        $"WHERE {nameof(LockTagDbEntity.TagId)} = @{nameof(LockTagDbEntity.TagId)} " +
                        $"AND {nameof(LockDbEntity.CreatedBy)} = @{nameof(LockDbEntity.CreatedBy)}",
                        new { TagId = tagId, CreatedBy = userId })
                    .ConfigureAwait(false);

                return result != null;
            }
        }

        public async Task<bool> DeleteLinkAsync(LockTag link, string userId)
        {
            using (var db = Connection)
            {
                var count = await db.ExecuteAsync(
                        "DELETE FROM LockTags " +
                        $"WHERE {nameof(LockTagDbEntity.TagId)} = @{nameof(LockTagDbEntity.TagId)} " +
                        $"AND {nameof(LockTagDbEntity.LockId)} = @{nameof(LockTagDbEntity.LockId)} " +
                        $"AND {nameof(LockDbEntity.CreatedBy)} = @{nameof(LockDbEntity.CreatedBy)}",
                        new {TagId = link.TagId, LockId = link.LockId, CreatedBy = userId})
                    .ConfigureAwait(false);

                return count == 1;
            }
        }

        public async Task<IEnumerable<Guid>> GetLinkedTagsByLockAsync(Guid lockId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryAsync<Guid>(
                    $"SELECT {nameof(LockTagDbEntity.TagId)} FROM LockTags " +
                    $"WHERE {nameof(LockTagDbEntity.LockId)} = @{nameof(LockTagDbEntity.LockId)} ",
                    new { LockId = lockId }).ConfigureAwait(false);

                return result.ToArray();
            }
        }

        public async Task<LockTag> TryCreateAsync(LockTag link, string userId)
        {
            try
            {
                var dbEntity = ConvertToDb(link, userId);
                using (var db = Connection)
                {
                    await db.ExecuteAsync(
                        "INSERT INTO LockTags " +
                        $"({nameof(LockTagDbEntity.Id)}, {nameof(LockTagDbEntity.CreatedBy)}, {nameof(LockTagDbEntity.CreatedDate)}, " +
                        $"{nameof(LockTagDbEntity.ModifiedBy)}, {nameof(LockTagDbEntity.ModifiedDate)}, {nameof(LockTagDbEntity.LockId)}, " +
                        $"{nameof(LockTagDbEntity.TagId)}) " +
                        "VALUES " +
                        $"(@{nameof(LockTagDbEntity.Id)}, @{nameof(LockTagDbEntity.CreatedBy)}, @{nameof(LockTagDbEntity.CreatedDate)}, " +
                        $"@{nameof(LockTagDbEntity.ModifiedBy)}, @{nameof(LockTagDbEntity.ModifiedDate)}, @{nameof(LockTagDbEntity.LockId)}, " +
                        $"@{nameof(LockTagDbEntity.TagId)}) ",
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

        private LockTagDbEntity ConvertToDb(LockTag @lock, string userId)
        {
            return new LockTagDbEntity
            {
                Id = @lock.Id,
                CreatedBy = userId,
                CreatedDate = @lock.CreatedDate,
                ModifiedBy = userId,
                ModifiedDate = DateTime.UtcNow,
                TagId = @lock.TagId,
                LockId = @lock.LockId
            };
        }

        private LockTag ConvertFromDb(LockTagDbEntity @lock)
        {
            return new LockTag
            {
                Id = @lock.Id,
                CreatedDate = @lock.CreatedDate,
                CreatedBy = @lock.CreatedBy,
                TagId = @lock.TagId,
                LockId = @lock.LockId
            };
        }
    }
}

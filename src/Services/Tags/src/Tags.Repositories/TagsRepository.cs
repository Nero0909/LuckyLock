using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Tags.Entities;
using Tags.Repositories.Entities;
using Tags.Repositories.Extensions;

namespace Tags.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private readonly IConfiguration _config;

        public TagsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        private IDbConnection Connection => new SqliteConnection(_config.GetConnectionString("Tags.db"));

        public async Task<Tag> TryCreateAsync(Tag tag, string userId)
        {
            try
            {
                var dbEntity = ConvertToDb(tag, userId);
                using (var db = Connection)
                {
                    await db.ExecuteAsync(
                        "INSERT INTO Tags " +
                        $"({nameof(TagDbEntity.Id)}, {nameof(TagDbEntity.CreatedBy)}, {nameof(TagDbEntity.CreatedDate)}, " +
                        $"{nameof(TagDbEntity.ModifiedBy)}, {nameof(TagDbEntity.ModifiedDate)}, {nameof(TagDbEntity.Name)}, {nameof(TagDbEntity.UniqueNumber)}) " +
                        "VALUES " +
                        $"(@{nameof(TagDbEntity.Id)}, @{nameof(TagDbEntity.CreatedBy)}, @{nameof(TagDbEntity.CreatedDate)}, " +
                        $"@{nameof(TagDbEntity.ModifiedBy)}, @{nameof(TagDbEntity.ModifiedDate)}, @{nameof(TagDbEntity.Name)}, @{nameof(TagDbEntity.UniqueNumber)}) ",
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

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            using (var db = Connection)
            {
                var count = await db.ExecuteAsync(
                    "DELETE FROM Tags " +
                    $"WHERE {nameof(TagDbEntity.Id)} = @{nameof(TagDbEntity.Id)} " +
                    $"AND {nameof(TagDbEntity.CreatedBy)} = @{nameof(TagDbEntity.CreatedBy)}",
                    new { Id = id, CreatedBy = userId })
                .ConfigureAwait(false);

                return count == 1;
            }
        }

        public async Task<Tag> GetByIdAsync(Guid id, string userId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryFirstOrDefaultAsync<TagDbEntity>(
                        "SELECT * FROM Tags " +
                        $"WHERE {nameof(TagDbEntity.Id)} = @{nameof(TagDbEntity.Id)} " +
                        $"AND {nameof(TagDbEntity.CreatedBy)} = @{nameof(TagDbEntity.CreatedBy)}",
                        new {Id = id, CreatedBy = userId})
                    .ConfigureAwait(false);

                return result != null ? ConvertFromDb(result) : null;
            }
        }

        public async Task<IEnumerable<Tag>> GetByUserAsync(string userId)
        {
            using (var db = Connection)
            {
                var result = await db.QueryAsync<TagDbEntity>(
                    "SELECT * FROM Tags " +
                    $"WHERE {nameof(TagDbEntity.CreatedBy)} = @{nameof(TagDbEntity.CreatedBy)}",
                    new {CreatedBy = userId}).ConfigureAwait(false);

                return result.Select(ConvertFromDb).ToArray();
            }
        }

        private TagDbEntity ConvertToDb(Tag tag, string userId)
        {
            return new TagDbEntity
            {
                Id = tag.Id,
                CreatedBy = userId,
                CreatedDate = tag.CreatedDate,
                ModifiedBy = userId,
                ModifiedDate = DateTime.UtcNow,
                Name = tag.Name,
                UniqueNumber = tag.UniqueNumber
            };
        }

        private Tag ConvertFromDb(TagDbEntity tag)
        {
            return new Tag
            {
                Id = tag.Id,
                Name = tag.Name,
                UniqueNumber = tag.UniqueNumber,
                CreatedDate = tag.CreatedDate,
                CreatedBy = tag.CreatedBy,
            };
        }
    }
}

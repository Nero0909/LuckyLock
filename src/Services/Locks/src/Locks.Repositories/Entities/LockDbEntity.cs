namespace Locks.Repository.Entities
{
    public class LockDbEntity : DbEntity
    {
        public string Name { get; set; }

        public string UniqueNumber { get; set; }

        public string State { get; set; }
    }
}

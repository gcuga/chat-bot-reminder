
namespace Reminder.Storage.Core
{
    public interface ICruStorage<T>
    {
        long GetNextId();
        T Get(long id);
        bool TryGet(long id, out T item);
        void Insert(T item);
        void Update(T item);
        void Upsert(T item);
    }
}

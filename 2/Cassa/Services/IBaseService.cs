using TblComment = lab_1.Cassa.Entities.TblComment;

namespace lab_1.Cassa.Services
{
    public interface IBaseService<T, V>
    {
        public V? Create(T dto);

        V? Read(long id);

        public V? Update(T dto);

        public bool Delete(long id);

        public IEnumerable<V> GetAll();

        public void AddComment(TblComment comment);
        
        public Task<bool> SendOrderRequest(string topic, string message);

        public void UpdateComment(TblComment comment);
    }
}
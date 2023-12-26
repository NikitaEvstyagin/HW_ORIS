namespace MyORM;

public interface IDatabaseOperation
{
    bool Add<T>(T obj);
    bool Update<T>(T obj);
    bool Delete<T>(int id);
    List<T> Select<T>();
    T SelectById<T>(int id);
}
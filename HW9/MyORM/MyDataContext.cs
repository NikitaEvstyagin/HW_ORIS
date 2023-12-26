using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyORM
{
    public class MyDataContext : IDatabaseOperation
    {
        private readonly string connectionString;
        private SqlConnection connection;

        public MyDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public bool Add<T>(T obj)
        {
            string tableName = typeof(T).Name;
            var properties = obj.GetType().GetProperties();

            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (var property in properties)
            {
                if (property.Name != "Id") // есть автоинкрементное поле Id и не включаю его в список столбцов
                {
                    columns.Append(property.Name + ", ");
                    values.Append("@" + property.Name + ", ");
                }
            }

            columns.Length -= 2; // Удаляем лишнюю запятую и пробел в конце
            values.Length -= 2; // Удаляем лишнюю запятую и пробел в конце

            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                var command = new SqlCommand(query, connection);

                foreach (var property in properties)
                {
                    if (property.Name != "Id")
                    {
                        command.Parameters.AddWithValue("@" + property.Name, property.GetValue(obj));
                    }
                }

                return command.ExecuteNonQuery() > 0;
            }
        }
        public bool Update<T>(T entity)
        {
            var type = entity?.GetType();
            var tableName = type?.Name;
            var id = type?.GetProperty("id");
            var props = type!.GetProperties()
                .Where(x => !x.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var sqlExpression = $"SELECT * FROM \"{tableName}\" WHERE \"id\" = {id?.GetValue(entity)}";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var adapter = new SqlDataAdapter(sqlExpression, connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            var entityFromDatabase = dataSet.Tables[0];
            var rowToUpdate = entityFromDatabase.Rows[0];

            foreach (var prop in props)
            {
                var val = prop.GetValue(entity);
                rowToUpdate[prop.Name] = val ?? DBNull.Value;
            }

            var commandBuilder = new SqlCommandBuilder(adapter);
            adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            adapter.Update(dataSet);

            return true;
        }
        public bool Delete<T>(int id) {
            using (connection = new SqlConnection(connectionString)) {
                string tableName = typeof(T).Name;

                connection.Open();
                SqlCommand command = new SqlCommand($"DELETE FROM {tableName} WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                return command.ExecuteNonQuery() > 0;
            }
        }
        public List<T> Select<T>()
        {
            using (connection = new SqlConnection(connectionString))
            {
                var type = typeof(T);
                var propers = type.GetProperties();
                string tableName = type.Name;
                connection.Open();
                var command = new SqlCommand($"SELECT * FROM {tableName}", connection);
                using (var reader = command.ExecuteReader())
                {
                    List<T> resultList = new List<T>();

                    while (reader.Read())
                    {
                        T obj = Activator.CreateInstance<T>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object columnValue = reader.GetValue(i);

                            PropertyInfo property = typeof(T).GetProperty(columnName);
                            if (property != null && columnValue != DBNull.Value)
                            {
                                property.SetValue(obj, columnValue);
                            }
                        }
                        resultList.Add(obj);
                    }

                    return resultList;
                }
            }
        }
        public T SelectById<T>(int id)
        {
            using (connection = new SqlConnection(connectionString))
            {
                var type = typeof(T);
                var propers = type.GetProperties();
                string tableName = type.Name;
                connection.Open();
                var command = new SqlCommand($"SELECT * FROM {tableName} WHERE Id = {id}", connection);

                using (var reader = command.ExecuteReader())
                {
                    T obj = Activator.CreateInstance<T>();
                    string columnName = reader.GetName(0);
                    object columnValue = reader.GetValue(0);

                    PropertyInfo property = typeof(T).GetProperty(columnName);
                    if (property != null && columnValue != DBNull.Value)
                    {
                        property.SetValue(obj, columnValue);
                        return obj;
                    }
                    else
                    {
                        return default(T);
                    }

                }

            }
        }
    }
}

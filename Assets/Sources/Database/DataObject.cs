using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Sources.Database {
    public interface IDataObject {
        public int GetId();
    }

    public abstract class DataObject<T> where T : IDataObject, new() {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int GetId() {
            return Id;
        }

        public static T Get(int id) {
            return GetAll().First(x => x.GetId() == id);
        }

        public static T ByName(string name) {
            return GetConnection().Query<T>($"select * from {GetTable().Table.TableName} where Name = '{name}' limit 1;").First();
        }

        public static T First(Func<T, bool> expression) {
            return Find(expression).First();
        }

        public static T[] Find(Func<T, bool> expression) {
            return GetAll().Where(expression).ToArray();
        }

        public static T[] GetAll() {
            return GetTable().ToArray();
        }

        protected static TableQuery<T> GetTable() {
            return GetConnection().Table<T>();
        }

        protected static SQLiteConnection GetConnection() {
            return DatabaseConnector.getInstance().GetConnection();
        }
    }
}

using Sources.Support;
using SQLite;
using UnityEngine;

namespace Sources.Database {
    public class DatabaseConnector : Singleton<DatabaseConnector> {
        private SQLiteConnection connection;

        public DatabaseConnector() {
            connection = new SQLiteConnection(GetDBPath());
        }

        public SQLiteConnection GetConnection() {
            return connection;
        }
        
        private static string GetDBPath() {
            switch (Application.platform) {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsPlayer:
                    return Application.dataPath + "/StreamingAssets/db.bytes";
                case RuntimePlatform.IPhonePlayer:
                    return Application.dataPath + "/Raw/db.bytes";
                case RuntimePlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/db.bytes";
                default:
                    Debug.LogError("Cannot build database path: unknown system!");
                    break;
            }

            return null;
        }
    }
}

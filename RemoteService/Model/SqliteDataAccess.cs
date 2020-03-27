using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;

namespace RemoteService.Model
{
    public class SqliteDataAccess
    {

        //GET CONFIG
        public static List<Config> GetConfig()
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Config>("SELECT * FROM Config", new DynamicParameters());
                return output.ToList();
            }
        }

        //SAVE CONFIG
        public static void SaveConfig(Config config)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                
                var conf = cnn.Query<Config>($"SELECT * FROM Config").FirstOrDefault();
                if (conf == null)
                    cnn.Execute("INSERT INTO Config (Server, Domain, Username, Password, Query, AutoStart, Notification, Minimized, Duration)" +
                        " VALUES (@Server, @Domain, @Username, @Password, @Query, @AutoStart, @Notification, @Minimized, @Duration)", config);
                else
                    cnn.Execute($"UPDATE Config SET Server='{config.Server}', Domain='{config.Domain}', Username='{config.Username}', " +
                        $"Password='{config.Password}', Query='{config.Query}', AutoStart='{config.AutoStart}', Notification='{config.Notification}', " +
                        $"Minimized='{config.Minimized}', Duration='{config.Duration}'");
            }
        }



        //CONNECTION STRING
        private static string LoadConnectionString(string id = @"SQLite")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}

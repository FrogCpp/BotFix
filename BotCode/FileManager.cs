using Newtonsoft.Json;
using System.Reflection;

namespace BotFix
{
    internal class FileManager : IDisposable
    {
        public List<UserSettings> MyUsers;
        private string way = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/users.json";
        public FileManager()
        {
            try
            {
                var a = File.ReadAllText(way);
                if (a != null)
                    MyUsers = JsonConvert.DeserializeObject<List<UserSettings>>(a);
            }
            catch
            {
                MyUsers = new List<UserSettings>();
            }
            return;
        }

        public bool TryGetUser(long userId, out UserSettings[] users)
        {
            if (MyUsers == null || MyUsers.Count < 1)
            {
                users = Array.Empty<UserSettings>();
                return false;
            }

            users = MyUsers.Where(u => u.userID == userId).ToArray();
            return users.Length > 0;
        }

        public bool TryGetUser(string userId, out UserSettings[] users)
        {
            if (MyUsers == null || MyUsers.Count < 1)
            {
                users = Array.Empty<UserSettings>();
                return false;
            }

            users = MyUsers.Where(u => u.FriendKey == userId).ToArray();
            return users.Length > 0;
        }
        public void Dispose()
        {
            string json = JsonConvert.SerializeObject(MyUsers, Formatting.Indented);
            bool wait = true;
            while (wait)
            {
                try
                {
                    File.WriteAllText(way, json);
                    wait = false;
                }
                catch
                {
                    Console.WriteLine("waiting!");
                }
            }
        }

    }
}

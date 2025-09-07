using Newtonsoft.Json;

namespace BotFix
{
    internal class FileManager : IDisposable
    {
        public List<UserSettings> MyUsers;
        private string way;
        public FileManager(string way)
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

            this.way = way;
        }

        public bool TryGetUser(long userId, out UserSettings user)
        {
            if (MyUsers == null || MyUsers.Count < 1)
            {
                user = new UserSettings();
                return false;
            }
            user = MyUsers.FirstOrDefault(u => u.userID == userId);
            return user.userID != 0;
        }
        public void Dispose()
        {
            string json = JsonConvert.SerializeObject(MyUsers, Formatting.Indented);
            File.WriteAllText(way, json);
        }

    }
}

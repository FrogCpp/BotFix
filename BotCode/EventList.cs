using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFix
{
    public class UserSettings //класс а не стракт потому, что надо с ссылками работать
    {
        public UserSettings() 
        {
            FriendKey = GenerateRandomString();
        }

        public short registrSteps;
        public long userID;
        public string FriendKey;
        public string usrName;
        public bool guest;
        public List<List<Subject>> MyLessonsList;

        private static string GenerateRandomString()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 10)
                .Select(_ => (char)random.Next(33, 127)).ToArray());
        }
    }
    internal class EventList
    {
        private TelegramController _tgMethods;
        public EventList(TelegramController tgMethods)
        {
            _tgMethods = tgMethods;
        }

        public void startE(string text, long uID)
        {
            if (text != "/start")
                return;

            using (var f = new FileManager("/Users.json"))
            {
                var file = f.MyUsers;
                if (!file.Any(user => user.userID == uID))
                {
                    _tgMethods.SendMessage("Здравствуй новый пользователь!\nМы тебя еще не знаем, давай познакомимся.\nвведи своё имя:", uID);
                    var usr = new UserSettings();
                    usr.registrSteps = 1;
                    usr.userID = uID;
                    file.Add(usr);
                }
                else
                {
                    _tgMethods.SendMessage("ты уже зарегестрирован, не переусердствуй тут (👉ﾟヮﾟ)👉", uID);
                }
            }
        }

        public void GetUserName(string text, long uID)
        {
            if (text[0] == '/')
                return;
            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out us)){
                    if (us.registrSteps == 1)
                    {
                        us.usrName = text;
                        _tgMethods.SendMessage($"здравствуй {us.usrName}, теперь мы тебя знаем.\nСледующий шаг: добавь человека, с которым ты поделишь учебники.\nЕсли хочешь поделиться сам, то вот твой ID: {us.FriendKey}", uID);
                        _tgMethods.SendMessage($"{us.usrName}, если ты гад, и накосячишь в процессе регестрации, то пропиши /fail\nэто откатит тебя в самое начало. Ты будешь вынужден пройти регистрацию заново.", uID);
                        _tgMethods.SendMessage($"Ты, {us.usrName}, можешь быть либо адмнистратором деления, либо приглашенным.\nАдминистратор: управляет делением, он должен загрузить расписание.\nПодключенный может быть только один (сосед).\n\nЕсли ты администратор, то просто пропиши /Admin\nЕсли подключаешься, то впиши айди администратора.", uID);
                        us.registrSteps = 2;
                    }
                }
            }
        }

        public void GetFriend(string text, long uID)
        {
            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out us))
                {
                    if (us.registrSteps == 2)
                    {
                        us.guest = text != "/Admin";
                        if (us.guest)
                        {
                            us.FriendKey = text;
                        }
                        if (us.guest)
                        {
                            _tgMethods.SendMessage($"Молодец, сиди отдыхай.\nВремя отправки еще настрой, и отдыхай", uID);
                            us.registrSteps = 4; // пропустим пункт с настройкой расписания
                        }
                        else
                        {
                            _tgMethods.SendMessage($"Молодец, теперь твоя задача заполнить полностью расписание\nа после еще и время отправки для себя настроить", uID);
                            _tgMethods.SendMessage($"Расписание важно указать точно по шаблону:\nпредмет1 340\nпредмет2 512\n. . .\n* (* - разделитель между днями недели)\nпредмет1 1024\nпредмет2 112\n. . .\n* (и так далее)\nцифра, через пробел после предмета, это вес учебника. его нужно указать в граммах. Для учебников вес необходимо указать. Есле предмет не имеет учебника, то стоит указать не \"0\", а просто не указывать ничего.", uID);
                            _tgMethods.SendMessage($"Важно!\nни в начале ни в конце не должно быть ничего: сразу расписание без другого текста. После звездочек-разделителей пробелов нет. Выполняй все точно по шаблону.", uID);
                            us.registrSteps = 3;
                        }
                    }
                }
            }
        }

        public void GetLessonsList(string text, long uID)
        {
            if (text[0] == '/')
                return;

            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out us))
                {
                    if (us.registrSteps == 3)
                    {
                        int day = 0;
                        List<List<Subject>> LessonsLst = new List<List<Subject>>(0);
                        List<string> a = text.Split("*\n").ToList<string>();
                        for (int i = 0; i < a.Count; i++)
                        {
                            List<Subject> c = new List<Subject>();
                            foreach (string lesson in a[i].Split('\n'))
                            {
                                if (lesson.Contains(' '))
                                {
                                    var b = lesson.Split(' ');
                                    c.Add(new Subject(b[0], uint.Parse(b[1])));
                                }
                                else
                                {
                                    c.Add(new Subject(lesson));
                                }
                            }
                            LessonsLst.Add(c);
                        }
                        us.MyLessonsList = LessonsLst;
                    }
                }
            }
        }

        public void Fuckup(string text, long uID)
        {
            if (text != "/fail")
                return;
            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out us))
                {
                    us.usrName = "lox";
                    us.registrSteps = 1;
                    var rand = new Random();
                    us.FriendKey = new string(Enumerable.Range(0, 10).Select(_ => (char)rand.Next(33, 127)).ToArray());
                    _tgMethods.SendMessage("Здравствуй новый пользователь!\nМы тебя уже знаем, но давай познакомимся еще раз.\nвведи своё имя:", uID);
                }
            }
        }
    }
}

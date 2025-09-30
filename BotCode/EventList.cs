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
        public string MyLessonsList;

        static private string GenerateRandomString()
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
                if (f.TryGetUser(uID, out var usv)){
                    us = usv[0];
                    if (us.registrSteps == 1)
                    {
                        us.usrName = text;
                        _tgMethods.SendMessage($"здравствуй {us.usrName}, теперь мы тебя знаем.\nСледующий шаг: добавь человека, с которым ты поделишь учебники.\nЕсли хочешь поделиться сам, то вот твой ID: {us.FriendKey}", uID);
                        _tgMethods.SendMessage($"{us.usrName}, если ты гад, и накосячишь в процессе регестрации, то пропиши /fail\nэто откатит тебя в самое начало. Ты будешь вынужден пройти регистрацию заново.", uID);
                        _tgMethods.SendMessage($"Ты, {us.usrName}, можешь быть либо адмнистратором деления, либо приглашенным.\nАдминистратор: управляет делением, он должен загрузить расписание.\nПодключенный может быть только один (сосед).\n\nЕсли ты администратор, то просто пропиши /Admin\nЕсли подключаешься, то впиши айди администратора. (Попроси своего админа выдать тебе ключ)", uID);
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
                if (f.TryGetUser(uID, out var usv))
                {
                    us = usv[0];
                    if (us.registrSteps == 2)
                    {
                        us.guest = text != "/Admin";
                        if (us.guest)
                        {
                            us.FriendKey = text;
                        }
                        if (us.guest)
                        {
                            _tgMethods.SendMessage($"Молодец, сиди отдыхай.\nВремя отправки еще настрой, и отдыхай\nесли просчитался с ключем, ничего страшного, пропиши команду /setFriendKey и замени себе ключь. (в отличии от остальных комманд, здесь нужно указать аргумент (сам ключь) через пробел после самой комманды (примерно так /setFriendKey Кл:Ю:Чь))", uID);
                            us.registrSteps = 4; // пропустим пункт с настройкой расписания
                            _tgMethods.SendMessage($"так же, если ты правильно указал ключь друга, то и твоему другу мы тоже сообщим, что ты к нему подключен :)", uID);

                            if (f.TryGetUser(us.FriendKey, out var friend))
                            {
                                if (friend.Length == 1)
                                {
                                    _tgMethods.SendMessage("Друзей у тебя, похоже, нет. Либо ты ошибся с ключем. . . проверь хорошенько. (мы попытались найти человека с таким ключем, но никого нет. . .)", uID);
                                }
                                else
                                {
                                    foreach (var e in friend)
                                    {
                                        if (e.userID != us.userID)
                                        {
                                            _tgMethods.SendMessage($"к тебе добавился твой друг {us.usrName}", e.userID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _tgMethods.SendMessage("Друзей у тебя, похоже, нет. Либо ты ошибся с ключем. . . проверь хорошенько. (мы попытались найти человека с таким ключем, такого больше нет. . .)", uID);
                            }
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
                if (f.TryGetUser(uID, out var usv))
                {
                    us = usv[0];
                    if (us.registrSteps == 3)
                    {
                        us.MyLessonsList = text;
                        _tgMethods.SendMessage($"с созданием расписания справились, молодец! но если молодец не до конца, и при вводе сделал ошибку, то пропиши /fail это сотрет твою учетку, и тебе придется заново создовать ее. Если гость уже зарегался до тебя, и успел ввести твой ключь, то пусть пропишет команду /setFriendKey и заменит себе ключь. (в отличии от остальных комманд, здесь нужно указать аргумент (сам ключь) через пробел после самой комманды (примерно так /setFriendKey Кл:Ю:Чь))\n\nтебе остается только настроить время!", uID);
                    }
                }
            }
        }

        /*
        public void GetTime(string text, long uID)
        {
            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out var usv))
                {
                    us = usv[0];
                    if (us.registrSteps == 4)
                    {

                    }
                }
            }
        }
        */

        public void Fuckup(string text, long uID)
        {
            if (text != "/fail")
                return;
            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out var usv))
                {
                    us = usv[0];
                    us.usrName = "lox";
                    us.registrSteps = 1;
                    var rand = new Random();
                    us.FriendKey = new string(Enumerable.Range(0, 10).Select(_ => (char)rand.Next(33, 127)).ToArray());
                    _tgMethods.SendMessage("Здравствуй новый пользователь!\nМы тебя уже знаем, но давай познакомимся еще раз.\nвведи своё имя:", uID);
                }
            }
        }

        public void fKey(string text, long uID)
        {
            if (!text.Contains("/setFriendKey"))
                return;

            string[] tx = text.Split(' ');
            _tgMethods.SendMessage($"Что же, похоже твой админ играл с регестрацией и проиграл, либо ты неправельно вставил ключь дружбы. Ничего страшного, бывает, обнови его просто отправив нужный ключь сообщением.\nНе забывай, что сколько сильно ты бы не поломал свою учетку, все всегда можно исправить (достаточно прописать /fail )\n;)", uID);
            using (var f = new FileManager("/Users.json"))
            {
                UserSettings us;
                if (f.TryGetUser(uID, out var usv))
                {
                    us = usv[0];
                    us.FriendKey = tx[1];
                    _tgMethods.SendMessage($"твой новый ключь выглядит так: {us.FriendKey}", uID);
                    _tgMethods.SendMessage($"так же, если ты правильно указал ключь друга, то и твоему другу мы тоже сообщим, что ты к нему подключен :)", uID);

                    if (f.TryGetUser(us.FriendKey, out var friend))
                    {
                        if (friend.Length == 1)
                        {
                            _tgMethods.SendMessage("Друзей у тебя, похоже, нет. Либо ты ошибся с ключем. . . проверь хорошенько. (мы попытались найти человека с таким ключем, такого больше нет. . .)", uID);
                        }
                        else
                        {
                            foreach(var e in friend)
                            {
                                if (e.userID != us.userID)
                                {
                                    _tgMethods.SendMessage($"к тебе добавился твой друг {us.usrName}", e.userID);
                                }
                            }
                        }
                    }
                    else
                    {
                        _tgMethods.SendMessage("Друзей у тебя, похоже, нет. Либо ты ошибся с ключем. . . проверь хорошенько. (мы попытались найти человека с таким ключем, такого больше нет. . .)", uID);
                    }
                }
            }
        }

        public void Test(string text, long uID)
        {
            Console.WriteLine($"{uID}: {text}");
        }
    }
}

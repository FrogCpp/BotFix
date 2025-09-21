using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFix
{
    internal class TimeController
    {
        private DateTime targetTime;
        private Timer checkTimer;
        private bool isFunctionExecutedToday = false;
        private TelegramController tgc;
        public TimeController(float time, TelegramController tgControl) 
        {
            //Console.WriteLine(DateTime.Now);
            targetTime = DateTime.Today.AddHours(time);

            checkTimer = new Timer(CheckTime, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            tgc = tgControl;
        }

        private void CheckTime(object state)
        {
            DateTime now = DateTime.Now;

            if (!isFunctionExecutedToday && now >= targetTime)
            {
                mainFunc();

                isFunctionExecutedToday = true;

                targetTime = targetTime.AddDays(1);
            }

            if (now.Hour == 0 && now.Minute == 0)
            {
                isFunctionExecutedToday = false;
            }
        }

        private void mainFunc()
        {
            using (var f = new FileManager("/Users.json"))
            {
                foreach (var i in f.MyUsers)
                {
                    var a = i.MyLessonsList;
                    int dayNumber = ((int)DateTime.Now.DayOfWeek + 6) % 7;
                    if (a.Count >= dayNumber)
                    {
                        tgc.SendMessage($"{i.usrName}, вот твое расписание на завтрашний день!\n*леша не забудь починить это, после того, как егор исправит сплитер*", i.userID);
                    }
                }
            }
        }

        public void Dispose()
        {
            checkTimer?.Dispose();
        }
    }
}

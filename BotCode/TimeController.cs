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

        static private Weekday intToWeekday(int input)
        {
            return input switch
            {
                1 => Weekday.Monday,
                2 => Weekday.Tuesday,
                3 => Weekday.Wednesday,
                4 => Weekday.Thursday,
                5 => Weekday.Friday,
                6 => Weekday.Saturday,
                0 => Weekday.Sunday,
                _ => Weekday.Undefined
            };
        }

        public void Dispose()
        {
            checkTimer?.Dispose();
        }

        private List<DaySchedule> SplitMyString(string text)
        {
            List<DaySchedule> LessonsLst = [];
            List<string> a = text.Split("*\n").ToList<string>();
            for (int j = 0; j < a.Count; j++)
            {
                foreach (string lesson in a[j].Split('\n'))
                {
                    DaySchedule c = new DaySchedule();
                    if (lesson.Contains(' '))
                    {
                        var b = lesson.Split(' ');
                        c.AddSubject(new Subject(b[0], uint.Parse(b[1])));
                    }
                    else
                    {
                        c.AddSubject(new Subject(lesson));
                    }
                    LessonsLst.Add(c);
                }
            }
            return LessonsLst;
        }
    }
}

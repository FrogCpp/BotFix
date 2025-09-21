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

        public void mainFunc()
        {
            Console.WriteLine("Start!");
            using (var f = new FileManager("/Users.json"))
            {
                Console.WriteLine(f);
                foreach (var i in f.MyUsers)
                {
                    Console.WriteLine(i.usrName);
                    List<List<Subject>> a = [];
                    a.AddRange(i.MyLessonsList);
                    int dayNumber = (int)DateTime.Now.DayOfWeek;
                    if (a.Count >= dayNumber)
                    {
                        foreach (var t in i.MyLessonsList)
                        {
                            Console.WriteLine(t.Count);
                        }
                        List<DaySchedule> splitResult = DaySchedule.Convert(Split.NextFor2(a, intToWeekday(dayNumber)));
                        Console.WriteLine("========");
                        foreach (var t in i.MyLessonsList)
                        {
                            Console.WriteLine(t.Count);
                        }
                        string outp = "";
                        int userNum = (i.guest ? 0 : 1);
                        int count = 0;
                        for (var j = 1; j <= splitResult[userNum].Count; j++)
                        {
                            Console.WriteLine(splitResult[userNum].S[j - 1].Title);
                            outp += $"{j}: {splitResult[userNum].S[j - 1].Title}, {splitResult[userNum].S[j - 1].WeightG}g\n";
                            count = j;
                        }
                        outp += $"у тебя сегодня с собой целых {count.ToString()} учебников!";
                        tgc.SendMessage($"{i.usrName}, вот твое расписание на завтрашний день!\n{outp}", i.userID);
                        Console.WriteLine("End!");
                    }
                }
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
    }
}

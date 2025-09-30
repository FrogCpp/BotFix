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

                    List<DaySchedule> splited = SplitMyString(a);
                    for (int ge = 0; ge < splited.Count; ge++)
                    {
                        for (int gah = 0; gah < splited[ge].Count; gah++)
                        {
                            var subject = splited[ge].S[gah];
                            Console.WriteLine(subject.Title);
                        }
                    }

                    List<DaySchedule> splitResult = Split.NextFor2(splited, IntToWeekday(dayNumber));

                    for (int yte = 0; yte < splitResult.Count; yte++)
                    {
                        Console.WriteLine(splitResult[yte].Count);
                        for (int gah = 0; gah < splitResult[yte].Count; gah++)
                        {
                            var subject = splitResult[yte].S[gah];
                            Console.WriteLine(subject.Title);
                            //string title = subject.Title;
                            //uint weight = subject.WeightG;
                            //Console.WriteLine($"- {title} [{weight}г]");
                        }
                    }
                    //tgc.SendMessage($"{i.usrName}, вот твое расписание на завтрашний день!\n*леша не забудь починить это, после того, как егор исправит сплитер*", i.userID);
                }
            }
        }

        public void Dispose()
        {
            checkTimer?.Dispose();
        }

        static private Weekday IntToWeekday(int input)
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
                        Console.WriteLine($"{b[0]}, {b[1]}g");
                    }
                    else
                    {
                        c.AddSubject(new Subject(lesson));
                        Console.WriteLine($"{lesson}, -1g");
                    }
                    LessonsLst.Add(c);
                }
            }
            return LessonsLst;
        }
    }
}

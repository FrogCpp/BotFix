using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BotFix
{
    internal class TimeController
    {
        private Timer checkTimer;
        private TelegramController tgc;
        public TimeController(TelegramController tgControl) 
        {
            checkTimer = new Timer(CheckTime, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            tgc = tgControl;
        }

        private void CheckTime(object state)
        {
            DateTime now = DateTime.Now;

            using (var f = new FileManager())
            {
                foreach (var i in f.MyUsers)
                {
                    if (!i.isSendedToday && now >= i.Time)
                    {
                        mainFunc(i.userID);

                        i.isSendedToday = true;

                        i.Time = i.Time.AddDays(1);
                    }

                    if (now.Hour == 0 && now.Minute == 0)
                    {
                        i.isSendedToday = false;
                    }
                }
            }
        }

        private void mainFunc(long usID)
        {
            using (var f = new FileManager())
            {
                if (f.TryGetUser(usID, out var ii))
                {
                    var i = ii[0];
                    string a = i.MyLessonsList; ;
                    if (i.guest)
                    {
                        if(f.TryGetUser(i.FriendKey, out var usrs))
                        {
                            foreach(var ex in usrs)
                            {
                                if (!ex.guest)
                                {
                                    a = ex.MyLessonsList;
                                }
                            }
                        }
                    }

                    if (a == null)
                        return;
                    int dayNumber = (int)DateTime.Now.DayOfWeek;

                    try
                    {
                        List<DaySchedule> splited = SplitMyString(a);


                        List<DaySchedule> splitResult = Split.NextFor2(splited, IntToWeekday(dayNumber));


                        string outp = "\n";

                        int yte = (i.guest ? 1 : 0);

                        for (int gah = 0; gah < splitResult[yte].Count; gah++)
                        {
                            var subject = splitResult[yte].S[gah];
                            string title = subject.Title;
                            uint weight = subject.WeightG;
                            outp += $"- {title} [{weight}г]\n";
                        }
                        tgc.SendMessage($"{i.usrName}, вот твое расписание на завтрашний день!{outp}", i.userID);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        tgc.SendMessage($"{i.usrName}, похоже, ты плохо заполнил поле, или сделал это некорректно (либо твой админ хаха)\nЕсли ты умный, и можешь сам все починить, то вот ошибка в твоем расписании:\n{e.Message}", i.userID);
                        return;
                    }
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
                DaySchedule c = new DaySchedule();
                foreach (string lesson in a[j].Split('\n'))
                {
                    if (lesson.Contains(' '))
                    {
                        var b = lesson.Split(' ');
                        foreach(var bb in b)
                        {
                            Console.WriteLine(bb);
                        }
                        c.AddSubject(new Subject(b[0], uint.Parse(b[1])));
                    }
                    else
                    {
                        c.AddSubject(new Subject(lesson));
                    }
                }
                LessonsLst.Add(c);
            }
            return LessonsLst;
        }
    }
}

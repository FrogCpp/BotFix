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
                    if (!i.isSendedToday && now.Hour >= i.Time.Hour && now.Minute >= i.Time.Minute)
                    {
                        mainFunc(i.userID);

                        i.isSendedToday = true;

                    }

                    if (now.Hour == 0 && now.Minute == 1)
                    {
                        i.isSendedToday = false;

                        int Hours = i.Time.Hour, Minutes = i.Time.Minute;

                        i.Time = now;
                        i.Time.AddHours(Hours);
                        i.Time.AddMinutes(Minutes);

                        if (now.Month == 1 && now.Day == 1)
                        {
                            tgc.SendMessage($"{i.usrName}, с новым годом!", i.userID);
                        }
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
                    string a = i.MyLessonsList;
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

                    try
                    {
                        List<DaySchedule> splited = SplitMyStringBeta(a);

                        List<DaySchedule> splitResult = Split.NextFor2(splited, GetCurrentWeekday(1));


                        string outp = "\n";

                        int yte = (i.guest ? 1 : 0);

                        for (int gah = 0; gah < splitResult[yte].Count; gah++)
                        {
                            var subject = splitResult[yte].S[gah];
                            string title = subject.Title;
                            uint weight = subject.WeightG;
                            outp += $"- {title} [{weight}г]\n";
                        }
                        tgc.SendMessage($"{i.usrName}, вот твое расписание на {WeekdayToString(GetCurrentWeekday(1))}!{outp}", i.userID);
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


        public void FixDeathConsequences()
        {
            DateTime now = DateTime.Now;

            using (var f = new FileManager())
            {
                foreach (var i in f.MyUsers)
                {
                    i.isSendedToday = false;
                }
            }
        }
        public Weekday GetCurrentWeekday(int offset = 0)
        {
            DateTime targetDate = DateTime.Now.AddDays(offset);
            int dayNumber = (int)targetDate.DayOfWeek;
            return dayNumber == 0 ? Weekday.Sunday : (Weekday)dayNumber;
        }

        static private string WeekdayToString(Weekday weekday)
        {
            return weekday switch
            {
                Weekday.Monday => "Понедельник",
                Weekday.Tuesday => "Вторник",
                Weekday.Wednesday => "Среду",
                Weekday.Thursday => "Четверг",
                Weekday.Friday => "Пятницу",
                Weekday.Saturday => "Субботу",
                Weekday.Sunday => "Воскресенье",
                _ => "Undefined"
            };
        }


        private List<DaySchedule> SplitMyStringBeta(string text)
        {
            List<DaySchedule> lessonsList = new();

            string[] dayBlocks = text.Split("*\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string dayBlock in dayBlocks)
            {
                DaySchedule daySchedule = new DaySchedule();
                string[] lessons = dayBlock.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (string lesson in lessons)
                {
                    string[] parts = lesson.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    switch (parts.Length)
                    {
                        case 3:
                            string name = parts[0];
                            if (uint.TryParse(parts[1], out uint duration))
                            {
                                daySchedule.AddSubject(new Subject(name, duration, parts[2] == "1", true));
                            }
                            break;

                        case 2:
                            if (uint.TryParse(parts[1], out uint duration2))
                            {
                                daySchedule.AddSubject(new Subject(parts[0], duration2));
                            }
                            break;

                        case 1:
                            daySchedule.AddSubject(new Subject(parts[0], 0));
                            break;
                    }
                }
                lessonsList.Add(daySchedule);
            }

            return lessonsList;
        }
    }
}

using System;
using System.Reflection;

using static System.Console;



namespace BotFix
{
    public class SplitDemo
    {
        static public void Run()
        {
            Title = "Split demo v1.5.0";

            Write("\n\t\tEnter weekday number (1-7) or 0 to exit: ");
            string weekdayInput = ReadLine() ?? "";


            while (weekdayInput != "0" && weekdayInput != "")
            {
                Weekday weekday = StringToWeekday(weekdayInput);
                List<List<Subject>>? splitResult = Split.NextFor2(schedule, weekday);


                if (splitResult != null)
                {
                    var userCountAfterSplit = splitResult.Count;

                    if (userCountAfterSplit == 2)
                    {
                        Int32 weightUser1 = CalculateTotalWeight(splitResult[0]), weightUser2 = CalculateTotalWeight(splitResult[1]);

                        ForegroundColor = ConsoleColor.Green;
                        Write("\n\t\tSuccessfull splitting!");
                        ForegroundColor = ConsoleColor.Gray;


                        Write("\n\t\tWeight difference: ");
                        ForegroundColor = ConsoleColor.Green;
                        Write(Math.Abs(weightUser1 - weightUser2) + " g\n");
                        ForegroundColor = ConsoleColor.Gray;


                        for (var userNum = 0; userNum < 2; userNum++)
                        {
                            Write($"\n\t\tUser { userNum + 1 }: " + splitResult[userNum].Count + " textbooks " +
                                    "[" + CalculateTotalWeight(splitResult[userNum]) + " g]");

                            for (var i = 0; i < splitResult[userNum].Count; i++)
                                Write("\n\t\t - " + splitResult[userNum][i].Title 
                                            + " [" + splitResult[userNum][i].WeightG + " g]");
                        }
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        Write("\n\t\tSplit error! ");
                        ForegroundColor = ConsoleColor.Gray;
                        Write("\n\t\tUnexpected amount of users: " + userCountAfterSplit);
                    }
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("\n\t\tFatal error! ");
                    ForegroundColor = ConsoleColor.Gray;
                    Write("\n\t\tSplit result is null");
                }

                Write("\n\n\t\tEnter weekday number (1-7) or 0 to exit: ");
                weekdayInput = ReadLine() ?? "";
            }
        }

        static private Weekday StringToWeekday(string input)
        {
            return input.ToLower() switch
            {
                "1" or "mon" or "monday"    => Weekday.Monday,
                "2" or "tue" or "tuesday"   => Weekday.Tuesday,
                "3" or "wed" or "wednesday" => Weekday.Wednesday,
                "4" or "thu" or "thursday"  => Weekday.Thursday,
                "5" or "fri" or "friday"    => Weekday.Friday,
                "6" or "sat" or "saturday"  => Weekday.Saturday,
                "7" or "sun" or "sunday"    => Weekday.Sunday,
                _                           => Weekday.Undefined
            };
        }



        static private Int32 CalculateTotalWeight(List<Subject> subjects)
        {
            Int32 totalWeight = 0;
            foreach (var subject in subjects) totalWeight += (Int32)subject.WeightG;

            return totalWeight;
        }



        static private readonly List<List<Subject>> schedule =
                [
                    [
                    new("РоВ"),
                    new("Физика", 599),
                    new("Алгебра", 567),
                    new("Алгебра", 567),
                    new("Русский", 577),
                    new("Русский", 577)
                ],

                [
                    new("Информатика", 408),
                    new("Информатика", 408),
                    new("Геометрия", 364),
                    new("Геометрия", 364),
                    new("Физкультура"),
                    new("Всеобщая история", 392),
                    new("Всеобщая история", 392)
                ],

                [
                    new("Английский", 538),
                    new("Английский", 538),
                    new("Физика", 599),
                    new("Физика", 599),
                    new("Биология", 585),
                    new("ОБЗР")
                ],

                [
                    new("Химия", 294),
                    new("Алгебра", 567),
                    new("Алгебра", 567),
                    new("ТеорВер"),
                    new("Литература", 480),
                    new("Литература", 480),
                    new("География", 483),
                ],

                [
                    new("Информатика", 408),
                    new("Информатика", 408),
                    new("Английский", 538),
                    new("Английский", 538),
                    new("Физика", 599),
                    new("Физика", 599),
                ],

                [
                    new("Физкультура"),
                    new("Геометрия", 364),
                    new("СВМ"),
                    new("Литература", 480),
                    new("ПкСТ"),
                    new("Общество"),
                    new("Общество")
                ]
                ];
    }
}

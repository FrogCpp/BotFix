using System;
using System.Reflection;
using static System.Console;

namespace BotFix
{
    public class SplitDemo
    {
        static public void Run()
        {
            Title = "Split demo v1.3";

            Write("\n\t\tEnter weekday number (1-7) or 0 to exit: ");
            string weekdayInput = ReadLine() ?? "";

            while (weekdayInput != "0" && weekdayInput != "")
            {
                Weekday weekday = StringToWeekday(weekdayInput);
                List<List<Subject>> schedule =
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
                List<List<Subject>>? splitResult = Split.NextFor2(schedule, weekday);


                if (splitResult != null)
                {
                    var userCountAfterSplit = splitResult.Count;
                    if (userCountAfterSplit == 2)
                    {
                        var textbookCountAfterSplited = splitResult[0].Count + splitResult[1].Count;

                        if (textbookCountAfterSplited == 5 || true)
                        {
                            ForegroundColor = ConsoleColor.Green;
                            Write("\n\t\tSuccessfull splitting!");
                            ForegroundColor = ConsoleColor.Gray;

                            Write("\n\t\tUser 1: " + splitResult[0].Count);
                            for (var i = 0; i < splitResult[0].Count; i++)
                                Write("\n\t\t - " + splitResult[0][i].Title);

                            Write("\n\n\t\tUser 2: " + splitResult[1].Count);
                            for (var i = 0; i < splitResult[1].Count; i++)
                                Write("\n\t\t - " + splitResult[1][i].Title);
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            Write("\n\t\tSplit error! ");
                            ForegroundColor = ConsoleColor.Gray;
                            Write("\n\t\tOriginal textbook amount does not match the result sum");
                            Write("\n\t\tOriginal count: " + 5 + ", count after split: " + textbookCountAfterSplited);
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
                weekdayInput = ReadLine();
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
    }
}

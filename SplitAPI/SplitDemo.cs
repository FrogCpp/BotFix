using System;
using System.Reflection;

using static System.Console;



namespace BotFix
{
    public class SplitDemo
    {
        static public void Run()
        {
            InitDemo();
            Write("\n\n\n\t\tEnter weekday number (1-6) or 0 to exit: ");
            string weekdayInput = ReadLine() ?? "";


            while (weekdayInput != "0" && weekdayInput != "")
            {
                Weekday weekday = StringToWeekday(weekdayInput);

                InitDemo();
                if (WindowHeight < 26) Clear();
                Write("\n\n");

                if (weekday != Weekday.Undefined && (Byte)weekday < schedule.Count + 1)
                {
                    List<DaySchedule> splitResult = Split.NextFor2(schedule, weekday);

                    if (splitResult != null)
                    {
                        var userCountAfterSplit = splitResult.Count;

                        if (userCountAfterSplit == 2) SuccessMessage(splitResult, weekday);
                        else BroadcastError("Unexpected amount of users: " + userCountAfterSplit.ToString(), "Split");
                    }
                    else BroadcastError("Split result is null");
                }
                else BroadcastError("Invalid weekday input: " + weekdayInput, "Input");


                Write("\n\n\n\n\t\tEnter weekday number (1-6) or 0 to exit: ");
                weekdayInput = ReadLine() ?? "";
                Clear();
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
        static private string WeekdayToString(Weekday weekday)
        {
            return weekday switch
            {
                Weekday.Monday    => "Monday",
                Weekday.Tuesday   => "Tuesday",
                Weekday.Wednesday => "Wednesday",
                Weekday.Thursday  => "Thursday",
                Weekday.Friday    => "Friday",
                Weekday.Saturday  => "Saturday",
                Weekday.Sunday    => "Sunday",
                _                 => "Undefined"
            };
        }

        static private void SuccessMessage(List<DaySchedule> splitResult, Weekday weekday)
        {
            ForegroundColor = ConsoleColor.Green;
            Write("\n\t\tSuccessfull splitting!");



            ForegroundColor = ConsoleColor.Gray;
            Write("\n\t\tTextbooks for ");

            ForegroundColor = ConsoleColor.DarkCyan;
            Write(WeekdayToString(weekday));

            ForegroundColor = ConsoleColor.Gray;
            Write(" schedule:\n");



            SuccessMessage(splitResult);
        }
        static private void SuccessMessage(List<DaySchedule> split)
        {
            Int32[] weights =
            [
                split[0].TotalSigned,
                split[1].TotalSigned
            ];


            float[] avg = [split[0].Avg, split[1].Avg];
            float minAvg = Math.Min(avg[0], avg[1]), delta = Math.Abs(weights[0] - weights[1]);

            Write("\n\t\tWeight difference: ");
            ForegroundColor = delta < minAvg / 2 ? ConsoleColor.Green : ConsoleColor.Red;
            Write(delta + " g\n"); ForegroundColor = ConsoleColor.Gray;


            for (var userNum = 0; userNum < 2; userNum++)
            {
                Write($"\n\t\tUser {userNum + 1}: ");

                ForegroundColor = split[userNum].Count < split[1 - userNum].Count ? ConsoleColor.Green
                                : split[userNum].Count > split[1 - userNum].Count ? ConsoleColor.Red : ConsoleColor.DarkYellow;
                Write(split[userNum].Count + " textbooks ");

                ForegroundColor = ConsoleColor.DarkGray;
                Write("[");

                ForegroundColor = weights[userNum] < weights[1 - userNum] ? ConsoleColor.Green
                                : weights[userNum] > weights[1 - userNum] ? ConsoleColor.Red : ConsoleColor.DarkYellow;
                Write(weights[userNum]);

                ForegroundColor = ConsoleColor.DarkGray;
                Write("g]");

                for (var i = 0; i < split[userNum].Count; i++)
                {
                    ForegroundColor = ConsoleColor.Gray;
                    Write("\n\t\t - " + split[userNum].S[i].Title);
                    ForegroundColor = ConsoleColor.DarkGray;
                    Write(" [");

                    ForegroundColor = split[userNum].S[i].WeightG < avg[userNum] ? ConsoleColor.Green
                                    : split[userNum].S[i].WeightG > avg[userNum] ? ConsoleColor.Red : ConsoleColor.DarkYellow;
                    Write(split[userNum].S[i].WeightG);

                    ForegroundColor = ConsoleColor.DarkGray;
                    Write(" g]");
                }
                ForegroundColor = ConsoleColor.Gray;
            }
        }

        static private void InitDemo()
        {
            Title = "Split demo v1.6.0";
            const Int32 minWidth = 76, minHeight = 13, largeWidth = 143, largeHeight = 30;

            if (WindowWidth < largeWidth || WindowHeight < largeHeight)
            {
                do
                {
                    Clear(); Write("\n");


                    Write("\n\t\t\tSplitAPI demo v1.6.0\n");

                    Write($"\n\t[!]  - Please resize the console window to at least {minWidth}x{minHeight}");

                    Write( "\n\t       Current size: ");
                    ForegroundColor = WindowWidth >= minWidth ? ConsoleColor.Green : ConsoleColor.Red;
                    Write(WindowWidth);

                    ForegroundColor = ConsoleColor.DarkGray;
                    Write(" (X) x ");

                    ForegroundColor = WindowHeight >= minHeight ? ConsoleColor.Green : ConsoleColor.Red;
                    Write(WindowHeight);

                    ForegroundColor = ConsoleColor.DarkGray;
                    Write(" (Y)");
                    ForegroundColor = ConsoleColor.Gray;

                    if (WindowWidth < minWidth || WindowHeight < minHeight)
                    {
                        Write("\n\n\t[i]  - Any key press will refresh the current window size");

                        ForegroundColor = ConsoleColor.Black; 
                        ReadKey();
                        ForegroundColor = ConsoleColor.Gray;
                    }
                } while (WindowWidth < minWidth || WindowHeight < minHeight);


                Clear();
                SmallLogo();
            }
            else
            {
                Clear();
                LargeLogo();
            }
        }
        static private void SmallLogo()
        {
            Int32 width = WindowWidth;
            string padding = new(' ', (width - 76) / 2);


            Write("\n" + padding + " ad88888ba                88  88                  db         88888888ba   88");
            Write("\n" + padding + "d8\"     \"8b               88  \"\"    ,d           d88b        88      \"8b  88");
            Write("\n" + padding + "Y8,                       88        88          d8'`8b       88      ,8P  88");
            Write("\n" + padding + "`Y8aaaaa,    8b,dPPYba,   88  88  MM88MMM      d8'  `8b      88aaaaaa8P'  88");
            Write("\n" + padding + "  `\"\"\"\"\"8b,  88P'    \"8a  88  88    88        d8YaaaaY8b     88\"\"\"\"\"\"'    88");
            Write("\n" + padding + "        `8b  88       d8  88  88    88       d8\"\"\"\"\"\"\"\"8b    88           88");
            Write("\n" + padding + "Y8a     a8P  88b,   ,a8\"  88  88    88,     d8'        `8b   88           88");
            Write("\n" + padding + " \"Y88888P\"   88`YbbdP\"'   88  88    \"Y888  d8'          `8b  88           88");
            Write("\n" + padding + "             88");
            Write("\n" + padding + "             88");
        }
        static private void LargeLogo()
        {
            Int32 width = WindowWidth;
            string padding = new(' ', (width - 143) / 2);


            Write("\n" + padding + " ad88888ba                88  88                  db         88888888ba   88                           88       88888888888P       ,a8888a,");
            Write("\n" + padding + "d8\"     \"8b               88  \"\"    ,d           d88b        88      \"8b  88                         ,d88               ,8P      ,8P\"'  `\"Y8,");
            Write("\n" + padding + "Y8,                       88        88          d8'`8b       88      ,8P  88                       888888              d8\"      ,8P        Y8,");
            Write("\n" + padding + "`Y8aaaaa,    8b,dPPYba,   88  88  MM88MMM      d8'  `8b      88aaaaaa8P'  88         8b       d8       88         d888888b      88          88");
            Write("\n" + padding + "  `\"\"\"\"\"8b,  88P'    \"8a  88  88    88        d8YaaaaY8b     88\"\"\"\"\"\"'    88         `8b     d8'       88           d8\"         88          88");
            Write("\n" + padding + "        `8b  88       d8  88  88    88       d8\"\"\"\"\"\"\"\"8b    88           88          `8b   d8'        88         ,8P'          `8b        d8'");
            Write("\n" + padding + "Y8a     a8P  88b,   ,a8\"  88  88    88,     d8'        `8b   88           88           `8b,d8'         88  888   d8\"       888   `8ba,  ,ad8'");
            Write("\n" + padding + " \"Y88888P\"   88`YbbdP\"'   88  88    \"Y888  d8'          `8b  88           88             \"8\"           88  888  8P'        888     \"Y8888P\"");
            Write("\n" + padding + "             88");
            Write("\n" + padding + "             88");
            Write("\n" + padding + "             88");
        }

        static private void BroadcastError(string errorMessage, string errorType = "Fatal")
        {
            ForegroundColor = ConsoleColor.Red;
            Write($"\n\t\t{errorType} error! ");

            ForegroundColor = ConsoleColor.Gray;
            Write($"\n\t\t{errorMessage}");
        }



        //  Sample schedule for testing
        static private readonly List<DaySchedule> schedule =
        [
            new
            (
                [
                    new ("РоВ"),
                    new ("Физика", 599),
                    new ("Алгебра", 567),
                    new ("Алгебра", 567),
                    new ("Русский", 577),
                    new ("Русский", 577)
                ]
            ),

            new
            (
                [
                    new ("Информатика", 408),
                    new ("Информатика", 408),
                    new ("Геометрия", 364),
                    new ("Геометрия", 364),
                    new ("Физкультура"),
                    new ("Всеобщая история", 392),
                    new ("Всеобщая история", 392)
                ]
            ),

            new
            (
                [
                    new("Английский", 538),
                    new("Английский", 538),
                    new("Физика", 599),
                    new("Физика", 599),
                    new("Биология", 585),
                    new("ОБЗР")
                ]
            ),

            new
            (
                [
                    new("Химия", 294),
                    new("Алгебра", 567),
                    new("Алгебра", 567),
                    new("ТеорВер"),
                    new("Литература", 480),
                    new("Литература", 480),
                    new("География", 483),
                ]
            ),

            new
            (
                [
                    new("Информатика", 408),
                    new("Информатика", 408),
                    new("Английский", 538),
                    new("Английский", 538),
                    new("Физика", 599),
                    new("Физика", 599),
                ]
            ),

            new
            (
                [
                    new("Физкультура"),
                    new("Геометрия", 364),
                    new("СВМ"),
                    new("Литература", 480),
                    new("ПкСТ"),
                    new("Общество"),
                    new("Общество")
                ]
            )
        ];
    }
}

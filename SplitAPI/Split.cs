using System;
using System.Linq;
using System.Collections.Generic;


namespace BotFix
{
    public class Split
    {
        public static List<List<Subject>>? NextFor2(List<List<Subject>> fullSchedule, Weekday weekday, out Int32 bitMaskResult, bool throwException = true)
        {
            WeekdayOverloadValidate(fullSchedule, weekday, throwException);

            if (throwException) return NextFor2(fullSchedule[(Byte)weekday - 1], out bitMaskResult);
            else
            {
                try
                {
                    return NextFor2(fullSchedule[(Byte)weekday - 1], out bitMaskResult);
                }
                catch 
                { 
                    bitMaskResult = 0;
                    return null; 
                }
            }
        }
        public static List<List<Subject>>? NextFor2(List<List<Subject>> fullSchedule, Weekday weekday, bool throwException = true)
        {
            WeekdayOverloadValidate(fullSchedule, weekday, throwException);

            if (throwException) return NextFor2(fullSchedule[(Byte)weekday - 1]);
            else
            {
                try
                {
                    return NextFor2(fullSchedule[(Byte)weekday - 1]);
                }
                catch 
                { 
                    return null; 
                }
            }
        }

        private static bool WeekdayOverloadValidate(List<List<Subject>> fullSchedule, Weekday weekday, bool throwException = true)
        {
            bool valid = false;

            if ((Byte)weekday < 1 || (Byte)weekday > 7)
            {
                if (throwException)
                {
                    throw new ArgumentOutOfRangeException
                    (
                        nameof(weekday),
                        "Invalid weekday code, must be between 1 (monday) and 7 (sunday), selected: "
                        + weekday
                    );
                }
            }
            else if (fullSchedule == null || fullSchedule.Count == 0)
            {
                if (throwException)
                {
                    throw new ArgumentException
                    (
                        "The schedule list cannot be null or empty.",
                        nameof(fullSchedule)
                    );
                }
            }
            else if (fullSchedule.Count < (Byte)weekday)
            {
                if (throwException)
                {
                    throw new ArgumentException
                    (
                        "Invalid schedule for selected day: " + weekday,
                        nameof(weekday) + ", " + nameof(fullSchedule)
                    );
                }
            }

            else valid = true;

            return valid;
        }



        public static List<List<Subject>>? NextFor2(List<Subject> currentSubjects, out Int32 bitMaskResult, bool throwException = true)
        {
            float perfectWeight = 0;
            UInt32 min = UInt32.MaxValue, max = 0, buffer;
            Int32 minId = 0, maxId = 0;


            for (var id1 = 0; id1 < currentSubjects.Count; id1++)
            {
                perfectWeight += currentSubjects[id1].WeightG;

                for (var id2 = id1 + 1; id2 < currentSubjects.Count; id2++)
                {
                    if (currentSubjects[id1].Id == currentSubjects[id2].Id)
                    { 
                        currentSubjects.RemoveAt(id1);
                        perfectWeight -= currentSubjects[id1].WeightG;

                        id2 += currentSubjects.Count;
                        id1--;
                    }
                }
            }
            perfectWeight /= 2;

            for (var i = 0; i < currentSubjects.Count; i++)
            {
                if (!currentSubjects[i].HasTextbook)
                {
                    currentSubjects.RemoveAt(i);
                    i--;
                    continue;
                }

                for (var j = i; j < currentSubjects.Count - i; j++)
                {
                    if (currentSubjects[j].WeightG < min)
                    {
                        min = currentSubjects[i].WeightG;
                        minId = j;
                    }
                    else if (currentSubjects[j].WeightG > max)
                    {
                        max = currentSubjects[i].WeightG;
                        maxId = j;
                    }
                }

                buffer = currentSubjects[i].WeightG;
                currentSubjects[i].ChangeWeight(min);
                currentSubjects[minId].ChangeWeight(buffer);

                buffer = currentSubjects[^(i + 1)].WeightG;
                currentSubjects[^(i + 1)].ChangeWeight(max);
                currentSubjects[maxId].ChangeWeight(buffer);
            }


            List<Subject> sortedUniqueSubjects = currentSubjects;

            for (var i = sortedUniqueSubjects.Count - 1; i > 0; i--)
            {
                if (sortedUniqueSubjects[i].WeightG > perfectWeight)
                    sortedUniqueSubjects.RemoveAt(i);
                else break;
            }

            if (throwException)  return FilteredDataSplitFor2(sortedUniqueSubjects, perfectWeight, out bitMaskResult);
            else
            {
                try
                {
                    return FilteredDataSplitFor2(sortedUniqueSubjects, perfectWeight, out bitMaskResult);
                }
                catch
                {
                    bitMaskResult = 0;
                    return null;
                }
            }
        }
        public static List<List<Subject>>? NextFor2(List<Subject> currentSubjects, bool throwException = true) 
            => NextFor2(currentSubjects, out _, throwException);



        private static List<List<Subject>> FilteredDataSplitFor2(List<Subject> allUnique, float perfectWeight, out Int32 bitMaskOfBestIteration)
        {
            float minDiff = float.MaxValue, curCalcSum;
            Int32 curIterationBitMask, subjectCount = allUnique.Count;
            var totalIterations = Math.Pow(2, subjectCount - 1);

            bitMaskOfBestIteration = 0;  //  End result as a 32 bit mask

            for (var curIteration = 0; curIteration < totalIterations; curIteration++)
            {
                var curCombo = curIteration;
                curIterationBitMask = 0;
                curCalcSum = 0;

                for (Int32 j = 0; j < subjectCount; j++)
                {
                    if ((curCombo & 1) == 1)
                    {
                        curCalcSum += allUnique[j].WeightG;
                        curIterationBitMask |= 1 << j;
                    }
                    curCombo >>= 1;

                    if (curCalcSum > perfectWeight)
                    {
                        var largeDelta = Math.Abs(perfectWeight - curCalcSum + allUnique[j].WeightG);
                        if (largeDelta < minDiff)
                        {
                            minDiff = curCalcSum - allUnique[j].WeightG;
                            bitMaskOfBestIteration = curIterationBitMask & ~(1 << j);
                        }

                        j += subjectCount; // break loop after exceeding half weight point
                    }
                }

                var delta = Math.Abs(curCalcSum - perfectWeight);
                if (delta < minDiff)
                {
                    minDiff = delta;
                    bitMaskOfBestIteration = curIterationBitMask;
                }
            }


            List<Subject> user1 = [], user2 = [];
            curIterationBitMask = bitMaskOfBestIteration;

            for (Int32 i = 0; i < allUnique.Count; i++)
            {
                if ((curIterationBitMask & 1) == 1) user1.Add(allUnique[i]);
                else user2.Add(allUnique[i]);
                curIterationBitMask >>= 1;
            }


            return [user1, user2];
        }
    }
}

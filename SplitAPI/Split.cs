using System;
using System.Linq;
using System.Collections.Generic;



namespace BotFix
{
    public class Split
    {
        static private bool ValidateWeekday(List<DaySchedule> fullSchedule, Weekday weekday, bool throwException = true)
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



        static public List<DaySchedule> NextFor2(List<DaySchedule> fullSchedule, Weekday weekday, out Int32 bitMaskResult, bool throwException = true)
        {
            ValidateWeekday(fullSchedule, weekday, throwException);

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
                    return []; 
                }
            }
        }
        static public List<DaySchedule> NextFor2(List<DaySchedule> fullSchedule, Weekday weekday, bool throwException = true)
            => NextFor2(fullSchedule, weekday, out _, throwException);

        static public List<DaySchedule> NextFor2(DaySchedule currentSubjects, out Int32 bitMaskResult, bool throwException = true)
        {
            float perfectWeight = 0;
            List<Subject> unsplittableTextbooks = [];

            for (var id1 = 0; id1 < currentSubjects.Count; id1++)
            {
                perfectWeight += currentSubjects.S[id1].WeightG;

                if (!currentSubjects.S[id1].HasTextbook)
                {
                    perfectWeight -= currentSubjects.S[id1].WeightG;
                    currentSubjects.S.RemoveAt(id1);

                    id1--;
                    continue;
                }
                if (!currentSubjects.S[id1].CanBeSplit)
                {
                    unsplittableTextbooks.Add(currentSubjects.S[id1]);

                    perfectWeight -= currentSubjects.S[id1].WeightG;
                    currentSubjects.S.RemoveAt(id1);

                    id1--;
                    continue;
                }
                for (var id2 = id1 + 1; id2 < currentSubjects.Count; id2++)
                {
                    if (currentSubjects.S[id1].Id == currentSubjects.S[id2].Id)
                    { 
                        perfectWeight -= currentSubjects.S[id1].WeightG;
                        currentSubjects.S.RemoveAt(id1);

                        id1--;
                        id2 += currentSubjects.Count;
                    }
                }
            }
            perfectWeight /= 2;

            
            List<Subject> sortedUniqueSubjects = DaySchedule.Sorted(currentSubjects.Subjects, SortType.LowerWeightFirst);

            var heaviestSortedId = sortedUniqueSubjects.Count - 1;
            if (heaviestSortedId >= 0 && sortedUniqueSubjects[heaviestSortedId].WeightG >= perfectWeight)
            {
                Subject heaviest = sortedUniqueSubjects[heaviestSortedId];
                sortedUniqueSubjects.RemoveAt(heaviestSortedId);

                bitMaskResult = 1 << heaviestSortedId;
                return 
                [
                    new ( CombineLists(   unsplittableTextbooks, [ heaviest ]   ) ),
                    new ( CombineLists(   unsplittableTextbooks, sortedUniqueSubjects   ) )
                ];  
            }

            if (throwException) return SplitCombineLogic(sortedUniqueSubjects, perfectWeight, out bitMaskResult, unsplittableTextbooks);
            else
            {
                try
                {
                    return SplitCombineLogic(sortedUniqueSubjects, perfectWeight, out bitMaskResult, unsplittableTextbooks);
                }
                catch
                {
                    bitMaskResult = 0;
                    return [];
                }
            }
        }
        static public List<DaySchedule> NextFor2(DaySchedule currentSubjects, bool throwException = true) 
            => NextFor2(currentSubjects, out _, throwException);



        static private List<T> CombineLists<T>(List<T> list1, List<T> list2)
        {
            List<T> combined = new(list1);
            combined.AddRange(list2);

            return combined;
        }
        static private List<DaySchedule> SplitCombineLogic(List<Subject> allUnique, float perfectWeight, out Int32 bitMaskOfBestIteration, List<Subject> unsplittableTextbooks)
        {
            List<DaySchedule> split = FilteredDataSplitFor2(allUnique, perfectWeight, out bitMaskOfBestIteration);

            split[0].S.AddRange(unsplittableTextbooks);
            split[1].S.AddRange(unsplittableTextbooks);

            return split;
        }
        static private List<DaySchedule> FilteredDataSplitFor2(List<Subject> allUnique, float perfectWeight, out Int32 bitMaskOfBestIteration)
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


            return [ new (user1), new (user2) ];
        }
    }
}

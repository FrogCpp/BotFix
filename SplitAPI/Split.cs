using System;
using System.Linq;
using System.Collections.Generic;


namespace TextbookSplitterAPI
{
    public class Split
    {
        public static List<List<Subject>> NextFor2(List<List<Subject>> fullSchedule, Weekday weekDay)
        {
            if ((Byte)weekDay < 0 || (Byte)weekDay > 7)
                throw new ArgumentOutOfRangeException
                    ("Invalid weekday code, must be between 1 (monday) and 7 (sunday), selected: " + weekDay);

            if (fullSchedule == null || fullSchedule.Count == 0)
                throw new ArgumentException("The schedule list cannot be null or empty.");

            if (fullSchedule.Count <= (Byte)weekDay)
                throw new ArgumentException
                    ("Invalid schedule for selected day: " + weekDay);

            return NextFor2(fullSchedule[(Byte)weekDay]);
        }
        public static List<List<Subject>> NextFor2(List<Subject> currentSubjects, out Int32 bitMaskResult)
        {
            float perfectWeight = 0;
            UInt32 min = UInt32.MaxValue, max = 0, buffer;
            Int32 minId = 0, maxId = 0;


            foreach (var subject in currentSubjects)
                perfectWeight += subject.WeightG;
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

                buffer = currentSubjects[currentSubjects.Count - i].WeightG;
                currentSubjects[currentSubjects.Count - i].ChangeWeight(max);
                currentSubjects[maxId].ChangeWeight(buffer);
            }


            List<Subject> sortedUniqueSubjects = currentSubjects.Distinct().ToList();

            for (var i = sortedUniqueSubjects.Count - 1; i > 0; i--)
            {
                if (sortedUniqueSubjects[i].WeightG > perfectWeight)
                    sortedUniqueSubjects.RemoveAt(i);
                else break;
            }

            return FilteredDataSplitFor2(sortedUniqueSubjects, perfectWeight, out bitMaskResult);
        }
        public static List<List<Subject>> NextFor2(List<Subject> currentSubjects) => NextFor2(currentSubjects, out _);



        private static List<List<Subject>> FilteredDataSplitFor2(List<Subject> allUnique, float perfectWeight, out Int32 bitMaskOfBestIteration)
        {
            float minDiff = float.MaxValue, curCalcSum;
            Int32 curIterationBitMask, subjectCount = allUnique.Count;
            var totalIterations = Math.Pow(2, subjectCount);

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
                        if ((curCalcSum - perfectWeight) < minDiff)
                        {
                            minDiff = curCalcSum - perfectWeight;
                            bitMaskOfBestIteration = curIterationBitMask;
                        }
                        if (Math.Abs(perfectWeight - curCalcSum + allUnique[j].WeightG) < minDiff)
                        {
                            minDiff = curCalcSum - allUnique[j].WeightG;
                            bitMaskOfBestIteration = curIterationBitMask & ~(1 << j);
                        }

                        j += subjectCount; // break loop after exceeding half weight point
                    }
                }
            }


            List<Subject> person1 = new List<Subject>();
            List<Subject> person2 = new List<Subject>();
            curIterationBitMask = bitMaskOfBestIteration;

            for (Int32 i = 0; i < allUnique.Count; i++)
            {
                if ((curIterationBitMask & 1) == 1) person1.Add(allUnique[i]);
                else person2.Add(allUnique[i]);
                curIterationBitMask >>= 1;
            }


            return new List<List<Subject>> { person1, person2 };
        }
    }
}

using System;



namespace BotFix
{
    public enum Weekday
    {
        Undefined = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }
    public enum SortType
    {
        HigherWeightFirst = -1,
        LowerWeightFirst  = 1,
    }

    public class Subject
    {
        private Int32  _id;
        private string _title;
        private UInt32 _weightG;
        private bool   _hasTextbook, _canBeSplit;



        public Subject(string title, UInt32 weightG, bool hasTextBook = true, bool canBeSplit = true)
        {
            _title = title;
            _weightG = weightG;
            _hasTextbook = hasTextBook;
            _canBeSplit = canBeSplit;

            AssignSubjectId();
        }
        public Subject(string title)
        {
            _title = title;
            _weightG = 0;
            _hasTextbook = false;
            _canBeSplit = false;

            AssignSubjectId();
        }



        public void HardChangeTitle(string newTitle) => _title = newTitle;
        public void ChangeTitle(string newTitle)
        {
            _title = newTitle;
            AssignSubjectId();
        }
        public void ChangeWeight(UInt32 newWeightG) => _weightG = newWeightG;
        public void ChangeFlags(bool hasTextbook, bool canBeSplit)
        {
            _hasTextbook = hasTextbook;
            _canBeSplit = canBeSplit;
        }



        private void AssignSubjectId() => AssignSubjectId(_title);
        private void AssignSubjectId(string subjectTitle)
        {
            _id = 0;

            var uniqueCount = ValidSubjects.Titles.Count;
            for (var unique = 0; unique < uniqueCount; unique++)
            {
                var localCount = ValidSubjects.Titles[unique].Count;
                for (var local = 0; local < localCount; local++)
                {
                    if (subjectTitle.Trim().ToLower() == ValidSubjects.Titles[unique][local])
                    {
                        _id = unique;

                        local += localCount;   //  After successfull id match
                        unique += uniqueCount;  //  Exit both seacrh loops
                    }
                }
            }
        }



        public Int32 Id => _id;
        public string Title => _title;
        public UInt32 WeightG => _weightG;
        public bool HasTextbook => _hasTextbook;
        public bool CanBeSplit => _canBeSplit;
    }


    public class DaySchedule
    {
        private Weekday _weekDay;
        private List<Subject> _subjects = [];



        public DaySchedule(List<Subject> subjects, Weekday weekDay)
        {
            _subjects.AddRange(subjects);
            _weekDay = weekDay;
        }
        public DaySchedule(List<Subject> subjects)
        {
            _subjects.AddRange(subjects);
            _weekDay = Weekday.Undefined;
        }
        public DaySchedule(DaySchedule daySchedule)
        {
            _subjects.AddRange(daySchedule.Subjects);
            _weekDay = daySchedule.WeekDay;
        }
        static public List<DaySchedule> Convert(List<List<Subject>> subjects)
        {
            List<DaySchedule> schedules = [];
            for (var i = 0; i < subjects.Count; i++)
                schedules.Add(new (subjects[i]));
            
            return schedules;
        }
        public void FullReset()
        {
            _subjects.Clear();
            _weekDay = Weekday.Undefined;
        }


        public List<Subject> Subjects => _subjects;
        public List<Subject> S => _subjects;    //  Short alias
        public Int32 Count => _subjects.Count;
        public Weekday WeekDay => _weekDay;



        public UInt32 MaxWeightG()
        {
            UInt32 max = 0;

            foreach (var subject in _subjects)
                if (subject.WeightG > max)
                    max = subject.WeightG;

            return max;
        }
        public UInt32 Max => MaxWeightG();      //  Short alias
        public UInt32 MinWeightG()
        {
            UInt32 min = UInt32.MaxValue;

            foreach (var subject in _subjects)
                if (subject.WeightG < min)
                    min = subject.WeightG;

            return min;
        }
        public UInt32 Min => MinWeightG();      //  Short alias
        public float AverageWeightG()
        {
            if (_subjects.Count == 0) return 0;
            return (float)TotalWeightG() / TextbookCount();
        }
        public float Avg => AverageWeightG();   //  Short alias
        public UInt32 TotalWeightG()
        {
            UInt32 total = 0;
            foreach (var subject in _subjects) total += subject.WeightG;
            return total;
        }
        public UInt32 Total => TotalWeightG();  //  Short alias
        public Int32 TotalSignedWeightG() => (Int32)TotalWeightG();
        public Int32  TotalSigned => (Int32)TotalWeightG();  //  Short alias



        static public List<Subject> USTsubjects(List<Subject> subjects)
        {
            List<Subject> usts = [];
            List<Int32> uniqueIds = [];

            foreach (var subject in subjects)
            {
                if (subject.HasTextbook && subject.CanBeSplit && !uniqueIds.Contains(subject.Id))
                {
                    uniqueIds.Add(subject.Id);
                    usts.Add(subject);
                }
            }

            return usts;
        }
        public List<Subject> USTsubjects() => USTsubjects(_subjects);

        public List<Subject> UniqueSubjects() => UniqueSubjects(_subjects);
        static public List<Subject> UniqueSubjects(List<Subject> subjects)
        {
            List<Subject> uniqueSubjects = [];
            List<Int32> uniqueIds = [];

            foreach (var subject in subjects)
            {
                if (!uniqueIds.Contains(subject.Id))
                {
                    uniqueIds.Add(subject.Id);
                    uniqueSubjects.Add(subject);
                }
            }

            return uniqueSubjects;
        }

        public List<Subject> SplittableTextbookSubjects() => SplittableTextbookSubjects(_subjects);
        static public List<Subject> SplittableTextbookSubjects(List<Subject> subjects)
        {
            List<Subject> textbooks = [];

            foreach (var subject in subjects)
                if (subject.HasTextbook && subject.CanBeSplit)
                    textbooks.Add(subject);

            return textbooks;
        }

        public List<Subject> TextbookSubjects() => TextbookSubjects(_subjects);
        static public List<Subject> TextbookSubjects(List<Subject> subjects)
        {
            List<Subject> textbooks = [];

            foreach (var subject in subjects)
                if (subject.HasTextbook)
                    textbooks.Add(subject);

            return textbooks;
        }



        public Int32 USTcount() => USTcount(_subjects);
        static public Int32 USTcount(List<Subject> subjects)
        {
            Int32 count = 0;
            List<Int32> uniqueIds = [];

            foreach (var subject in subjects)
            {
                if (subject.HasTextbook && subject.CanBeSplit && !uniqueIds.Contains(subject.Id))
                {
                    uniqueIds.Add(subject.Id);
                    count++;
                }
            }

            return count;
        }

        public Int32 UniqueCount() => UniqueCount(_subjects);
        static public Int32 UniqueCount(List<Subject> subjects)
        {
            List<Int32> uniqueIds = [];

            foreach (var subject in subjects)
                if (!uniqueIds.Contains(subject.Id))
                    uniqueIds.Add(subject.Id);

            return uniqueIds.Count;
        }

        public Int32 SplittableTextbookCount() => SplittableTextbookCount(_subjects);
        static public Int32 SplittableTextbookCount(List<Subject> subjects)
        {
            Int32 count = 0;

            foreach (var subject in subjects)
                if (subject.HasTextbook && subject.CanBeSplit)
                    count++;

            return count;
        }

        public Int32 TextbookCount() => TextbookCount(_subjects);
        static public Int32 TextbookCount(List<Subject> subjects)
        {
            Int32 count = 0;

            foreach (var subject in subjects)
                if (subject.HasTextbook)
                    count++;

            return count;
        }



        public List<Subject> Sorted(SortType sortType) => Sorted(_subjects, sortType);
        static public List<Subject> Sorted(List<Subject> initial, SortType sortType)
        {
            List<Subject> sorted = [.. initial];
            Int32 minId = 0, maxId = 0;

            //  Voodoo magic for fast sorting type 
            Int32 direction = (Int32)sortType;
            Int32 Iterations = sorted.Count / 2 * (1 + direction);


            for (var i = sorted.Count - Iterations; i < Iterations; i += direction)
            {
                UInt32 min = UInt32.MaxValue, max = 0;

                for (var j = i; j < sorted.Count - i; j++)
                {
                    if (sorted[j].WeightG < min)
                    {
                        min = sorted[j].WeightG;
                        minId = j;
                    }
                    if (sorted[j].WeightG > max)
                    {
                        max = sorted[j].WeightG;
                        maxId = j;
                    }
                }

                (sorted[minId], sorted[i]) = (sorted[i], sorted[minId]);
                (sorted[maxId], sorted[^(i + 1)]) = (sorted[^(i + 1)], sorted[maxId]);
            }

            return sorted;
        }



        public void ChangeSubjects(List<Subject> newSubjects) => _subjects = newSubjects;
        public void AddSubject(Subject newSubject) => _subjects.Add(newSubject);
        public void RemoveSubject(Subject subject) => _subjects.Remove(subject);
        public void RemoveSubjectAt(Int32 index) => _subjects.RemoveAt(index);

        public void ChangeWeekday(Weekday newWeekday) => _weekDay = newWeekday;
    }

}

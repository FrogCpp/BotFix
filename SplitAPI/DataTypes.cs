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

    public class Subject
    {

        private Int32  _id;
        private string _title;
        private UInt32 _weightG;
        private readonly bool _hasTextbook;

        public Subject() { }// костыль для инициализации
        public Subject(string title, UInt32 weightG, bool hasTextBook = true)
        {
            _title = title;
            _weightG = weightG;
            _hasTextbook = hasTextBook;

            AssignSubjectId();
        }
        public Subject(string title)
        {
            _title = title;
            _weightG = 0;
            _hasTextbook = false;

            AssignSubjectId();
        }


        public void HardChangeTitle(string newTitle) => _title = newTitle;
        public void ChangeTitle (string newTitle)
        {
            _title = newTitle;
            AssignSubjectId();
        }
        public void ChangeWeight(UInt32 newWeightG) => _weightG = newWeightG;

        
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



        public Int32  Id        => _id;
        public string Title     => _title;
        public UInt32 WeightG   => _weightG;
        public bool HasTextbook => _hasTextbook;
    }
}

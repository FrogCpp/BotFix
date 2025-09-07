using System;

namespace TextbookSplitterAPI
{
    public enum Weekday
    {
        Undefined = 0,
        Monday    = 1,
        Tuesday   = 2,
        Wednesday = 3,
        Thursday  = 4,
        Friday    = 5,
        Saturday  = 6,
        Sunday    = 7
    }

    public class Subject
    {
        private string _title;
        private UInt32 _weightG;
        private readonly bool _hasTextbook;

        public Subject(string title, UInt32 weightG, bool hasTextBook = true)
        {
            _title = title;
            _weightG = weightG;
            _hasTextbook = hasTextBook;
        }
        public Subject(string title)
        {
            _title = title;
            _weightG = 0;
            _hasTextbook = false;
        }

        public void ChangeTitle (string newTitle)   => _title   = newTitle;
        public void ChangeWeight(UInt32 newWeightG) => _weightG = newWeightG;


        public string Title => _title;
        public UInt32 WeightG => _weightG;
        public bool HasTextbook => _hasTextbook;
    }
}

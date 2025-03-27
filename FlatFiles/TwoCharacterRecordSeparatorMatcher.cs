﻿namespace FlatFiles
{
    internal sealed class TwoCharacterRecordSeparatorMatcher : IRecordSeparatorMatcher
    {
        private readonly RetryReader reader;
        private readonly char first;
        private readonly char second;

        public TwoCharacterRecordSeparatorMatcher(RetryReader reader, char first, char second)
        {
            this.reader = reader;
            this.first = first;
            this.second = second;
        }

        public int Size => 2;

        public bool IsMatch()
        {
            return reader.IsMatch2(first, second);
        }

        public string Trim(string value)
        {
            var length = value.Length;
            if (length >= 2 && value[length - 2] == first && value[length - 1] == second)
            {
                return value.Substring(0, length - 2);
            }
            return value;
        }
    }
}

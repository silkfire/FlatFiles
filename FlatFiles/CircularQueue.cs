﻿using System;

namespace FlatFiles
{
    internal sealed class CircularQueue<T>
    {
        private readonly T[] items;
        private int front;
        private int back;

        public CircularQueue(int bufferSize)
        {
            items = new T[bufferSize];
        }

        public int Count { get; private set; }

        public ArraySegment<T> PrepareBlock()
        {
            var size = items.Length - Count;
            // The buffer is large enough to hold the new items.
            // Determine if we need to shift the items to create a contiguous block.
            if (back >= front && items.Length - back < size)
            {
                Array.Copy(items, front, items, 0, Count);
                front = 0;
                back = Count;
            }
            return new ArraySegment<T>(items, back, size);
        }

        public void RecordGrowth(int size)
        {
            Count += size;
            back += size;
            if (back >= items.Length)
            {
                back -= items.Length;
            }
        }

        public T Peek()
        {
            return items[front];
        }

        public T Peek(int index)
        {
            var position = front + index;
            if (position >= items.Length)
            {
                position -= items.Length;
            }
            return items[position];
        }

        public void Dequeue(int count)
        {
            front += count;
            if (front >= items.Length)
            {
                front -= items.Length;
            }
            Count -= count;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

namespace Algorithms.Collections
{
    public class LinkedList<T> : IList<T>
    {
        protected Node<T> firstNode;
        protected Node<T> lastNode;
        protected int count;

        public IEnumerator<T> GetEnumerator()
        {
            for (var x = firstNode; x != null; x = x.Next)
            {
                yield return x.Item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(T item)
        {

            if (firstNode == null)
            {
                var node = new Node<T> { Item = item };
                firstNode = node;
                lastNode = node;
            }
            else
            {
                lastNode = new Node<T> { Item = item, Previous = lastNode };
                lastNode.Previous.Next = lastNode;
            }

            count++;
        }

        public virtual void Clear()
        {
            firstNode = null;
            lastNode = null;
            count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var c = arrayIndex;
            for (var node = firstNode; node != null; node = node.Next)
            {
                array[c] = node.Item;
            }
        }

        public bool Remove(T item)
        {
            for (var node = firstNode; node != null; node = node.Next)
            {
                if (node.Item.Equals(item))
                {
                    if (node.Next != null)
                    {
                        node.Next.Previous = node.Previous;
                    }
                    if (node.Previous != null)
                    {
                        node.Previous.Next = node.Next;
                    }
                    if (node == firstNode)
                    {
                        firstNode = firstNode.Next;
                    }
                    else if (node == lastNode)
                    {
                        lastNode = lastNode.Previous;
                    }
                    count--;
                    return true;
                }
            }
            return false;
        }

        public virtual int Count { get { return count; } }

        public bool IsReadOnly { get { return false; } }

        public int IndexOf(T item)
        {
            var c = 0;
            for (var node = firstNode; node != null; node = node.Next)
            {
                if (node.Item.Equals(item))
                {
                    return c;
                }
                c++;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0)
                throw new IndexOutOfRangeException();
            if (index > count)
                throw new IndexOutOfRangeException();

            // 4 cases.
            if (index == count || (index == 0 && count == 0)) // insert at end or insert into empty.
            {
                Add(item);
            }
            else if (index == 0 && count != 0) // insert at start
            {
                firstNode = new Node<T> { Item = item, Next = firstNode };
                firstNode.Next.Previous = firstNode;
            }
            else if (count / index > 1) // traverse from start
            {
                var current = firstNode;
                var next = firstNode.Next; // idx=0
                for (int i = 1; i < count; i++)
                {
                    if (index == i)
                    {
                        var node = new Node<T> { Item = item, Next = next, Previous = current };
                        current.Next = node;
                        next.Previous = node;
                        break;
                    }
                    current = next;
                    next = current.Next;
                }
            }
            else // traverse from end
            {
                Node<T> current = null;
                var previous = lastNode; // idx=0
                for (int i = count; i > 0; i--)
                {
                    if (index == i)
                    {
                        current.Previous = new Node<T> { Item = item, Next = current, Previous = previous };
                        previous.Next = current.Previous;
                        break;
                    }
                    current = previous;
                    previous = current.Previous;
                }
            }
            count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException();
            if (index >= count)
                throw new IndexOutOfRangeException();
            if (count == 0)
                throw new IndexOutOfRangeException();

            if (index == 0) // remove start
            {
                firstNode = firstNode.Next;
                firstNode.Previous = null;
            }
            else if (index == count - 1) // remove end
            {
                lastNode = lastNode.Previous;
                lastNode.Next = null;
            }
            else if ((count - 1) / index > 1) // traverse from start
            {
                var current = firstNode;
                var next = firstNode.Next; // idx=0
                for (int i = 1; i < count; i++)
                {
                    if (index == i)
                    {
                        current.Next = next.Next;
                        current.Next.Previous = current; // the original 'next' is gone
                        break;
                    }
                    current = next;
                    next = current.Next;
                }
            }
            else // traverse from end
            {
                Node<T> current = null;
                var previous = lastNode; // idx=0
                for (int i = count; i > 0; i--)
                {
                    if (index == i)
                    {
                        previous.Next = current.Next;
                        previous.Next.Previous = previous;
                        break;
                    }
                    current = previous;
                    previous = current.Previous;
                }
            }
            count--;
        }

        public T this[int index]
        {
            get
            {
                if (index >= count || index < 0)
                    throw new IndexOutOfRangeException();

                var c = 0;
                for (var node = firstNode; node != null; node = node.Next)
                {
                    if (c == index)
                        return node.Item;
                    c++;
                }
                throw new InvalidOperationException();
            }
            set
            {
                if (index >= count || index < 0)
                    throw new IndexOutOfRangeException();

                var c = 0;
                for (var node = firstNode; node != null; node = node.Next)
                {
                    if (c == index)
                    {
                        node.Item = value;
                        break;
                    }
                    c++;
                }
            }
        }

        protected class Node<TN>
        {
            internal TN Item;
            internal Node<TN> Next;
            internal Node<TN> Previous;

            public override string ToString()
            {
                return string.Format("Item: {0}, Next: {1}, Previous: {2}", Item, Next.Item, Previous.Item);
            }
        }
    }
}
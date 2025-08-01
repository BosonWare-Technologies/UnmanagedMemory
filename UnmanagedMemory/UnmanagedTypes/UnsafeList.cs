using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnmanagedMemory.Annotations;
using UnmanagedMemory.Diagnostics;

namespace UnmanagedMemory.UnmanagedTypes;

/// <summary>
/// Represents a native list of elements.
/// <para>
/// This class is Experimental.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerTypeProxy(typeof(UnsafeListDebuggerProxy<>))]
public unsafe partial class UnsafeList<T> 
    : UnmanagedObject, IEnumerable<T> where T : unmanaged
{
    /// <summary>
    /// Enumerator for <see cref="UnsafeMemory{T}"/>, enabling iteration over the elements.
    /// </summary>
    public struct Enumerator(UnsafeList<T> list) : IEnumerator<T>
    {
        private int _index = -1;

        public readonly T Current => list[_index];

        readonly object IEnumerator.Current => Current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_index + 1 >= list.Count)
                return false;

            _index++;

            return true;
        }

        public void Reset() => _index = -1;

        public readonly void Dispose() { }
    }

    internal readonly UnsafeMemory<T> _items;
    
    public int GrowthRate { get; set; }

    public int Capacity => _items.Length;
    
    public int Count { get; private set; }
    
    public int Size => Count * sizeof(T);

    /// <summary>
    /// Gets a reference to the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.
    /// </exception>
    public ref T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if (index < 0 || index >= Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return ref _items[index];
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeList{T}"/> 
    /// class with the specified length.
    /// </summary>
    public UnsafeList() : this(16) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeMemory{T}"/> 
    /// class with the specified length.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    /// <param name="growthRate">The rate at which the buffer grows.</param>
    public UnsafeList(int capacity, int growthRate = 32)
    {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity),
                "Capacity must be greater than or equal to zero.");
        }

        if (growthRate <= 0) {
            throw new ArgumentOutOfRangeException(nameof(growthRate),
                "GrowthRate must be greater than or equal to zero.");
        }

        GrowthRate = growthRate;
        _items = new UnsafeMemory<T>(capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(ReadOnlySpan<T> items)
    {
        GrowIfNeeded(items.Length);

        items.CopyTo(AsSpan()[..Count]);
        
        Count += items.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        GrowIfNeeded(1);
        
        _items[Count++] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T item)
    {
        var index = IndexOf(item);
        
        if (index < 0)
            return false;
        
        RemoveAt(index);
        
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAt(int index)
    {
        if (index == Count - 1) {
            Count--;
            
            return;
        }
        
        var span = _items.AsSpan(0, Count);

        var source = span.Slice(index + 1, Count - index - 1);
        
        var destination = span.Slice(index, Count - index);
        
        source.CopyTo(destination);

        Count--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(T item)
    {
        var span = _items.AsSpan();
        for (var i = 0; i < Count; i++) {
            if (span[i].Equals(item)) {
                return i;
            }
        }
        
        return -1;
    }
    
    public Span<T> AsSpan() => _items.AsSpan(0, Count);

    protected override void Free() => _items.FreeUnmanaged();

    /// <summary>
    /// Returns a raw pointer to the allocated memory.
    /// </summary>
    /// <returns></returns>
    [UnsafeApi]
    public T* AsUnsafePointer() => _items.AsUnsafePointer();

    private void GrowIfNeeded(int newItems)
    {
        if (Count + newItems >= Capacity) {
            _items.Resize(Capacity + newItems + GrowthRate, keepOriginal: true);
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
}

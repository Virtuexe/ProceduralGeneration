public struct Pool<T>
{
    public int Count { get; private set; }
    public int Length { get { return array.Length; } }
    private T[] array;
    public T this[int index]
    {
        get { return array[index]; }
        set { array[index] = value; }
    }
    public Pool(int size)
    {
        Count = 0;
        array = new T[size];
    }
    public void Add(T item)
    {
        array[Count++] = item;
    }
    public void Remove()
    {
        Count--;
    }
    public void Clear()
    {
        Count = 0;
    }
}

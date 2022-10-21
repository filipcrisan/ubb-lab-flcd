namespace lab;

public class HashTable<T>
{
    private readonly List<List<T>> _buckets;

    public HashTable(int bucketCount)
    {
        _buckets = new List<List<T>>();
        for (var i = 0; i < bucketCount; i++)
        {
            _buckets.Add(new List<T>());
        }
    }
    
    public (int, int) Add(T element)
    {
        var positions = Get(element);

        if (positions != (-1, -1))
        {
            return positions;
        }

        var bucketIndex = Hash(element);
        if (bucketIndex == -1)
        {
            return (-1, -1);
        }

        _buckets[bucketIndex].Add(element);

        return (bucketIndex, _buckets[bucketIndex].Count - 1);
    }
    
    public (int, int) Get(T element)
    {
        if (element is null)
        {
            return (-1, -1);
        }
        
        var bucketIndex = Hash(element);
        if (bucketIndex == -1)
        {
            return (-1, -1);
        }
        
        for (var i = 0; i < _buckets[bucketIndex].Count; i++)
        {
            if (element.Equals(_buckets[bucketIndex][i]))
            {
                return (bucketIndex, i);
            }
        }

        return (-1, -1);
    }

    public T Search(int bucketIndex, int listIndex)
    {
        if (bucketIndex < 0 || bucketIndex >= _buckets.Count)
        {
            throw new Exception("Invalid bucket index");
        }

        if (listIndex < 0 || listIndex >= _buckets[bucketIndex].Count)
        {
            throw new Exception("Invalid list index");
        }

        return _buckets[bucketIndex][listIndex];
    }
    
    #region Private methods

    private int Hash(T element)
    {
        return element is null ? -1 : Math.Abs(element.GetHashCode() % _buckets.Count);
    }

    #endregion
}
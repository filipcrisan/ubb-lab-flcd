using lab;

var ht = new HashTable<int>(5);
Console.WriteLine(ht.Add(4));
Console.WriteLine(ht.Get(4));

Console.WriteLine(ht.Add(4));
Console.WriteLine(ht.Get(4));

Console.WriteLine(ht.Add(9));

Console.WriteLine(ht.Search(4, 2));

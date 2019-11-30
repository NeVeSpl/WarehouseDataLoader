## WarehouseDataLoader
Sample C# parser showing how to change text to business objects.

We are importing state of a warehouse. Every item can be located on many shelves in different quantities. We want to know what items are located on which shelf and in which quantity.

### Assumptions
- data is streamed, line after line
- the goal is to achieve decent performance in terms of memory usage and speed

### Input data format
```
item name;item id;shelf,amount|shelf,amount|shelf,amount
```
First Header | min length | max length | allowed characters
------------ | ---------- | -----------| ------------
item name    | 1          | 255        | any character except ;
item id      | 1          | 64         | any character except ;
shelf        | 1          | 64         | any character except ,
quantity     | 1          | 9          | [0-9]

- every line should contain at least one pair of (shelf, amount)
- the same pair (item,shelf) can appear in many lines in that case we should add quantities
- a line that starts with # is the comment and should be ignored
- some lines may be incorrectly formatted we should detect them

### Output data format
- shelves should be sorted by the total number of items and then by name
- items contained on each shelf should be sorted in ascending order by ID and displayed in a format so that next to the item id
the total number of items has appeared

### Sample input
```
# Inventory initial state as of Jan 27 2019
The Hunger Games (The Hunger Games, #1);978-0439023481;S-A,5|S-B,10
Harry Potter and the Order of the Phoenix;978-0439358071;S-A,15
To Kill a Mockingbird;978-0469653981;S-A,10|S-B,6|S-C,2
Pride and Prejudice;978-0679783268;S-A,10|S-B,11
The Hobbit;978-0345538376;S-C,5
The Martian;978-0804139021;
# restocked
Twilight;978-0316015844;S-C,13|S-B,5
The Book Thief;978-0375831003;S-A,10|S-B,1
Animal Farm;978-0452284241;S-C,10
The Hobbit;978-0345538376;S-C,4
Ready Player One;978-0307887436;S-A,l
```


### Sample output
```
S-A (total 50)
978-0375831003: 10
978-0439023481: 5
978-0439358071: 15
978-0469653981: 10
978-0679783268: 10

S-C (total 34)
978-0316015844: 13
978-0345538376: 9
978-0452284241: 10
978-0469653981: 2

S-B (total 33)
978-0316015844: 5
978-0375831003: 1
978-0439023481: 10
978-0469653981: 6
978-0679783268: 11


# Invalid lines
The Martian;978-0804139021;
Ready Player One;978-0307887436;S-A,l
```

## Result : Benchmarks

|                  Method |     Mean |     Error |    StdDev | Ratio | RatioSD | Rank |     Gen 0 | Gen 1 | Gen 2 |  Allocated |
|------------------------ |---------:|----------:|----------:|------:|--------:|-----:|----------:|------:|------:|-----------:|
|              IndexBased | 1.188 ms | 0.0232 ms | 0.0217 ms |  0.32 |    0.01 |    1 |  320.3125 |     - |     - |   490.5 KB |
|               SpanBased | 1.402 ms | 0.0328 ms | 0.0547 ms |  0.37 |    0.01 |    2 |  318.3594 |     - |     - |  490.51 KB |
| SpanBasedWithStringPool | 2.039 ms | 0.0243 ms | 0.0215 ms |  0.55 |    0.01 |    3 |    3.9063 |     - |     - |    9.54 KB |
|              SplitBased | 3.707 ms | 0.0735 ms | 0.0902 ms |  1.00 |    0.00 |    4 | 1566.4063 |     - |     - | 2400.63 KB |
|              RegexBased | 8.831 ms | 0.0968 ms | 0.0756 ms |  2.38 |    0.06 |    5 | 2828.1250 |     - |     - | 4335.79 KB |
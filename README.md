## WarehouseDataLoader
Sample C# parser showing how to change text to business objects.

We are importing state of a warehouse. Every item can be located on many shelves in different amounts. We want to know what items are located on which shelf and in which amount.

### Assumptions
- data is streamed, line after line
- the goal is to achieve decent performance, in terms of memory usage and speed

### Input data format
```
item name;item id;shelf,amount|shelf,amount|shelf,amount
```
First Header | min length | max length | allowed characters
------------ | ---------- | -----------| ------------
item name    | 1          | 255        | any character except ;
item id      | 1          | 64         | any character except ;
shelf        | 1          | 64         | any character except ,
amount       | 1          | 9          | [0-9]

- every line should contain at least one pair of (shelf, amount)
- the same item can appear in many lines with the same shelf, in that case, we should add amounts
- a line that starts with # is the comment and should be ignored
- some lines may be incorrectly formatted, we should detect them and store

### Output data format
- shelves should be sorted by the total amount of items and then by name
- items contained on each shelf should be sorted in ascending order by ID and displayed in a format so that next to the item id
the total amount of items has appeared

### Sample input
```
# Inventory initial state as of Jan 27 2019
The Hunger Games (The Hunger Games, #1);978-0439023481;S-A,5|S-B,10
Harry Potter and the Order of the Phoenix;978-0439358071;S-A,15
To Kill a Mockingbird;978-0469653981;S-A,10|S-B,6|S-C,2
Pride and Prejudice;978-0679783268;S-A,10|S-B,11
The Hobbit;978-0345538376;S-C,5
# restocked
Twilight;978-0316015844;S-C,13|S-B,5
The Book Thief;978-0375831003;S-A,10|S-B,1
Animal Farm;978-0452284241;S-C,10
The Hobbit;978-0345538376;S-C,4
```


### Sample output
```
S-A (total 50)
978-0375831003: 10
978-0439023481: 5
978-0439358071: 15
978-0469653981: 10
978-0679783268: 10

S-B (total 33)
978-0316015844: 5
978-0375831003: 1
978-0439023481: 10
978-0469653981: 6
978-0679783268: 11

S-C (total 32)
978-0316015844: 13
978-0345538376: 3
978-0345538376: 4
978-0452284241: 10
978-0469653981: 2
```

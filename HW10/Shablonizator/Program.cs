using Shablonizator;
using Shablonizator.Models;

Console.WriteLine(PhrasesShablon.Ex1("Лейсан"));
Console.WriteLine();
Console.WriteLine(PhrasesShablon.Ex2(new ex2 { Address = "Ул.Пушкина" }));
Console.WriteLine();
Console.WriteLine(PhrasesShablon.Ex3(new ex3()));
Console.WriteLine();

var table = new ex4();
table.Students.Add(new studForEx4 { Fio = "Лейсан Нонская", Grade = 40 });
table.Students.Add(new studForEx4 { Fio = "Никита Евстягин", Grade = 100 });
table.Students.Add(new studForEx4 { Fio = "Иван Сосорин", Grade = 80 });
table.Students.Add(new studForEx4 { Fio = "Марат Гаффаров", Grade = 56 });

Console.WriteLine(PhrasesShablon.Ex4(table));
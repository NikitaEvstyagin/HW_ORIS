

namespace Shablonizator
{
    public class PhrasesShablon
    {
        public static string Ex1(string? name) => "Здравствуйте, @{name}, вы отчислены".Replace("@{name}", name);

        public static string Ex2(object? obj)
        {
            const string phrase = "Здравствуйте, @{name} вы прописаны по адресу @{address}";

            return phrase.Substitute(obj);
        }

        public static string Ex3(object? obj)
        {
            const string phrase = "Здравствуйте, @{if(temperature >= 37)} @then{Выздоравливайте} @else{Прогульщица}";

            return phrase.Substitute(obj);
        }

        public static string Ex4(object? obj)
        {
            const string phrase = "Здравствуйте, студенты группы @{group}.\nВаши баллы по ОРИС:\n@for(item in students) {@{item.FIO} - @{item.grade}}";

            return phrase.Substitute(obj);
        }
    }
}

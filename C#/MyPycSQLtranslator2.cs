﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MyPycSQLtranslator2.PycSQLtranslator2;

namespace MyPycSQLtranslator2
{
    /// <summary>
    /// Represents a C# class for translating queries from a custom Cyrillic-based SQL-like interpreted-like language to English.
    /// </summary>
    public static class PycSQLtranslator2
    {
        /// <summary>
        /// Represents the max lenght of line of words that can be a embedded token
        /// </summary>
        public const int TokenLenght = 4;

        /// <summary>
        /// Represents a token with its word and type.
        /// </summary>
        public struct Token
        {
            /// <summary>
            /// The word contained in the token.
            /// </summary>
            public string Word { set; get; }

            /// <summary>
            /// The type of the token.
            /// </summary>
            public string Type { set; get; }
        }

        /// <summary>
        /// Dictionary for mapping Cyrillic keywords to their English equivalents.
        /// </summary>
        public static Dictionary<string, string> PycSQLdictionary = new Dictionary<string, string>() {
            { "выбрать", "select" }, { "выбери", "select" },
            { "где", "where" },
            { "из", "from" },
            { "использовать", "use" }, { "используй", "use" }, { "исп", "use" }, { "юз", "use" },
            { "между", "between" }, { "меж", "between" },
            { "похож", "like" }, { "схож", "like" }, { "сродни", "like" }, { "подобен", "like" },
            { "в", "in" },
            { "и", "and" },
            { "или", "or" },
            { "не", "not" },
            { "отсортированно", "order by" }, { "отсортировать", "order by" }, { "сорт", "order by" },
            { "обратно", "desc" },
            { "прямо", "asc" },
            { "добавить", "insert" },{ "добавь", "insert" }, { "доб", "insert" }, { "вставить", "insert" }, { "вставь", "insert" }, { "встав", "insert" },
            { "к", "into" },
            { "значения", "values" }, { "значение", "values" }, { "знач", "values" }, { "величины", "values" }, { "величина", "values" }, { "величину", "values" }, { "велич", "values" },
            { "является", "is" }, { "это", "is" },
            { "ноль", "null" }, { "нуль", "null" }, { "нул", "null" }, { "зеро", "null" },
            { "обновить", "update" }, { "обнови", "update" }, { "обнов", "update" }, { "апд", "update" },
            { "установить", "set" }, { "установи", "set" }, { "уст", "set" }, { "поставить", "set" }, { "поставь", "set" }, { "пост", "set" },
            { "удалить", "delete" }, { "удали", "delete" }, { "удал", "delete" }, { "уд", "delete" },
            { "лимит", "limit" }, { "граница", "limit" }, { "край", "limit" },
            { "минимум", "min" }, { "мин", "min" },
            { "максимум", "max" }, { "макс", "max" },
            { "как", "as" },
            { "количество", "count" }, { "кол", "count" }, { "счет", "count" }, { "счёт", "count" },
            { "среднее", "avg" }, { "средн", "avg" }, { "авг", "avg" }, { "средний", "avg" },
            { "сумма", "sum" }, { "сумм", "sum" }, { "сум", "sum" },
            { "внутренний", "inner" }, { "внутр", "inner" },
            { "соединение", "join" }, { "соедени", "join" }, { "соединенить", "join" }, { "соед", "join" }, { "джоин", "join" },
            { "скрещенный", "cross" }, { "накрест", "cross" }, { "крест", "cross" }, { "крос", "cross" }, { "кросс", "cross" },
            { "правый", "right" }, { "прав", "right" }, { "право", "right" },
            { "левый", "left" }, { "лев", "left" }, { "лево", "left" },
            { "на", "on" },
            { "союз", "union" }, { "объединение", "union" }, { "объед", "union" }, { "обед", "union" }, { "обд", "union" },
            { "все", "all" }, { "всё", "all" },
            { "группируя", "group by" }, { "группируй", "group by" }, { "групп", "group by" }, { "груп", "group by" },
            { "учитывая", "having" }, { "учтя", "having" },
            { "существует", "exist" }, { "сущ", "exist" },
            { "любой", "any" }, { "всяк", "any" },
            { "хотяб", "some" }, { "хотябы", "some" },
            { "случай", "case" },
            { "когда", "when" },
            { "тогда", "then" },
            { "иначе", "else" },
            { "конец", "end" },
            { "еслиноль", "ifnull" }, { "еслинуль", "ifnull" }, { "нулевыйли", "ifnull" }, { "нулевли", "ifnull" }, { "нулёвыйли", "ifnull" }, { "нулёвли", "ifnull" },
            { "перв", "coalesce" }, { "ненулевый", "coalesce" }, { "ненулёвый", "coalesce" },
            { "создать", "create" }, { "создай", "create" },
            { "базаданных", "database" }, { "бд", "database" }, { "базуданных", "database" }, { "базыданных", "database" },
            { "сбросить", "drop" }, { "сбрось", "drop" }, { "сброс", "drop" }, { "дроп", "drop" },
            { "таблица", "table" }, { "таблицу", "table" }, { "тб", "table" },
            { "таблицы", "tables" },
            { "урезать", "truncate" }, { "урежь", "truncate" },
            { "изменить", "alter" }, { "измени", "alter" },
            { "включить", "add" }, { "включи", "add" },
            { "колонка", "column" }, { "колонку", "column" },
            { "модифицировать", "modify" }, { "модифицируй", "modify" }, { "мод", "modify" },
            { "ограничение", "constraint" }, { "огр", "constraint" }, { "ограничить", "constraint" }, { "огранич", "constraint" },
            { "примключ", "primary key" }, { "прим", "primary key" }, { "первичныйключ", "primary key" }, { "первичный", "primary key" },
            { "внешключ", "foreign key" }, { "внеш", "foreign key" }, { "внешнийключ", "foreign key" }, { "внешний", "foreign key" },
            { "чек", "check" }, { "контролировать", "check" }, { "контроль", "check" },
            { "дефолт", "default" }, { "поумолчанию", "default" }, { "обычный", "default" }, { "обыденный", "default" },
            { "уникальный", "unique" }, { "уник", "unique" },
            { "авто", "auto_increment" }, { "автоувеличение", "auto_increment" },
            { "индекс", "index" },
            { "отсылаетсяна", "references" }, { "отсыл", "references" }, { "отсылканаДжоДжо", "references" },
            { "сегодня", "current_date" },
            { "дата", "date" }, { "дату", "date" },
            { "датавремя", "datetime" },
            { "отметка", "timestamp" },
            { "год", "year" },
            { "вид", "view" }, { "взгляд", "view" },
            { "заменить", "replace" }, { "замени", "replace" },
            { "символ", "char" }, { "чар", "char" },
            { "строка", "varchar" }, { "варчар", "varchar" },
            { "бинарный", "binary" },
            { "бинстр", "varbinary" },
            { "миниббоб", "tinyblob" }, { "миниблоб", "tinyblob" },
            { "минитекст", "tinytext" },
            { "текст", "text" },
            { "ббоб", "blob" }, { "блоб", "blob" },
            { "средтекст", "mediumtext" },
            { "средббоб", "mediumblob" }, { "средблоб", "mediumblob" },
            { "длинтекст", "longtext" },
            { "длинббоб", "longblob" }, { "длинблоб", "longblob" },
            { "энум", "enum" }, { "нумерованный", "enum" },
            { "набор", "set" }, { "сет", "set" },
            { "бит", "bit" },
            { "миниинт", "tinyint" }, { "байт", "tinyint" },
            { "бул", "bool" },
            { "булевый", "boolean" },
            { "шорт", "smallint" }, { "малинт", "smallint" },
            { "срединт", "mediumint" },
            { "инт", "int" }, { "целый", "int" }, { "число", "int" },
            { "болинт", "bigint" },
            { "плот", "float" }, { "плавок", "float" },
            { "дабл", "double" }, { "двойной", "double" },
            { "десятичный", "decimal" }, { "десят", "decimal" }, { "дес", "decimal" }, { "десять", "decimal" },
            { "до", "before" },
            { "после", "after" },
            { "триггер", "trigger" },
            { "каждый", "each" },
            { "ров", "row" }, { "слой", "row" },
            { "новый", "new" },
            { "предшествует", "precedes" },
            { "ограничитель", "delimiter" },
            { "если", "if" },
            { "иначеесли", "elseif" }, { "элиф", "elseif" },
            { "асции", "ASCII" },
            { "чардлина", "char_lenght" },
            { "сверни", "concat" }, { "спернуть", "concat" }, { "заверни", "concat" }, { "завернуть", "concat" },
            { "поле", "field" },
            { "свнернсеп", "concat_ws" },
            { "найтивсете", "find_in_set" },
            { "формат", "format" },
            { "первстрока", "instr" },
            { "низкой", "lcase" }, { "низкий", "lcase" }, { "низ", "lcase" },
            { "длина", "lenght" },
            { "позиция", "locate" },
            { "левсдвиг", "lpad" },
            { "правсдвиг", "rpad" },
            { "вырез", "trim" },
            { "подстрока", "mid" }, { "субстрока", "mid" },
            { "подстриндекс", "substring_intdex" }, { "субстриндекс", "substring_intdex" },
            { "повтор", "repeat" },
            { "переверни", "reverse" }, { "реверс", "reverse" },
            { "пробелы", "space" },
            { "стрсравн", "strcmp" },
            { "заглавн", "ucase" },
            { "абс", "abc" },
            { "акосинус", "acos" }, { "акос", "acos" },
            { "асинус", "asin" }, { "асин", "asin" },
            { "атангенс", "atan" }, { "атан", "atan" },
            { "атангенс2", "atan2" }, { "атан2", "atan2" },
            { "потолок", "ciel" },
            { "косинус", "cos" }, { "кос", "cos" },
            { "котангенс", "cot" }, { "кот", "cot" },
            { "градус", "degrees" },
            { "див", "div" },
            { "эксп", "exp" },
            { "пол", "floor" },
            { "наибольший", "greatest" },
            { "наименьший", "least" },
            { "лн", "ln" }, { "натлогарифм", "ln" },
            { "лог", "log" }, { "логарифм", "log" },
            { "лог10", "log10" }, { "логарифм10", "log10" },
            { "лог2", "log2" }, { "логарифм2", "log2" },
            { "остаток", "mod" },
            { "пи", "pi" },
            { "степень", "pow" },
            { "радинан", "radians" },
            { "рандом", "rand" }, { "случайный", "rand" }, { "случайное", "rand" },
            { "округление", "round" },
            { "знак", "sign" },
            { "син", "sin" }, { "синус", "sin" },
            { "корень", "sqrt" },
            { "тан", "tan" }, { "тангенс", "tan" },
            { "добдату", "adddate" },
            { "добвремя", "addtime" },
            { "сейчас", "current_time" },
            { "датразн", "datediff" },
            { "датаформат", "date_format" },
            { "поддата", "date_sub" }, { "субдата", "date_sub" },
            { "день", "day" },
            { "имядня", "dayname" },
            { "деньмесяца", "dayofmonth" },
            { "неделидень", "dayofweek" },
            { "изъять", "extract" },
            { "издней", "from_days" },
            { "час", "hour" },
            { "послдень", "last_day" }, { "последнийдень", "last_day" },
            { "мествремя", "localtime" }, { "местноевремя", "localtime" },
            { "мествремяотм", "localtimestamp" }, { "местноевремяотм", "localtimestamp" },
            { "сделдату", "makedate" },
            { "сделвремя", "maketime" },
            { "микросекунда", "microsecond" }, { "микросекунду", "microsecond" }, { "микросекунд", "microsecond" },
            { "минута", "minute" },{ "минуту", "minute" },{ "минут", "minute" },
            { "месяц", "month" }, { "месяцев", "month" },
            { "имямесяца", "monthname" },
            { "добпер", "period_add" },
            { "периодразн", "period_diff" },
            { "четверть", "quarter" }, { "времягода", "quarter" },
            { "втораядата", "second" },
            { "секвдату", "sec_to_date" },
            { "стрвдату", "str_to_date" },
            { "подвремя", "subtime" }, { "субвремя", "subtime" },
            { "системноевремя", "systime" }, {"сисвремя", "systime" },
            { "системнаядата", "sysdate" }, {"сисдата", "sysdate" },
            { "время", "time" },
            { "времяформат", "time_format" },
            { "времявсек", "time_to_sec" },
            { "времяразн", "timedeff" },
            { "вдни", "to_days" },
            { "неделя", "week" },
            { "деньнедели", "weekday" },
            { "неделягода", "weekofyear" },
            { "годнеделя", "yearweek" },
            { "бин", "bin" },
            { "конвертвдату", "cast" },
            { "соединениеайди", "connection_id" }, { "соединениеномер", "connection_id" }, { "номерсоединения", "connection_id" }, { "айдисоединения", "connection_id" },
            { "конв", "conv" },
            { "конвертировать", "convert" }, { "конвертируй", "convert" },
            { "текпользователь", "current_user" }, { "текюзер", "current_user" },
            { "сеспользователь", "session_user" }, { "сессияюзер", "session_user" },
            { "сиспользователь", "system_user" }, { "сисюзер", "system_user" },
            { "явлноль", "isnull" }, { "явлнуль", "isnull" },
            { "пользователь", "user" }, { "юзер", "user" },
            { "версия", "version" },
            { "показать", "show" }, { "покажи", "show" }, { "поведай", "show" }, { "лицезрей", "show" }
        };

        /// <summary>
        /// Dictionary that is consists of tokens and max lenght of embeddings that thay can be in
        /// </summary>
        public static Dictionary<string, int> WordsLens = new Dictionary<string, int>()
        {
            { "выбрать", 2 },
            { "из", 2 },
            { "добавить", 3 }
        };

        /// <summary>
        /// Two words embeddings
        /// </summary>
        public static Dictionary<string, string> EmbeddedPycDict2 = new Dictionary<string, string>()
        {
            { "добавить в", "insert into" },
            { "выбрать все", "select *"},
            { "из таблицы", "from" }
        };

        /// <summary>
        /// Three words embedding
        /// </summary>
        public static Dictionary<string, string> EmbeddedPycDict3 = new Dictionary<string, string>()
        {
            { "добавить в таблицу", "insert into" }
        };

        /// <summary>
        /// Four words embedding
        /// </summary>
        public static Dictionary<string, string> EmbeddedPycDict4 = new Dictionary<string, string>()
        {
            { "", "" }
        };

        /// <summary>
        /// All embedded dictionares in array
        /// </summary>
        public static Dictionary<string, string>[] EmbeddedPycDictMain = 
        {
            PycSQLdictionary,
            EmbeddedPycDict2,
            EmbeddedPycDict3,
            EmbeddedPycDict4,
        };

        /// <summary>
        /// Translates the input query from the custom Cyrillic-based language to English.
        /// </summary>
        /// <param name="query">The query to be translated.</param>
        /// <returns>The translated query in English 
        /// ALSO RETURNED STRING WILL BE TRIMMED OF SPACES IN START OF STRING AND LOWERED
        /// </returns>
        public static string Translate(string query)
        {
            query = query.ToLower().Replace('ё', 'е').TrimStart();
            var tokens = Tokenize(query);
            return string.Join("", TokenUp(ref tokens));
        }

        /// <summary>
        /// Determines if a character is Cyrillic.
        /// </summary>
        public static bool Cyrillic(char c) { return c > 1039 && c < 1104 || c == 1105; }

        /// <summary>
        /// Determines if a character is quotic
        /// </summary>
        public static bool Quotic(char c) { return c == '"' || c == '`' || c == "'"[0]; }

        /// <summary>
        /// Determines if a character is $ or _ or [0-9]
        /// </summary>
        public static bool Anotheric(char c) { return c == '$' || c == '_' || c == '@' || char.IsDigit(c); }

        /// <summary>
        /// Determines if a character is valid
        /// </summary>
        public static bool Valid(char c) { return Anotheric(c) || Cyrillic(c); }

        /// <summary>
        /// Tokenizes the input query into individual words based on the presence of Cyrillic characters.
        /// </summary>
        public static List<Token> Tokenize(string query)
        {
            query += " ";
            string word = "" + query[0];
            List<Token> tokens = new List<Token>();

            bool wordProceedPyc = Cyrillic(query[0]);
            bool wordProceedOther = !Cyrillic(query[0]);
            bool quoted = Quotic(query[0]);
            bool quotedProceed = quoted;

            for (int i = 1; i < query.Length - 1; i++)
            {
                if (Valid(query[i]) && query[i] != ' ' && wordProceedOther && !Quotic(query[i]) && !quotedProceed)
                {
                    tokens.Add(new Token() { Word = word, Type = "OTHER" });
                    word = "" + query[i];
                    wordProceedOther = false;
                    wordProceedPyc = true;
                }
                else if (!Valid(query[i]) && query[i] != ' ' && wordProceedPyc && !Quotic(query[i]) && !quotedProceed)
                {
                    tokens.Add(new Token() { Word = word, Type = "PYC" });
                    word = "" + query[i];
                    wordProceedPyc = false;
                    wordProceedOther = true;
                }
                else if (Quotic(query[i]))
                {
                    if (quoted)
                    {
                        word += query[i++];
                        tokens.Add(new Token() { Word = word, Type = "QUOTED" });
                        word = "" + query[i];
                        quotedProceed = false;
                    }
                    else
                    {
                        tokens.Add(new Token() { Word = word, Type = wordProceedPyc ? "PYC" : "OTHER" });
                        word = "" + query[i];
                        quotedProceed = true;
                    }
                    quoted = !quoted;
                }
                else if (query[i] == ' ')
                { 
                    tokens.Add(new Token() { Word = word, Type = wordProceedPyc ? "PYC" : wordProceedOther ? "OTHER" : "QUOTED" });
                    word = " ";

                    while (query[++i] == ' ' && i < query.Length - 1)
                        word += ' ';
                    tokens.Add(new Token() { Word = word, Type = "SPACE" });

                    word = "" + query[i];
                    wordProceedPyc = Cyrillic(query[i]);
                    wordProceedOther = !wordProceedPyc;
                    quoted = Quotic(query[i]);
                }
                else
                    word += query[i];
            }
            if (!string.IsNullOrEmpty(word))
                tokens.Add(new Token() { Word = word, Type = wordProceedPyc ? "PYC" : wordProceedOther ? "OTHER" : "QUOTED" });
            return tokens;
        }

        /// <summary>
        /// Applies translation logic based on the token types and embeddings and performs the query translation.
        /// </summary>
        private static List<string> TokenUp(ref List<Token> tokens)
        {
            var words = new List<string>();
            for (int i = 0; i < tokens.Count(); i++)
            {
                if (tokens[i].Type == "PYC")
                {
                    if (WordsLens.ContainsKey(tokens[i].Word)) {
                        string embedded = tokens[i].Word;
                        int lenght = 0;
                        for (int len = 1; len < WordsLens[tokens[i].Word]; len++)
                        {
                            string word = embedded + " " + tokens[i + len * 2].Word;
                            if (EmbeddedPycDictMain[len].ContainsKey(word))
                            {
                                embedded += " " + tokens[i + len * 2].Word;
                                lenght = len;
                            }
                            else
                                break;
                        }
                        words.Add(EmbeddedPycDictMain[lenght][embedded]);
                        i += lenght * 2;
                    }
                    else
                        if (PycSQLdictionary.ContainsKey(tokens[i].Word))
                            words.Add(PycSQLdictionary[tokens[i].Word]);
                        else
                            words.Add(tokens[i].Word);
                }
                else
                    words.Add(tokens[i].Word);
            }
            return words;
        }
    }
}
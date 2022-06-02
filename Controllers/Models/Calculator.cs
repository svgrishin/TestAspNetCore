using Microsoft.AspNetCore.Mvc.Rendering;

namespace TestAspNetCore.Controllers.Models
{
    public class Calculator
    {
        public string timeStampLabel;
        public string mrString;



        /// <summary>
        /// Флаг нажатия функции
        /// </summary>
        public bool funcFlag = false;
        /// <summary>
        /// Флаг нажатия результата
        /// </summary>
        public bool resBtnFlag = false;
        /// <summary>
        /// Флаг задействования памяти
        /// </summary>
        public bool mrFlag = false;


        /// <summary>
        /// Математический знак
        /// </summary>
        public string symbol;

        /// <summary>
        /// Описание делагата функций калькулятора для передачи в процедуру вычисления
        /// <param name="a">Функция вычисления</param>
        /// </summary>
        public delegate double funcDeleg(double[] a);

        /// <summary>
        /// Делегат функций
        /// </summary>
        public funcDeleg fDeleg;

        /// <summary>
        /// Массив с аргументами вычислений
        /// </summary>
        public double[] args = new double[2];

        /// <summary>
        /// Общий вид вычисления
        /// </summary>
        public string resultString;

        /// <summary>
        /// Определяет, к какому из двух арументов будет обращение
        /// </summary>
        public bool index;

        /// <summary>
        /// Строка с числом в процессе набора
        /// </summary>
        public string arg;

        /// <summary>
        /// Содержимое дисплея
        /// </summary>
        public string disp;

        /// <summary>
        /// Метка отрицательности для агрумента
        /// </summary>
        public bool minus;

        /// <summary>
        /// Массив памяти
        /// </summary>
        public double[] mr;

        /// <summary>
        /// Массив строк формата json для загрузки/сохранения состояний
        /// </summary>
        public string[] jsonString;

        /// <summary>
        /// Класс функций вычисления
        /// </summary>
        public class CalcFunction
        {
            public double Summ(double[] a)
            {
                return a[0] + a[1];
            }

            public double multiply(double[] a)
            {
                return a[0] * a[1];
            }

            public double divide(double[] a)
            {
                return a[0] / a[1];
            }

            public double differens(double[] a)
            {
                return a[0] - a[1];
            }

            public double sqrOf(double[] a)
            {
                return Math.Pow(a[0], 2);
            }

            public double sqrtOf(double[] a)
            {
                return Math.Sqrt(a[0]);
            }

            public double fraction(double[] a)
            {
                return 1 / a[0];
            }
        }

        /// <summary>
        /// Конструктор для клонирования калькулятора
        /// </summary>
        /// <param name="c">Исходный калькулятор</param>
        /// <returns>Клон калькулятора</returns>
        public Calculator(Calculator c)
        {
            resBtnFlag = c.resBtnFlag;
            funcFlag = c.funcFlag;
            mrFlag = c.mrFlag;

            symbol = c.symbol;

            args = new double[2];
            args[0] = c.args[0];
            args[1] = c.args[1]; ;

            resultString = c.resultString;

            index = true;
            funcFlag = false;

            arg = args[0].ToString();
            disp = c.disp;
            minus = c.minus;

            mr = c.mr;

            jsonString = c.jsonString;
        }

        /// <summary>
        /// Конструктор для калькулятора по умолчанию
        /// </summary>
        /// <returns>Калькулятор</returns>
        public Calculator()
        {
            index = false;
            arg = "";
            disp = "0";
            minus = false;

            mr = new double[1];

            jsonString = new string[1];

            fDeleg = null;

            args = new double[2];
        }

        /// <summary>
        /// Сброс состояния калькулятора
        /// </summary>
        public void ResetCalc()
        {
            resBtnFlag = false;
            funcFlag = false;
            mrFlag = false;

            arg = "";
            disp = "0";
            index = false;
            minus = false;
            fDeleg = null;
        }

        /// <summary>
        /// Ввод символов
        /// </summary>
        /// <param name="c">Вводимый символ</param>
        /// <returns>Форматированную строку для дисплея</returns>
        public string inputValues(char c)
        {
            if (funcFlag == true)
            {
                arg = "";
                funcFlag = false;
            }

            if (resBtnFlag == true) index = false;

            funcFlag = false;

            int i = 15;

            if (c == '-') i = 17;

            if (arg.Length <= i)
            {
                string s;

                if (c == '-')
                {
                    if (arg != "")
                    {
                        switch (minus)
                        {
                            case true: arg = c + arg; break;
                            case false: arg = arg.TrimStart(c); break;
                        }
                        disp = arg;
                        s = arg;
                    }
                    else s = disp;
                }
                else
                {
                    arg += c;
                    s = arg;
                }
                return displayOut(s);
            }
            else return displayOut(arg);
        }

        /// <summary>
        /// Удаление символа
        /// </summary>
        /// <returns>Форматированную строку для дисплея</returns>
        public string deleteSymbol()
        {
            if (arg.Length > 1 + Convert.ToInt16(minus))
            {
                arg = arg.Substring(0, arg.Length - 1);
                return displayOut(arg);
            }
            else
            {
                arg = "";
                disp = "0";
                return displayOut(disp);
            }
        }

        /// <summary>
        /// Форматирование строки дисплея
        /// </summary>
        /// <param name="s">Неотформатированная строка для дисплея</param>
        /// <returns>Строка для дисплея с пробелами по разрядам</returns>
        public string displayOut(string s)
        {
            bool minusFlag = false;
            if (s[0] == '-')
            {
                s = s.TrimStart('-');
                minusFlag = true;
            }
            if (s.Contains(',') == false)
            {
                for (int i = 3; i <= s.Length; i += 4)
                {
                    s = s.Insert(s.Length - i, " ");
                }
            }
            s = s.TrimStart(' ');
            if (minusFlag == true) s = s.Insert(0, "-");
            return s;
        }

        /// <summary>
        /// Добавление операции к списку операций текущего сеанса
        /// </summary>
        /// <param name="s">Операция целиком</param>
        private void addingCalcToList(string s)
        {
            if (s == "√" || s == "1/")
            {
                addToCalcString(symbol);
                addToCalcString(args[0]);
                addToCalcString("=");
            }
            else
            {
                addToCalcString(args[0]);
                addToCalcString(symbol);
                addToCalcString(args[1]);
                addToCalcString("=");
            }
        }

        /// <summary>
        /// Получение результата
        /// </summary>
        /// <param name="cf">Делагат функции вычисления результата</param>
        public void getResult(funcDeleg cf)
        {
            try
            {
                addingCalcToList(symbol);
            }
            catch { }

            try
            {
                args[0] = cf(args);
            }
            catch
            {
                args[0] = fDeleg(args);
            }

            fDeleg = cf;
            addToCalcString(args[0]);

            disp = displayOut(Convert.ToString(args[0]));

        }

        /// <summary>
        /// Вычисление функций одного арумента
        /// </summary>
        /// <param name="cf">Делагат функции вычисления результата</param>
        public void extraFunc(funcDeleg cf)
        {
            //функция для одного аргумента
            //особенность в том, что может быть успешно выполнена 
            //по упрощённому алгоритму

            tryToGetArg(arg);
            args[1] = 2;

            getResult(cf);

            arg = "";
            index = !index;
        }

        /// <summary>
        /// Присвоение аргументу значения
        /// </summary>
        /// <param name="s">Строка со значением для аргумента</param>
        public void tryToGetArg(string s)
        {
            try
            {
                args[Convert.ToByte(index)] = Convert.ToDouble(s);
                arg = "";
            }
            catch
            {
                if (resBtnFlag == false)
                {
                    index = true;
                    args[1] = args[0];
                    resBtnFlag = true;
                }
            }
            index = true;
            disp = s;
        }

        /// <summary>
        /// Построение строки с вычислением целиком
        /// </summary>
        /// <param name="s">Добавляемая часть к строке вычисления</param>
        /// <overloads>Принимает для добавления как строку, так и число</overloads>
        private void addToCalcString(string s)
        {
            resultString = string.Concat(resultString, s);
        }

        /// <summary>
        /// Построение строки с вычислением целиком
        /// </summary>
        /// <param name="s">Число для добавления к строке вычисления</param>
        /// <overloads>Принимает для добавления как строку, так и число</overloads>
        private void addToCalcString(double s)
        {
            resultString = string.Concat(resultString, s.ToString());
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestAspNetCore.Models;
using TestAspNetCore.Controllers.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TestAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //===============================================================================================================
        //===============================================================================================================
        //===============================================================================================================

        //public static string results { get; set; }

        private static string location = "C:/Users/Public/Documents/calc.json";

        public static List<SelectListItem> resultStrings = new List<SelectListItem>();
        public static List<SelectListItem> mrStrings = new List<SelectListItem>();
        public static List<SelectListItem> historySrings = new List<SelectListItem>();

        /// <summary>
        /// Объект калькулятора
        /// </summary>

        public static Calculator calc = new Calculator();

        /// <summary>
        /// Массив с калькуляторами для хранения объектов с различными состояниями
        /// </summary>
        public static Calculator[] calcs = new Calculator[0];

        public IActionResult Index()
        {
            TempData["display"] = "0";
            return View("Index");
        }

        
        public IActionResult resultsView()
        {
            ViewBag.resultStrings = resultStrings;
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Number_Click(string btn_number)
        {
            
            inputVal(btn_number[0]);

            return View("Index");
        }

        private void inputVal(char c)
        {
            TempData["display"] = calc.inputValues(c);
        }

        public IActionResult btn_Zero_Click()
        {
            if (calc.arg == "") typeZeroComa();
            else TempData["display"] = calc.inputValues('0');

            return View("Index");
        }

        public void typeZeroComa()
        {
            TempData["display"] = calc.inputValues('0');
            TempData["display"] = calc.inputValues(',');
        }

        public IActionResult btn_CE_Click1()
        {
            calc.arg = "";
            calc.disp = "0";
            TempData["display"] = calc.disp;

            return View("Index");
        }

        public IActionResult btn_C_Click()
        {
            calc.ResetCalc();
            Array.Clear(calc.args, 0, 1);
            TempData["display"] = "0";

            return View("Index");
        }

        public IActionResult btn_Backspase_Click()
        {
            TempData["display"] = calc.deleteSymbol();
            return View("Index"); 
        }

        public IActionResult btn_SQR_Click()
        {
            calc.symbol = "^";
            calc.fDeleg = new Calculator.CalcFunction().sqrOf;
            btn_Func_Click(calc.fDeleg, true);

            return View("Index");
        }

        public IActionResult btn_SQRT_Click()
        {
            calc.symbol = "√";
            calc.fDeleg = new Calculator.CalcFunction().sqrtOf;
            btn_Func_Click(calc.fDeleg, true);

            return View("Index");
        }

        public IActionResult btn_Plus_Click()
        {
            btn_click("+", new Calculator.CalcFunction().Summ, false);
            return View("Index");
        }

        public IActionResult btn_Minus_Click()
        {
            btn_click("-", new Calculator.CalcFunction().differens, false);
            return View("Index");
        }

        public IActionResult btn_Multiply_Click()
        {
            btn_click("×", new Calculator.CalcFunction().multiply, false);
            return View("Index");
        }

        public IActionResult btn_Divide_Click()
        {
            btn_click("÷", new Calculator.CalcFunction().divide, false);

            return View("Index");
        }

        private void btn_click(string s, Calculator.funcDeleg f, bool isExtraFunc)
        {
            calc.symbol = s;

            if (calc.funcFlag == true)
            {
                calc.fDeleg = f;
                calc.resBtnFlag = false;
            }
            else
            {
                btn_Func_Click(f, isExtraFunc);
            }
        }

        /// <summary>
        /// Нажатие одной из функций
        /// </summary>
        /// <param name = "f" > Делегат функции выполнения</param>
        /// <param name = "isExtraFunc" > Флаг функции одного аргумента</param>
        private void btn_Func_Click(Calculator.funcDeleg f, bool isExtraFunc)
        {
            if (isExtraFunc == true)
            {
                calc.extraFunc(f);
                TempData["display"] = calc.disp;
                saveStatus();
            }
            else funcClick(f);
        }

        private void funcClick(Calculator.funcDeleg f)
        {
            calc.resBtnFlag = false;

            switch (calc.index)
            {
                case false:
                    {
                        calc.fDeleg = f;
                        calc.tryToGetArg(calc.arg);
                        calc.funcFlag = true;
                        break;
                    }
                case true:
                    {
                        calc.tryToGetArg(calc.arg); ;
                        getResult(calc.fDeleg);
                        calc.funcFlag = true;
                        calc.fDeleg = f;
                        break;
                    }
            }

            TempData["display"] = calc.disp;
            calc.minus = false;
        }

        /// <summary>
        /// Инициатор получения результата
        /// </summary>
        /// <param name="cf">Делегат функции</param>
        private void getResult(Calculator.funcDeleg cf)
        {
            calc.getResult(cf);
            saveStatus();
        }


        private void saveStatus()
        {
            saveMe();
            if (calc.resultString != "") addToCalcList(calc);




            calc.resultString = "";
        }

        /// <summary>
        /// Добавление в список вычисления целиком
        /// </summary>
        /// <param name="c">Калькулятор</param>
        private void addToCalcList(Calculator c)
        {
            resultStrings.Add(new SelectListItem() { Text = c.resultString, Value = calcs.Length.ToString() });
            //ViewBag.testItems = resultStrings;
        }

        public void saveMe()
        {
            addCalc();
            try
            {
                SaverLoader.saveCalc(calc);
            }
            catch { }
        }

        private void addCalc()
        {
            int i = calcs.Length;
            Array.Resize(ref calcs, i + 1);
            calcs[calcs.Length - 1] = new Calculator(calc);
        }

        public IActionResult btn_History_Click()
        {
            SaverLoader.loadHistory(location, calc.resultString);

            ViewBag.resultStrings = resultStrings;
            return View("HistoryView");
        }

        public IActionResult btn_Negative_Click()
        {
            calc.minus = !calc.minus;
            inputVal('-');

            return View("Index");
        }

        public IActionResult btn_Coma_Click()
        {
            if (calc.arg.Contains(',') == false)
            {
                if (calc.arg == "")
                {
                    typeZeroComa();
                }
                else TempData["display"] = calc.inputValues(',');
            }

            else TempData["display"] = calc.arg;

            return View("Index");
        }

        public IActionResult btn_Result_Click()
        {
            try
            {
                calc.tryToGetArg(calc.arg);

                calc.resBtnFlag = true;
                calc.funcFlag = true;

                calc.getResult(calc.fDeleg);

                TempData["display"] = calc.disp;

                saveStatus();
            }
            catch { }

            return View("Index");
        }

        /// <summary>
        /// Загрузка калькулятора
        /// </summary>
        /// <param name="i">Индекс загружаемого калькулятора из массива калькуляторов></param>
        public IActionResult loadMe(int value)
        {
            int i = value-1;
                
            calc = new Calculator(calcs[i]);
            
            calc.resultString = "";

            TempData["display"] = calc.displayOut(calc.disp);

            calc.ResetCalc();

            calc.arg = calc.args[0].ToString();

            return View("Index");
        }

        public IActionResult MR_View()
        {
            ViewBag.mrStrings = mrStrings;
            return View();
        }
        
        public IActionResult btn_MList_Click(int value)
        {
            try
            {
                getFromMR(value-1);
            }
            catch
            { }

            return View("Index");
        }

        public void getFromMR(int indexOf)
        {
            calc.arg = calc.mr[indexOf].ToString();
            if (calc.mr[indexOf] < 0) calc.minus = true;
            calc.disp = calc.displayOut(calc.arg);

            TempData["display"] = calc.disp;
        }

        public IActionResult btn_MPlus_Click(string btn_MPlus)
        {
            setMR(calc.mr.Length - 1, 1);

            calc.mrFlag = true;

            TempData["display"] = btn_MPlus;
            return View("Index");
        }

        public void setMR(int indexOf, int negative)
        {
            try
            {
                calc.mr[indexOf] += Convert.ToDouble(calc.arg) * negative;
            }
            catch
            {
                calc.mr[indexOf] = calc.args[0];
                calc.arg = calc.mr[indexOf].ToString();
            }

            setMrList(indexOf);

            calc.funcFlag = true;
            calc.resBtnFlag = true;

            calc.mrFlag = true;
        }

        public void setMrList(int indexOf)
        {
            try
            {
                mrStrings[calc.mr.Length - 1].Text = calc.mr[indexOf].ToString();
            }
            catch
            {
                mrStrings.Add(new SelectListItem() { Text = calc.mr[indexOf].ToString(), Value = calc.mr.Length.ToString() });
            }
        }

        public IActionResult btn_MC_Click(string btn_MC)
        {
            calc.mr = new double[1];
            mrStrings.Clear();
            Array.Clear(calc.mr);

            TempData["display"] = btn_MC;

            return View("Index");
        }

        public IActionResult btn_MR_Click()
        {
            getFromMR(calc.mr.Length - 1);
            TempData["display"] = calc.arg;

            calc.mrFlag = true;

            return View("Index");
        }

        public IActionResult btn_MMinus_Click(string btn_MMinus)
        {
            setMR(calc.mr.Length - 1, -1);
            calc.mrFlag = true;

            TempData["display"] = btn_MMinus;

            return View("Index");
        }

        public IActionResult btn_MS_Click(string btn_MS)
        {
            int l = calc.mr.Length - 1;

            if (calc.mr.Length > 0)
            {
                Array.Resize(ref calc.mr, l + 2);
                l++;
            }
            setMR(l, 1);

            calc.mrFlag = true;

            TempData["display"] = btn_MS;
            return View("Index");
        }

        public IActionResult loadCalc(int value)
        {
            try
            {
                loadMe(value+1);
                resultStrings.Clear();
            }
            catch { }

            TempData["display"] = calc.arg;

            return View("Index");
        }
    }
}
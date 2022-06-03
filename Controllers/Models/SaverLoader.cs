using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TestAspNetCore.Controllers.Models
{
    public abstract class SaverLoader
    {
        /// <summary>
        /// Сохранение состояния калькулятора в файл
        /// </summary>
        public static void saveCalc(Calculator c)
        {
            Calculator newC = new Calculator(c);

            newC.fDeleg = null;

            string s = JsonConvert.SerializeObject(newC);

            string location = Directory.GetCurrentDirectory() + "/calc.json";
            File.AppendAllText(location, s + "\n");
        }

        public static void loadHistory(string location, string resultstring)
        {
            string[] s = File.ReadAllLines(location);
            int i = 0;
            foreach (string str in s)
            {
                Array.Resize(ref HomeController.calcs, i + 1);
                //Array.Resize(ref HomeController.resultStrings, i + 1);

                HomeController.calcs[i] = JsonConvert.DeserializeObject<Calculator>(str);

                //HomeController.resultStrings.Add(HomeController.calcs[i].resultString);
                HomeController.resultStrings.Add(new SelectListItem() { Text = HomeController.calcs[i].resultString, Value = i.ToString() });
                i++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECO.Busisness
{
    public static class ValidateHelper
    {
        public static string CheckWOValid(string WO)
        {
            if (!WO.StartsWith("2000"))
            {
                return "WO phải bắt đầu bằng 2000";
            }
            if (WO.Length < 10)
            {
                return "Wo phải đủ 10 số";
            }
            return "";

        }
        public static string ControlViewByDept(string dept)
        {
            if (dept == "MC:") return "groupMC";
            if (dept == "PE:") return "groupPE";
            return "";
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECO
{
    public class Constants
    {
        public static string ROLE = "";
    }

    public class ROLE
    {
        public static string VIEW_ONLY = "VIEW_ONLY";
        public static string CONFIRM = "CONFIRM";
    }

    public enum DEPT
    {
        FAT = 15
        , MC = 13
        , OQC = 18
        , SMT = 14
        , PE = 10
        , PE_ACTUAL = 16

    }
    public enum TYPE
    {
        //AI = 0, SMTA = 1, SMTB = 2, SMT = 3, FAT = 4, default = -1;
        AI = 0
            , SMTA = 1
            , SMTB = 2
            , SMT = 3
            , FAT = 4
            , ROM = 5
            , DEFAULT = -1
    }

    public enum PE_OPTION
    {
        FULL = 1,
        EDIT = 2,
        FLOW3 = 3,
        FLOW4 = 4,
        NONE = 0
    }
}

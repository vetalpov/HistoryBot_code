using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kurcova_test_1_bot
{
    public class Constants
    {
        public static string Host = "localhost";
        public static string Username = "postgres";
        public static string Password = "123";
        public static string DatabaseName = "postgres";
        public static string Connect => $"Host={Host};Username={Username};Password={Password};Database={DatabaseName}";
    }
}

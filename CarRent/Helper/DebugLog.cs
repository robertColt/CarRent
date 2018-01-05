using System;
using System.Diagnostics;

namespace CarRent.Helper
{
    public class DebugLog
    {
        public static void WriteLine(object obj)
        {
            if(obj != null)
            {
                Debug.WriteLine(">>" + obj.ToString() + "\n");
            }
        }

        public static void WriteLine(string message)
        {
            Debug.WriteLine(">>" + message + "\n");
        }

        public static void WriteLine(Exception exception)
        {
            Debug.WriteLine(">>" + exception.Message + " : \n" + exception.StackTrace + "\n");
        }
    }
}
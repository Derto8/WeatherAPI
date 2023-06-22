namespace WebClient.Classies
{
    internal static class DefineMonthClass
    {
        internal static string DefineMonth(string month)
        {
            string[] mas = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string result = "";

            int m = int.Parse(month);
            for (int i = 0; i <= mas.Length; i++)
            {
                if (m == i)
                {
                    result = mas[i - 1];
                    break;
                }
            }
            return result;
        }
    }
}

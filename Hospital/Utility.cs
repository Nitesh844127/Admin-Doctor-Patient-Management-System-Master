namespace Hospital
{
    public static class Utility
    {

        public static DateTime ToDate(this int date)
        {
            int d = date % 100;
            int m = (date / 100) % 100;
            int y = date / 10000;

            return new DateTime(y, m, d);
        }
        public static DateTime ToTime(this int time)
        {
            int SS = time % 100;
            int MM = (time / 100) % 100;
            int HH = time / 10000;

            return new DateTime(DateTime.Now.Year, 1, 1, HH, MM, SS);

        }

    }
}

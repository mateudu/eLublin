namespace eLublin
{
     public static class StaticData
    {
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static double Expvalu { get; set; }

        public static int Level { get; set; }
        public static string ServiceUrl => @"http://elublin.azurewebsites.net/";
    }
}

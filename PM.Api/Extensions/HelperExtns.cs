namespace PM.Api.Extensions
{
    public static class HelperExtns
    {
        public static string Stringify(this object input)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(input);
        }
    }
}
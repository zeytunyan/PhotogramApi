namespace Api.Exceptions
{
    public class ItemException : Exception
    {
        public string? Item { get; set; } 

        public ItemException(string item = "Undefined")
        {
            Item = item;
        }

        public override string Message => $"Exception with item: {Item}";
    }
}

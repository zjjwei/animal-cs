namespace backend {
    public static class Util {
    public const string ANSI_RESET = "\u001B[0m";
    public const string ANSI_BLACK = "\u001B[30m";
    public const string ANSI_RED = "\u001B[31m";
    public const string ANSI_GREEN = "\u001B[32m";
    public const string ANSI_YELLOW = "\u001B[33m";
    public const string ANSI_BLUE = "\u001B[34m";
    public const string ANSI_PURPLE = "\u001B[35m";
    public const string ANSI_CYAN = "\u001B[36m";
    public const string ANSI_WHITE = "\u001B[37m";
}


static class RandomExtensions
{
    public static void Shuffle<T> (this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}

}
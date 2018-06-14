using LogSystem;

namespace LogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Logger.Instance.Log("TEST!!!!!!!!!!!!!!!!!!!!");
            }
        }
    }
}

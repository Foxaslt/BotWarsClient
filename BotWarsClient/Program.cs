using BotWarsClient.Bots;
using System;

namespace BotWarsClient
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int port = 5999; //set port initially to 5999

            if (args.Length > 0)
            {
                port = (Convert.ToInt32(args[0]));
            }


            HttpListenerClass httpListener = new HttpListenerClass(port, new VanillaBot());
        
        }
    }
}

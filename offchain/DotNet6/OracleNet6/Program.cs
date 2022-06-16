using Nethereum.Web3;
//using Nethereum.JsonRpc.WebSocketClient;

namespace OracleNet6 // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Here we go!");
            await Run();
        }


        const string nodeUrl = "http://127.0.0.1:7545";
        private static async Task Run()
        {
            // Use this when need to connect to the websocket connection.
            //var wsClient = new WebSocketClient(nodeUrl);
            //var web3 = new Web3(wsClient);

            // Creating web3 using http connection
            var web3 = new Web3(nodeUrl);
            var accounts = await web3.Eth.Accounts.SendRequestAsync();

            Console.WriteLine();
            foreach (var account in accounts)
                Console.WriteLine(account);

            var acc0 = accounts[0];
            Console.WriteLine();
            Console.WriteLine($"Account 0 address: {acc0}");
        }
    }
}
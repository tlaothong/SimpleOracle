using Flurl.Http;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Threading.Tasks;
//using Nethereum.JsonRpc.WebSocketClient;

namespace OracleNet5
{
    internal class Program
    {
        private static string? Acc0 = null;

        static async Task Main(/*string[] args*/)
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

            Acc0 = accounts[0];
            Console.WriteLine();
            Console.WriteLine($"Account 0 address: {Acc0}");

            Console.WriteLine("Enter the smart contract address:");
            contractAddress = Console.ReadLine();

            Console.WriteLine();
            await SubscribeToEvent(web3, HandleEvent);
        }

        static string? contractAddress;
        private static async Task SubscribeToEvent(Web3 web3, Func<Web3, bool, string?, Task> handleEventAsync)
        {
            HexBigInteger? blockNumber = null;
            var myEventHandler = web3.Eth.GetEvent<MyEvent>(contractAddress);
            while (true)
            {
                var firstTime = blockNumber == null;
                var eventFilter = firstTime
                    ? myEventHandler.CreateFilterInput()
                    : myEventHandler.CreateFilterInput(fromBlock: new BlockParameter(blockNumber));
                var myEventLogs = await myEventHandler.GetAllChangesAsync(eventFilter);

                foreach (var elog in myEventLogs)
                {
                    Console.WriteLine("MyEvent Received: {0}, '{1}'",
                        elog.Event.Num, elog.Event.Text);
                    blockNumber = elog.Log.BlockNumber;
                    blockNumber.Value++;

                    await handleEventAsync(web3, firstTime, elog.Event.Text);
                }
            }
        }

        private static async Task HandleEvent(Web3 web3, bool isFirstTime, string? requestText)
        {
            if (isFirstTime)
                return;

            var webResult = "Oracle of " + requestText;
            // TODO: Uncomment the following code for an alternative webResult.
            //if (!Uri.IsWellFormedUriString(requestText, UriKind.Absolute))
            //    return;

            //var uri = new Uri(requestText);
            //webResult = await uri.GetStringAsync();
            //webResult = webResult.Substring(0, Math.Min(50, webResult.Length - 1));

            var callback = web3.Eth.GetContractTransactionHandler<CallbackFromOracleFunction>();
            var result = await callback.SendRequestAndWaitForReceiptAsync(contractAddress,
                new CallbackFromOracleFunction
                {
                    FromAddress = Acc0,
                    Result = webResult,
                });

            Console.WriteLine($"Callback result: {result.Succeeded()}");
        }
    }

    [Event(nameof(MyEvent))]
    public class MyEvent : IEventDTO
    {
        [Parameter("uint", "num", 1, false)]
        public int Num { get; set; }
        [Parameter("string", "text", 2, false)]
        public string? Text { get; set; }
    }

    [Function("callbackFromOracle")]
    public class CallbackFromOracleFunction : FunctionMessage
    {
        [Parameter("string", "result")]
        public string? Result { get; set; }
    }
}
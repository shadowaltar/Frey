using Automata.Core;
using Automata.Core.Extensions;
using Automata.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Automata.Mechanisms
{
    public class DataSource
    {
        public DataSource(DataAccess access)
        {
            this.access = access;
        }

        private DataAccess access;
        private PricesQueue awaitingPrices;

        private CancellationTokenSource cancellation;
        // private CancellationToken cancelToken;
        private bool isSendingData;
        private bool isReceivingData;
        private DataStatus currentDataStatus = DataStatus.Initializing;

        public IDataScope DataScope { get; set; }

        public event Action<HashSet<Price>> NotifyNewPriceData;
        public event Action<DataStatus> DataStatusChanged;

        public void Initialize()
        {
            awaitingPrices = PricesQueue.New();
            access.Initialize(DataScope);
        }

        public void Start()
        {
            cancellation = new CancellationTokenSource();

            isSendingData = true;
            isReceivingData = true;

            var task = Task.Factory.StartNew(() => Publish(), cancellation.Token);
            task.ContinueOnException(HandleDataSourceFailure);
            task.ContinueOnCompleted(t =>
            {
                currentDataStatus = DataStatus.ReachTimeEnd;
                InvokeDataStatusChanged();
            });
        }

        public void Stop()
        {
            // disable the loop and request a cancellation to the long-running thread.
            isSendingData = false;
            cancellation.Cancel();

            Console.WriteLine(Utilities.BracketNow + " Stopping DataSource.");
            access.Dispose();
        }

        private void HandleDataSourceFailure(Task<bool> result)
        {
            if (result != null && result.Exception != null)
                throw result.Exception;
        }

        private bool Publish()
        {
            while (isSendingData)
            {
                if (cancellation.Token.IsCancellationRequested)
                {
                    Console.WriteLine(Utilities.BracketNow + " Stop receiving data.");
                    isSendingData = false;
                    break;
                }

                if (isReceivingData)
                {
                    var items = access.Read();
                    if (items != null)
                        awaitingPrices.AddItems(items);
                }

                var prices = awaitingPrices.NextItems();
                if (prices == null)
                {
                    currentDataStatus = DataStatus.WaitingForData;
                    InvokeDataStatusChanged();

                    Console.WriteLine(Utilities.BracketNow + " Waiting for data.");
                    cancellation.Token.WaitHandle.WaitOne(50);
                }
                else
                {
                    if (currentDataStatus != DataStatus.HasDataInQueue)
                    {
                        currentDataStatus = DataStatus.HasDataInQueue;
                        InvokeDataStatusChanged();
                    }

                    // send data
                    Console.WriteLine(Utilities.BracketNow + " Sending data.");
                    InvokeNotifyDataArrive(prices);
                }
            }

            return false;
        }

        private void InvokeDataStatusChanged()
        {
            if (DataStatusChanged != null)
                DataStatusChanged(currentDataStatus);
        }

        private void InvokeNotifyDataArrive(HashSet<Price> prices)
        {
            if (NotifyNewPriceData != null)
                NotifyNewPriceData(prices);
        }
    }
}
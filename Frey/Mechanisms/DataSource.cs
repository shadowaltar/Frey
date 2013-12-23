using Automata.Core;
using Automata.Core.Extensions;
using Automata.Entities;
using System;
using System.Collections.Concurrent;
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
        private Queue<HashSet<Price>> awaitingPrices = new Queue<HashSet<Price>>();

        private CancellationTokenSource cancellation;
        // private CancellationToken cancelToken;
        private bool isSendingData;
        private bool isReceivingData;
        private DataStatus currentDataStatus = DataStatus.Initializing;

        public ITradingScope TradingScope { get; set; }

        public event Action<HashSet<Price>> NotifyNewPriceData;
        public event Action<DataStatus> DataStatusChanged;

        public void Initialize()
        {
            access.Initialize(TradingScope);
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
            Console.WriteLine(Utilities.BracketNow + " Stopping DataSource.");
            cancellation.Cancel();
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
                    var items = access.GetNextTimeslotPrices();
                    if (items != null)
                    {
                        awaitingPrices.Enqueue(items);
                    }
                }

                HashSet<Price> prices = null;
                if (awaitingPrices.Count > 0)
                    prices = awaitingPrices.Dequeue();

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
                    InvokeNotifyPriceDataArrive(prices);
                }
            }

            return false;
        }

        private void InvokeDataStatusChanged()
        {
            if (DataStatusChanged != null)
                DataStatusChanged(currentDataStatus);
        }

        private void InvokeNotifyPriceDataArrive(HashSet<Price> price)
        {
            if (NotifyNewPriceData != null)
                NotifyNewPriceData(price);
        }
    }
}
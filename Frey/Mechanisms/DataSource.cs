using Automata.Core;
using Automata.Core.Utils;
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

        private CancellationToken cancelToken;
        private bool isSendingData;
        private bool isReceivingData;
        private DataStatus currentDataStatus = DataStatus.Initializing;

        public IDataScope DataScope { get; set; }

        public event Action<HashSet<Price>> NotifyDataArrive;
        public event Action<DataStatus> SourceStatusChanged;
        public event Action<DataStatus> DataStatusChanged;

        public void Initialize()
        {
            awaitingPrices = PricesQueue.New();
            access = new HistoricalDatabaseAccess();
        }

        public void Start()
        {
            var cancelTokenSouce = new CancellationTokenSource();
            cancelToken = cancelTokenSouce.Token;

            isSendingData = true;
            isReceivingData = true;

            var task = Task.Factory.StartNew(() => Publish(), cancelToken);
            task.ContinueOnException(HandleDataSourceFailure);
            task.ContinueOnCompleted(t =>
            {
                currentDataStatus = DataStatus.ReachTimeEnd;
                InvokeDataStatusChanged();
            });
        }

        public void Stop()
        {
            isSendingData = false;
            Console.WriteLine("Stopping DataSource at: " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            access.Dispose();
        }

        private void HandleDataSourceFailure(Task<bool> result)
        {
            // report
            // recover
            // try restart
        }

        private bool Publish()
        {
            while (isSendingData)
            {
                if (isReceivingData)
                {
                    awaitingPrices.AddItems(access.Read(DataScope));
                }

                var prices = awaitingPrices.NextItems();
                if (prices == null)
                {
                    currentDataStatus = DataStatus.WaitingForData;
                    InvokeDataStatusChanged();

                    Console.WriteLine("Waiting for data at: " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                    cancelToken.WaitHandle.WaitOne(50);

                    continue;
                }
                else
                {
                    if (currentDataStatus != DataStatus.HasDataInQueue)
                    {
                        currentDataStatus = DataStatus.HasDataInQueue;
                        InvokeDataStatusChanged();
                    }

                    // send data
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
            if (NotifyDataArrive != null)
                NotifyDataArrive(prices);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Strategies;
using Automata.Entities;
using Automata.Core;

namespace Automata.Mechanisms
{
    public class BackTester
    {
        public BackTester(IDataScope testScope)
        {
            TestScope = testScope;

            DataSource = new DataSource(new HistoricalStaticDataFileAccess());
        }

        public IDataScope TestScope { get; set; }

        public List<Strategy> Strategies { get; set; }

        public DataSource DataSource { get; set; }

        #region lifecycle methods

        public void Start()
        {
            DataSource.DataScope = TestScope;
            DataSource.Initialize();
            SubscribeDataSource();
            DataSource.Start();
        }

        public void Stop()
        {
            DataSource.Stop();
            UnsubscribeDataSource();
        }

        private void SubscribeDataSource()
        {
            DataSource.NotifyNewPriceData += ProcessPriceData;
        }

        private void UnsubscribeDataSource()
        {
            DataSource.NotifyNewPriceData -= ProcessPriceData;
        }

        #endregion

        private void ProcessPriceData(HashSet<Price> prices)
        {

        }
    }
}
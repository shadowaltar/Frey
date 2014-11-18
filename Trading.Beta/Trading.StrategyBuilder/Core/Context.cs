using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using Ninject;
using Trading.Common.Data;
using Trading.StrategyBuilder.Data;

namespace Trading.StrategyBuilder.Core
{
    public class Context
    {
        [Inject]
        public IDataAccessFactory<Access> DataAccessFactory { get; set; }

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        private DataCriteria dataCriteria;

        public void Initialize(DateTime start, DateTime end, DataCriteria criteria)
        {
            Start = start;
            End = end;
            dataCriteria = criteria;
        }

        public void Run()
        {
            PrepareDataSource();
            PushData();
        }

        private void PrepareDataSource()
        {
            using (var access = DataAccessFactory.New())
            {
                Database.CachePrices(access.GetPrices(dataCriteria));
            }
        }

        private void PushData()
        {
            var prices = DataCache.PriceCache;
            var today = StartDate;
            for (int i = 0; i < TotalTradingDays; i++)
            {
                PushData(prices[today]);
                today = NextTradingDay(today);
            }
        }

        protected virtual void OnDataArrived(DateTime time)
        {

        }
    }

    public class DataCriteria : Condition
    {
        public string ToClause()
        {
            LeftOperand.ToClause();
            foreach (var condition in Conditions)
            {
                
            }
        }
    }
}
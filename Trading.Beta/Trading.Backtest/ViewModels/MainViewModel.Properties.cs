using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Policy;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Trading.Backtest.Data;
using Trading.Backtest.Reporting;
using Trading.Common.Entities;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel
    {
        public override string ProgramName { get { return "Backtest"; } }

        private ProgressDialogController progressIndicator;

        public BindableCollection<int> StartYears { get; private set; }
        public BindableCollection<int> EndYears { get; private set; }

        private int selectedStartYear;
        public int SelectedStartYear { get { return selectedStartYear; } set { SetNotify(ref selectedStartYear, value); } }
        private int selectedEndYear;
        public int SelectedEndYear { get { return selectedEndYear; } set { SetNotify(ref selectedEndYear, value); } }

        private string securityCodeToCheck;
        public string SecurityCodeToCheck { get { return securityCodeToCheck; } set { SetNotify(ref securityCodeToCheck, value); } }
        private int dayRangeToCheck;
        public int DayRangeToCheck { get { return dayRangeToCheck; } set { SetNotify(ref dayRangeToCheck, value); } }
        private int dateToCheck;
        public int DateToCheck { get { return dateToCheck; } set { SetNotify(ref dateToCheck, value); } }

        private Dictionary<DateTime, Dictionary<long, Price>> prices;

        private BacktestDataAccess commonAccess;
        private MySqlCommand commonCommand;

        private Core core;
        private DateTime testStart;
        private DateTime testEnd;
        private DateTime endOfData;

        private readonly HashSet<int> usNonMarketDates = new HashSet<int>();

        public BindableCollection<PriceReportEntry> SecurityToCheckPrices { get; private set; }
    }
}
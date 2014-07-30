using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.PagReport.Entities;

namespace Maintenance.PagReport.ViewModels
{
    public partial class MainViewModel
    {
        private readonly Dictionary<long, PagIndex> allPagIndexes = new Dictionary<long, PagIndex>();
        private readonly Dictionary<long, PagPortfolio> allPagPortfolios = new Dictionary<long, PagPortfolio>();
        private readonly Dictionary<HeadStockOverrideKey, HeadStockOverride> allHeadStockOverrides
            = new Dictionary<HeadStockOverrideKey, HeadStockOverride>();
        private readonly Dictionary<BarraIdOverrideKey, BarraIdOverride> allBarraIdOverrides
            = new Dictionary<BarraIdOverrideKey, BarraIdOverride>();

        private readonly BindableCollection<PagIndex> pagIndexes = new BindableCollection<PagIndex>();
        public BindableCollection<PagIndex> Indexes { get { return pagIndexes; } }

        private readonly BindableCollection<PagPortfolio> pagPortfolios = new BindableCollection<PagPortfolio>();
        public BindableCollection<PagPortfolio> Portfolios { get { return pagPortfolios; } }

        private readonly BindableCollection<HeadStockOverride> headStockOverrides = new BindableCollection<HeadStockOverride>();
        public BindableCollection<HeadStockOverride> HeadStockOverrides { get { return headStockOverrides; } }

        private readonly BindableCollection<BarraIdOverride> barraIdOverrides = new BindableCollection<BarraIdOverride>();
        public BindableCollection<BarraIdOverride> BarraIdOverrides { get { return barraIdOverrides; } }

        private bool isTabsEnabled;
        public bool IsTabsEnabled
        {
            get { return isTabsEnabled; }
            set { SetNotify(ref isTabsEnabled, value); }
        }

        private bool isInIndexView;
        public bool IsInIndexView
        {
            get { return isInIndexView; }
            set { SetNotify(ref isInIndexView, value); }
        }

        private bool isInPortfolioView;
        public bool IsInPortfolioView
        {
            get { return isInPortfolioView; }
            set { SetNotify(ref isInPortfolioView, value); }
        }

        private bool isInHeadStockOverrideView;
        public bool IsInHeadStockOverrideView
        {
            get { return isInHeadStockOverrideView; }
            set { SetNotify(ref isInHeadStockOverrideView, value); }
        }

        private bool isInBarraIdOverrideView;
        public bool IsInBarraIdOverrideView
        {
            get { return isInBarraIdOverrideView; }
            set { SetNotify(ref isInBarraIdOverrideView, value); }
        }
        
        private bool isEditIndexFlyoutOpened;
        public bool IsEditIndexFlyoutOpened
        {
            get { return isEditIndexFlyoutOpened; }
            set { SetNotify(ref isEditIndexFlyoutOpened, value); }
        }

        private bool isEditPortfolioFlyoutOpened;
        public bool IsEditPortfolioFlyoutOpened
        {
            get { return isEditPortfolioFlyoutOpened; }
            set { SetNotify(ref isEditPortfolioFlyoutOpened, value); }
        }

        private bool isEditHeadStockOverrideFlyoutOpened;
        public bool IsEditHeadStockOverrideFlyoutOpened
        {
            get { return isEditHeadStockOverrideFlyoutOpened; }
            set { SetNotify(ref isEditHeadStockOverrideFlyoutOpened, value); }
        }

        private bool isEditBarraIdOverrideFlyoutOpened;
        public bool IsEditBarraIdOverrideFlyoutOpened
        {
            get { return isEditBarraIdOverrideFlyoutOpened; }
            set { SetNotify(ref isEditBarraIdOverrideFlyoutOpened, value); }
        }

        private bool canToggleEdit;
        public bool CanToggleEdit
        {
            get { return canToggleEdit; }
            set { SetNotify(ref canToggleEdit, value); }
        }

        public PagIndex SelectedIndex { get; set; }

        public PagPortfolio SelectedPortfolio { get; set; }
        
        public HeadStockOverride SelectedHeadStockOverride { get; set; }

        public BarraIdOverride SelectedBarraIdOverride { get; set; }
    }
}
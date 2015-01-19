using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.ViewModels;
using Trading.StrategyBuilder.Views;
using Trading.StrategyBuilder.Views.Items;

namespace Trading.StrategyBuilder.Tests
{
    [TestFixture]
    public class CreateDecisionViewModelTests
    {
        [Test]
        [STAThread]
        public void TestView()
        {
            var csvm = new CreateDecisionViewModel();
            csvm.SelectedDecisionType = new ComboboxItem<DecisionType>("Buy", DecisionType.Long);

        }
    }
}

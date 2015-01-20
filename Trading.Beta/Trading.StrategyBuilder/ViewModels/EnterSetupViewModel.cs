using Caliburn.Micro;
using Ninject;
using PropertyChanged;
using System.Collections.Generic;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.Utils;
using Trading.StrategyBuilder.ViewModels.Entities;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        [Inject]
        public ICreateFilterViewModel CreateFilter { get; set; }
        [Inject]
        public ICreateDecisionViewModel CreateDecision { get; set; }
        public IViewService ViewService { get; set; }

        public bool IsInEditMode { get; set; }
        public EditMode EditMode { get; set; }

        public BindableCollection<FilterViewModel> Filters { get; private set; }

        private readonly List<Filter> filters = new List<Filter>();

        public EnterSetupViewModel()
        {
            Filters = new BindableCollection<FilterViewModel>();
        }

        protected override void OnViewLoaded(object view)
        {
            CreateFilter.ViewService = ViewService;
        }

        public async void AddFilter()
        {
            var r = await ViewService.ShowDialog(CreateFilter);
            if (r.HasValue && (bool)r)
            {
                var filter = CreateFilter.Generate();
                ConditionManager.AllFilters.AddIfNotExist(filter);
                filters.Add(filter);
                Filters.Add(filter.CreateViewModel());
            }
        }

        public async void AddDecision()
        {
            CreateDecision.ConditionResults.ClearAndAddRange(ConditionManager.AllConditionResults);
            await ViewService.ShowDialog(CreateDecision);
        }

        //public async void AddDecision()
        //{
        //    var result = await ViewService.ShowDialog(CreateDecision);
        //    if (!result.IsTrue())
        //    {
        //        return;
        //    }
        //    //var stage = CreateDecision.Yield();
        //    //stages.Add(stage);
        //    //Stages.Add(stage.CreateViewModel());
        //}

        //public async void AddCondition()
        //{
        //    if (SelectedStage == null)
        //        return;

        //    var result = await ViewService.ShowDialog(CreateCondition);
        //    if (!result.IsTrue())
        //    {
        //        return;
        //    }

        //    SelectedStage.Conditions.Add(CreateCondition.Yield());
        //}

        //public void EditStep()
        //{

        //}

        //public void LinkStep()
        //{
        //    IsInEditMode = true;
        //    EditMode = EditMode.Link;
        //}

        //public void RemoveStep()
        //{
        //    IsInEditMode = true;
        //    EditMode = EditMode.Delete;
        //}

        //private void ExitEditMode()
        //{
        //    IsInEditMode = false;
        //    EditMode = EditMode.None;
        //}
    }

    public enum EditMode
    {
        None,
        Link,
        Delete,
    }

    public interface IEnterSetupViewModel : IHasViewService
    {
    }

    //internal class CanvasWorker
    //{
    //    private DecisionGraphCanvas canvas;

    //    public CanvasWorker(DecisionGraphCanvas canvas)
    //    {
    //        this.canvas = canvas;
    //    }

    //    public bool AddCondition(Condition condition)
    //    {
    //        canvas.AddCondition(condition);
    //        return false;
    //    }

    //    public bool LinkConditions(Condition c1, Condition c2)
    //    {
    //        canvas.LinkConditionsWithAnd(c1, c2);
    //        return true;
    //    }
    //}
}
using Caliburn.Micro;
using Ninject;
using PropertyChanged;
using System.Collections.Generic;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.Utils;
using Trading.StrategyBuilder.Views.Controls;
using Condition = Trading.StrategyBuilder.Core.Condition;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        [Inject]
        public ICreateConditionViewModel CreateCondition { get; set; }
        [Inject]
        public ICreateStageViewModel CreateStage { get; set; }
        public IViewService ViewService { get; set; }

        public bool IsInEditMode { get; set; }
        public EditMode EditMode { get; set; }

        public IStageViewModel SelectedStage { get; set; }
        public BindableCollection<StageViewModel> Stages { get; private set; }

        private readonly List<Stage> stages = new List<Stage>();

        public EnterSetupViewModel()
        {
            Stages = new BindableCollection<StageViewModel>();
        }

        public async void AddStage()
        {
            var result = await ViewService.ShowDialog(CreateStage);
            if (!result.IsTrue())
            {
                return;
            }
            var stage = CreateStage.Yield();
            stages.Add(stage);
            Stages.Add(stage.CreateViewModel());
        }

        public async void AddCondition()
        {
            if (SelectedStage == null)
                return;

            var result = await ViewService.ShowDialog(CreateCondition);
            if (!result.IsTrue())
            {
                return;
            }

            SelectedStage.Conditions.Add(CreateCondition.Yield());
        }

        public void EditStep()
        {

        }

        public void LinkStep()
        {
            IsInEditMode = true;
            EditMode = EditMode.Link;
        }

        public void RemoveStep()
        {
            IsInEditMode = true;
            EditMode = EditMode.Delete;
        }

        private void ExitEditMode()
        {
            IsInEditMode = false;
            EditMode = EditMode.None;
        }
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

    internal class CanvasWorker
    {
        private DecisionGraphCanvas canvas;

        public CanvasWorker(DecisionGraphCanvas canvas)
        {
            this.canvas = canvas;
        }

        public bool AddCondition(Condition condition)
        {
            canvas.AddCondition(condition);
            return false;
        }

        public bool LinkConditions(Condition c1, Condition c2)
        {
            canvas.LinkConditionsWithAnd(c1, c2);
            return true;
        }
    }
}
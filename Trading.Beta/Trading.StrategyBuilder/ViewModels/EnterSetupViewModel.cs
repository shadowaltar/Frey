using System.Windows;
using Caliburn.Micro;
using Ninject;
using PropertyChanged;
using System;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Views;
using Trading.StrategyBuilder.Views.Controls;
using Condition = Trading.StrategyBuilder.Core.Condition;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        [Inject]
        public ICreateConditionViewModel CreateCondition { get; set; }
        public IViewService ViewService { get; set; }

        public bool IsInEditMode { get; set; }
        public EditMode EditMode { get; set; }

        //public BindableCollection<RuleViewModel> Rules { get; private set; }
        public BindableCollection<Node> Nodes { get; private set; }
        private CanvasWorker canvasWorker;

        public EnterSetupViewModel()
        {
            Nodes = new BindableCollection<Node>();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            InitializeCanvasWorker(((EnterSetupView)view).Canvas);
        }

        private void InitializeCanvasWorker(DecisionGraphCanvas graphCanvas)
        {
            canvasWorker = new CanvasWorker(graphCanvas);
        }

        public void AddFilter()
        {
        }

        public async void AddCondition()
        {
            var result = await ViewService.ShowDialog(CreateCondition);
            if (!result.IsTrue())
            {
                return;
            }

            canvasWorker.AddCondition(CreateCondition.Get());
        }

        private void OnVertexSelected(object sender, EventArgs args)
        {
            //selectedVertexes.Add((ActionVertex)sender);

            switch (EditMode)
            {
                case EditMode.Delete:
                    //     if (selectedVertexes.Count == 1)
                    {
                        //var vertex = selectedVertexes.First();
                        //vertex.SelectEvent -= OnVertexSelected;
                        //selectedVertexes.Remove(vertex);
                        //   Graph.RemoveVertex(vertex);
                        ExitEditMode();
                    }
                    break;
                case EditMode.Link:
                    //if (selectedVertexes.Count == 2)
                    {
                        //    var vertexes = selectedVertexes.ToList();
                        //    edge = new ActionEdge(vertexes[0], vertexes[1]);
                        ////    Graph.AddEdge(edge);
                        //    selectedVertexes.Clear();

                        ExitEditMode();
                    }
                    break;
            }
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
    }
}
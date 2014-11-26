using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using PropertyChanged;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core.Visualization;
using Trading.StrategyBuilder.Views;
using LayoutMode = GraphSharp.Controls.LayoutMode;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        public ICreateConditionViewModel CreateCondition { get; set; }
        public IViewService ViewService { get; set; }

        public StrategyGraph Graph { get; set; }
        public StrategyGraphLayout GraphLayout { get; set; }

        public string LayoutAlgorithmType { get; set; }
        public bool IsInEditMode { get; set; }
        public EditMode EditMode { get; set; }

        public BindableCollection<RuleViewModel> Rules { get; private set; }

        private readonly HashSet<ActionVertex> selectedVertexes = new HashSet<ActionVertex>();

        public ILayoutParameters X { get; set; }

        public EnterSetupViewModel(ICreateConditionViewModel createCondition)
        {
            CreateCondition = createCondition;
            Rules = new BindableCollection<RuleViewModel>();

            Graph = new StrategyGraph();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            LayoutAlgorithmType = "Tree";
            GraphLayout = ((EnterSetupView)view).GraphLayout;
            Graph.AddVertex(new ActionVertex { Expression = "BEGIN" });
            var param = GraphLayout.LayoutParameters as SimpleTreeLayoutParameters;
            param.Direction = LayoutDirection.LeftToRight;
            param.LayerGap = 30;
            param.VertexGap = 30;
        }

        public void AddFilter()
        {
            //var vertex = new ActionVertex();
            //vertex.SelectEvent += OnVertexSelected;
            //vertex.Expression = "This is a filter";
            //Graph.AddVertex(vertex);


            var vertex1 = new ActionVertex { Expression = "1" };
            var vertex2 = new ActionVertex { Expression = "2" };
            var vertex3 = new ActionVertex { Expression = "3" };
            Graph.AddVerticesAndEdge(new ActionEdge(vertex1, vertex2));
            Graph.AddVerticesAndEdge(new ActionEdge(vertex1, vertex3));
        }

        public void AddAction()
        {
            var vertex = new ActionVertex();
            vertex.SelectEvent += OnVertexSelected;
            vertex.Expression = DateTime.Now.ToTimeDouble().ToString(); // fake
            Graph.AddVertex(vertex);
        }

        private void OnVertexSelected(object sender, EventArgs args)
        {
            selectedVertexes.Add((ActionVertex)sender);

            switch (EditMode)
            {
                case EditMode.Delete:
                    if (selectedVertexes.Count == 1)
                    {
                        var vertex = selectedVertexes.First();
                        vertex.SelectEvent -= OnVertexSelected;
                        selectedVertexes.Remove(vertex);
                        Graph.RemoveVertex(vertex);
                        ExitEditMode();
                    }
                    break;
                case EditMode.Link:
                    if (selectedVertexes.Count == 2)
                    {
                        var vertexes = selectedVertexes.ToList();
                        Graph.AddEdge(new ActionEdge(vertexes[0], vertexes[1]));
                        selectedVertexes.Clear();
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
            foreach (var vertex in Graph.Vertices)
            {
                vertex.IsSelected = false;
                vertex.CanSelect = true;
            }
            GraphLayout.Relayout();
        }

        public void RemoveStep()
        {
            IsInEditMode = true;
            EditMode = EditMode.Delete;
            foreach (var vertex in Graph.Vertices)
            {
                vertex.IsSelected = false;
                vertex.CanSelect = true;
            }
        }

        private void ExitEditMode()
        {
            LayoutAlgorithmType = "Tree";
            IsInEditMode = false;
            EditMode = EditMode.None;
            foreach (var vertex in Graph.Vertices)
            {
                vertex.IsSelected = false;
                vertex.CanSelect = false;
            }
            //if (LayoutAlgorithmType == "Tree")
            //    LayoutAlgorithmType = "EfficientSugiyama";
            //GraphLayout.LayoutMode=LayoutMode.Compound;
            GraphLayout.Relayout();
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
}
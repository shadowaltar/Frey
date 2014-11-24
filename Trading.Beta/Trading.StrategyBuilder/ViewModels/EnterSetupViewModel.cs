using System.Threading;
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.Core.Visualization;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        public ICreateConditionViewModel CreateCondition { get; set; }
        public IViewService ViewService { get; set; }

        public StrategyGraph Graph { get; set; }
        public StrategyGraphLayout GraphLayout { get; set; }
        public ActionVertex SourceVertex { get; set; }
        public ActionVertex TargetVertex { get; set; }

        public string LayoutAlgorithmType { get; set; }

        public BindableCollection<RuleViewModel> Rules { get; private set; }

        private int ruleIndex;

        public EnterSetupViewModel(ICreateConditionViewModel createCondition)
        {
            CreateCondition = createCondition;
            Rules = new BindableCollection<RuleViewModel>();

            Graph = new StrategyGraph();
            AddVertex();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            LayoutAlgorithmType = "Tree";
        }

        public void AddFilter()
        {
            var vertex = new ActionVertex();
            vertex.Formula = "Do something";
            Graph.AddVertex(vertex);
            // ViewService.ShowDialog()
        }

        public void AddAction()
        {
            var vertex = new ActionVertex();
            // ViewService.ShowDialog()
        }

        public void EditStep()
        {

        }

        public void LinkStep()
        {

        }

        public void RemoveStep()
        {

        }

        public void AddVertex()
        {
            var vertex = new ActionVertex();
            vertex.Formula = "ABC";
            Graph.AddVertex(vertex);
            var vertex2 = new ActionVertex();
            vertex2.Formula = "DEF";
            var edge = new ActionEdge(vertex, vertex2);
            Graph.AddVertex(vertex2);
            Graph.AddEdge(edge);

            if (GraphLayout != null)
                GraphLayout.Relayout();
        }

        public void RemoveVertex()
        {
            //var vertex = new ActionVertex();
        }

        public void LinkVertexes()
        {
            if (SourceVertex == null || TargetVertex == null)
                return;

            var link = new ActionEdge(SourceVertex, TargetVertex);
        }

        //public async void CreateSecurityRules()
        //{
        //    var isGood = await ViewService.ShowDialog(CreateCondition as ViewModelBase);
        //    if (isGood.HasValue && (bool)isGood)
        //    {
        //        var condition = new Condition(CreateCondition.SourceValue, CreateCondition.SelectedOperator.FromSymbol(),
        //            CreateCondition.TargetValue);

        //        var i = Interlocked.Increment(ref ruleIndex);
        //        if (Rules.Count == 0)
        //        {
        //            var conds = new FilterSecurityConditions { condition };
        //            Rules.Add(new RuleViewModel(conds) { RuleIndex = i });
        //        }
        //        else
        //        {
        //            Rules[0].Conditions.Add(condition);
        //        }
        //    }
        //}
    }

    public interface IEnterSetupViewModel : IHasViewService
    {
    }
}
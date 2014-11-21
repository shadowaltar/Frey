using QuickGraph;

namespace Trading.StrategyBuilder.Core.Visualization
{
    public class ActionEdge : Edge<ActionVertex>
    {
        public ActionEdge(ActionVertex source, ActionVertex target) : base(source, target)
        {
        }

        public string Id { get; set; } 
    }
}
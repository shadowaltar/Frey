using System;
using GraphSharp.Controls;

namespace Trading.StrategyBuilder.Core.Visualization
{
    public class StrategyGraphLayout : GraphLayout<ActionVertex, ActionEdge, StrategyGraph>
    {
        private int previousEdgeControlCount = 0;

        public event LayoutFinishedDelegate LayoutFinished;
        public event EdgeControlCountChangedDelegate EdgeControlCountChanged;

        protected virtual void InvokeLayoutFinished()
        {
            LayoutFinishedDelegate handler = LayoutFinished;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void InvokeEdgeControlCountChanged(int oldCount, int newCount)
        {
            EdgeControlCountChangedDelegate handler = EdgeControlCountChanged;
            if (handler != null) handler(this, new EdgeControlCountChangedDelegateArgs(oldCount, newCount));
        }

        protected override void OnLayoutFinished()
        {
            base.OnLayoutFinished();
            InvokeLayoutFinished();

            var currentEdgeControlCount = _edgeControls.Count;
            if (previousEdgeControlCount != currentEdgeControlCount)
            {
                InvokeEdgeControlCountChanged(previousEdgeControlCount, currentEdgeControlCount);
                previousEdgeControlCount = currentEdgeControlCount;
            }
        }
    }

    public delegate void EdgeControlCountChangedDelegate(object sender, EdgeControlCountChangedDelegateArgs args);

    public class EdgeControlCountChangedDelegateArgs
    {
        public int Before { get; set; }
        public int After { get; set; }

        public EdgeControlCountChangedDelegateArgs(int before, int after)
        {
            Before = before;
            After = after;
        }
    }

    public delegate void LayoutFinishedDelegate(object sender, EventArgs args);
}
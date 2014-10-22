using System.Windows.Documents;
using Trading.Common.ViewModels;
using Trading.SecurityResearch.ViewModels.Entities;
using Trading.SecurityResearch.Views;

namespace Trading.SecurityResearch.ViewModels
{
    public class ResearchReportViewModel : ViewModelBase, IResearchReportViewModel
    {
        private string securitySymbol;
        public string SecuritySymbol
        {
            get { return securitySymbol; }
            set { SetNotify(ref securitySymbol, value); }
        }

        private string securityName;
        public string SecurityName
        {
            get { return securitySymbol; }
            set { SetNotify(ref securityName, value); }
        }

        private string securityReportDate;
        public string SecurityReportDate
        {
            get { return securityReportDate; }
            set { SetNotify(ref securityReportDate, value); }
        }

        private string currentPrice;
        public string CurrentPrice
        {
            get { return currentPrice; }
            set { SetNotify(ref currentPrice, value); }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var report = (ResearchReportView)view;

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(System.IO.File.ReadAllText("SampleResearchDocument.txt"));
            var document = new FlowDocument(paragraph);

            report.DocumentReader.Document = document;
        }
    }

    public interface IResearchReportViewModel
    {
        string SecuritySymbol { get; set; }
        string SecurityName { get; set; }
        string SecurityReportDate { get; set; }
        string CurrentPrice { get; set; }
    }
}
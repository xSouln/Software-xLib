using xLibV100.Controls;

namespace xLibV100.UI
{
    public interface IPortsViewModel
    {
        TerminalObject Device { get; set; }
        void Subscribe();
        void Unsubscribe();
        void SelectTxLine();
        void ResetTxLine();
    }
}

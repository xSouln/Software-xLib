using xLibV100.UI;

namespace xLibV100.Common.UI
{
    public class MediatorViewModelParser : UINotifyPropertyChanged
    {
        public virtual int Parse(MediatorViewModel viewModel, object model, MediatorViewModel.ParseParameters parameters)
        {
            return -1;
        }
    }
}

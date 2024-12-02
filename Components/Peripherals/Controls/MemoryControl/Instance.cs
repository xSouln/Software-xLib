using System.Threading.Tasks;
using xLibV100.Controls;
using xLibV100.Transactions.Common;

namespace xLibV100.Peripherals.MemoryControl
{
    public class Instance : Instance<MemoryControl>
    {
        protected string description;
        protected int countOfSectors;
        protected int startAddress;
        protected int spaceSize;

        public Instance(PeripheralBase model) : base(model)
        {

        }

        public async Task DeleteFileAsync(byte[] name)
        {
            try
            {
                var transactions = Services.GetByType<Transactions.Control>();
                var control = Services.GetByType<PeripheralControl>();

                var response = await control.SendRequestAsync(transactions.DeleteFile.Prepare(new Transactions.Control.RequestDeleteFile(name)));
            }
            catch
            {

            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string Description
        {
            get => description;
            protected set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description), description);
                }
            }
        }
    }
}

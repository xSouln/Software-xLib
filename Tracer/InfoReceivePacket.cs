using xLibV100.Transceiver;
using xLibV100.UI;

namespace xLibV100.Components
{
    public partial class xTracer
    {
        public class ReceivePacketInfo
        {
            public string Time { get; set; } = "";

            public string Note { get; set; } = "";

            public string Data { get; set; } = "";
        }

        public class RequestInfoElement : UINotifyPropertyChanged
        {
            public string Time { get; set; } = "";
            public string Module { get; set; } = "";
            public string RequestName { get; set; } = "";
            public TxStatus RequestResult { get; set; }
            public TxResponses ResponseResult { get; set; }
            public int ResponseTime { get; set; }
        }
    }
}

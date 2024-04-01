using System;
using System.Collections.ObjectModel;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Components
{
    public partial class xTracer
    {
        public static ObservableCollection<ReceivePacketInfo> Info { get; set; } = new ObservableCollection<ReceivePacketInfo>();
        public static ObservableCollection<RequestInfoElement> RequestInfo { get; set; } = new ObservableCollection<RequestInfoElement>();

        public static ActionAccessUI PointEntryUI = xSupport.ActionThreadUI;

        public static bool Pause;

        public static void Message(string note, string data)
        {
            if (Pause)
            {
                return;
            }

            try
            {
                PointEntryUI?.Invoke((RequestUI) =>
                {
                    if (Info.Count > 500)
                    {
                        Info.RemoveAt(Info.Count - 1);
                    }

                    Info.Insert(0, new ReceivePacketInfo
                    {
                        Time = DateTime.Now.ToUniversalTime().ToString(),
                        Note = note,
                        Data = data
                    });
                }, null);
            }
            catch
            {

            }
        }

        public static void Message(RequestBase request, string name)
        {
            if (request != null)
            {
                Message("module: " + name + "\r" +
                    "request: " + request.Name + "\r" +
                    "request result: " + request.Status + "\r" +
                    "response result = " + request.ResponseResult + "\r" +
                    "response time: " + request.ResponseTime);
            }
            else
            {
                Message(" request = null");
            }
        }

        public static void Message(string data)
        {
            Message("info:", data);
        }

        public static void Trace(RequestBase request, string name)
        {
            if (Pause || request == null)
            {
                return;
            }

            try
            {
                PointEntryUI?.Invoke((RequestUI) =>
                {
                    if (RequestInfo.Count > 500)
                    {
                        RequestInfo.RemoveAt(RequestInfo.Count - 1);
                    }

                    RequestInfo.Insert(0, new RequestInfoElement
                    {
                        Time = DateTime.Now.ToUniversalTime().ToString(),
                        Module = name,
                        RequestName = request.Name,
                        RequestResult = request.Status,
                        ResponseResult = request.ResponseResult,
                        ResponseTime = request.ResponseTime
                    });
                }, null);
            }
            catch
            {

            }
        }
    }
}

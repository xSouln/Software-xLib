using System;
using System.Collections.Generic;
using System.IO;
using xLibV100.Common;
using xLibV100.Controls;
using xLibV100.Ports;

namespace xLibV100.Serializers
{
    public class PortSubscriptions
    {
        public class PortSubscriptionInfo
        {
            public string Name { get; set; }
            public long Id { get; set; }
        }

        public List<PortSubscriptionInfo> Ports { get; set; } = new List<PortSubscriptionInfo>();
        public PortSubscriptionInfo SelectedPort { get; set; }

        public static int Save(TerminalObject device, string subDirectory)
        {
            if (device != null)
            {
                if (!(device.Ports.Count > 0))
                {
                    try
                    {
                        File.WriteAllText(Environment.CurrentDirectory + "\\" + subDirectory + "\\ports" + ".json", string.Empty);
                    }
                    catch { }

                    return 0;
                }

                var portSubscriptions = new PortSubscriptions();

                foreach (var elemnt in device.Ports)
                {
                    portSubscriptions.Ports.Add(new PortSubscriptionInfo
                    {
                        Name = elemnt.Name,
                        Id = elemnt.Id
                    });
                }

                if (device.SelectedPort != null)
                {
                    portSubscriptions.SelectedPort = new PortSubscriptionInfo
                    {
                        Name = device.SelectedPort.Name,
                        Id = device.SelectedPort.Id
                    };
                }

                Json.CreateFolder(Environment.CurrentDirectory, subDirectory);
                return Json.Save(Environment.CurrentDirectory + "\\" + subDirectory + "\\ports" + ".json", portSubscriptions);
            }
            return -1;
        }

        public static int Open(TerminalObject device, string subDirectory)
        {
            if (device != null)
            {
                PortSubscriptions subscriptions;
                Json.Open(Environment.CurrentDirectory + "\\" + subDirectory + "\\ports.json", out subscriptions);

                if (subscriptions == null)
                {
                    return 1;
                }

                //var ports = device.Terminal.AvailablePorts.Where(x => subscriptions.Ports.Any(y => x.Id == y.Id && x.Name == y.Name))?.ToList();
                List<PortBase> ports = new List<PortBase>();

                foreach (var subscription in subscriptions.Ports)
                {
                    foreach (var element in device.Terminal.AvailablePorts)
                    {
                        if (element.Id == subscription.Id && element.Name == subscription.Name)
                        {
                            ports.Add(element);
                        }
                        else if (element.SubPorts != null)
                        {
                            foreach (var subPort in element.SubPorts)
                            {
                                if (subPort.Id == subscription.Id && subPort.Name == subscription.Name)
                                {
                                    ports.Add(subPort);
                                }
                            }
                        }
                    }
                }

                if (ports != null && ports.Count > 0)
                {
                    foreach (var element in ports)
                    {
                        device.Subscribe(element);
                    }
                }

                if (subscriptions.SelectedPort != null)
                {
                    foreach (var element in device.Terminal.AvailablePorts)
                    {
                        if (element.Name == subscriptions.SelectedPort.Name && element.Id == subscriptions.SelectedPort.Id)
                        {
                            device.SelectedPort = element;
                            return 0;
                        }
                    }
                }

                return 0;
            }
            return -1;
        }
    }
}

using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;
using System;
using MQTTnet.Client.Options;
using System.Text;
using SimmeMqqt.Model;
using SimmeMqqt.Service;

namespace SimmeMqqt
{
    public class MQTT_Client
    {
        public static CancellationTokenSource cts = new CancellationTokenSource(); //TODO create token using the Timeout delay from config
        public static async Task Subscribe_Topic()
        {
            var EFMachineData = new MQTTMachineData();
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                .Build();
            try
            {
                mqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();
                    if(e.ApplicationMessage.Topic.Contains("timestamp"))
                    {
                        EFMachineData.Timestamp = Convert.ToDateTime(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if(e.ApplicationMessage.Topic.Contains("naam"))
                    {
                        EFMachineData.MachineName = Convert.ToString(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic.Contains("ideale_cyclus_tijd"))
                    {
                        EFMachineData.IdealCyclus = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic.Contains("totaal_geproduceerd"))
                    {
                        EFMachineData.TotalProduction = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic.Contains("goed_geproduceerd"))
                    {
                        EFMachineData.TotalGoodProduction = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic.Contains("id"))
                    {
                        EFMachineData.MachineID = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic.Contains("pauze"))
                    {
                        EFMachineData.Break = Convert.ToBoolean(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic.Contains("storing"))
                    {
                        EFMachineData.Failure = Convert.ToBoolean(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        DashboardService.UpdateDashboard(EFMachineData);
                    }
                });
                mqttClient.UseConnectedHandler(async e =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("gc/machine1/#").Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                });
                await mqttClient.ConnectAsync(options, cts.Token);
                // UNCOMMENT AND YOU WILL RECEIVE A MESSAGE Task.Run(() => mqttClient.PublishAsync("MyClientIDHere/Device_2/Instance_1","met=Temperature~data=29"));

            }

            catch (OperationCanceledException)
            {
                Console.WriteLine("task cancelled");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}
using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;
using System;
using MQTTnet.Client.Options;
using System.Text;

namespace SimmeMqqt
{

    class MQTT_Client
    {
        private static CancellationTokenSource cts = new CancellationTokenSource(); //TODO create token using the Timeout delay from config
        private static async Task Subscribe_Topic()
        {
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
﻿using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;
using System;
using MQTTnet.Client.Options;
using System.Text;
using SimmeMqqt.Model;
using SimmeMqqt.EntityFramework;
using MQTTnet.Extensions.ManagedClient;

namespace SimmeMqqt
{
    public class MQTT_Client
    {
        public MQTT_Client()
        {
            Console.WriteLine("JESUS FUCKIGN CHRITS");
            Subscribe_Topic();
        }

        public MQTTMachineData EFMachineData;
        public CancellationTokenSource cts = new CancellationTokenSource(); //TODO create token using the Timeout delay from config
        public async Task Subscribe_Topic()
        {
            EFMachineData = new MQTTMachineData();
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithTcpServer("185.229.236.100")
                    .Build())
                .Build();
            var mqttClient = new MqttFactory().CreateManagedMqttClient();
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
                    if (e.ApplicationMessage.Topic == "gc/machine1/timestamp")
                    {
                        EFMachineData.Timestamp = Convert.ToDateTime(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/ideale_cyclus_tijd")
                    {
                        EFMachineData.IdealCyclus = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/id")
                    {
                        EFMachineData.MachineID = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/naam")
                    {
                        EFMachineData.MachineName = Convert.ToString(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/storing")
                    {
                        EFMachineData.Failure = Convert.ToBoolean(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/pauze")
                    {
                        EFMachineData.Break = Convert.ToBoolean(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/totaal_geproduceerd")
                    {
                        EFMachineData.TotalProduction = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    }
                    else if (e.ApplicationMessage.Topic == "gc/machine1/goed_geproduceerd")
                    {
                        EFMachineData.TotalGoodProduction = Convert.ToInt32(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        delayedWork();
                    }
                });
                mqttClient.UseConnectedHandler(async e =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                        // Subscribe to a topic
                        await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("gc/machine1/#").Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                });
                await mqttClient.StartAsync(options);

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

        }

        private async Task delayedWork()
        {
            await Task.Delay(2000);
            AddSQLData(EFMachineData);
        }

        public void AddSQLData(MQTTMachineData data)
        {
            var EFMachineData = new MQTTMachineData();
            EFMachineData.Break = data.Break;
            EFMachineData.Failure = data.Failure;
            EFMachineData.MachineID = data.MachineID;
            EFMachineData.MachineName = data.MachineName;
            EFMachineData.TotalGoodProduction = data.TotalGoodProduction;
            EFMachineData.TotalProduction = data.TotalProduction;
            EFMachineData.IdealCyclus = data.IdealCyclus;
            EFMachineData.Timestamp = data.Timestamp;
            using (var context = new MachineData())
            {
                var Data = EFMachineData;

                context.Add(EFMachineData);
                context.SaveChanges();
            }
        }
    }
}
using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AMQP_ConsumerQueue_WorkQueue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "myQueue",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                    var consumer = new EventingBasicConsumer(channel);      //declaring consumer
                    consumer.Received += (model, ea) =>                     //assignment of the body of the response to the consumer when the received event happens
                    {                                                       
                        var body = ea.Body;                                 //gets the body of the message
                        var message = Encoding.UTF8.GetString(body);        //parses it to string to display it
                        Console.WriteLine(" [x] received {0}", message);    

                        int dots = message.Split(".").Length - 1;           //simulates the latency for different processes
                        Thread.Sleep(dots * 1000);

                        Console.WriteLine("[x] Done");
                    };

                    channel.BasicConsume(queue: "myQueue",                  //consumes the queue
                                            autoAck: true,
                                            consumer: consumer);
                    Console.WriteLine("press [enter] to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}

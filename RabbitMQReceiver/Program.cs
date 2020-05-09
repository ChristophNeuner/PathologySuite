using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            string q_name_preds = "pituitary_gland_adenomas_entity_classification_predictions";
            string connection_address = "localhost";

            var factory = new ConnectionFactory() { HostName = connection_address };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: q_name_preds, durable: true, exclusive: false, autoDelete: true, arguments: null);
                channel.QueueBind(queue: q_name_preds, exchange: "PathologySuite.AI", routingKey: "multiLabelClassification.pituitaryAdenomas.entities.prediction");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine(" [x] Received {0}", message);

                    // Manual message acknowledgment
                    // Note: it is possible to access the channel via
                    //       ((EventingBasicConsumer)sender).Model here
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: q_name_preds,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}

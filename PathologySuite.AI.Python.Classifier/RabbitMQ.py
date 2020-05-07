import pika
import time

q_name_work = "pituitary_gland_entity_classification_work";
q_name_preds = "pituitary_gland_entity_classification_preds";
connection_address = 'localhost'


### do work and send response
def callback(ch, method, properties, body):
    #work
    print(f" [x] Received {body}")
    for i in range(6):
        print(i)
        time.sleep(1)

    #send response
    connection = pika.BlockingConnection(pika.ConnectionParameters(connection_address))
    channel = connection.channel()
    channel.queue_declare(queue=q_name_preds)
    response = 'ACTH: 10%; LH: 95%'
    channel.basic_publish(exchange='',
                      routing_key=q_name_preds,
                      body=response)
    print(f'[x] Sent {response}')
    connection.close()
    #Manual message acknowledgment
    ch.basic_ack(delivery_tag = method.delivery_tag)


### receive work
connection = pika.BlockingConnection(pika.ConnectionParameters(connection_address))
channel = connection.channel()
channel.queue_declare(queue=q_name_work)
channel.basic_consume(queue=q_name_work,
                      auto_ack=False,
                      on_message_callback=callback)


print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()

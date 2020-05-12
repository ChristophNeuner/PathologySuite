import pika
import time
import pga_entity_classifier
import pathlib
from pathlib import Path

q_name_work = "pituitary_gland_adenomas_entity_classification_new_work";
connection_address = 'localhost'


### do work and send response
def callback(ch, method, properties, body):

    path_wsi = body.decode()

    print(f" [x] Received {path_wsi}")
    
    ###
    # make predictions
    ###
    path_exported_learner = pathlib.Path('D:/FAUbox/Projects/PathologySuite/PathologySuite.AI.Python.Classifier/n_13-iteration_4-weights_bestmodel_lr2=1e-7--lr3=1e-6_9.pkl')  
    preds = pga_entity_classifier.predict_pga_entity(path_exported_learner, path_wsi)
    
    ###
    #send response
    ###
    response = str(preds)

    connection = pika.BlockingConnection(pika.ConnectionParameters(connection_address))
    channel = connection.channel()    
    channel.basic_qos(prefetch_count=1) #Fair dispatch
    properties = pika.BasicProperties(delivery_mode = 2,) # make message persistent
    channel.basic_publish(exchange='PathologySuite.AI',
                      routing_key='multiLabelClassification.pituitaryAdenomas.entities.prediction',
                      body=response, 
                      properties=properties) 
    print(f'[x] Sent {response}')
    connection.close()
    
    ch.basic_ack(delivery_tag = method.delivery_tag) #Manual message acknowledgment


### receive work
connection = pika.BlockingConnection(pika.ConnectionParameters(connection_address))
channel = connection.channel()
#Fair dispatch
channel.basic_qos(prefetch_count=1)

channel.queue_declare(queue=q_name_work, durable=True, auto_delete=False)
channel.queue_bind(exchange='PathologySuite.AI', queue=q_name_work, routing_key='multiLabelClassification.pituitaryAdenomas.entities.newWork')
channel.basic_consume(queue=q_name_work, on_message_callback=callback, auto_ack=False)


print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()

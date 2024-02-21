## Steps:

1. Run RabbitMQ in Docker
   Commands:
   ```sh
   docker pull rabbitmq:3.11-management
   docker run --rm -it -p 15672:15672 -p 5672:5672 rabbitmq:3.11-management
   
2. Run Redis in Docker
   ```sh
   Copy code
   docker pull redis
   docker run -d --name microservices-redis -p 6379:6379 redis

3. Execute Database script named "MicroservicesScript.sql" in SQL server

4. Inside Visual Studio, Configure Startup Project and select all:
   ```sh
   AuthenticationMicroservice
   ProductsMicroservice
   WebApi
   WebUI

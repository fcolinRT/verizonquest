# verizonquest
Unity Socket
This server creates a simple socket to send the vehicles data.

How to run the script
Docker and docker compose are required for this project.

Make sure you have the mqtt script running before this server.

In order to run this script you should run the following commnads:

$ docker build -t verizon-socket .
$ docker run -it --network="host" -p 8000:8000 -e FROM_DATE=2020-01-22T03:42:00.674Z verizon-socket

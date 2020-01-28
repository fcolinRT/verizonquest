package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"time"

	"github.com/googollee/go-socket.io"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/bson"
)

type Velocity struct {
	X string
	Y string
	Z string
}

type PositionGPS struct {
	Latitude string
	Longitude string
	Altitude string
}

type Common struct {
	Timestamp time.Time
	ObjectType string
	Uuid string
}

type Response struct {
	Common Common
	PositionGPS PositionGPS
	Velocity Velocity
}

func getVehicles (client *mongo.Client, filter bson.M) []*Response {
	var vehicles []*Response
	verizonDatabase := client.Database("verizon")
	vehiclesCollection := verizonDatabase.Collection("vehicles")
	cur, err := vehiclesCollection.Find(context.TODO(), filter)
	if err != nil {
        log.Fatal("Error on Finding all the documents", err)
	}
	for cur.Next(context.TODO()) {
        var vehicle Response
        err = cur.Decode(&vehicle)
        if err != nil {
            log.Fatal("Error on Decoding the document", err)
        }
        vehicles = append(vehicles, &vehicle)
    }
    return vehicles
}

func main() {

	client, err := mongo.NewClient(options.Client().ApplyURI("mongodb://verzion:test123@127.0.0.1:27017"))
	if err != nil {
		log.Fatal(err)
	}
	ctx, _ := context.WithTimeout(context.Background(), 10*time.Second)
	err = client.Connect(ctx)
	if err != nil {
		log.Fatal(err)
	}
	defer client.Disconnect(ctx)


	server, err := socketio.NewServer(nil)
	if err != nil {
		log.Fatal(err)
	}

	layout := "2006-01-02T15:04:05.000Z"
	str := "2020-01-20T01:13:00.674Z"
	fromDate, err := time.Parse(layout, str)
	if err != nil {
		fmt.Println(err)
	}
	fmt.Println(fromDate)
	server.OnConnect("/", func(s socketio.Conn) error {
		s.SetContext("")
		fmt.Println("connected:", s.ID())
		uptimeTicker := time.NewTicker(10 * time.Second)
		vehicles := getVehicles(client, bson.M{
			"common.timestamp": bson.M{
			"$gte": fromDate,
			},
		})
		s.Emit("/reply", vehicles)
		for {
			select {
			case <-uptimeTicker.C:
				fmt.Println("funciono cada 5")
				fromDate = fromDate.Add(10 * time.Second)
				if err != nil {
					fmt.Println(err)
				}
				fmt.Println(fromDate)
				vehicles := getVehicles(client, bson.M{
					"common.timestamp": bson.M{
						"$gte": fromDate,
					},
				})
				s.Emit("/reply", vehicles)
			}
		}
		return nil
	})
	server.OnError("/", func(e error) {
		fmt.Println("meet error:", e)
	})
	server.OnDisconnect("/", func(s socketio.Conn, reason string) {
		fmt.Println("closed", reason)
	})
	go server.Serve()
	defer server.Close()

	http.Handle("/socket.io/", server)
	http.Handle("/", http.FileServer(http.Dir("./asset")))
	log.Println("Serving at localhost:8000...")
	log.Fatal(http.ListenAndServe(":8000", nil))
}
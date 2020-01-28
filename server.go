package main

import (
	"fmt"
	"context"
	"log"
	"time"
	"net/url"
	"strings"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"github.com/verizon/go-common-mec/va-subscriber/flatbuffers/metadata/object"
	/*"go.mongodb.org/mongo-driver/bson"*/
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"

)

var commonObjectFields object.CommonObjectFields
var encoding object.EncodingType
var sizeXYZ object.Float3d
var orientation object.EulerAngles
var positionGPS object.GPSCoordinate
var velocity object.Float3d

var uuidDashPositions = map[int]struct{}{4: struct{}{}, 6: struct{}{}, 8: struct{}{}, 10: struct{}{}}

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

func connect(clientId string, uri *url.URL) mqtt.Client {
	opts := createClientOptions(clientId, uri)
	client := mqtt.NewClient(opts)
	token := client.Connect()
	for !token.WaitTimeout(3 * time.Second) {
	}
	if err := token.Error(); err != nil {
		log.Fatal(err)
	}
	return client
}

func createClientOptions(clientId string, uri *url.URL) *mqtt.ClientOptions {
	opts := mqtt.NewClientOptions()
	opts.AddBroker(fmt.Sprintf("tcp://%s", uri.Host))
	opts.SetClientID(clientId)
	return opts
}

func createJson(msg mqtt.Message) Response {
	objMsg := object.GetRootAsObjMessage(msg.Payload(), 0)
	objMsg.Common(&commonObjectFields)
	/*sizeXYZPtr := objMsg.SizeXYZ(&sizeXYZ)*/
	orientationPtr := objMsg.Orientation(&orientation)
	positionGPSPtr := objMsg.PositionGPS(&positionGPS)
	velocityPtr := objMsg.Velocity(&velocity)

	timestampUsec := int64(commonObjectFields.Timestamp())
	unixTime := time.Unix(int64(timestampUsec/1e6), int64((timestampUsec%1e6)*1e3))

	// strip off the UUID
	var stringBuilder strings.Builder
	for i, b := range commonObjectFields.UUIDBytes() {
		_, dashPos := uuidDashPositions[i]
		if dashPos {
			stringBuilder.WriteString("-")
		}
		stringBuilder.WriteString(fmt.Sprintf("%02x", b))
	}
	uuid := stringBuilder.String()

	var yawStr, latStr, longStr, velocityXStr, velocityYStr, velocityZStr string

	if orientationPtr != nil {
		yawStr = fmt.Sprintf("%f", orientationPtr.Yaw())
	}
	if positionGPSPtr != nil {
		latStr = fmt.Sprintf("%2.7f", positionGPSPtr.Latitude())
		longStr = fmt.Sprintf("%3.7f", positionGPSPtr.Longitude())
	}
	if velocityPtr != nil {
		velocityXStr = fmt.Sprintf("%f", velocityPtr.X())
		velocityYStr = fmt.Sprintf("%f", velocityPtr.Y())
		velocityZStr = fmt.Sprintf("%f", velocityPtr.Z())
	}

	response := Response{
		Common: Common{
			Timestamp: unixTime,
			ObjectType: string(commonObjectFields.ObjectType()),
			Uuid: uuid,
		},
		PositionGPS: PositionGPS{
			Latitude: latStr,
			Longitude: longStr,
			Altitude: yawStr,
		},
		Velocity: Velocity{
			X: velocityXStr,
			Y: velocityYStr,
			Z: velocityZStr,
		},
	}
	return response
}

func listen(uri *url.URL, topic string, clientDB *mongo.Client) {
	client := connect("sub", uri)
	client.Subscribe(topic, 0, func(client mqtt.Client, msg mqtt.Message) {
		jsonResponse := createJson(msg)
		id := insertOneObject(clientDB, jsonResponse)
		fmt.Println(id)
		fmt.Println(jsonResponse)
	})
}

func insertOneObject(client *mongo.Client, vehicle Response) interface{} {
	verizonDatabase := client.Database("verizon")
	vehiclesCollection := verizonDatabase.Collection("vehicles")
	insertResult, err := vehiclesCollection.InsertOne(context.TODO(), vehicle)
	if err != nil {
		fmt.Println("error")
		log.Fatal(err)
	}
	fmt.Println(insertResult)
	return insertResult.InsertedID
}

func main() {
	client, err := mongo.NewClient(options.Client().ApplyURI("mongodb://verzion:test123@database:27017"))
	if err != nil {
		log.Fatal(err)
	}
	ctx, _ := context.WithTimeout(context.Background(), 10*time.Second)
	err = client.Connect(ctx)
	if err != nil {
		log.Fatal(err)
	}
	defer client.Disconnect(ctx)

	uri, err := url.Parse("tcp://172.27.160.193:1883")
	if err != nil {
		log.Fatal(err)
	}
	topic := "+/va-v2/obj/lossyfull"

	go listen(uri, topic, client)

	clientMqtt := connect("pub", uri)
	timer := time.NewTicker(1 * time.Second)
	for t := range timer.C {
		clientMqtt.Publish(topic, 0, false, t.String())
	}

/*	vehicle := Response{
		Common: Common{
			ObjectType: "car",
		},
		PositionGPS: PositionGPS{
			Latitude: "40.74391980967074",
			Longitude: "-73.99317472619931",
			Altitude: "0",
		},
		Velocity: Velocity{
			X: "10.2",
			Y: "3.13",
			Z: "0",
		},
	}

	id := insertOneObject(client, vehicle)
	fmt.Println(id)*/



}
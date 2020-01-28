FROM golang:1.11-stretch
RUN curl https://raw.githubusercontent.com/golang/dep/master/install.sh -o install.sh
RUN ls
RUN chmod +x install.sh
RUN ./install.sh
WORKDIR /go/src/github.com/tony1908/
RUN go get -u github.com/eclipse/paho.mqtt.golang
RUN go get -u github.com/gorilla/websocket
RUN go get -u golang.org/x/net/proxy
RUN go get -u github.com/google/flatbuffers/go
RUN dep init
COPY go-common-mec /go/src/github.com/verizon/go-common-mec
COPY server.go .
#RUN dep ensure -add "go.mongodb.org/mongo-driver/mongo"
CMD go run server.go
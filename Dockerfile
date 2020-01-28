FROM golang:1.11-stretch
RUN curl https://raw.githubusercontent.com/golang/dep/master/install.sh -o install.sh
RUN chmod +x install.sh
RUN ./install.sh
WORKDIR /go/src/github.com/tony1908/app
RUN go get -u github.com/googollee/go-socket.io
RUN go get -u github.com/dgrijalva/jwt-go
RUN dep init
COPY server.go .
RUN dep ensure -add "go.mongodb.org/mongo-driver/mongo"
CMD go run server.go
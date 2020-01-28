#!/bin/bash
pullOrClone () {
  REPOSRC=$1
  LOCALREPO=$2
  LOCALREPO_VC_DIR=$LOCALREPO/.git

  if [ ! -d $LOCALREPO_VC_DIR ]
  then
    git clone $REPOSRC $LOCALREPO
  else
    pushd $LOCALREPO
    git pull $REPOSRC
    popd
  fi
}

GOPATH=`pwd`

pullOrClone git@github.com:tony1908/go-common-mec.git $GOPATH/go-common-mec

sudo docker build -t verizon-test .

rm -rf $GOPATH/go-common-mec
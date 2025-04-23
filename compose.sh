#!/bin/bash

export DB_VOLUME="/home/etienne/Desktop/repository/Deckster/data"

docker compose build --build-arg CACHE_BUSTER=$(date +%s)
docker compose up -d

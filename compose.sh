#!/bin/bash

export DB_DATA="./data"
export SHARED_KEYS="./data"

docker compose build --build-arg CACHE_BUSTER=$(date +%s)
docker compose up -d

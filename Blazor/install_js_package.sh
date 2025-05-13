#!/bin/bash
npm install 

rm -rf wwwroot/Packages/signalr/*

cp ./node_modules/@microsoft/signalr/dist/browser/signalr.min.js  ./wwwroot/Packages/signalr/


rm -rf node_modules
#rm -f package-lock.json


#!/bin/bash
npm install 

rm -rf wwwroot/Packages/signalr/*
rm -rf wwwroot/Packages/pixijs/*

cp ./node_modules/@microsoft/signalr/dist/browser/signalr.min.js  ./wwwroot/Packages/signalr/
cp ./node_modules/pixi.js/dist/pixi.min.js  ./wwwroot/Packages/pixijs/

rm -rf node_modules
#rm -f package-lock.json


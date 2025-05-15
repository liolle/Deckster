#!/bin/bash

# This is meant to be used to build the app for local testing 

# Install dependencies
cd Blazor

./install_js_package.sh
dotnet build

cd ../API

dotnet build
cd ..
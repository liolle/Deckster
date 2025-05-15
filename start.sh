#!/bin/bash

# This is meant to be used to start the app for local testing (should build first: ./build.sh) 

# Make sure to have gnome-terminal installed
# Run projects

gnome-terminal -- bash -c "dotnet run --project Blazor --no-build; exec bash"
gnome-terminal -- bash -c "dotnet run --project API --no-build; exec bash"
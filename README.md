# HealthChecker
## Overview

This is a .NET console application that checks the health of a set of HTTP endpoints specified in a YAML configuration file. The program tests the health of the endpoints every 15 seconds and logs the availability percentage of each URL domain to the console.

## Prerequisites

Before running the program, make sure you have the following installed on your machine:

- .NET SDK 6.0 (Software Development Kit)

## Build

To build the program, open a terminal or command prompt, navigate to the project directory, and run the following command:

     dotnet build -o out

## Run

To run the program, open a terminal or command prompt, navigate to the project directory, and run the following command providing the path to your YAML configuration file as an argument:

     dotnet out/HealthChecker.dll path/to/your/config.yaml

## Exit

To exit the program, press `CTRL+C` in the terminal or command prompt.

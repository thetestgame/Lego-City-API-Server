#!/bin/bash

service dbus start
bluetoothd &

dotnet LegoCity.Api.dll

/bin/bash
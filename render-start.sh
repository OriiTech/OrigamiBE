#!/bin/sh

PORT_TO_USE=${PORT:-8080}
export ASPNETCORE_URLS="http://+:${PORT_TO_USE}"

exec dotnet Origami.API.dll


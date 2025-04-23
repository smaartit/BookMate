#!/bin/bash
echo "Waiting for Temporal to start..."
sleep 5

echo "🚀 Starting BookMate API..."
dotnet /app/BookMate.API/BookMate.API.dll &

echo "🔧 Starting BookMate Worker..."
dotnet /app/BookMate.Workflows/BookMate.Workflows.dll 

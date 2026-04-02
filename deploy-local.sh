#!/bin/bash
# RankUpAPI Local Deployment Script

echo "🚀 Starting RankUpAPI Local Deployment..."

# Step 1: Environment Setup
if [ ! -f .env ]; then
    echo "📋 Creating .env file from template..."
    cp .env.example .env
    echo "✅ .env file created. Edit it if needed."
fi

# Step 2: Build and Start Services
echo "🔨 Building and starting all services..."
docker compose -f docker/docker-compose.yml --env-file .env up --build -d

# Step 3: Wait for Services to Start
echo "⏳ Waiting for services to initialize..."
sleep 30

# Step 4: Check Service Status
echo "📊 Checking service status..."
docker compose -f docker/docker-compose.yml ps

# Step 5: Test Services
echo "🧪 Testing services..."
echo "🌐 Gateway: http://localhost:5087"
echo "🗄️  SQL Server: localhost:1433"

echo "✅ Local deployment complete!"
echo "📝 To view logs: docker compose -f docker/docker-compose.yml logs -f [service-name]"
echo "🛑 To stop: docker compose -f docker/docker-compose.yml down"

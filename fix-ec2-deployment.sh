#!/bin/bash

echo "🔧 Fixing EC2 Deployment Issues..."

# Step 1: Stop all containers
echo "🛑 Stopping existing containers..."
docker-compose -f docker/docker-compose.yml down

# Step 2: Remove volumes to reset database
echo "🗑️  Cleaning up old volumes..."
docker volume rm rankupapi_sqlserver-data 2>/dev/null || true

# Step 3: Copy production environment to docker folder
echo "📋 Setting up production environment..."
cp .env.production docker/.env

# Step 4: Build and start services
echo "🚀 Building and starting services..."
docker-compose -f docker/docker-compose.yml up -d --build

# Step 5: Wait for SQL Server to be ready
echo "⏳ Waiting for SQL Server to start..."
sleep 30

# Step 6: Run database scripts
echo "🗄️  Setting up databases..."
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -C -i /tmp/RankUp_AdminDB_Script.sql
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -C -i /tmp/RankUp_MasterDB_Script.sql  
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -C -i /tmp/RankUp_UserDB_Script.sql

# Step 7: Check container status
echo "📊 Checking service status..."
docker-compose -f docker/docker-compose.yml ps

# Step 8: Show logs for any failing services
echo "📋 Checking logs..."
docker-compose -f docker/docker-compose.yml logs --tail=20

echo "✅ Deployment fix completed!"
echo "🌐 Gateway should be available at: http://localhost:5087"

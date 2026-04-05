# Fix EC2 Deployment Issues
Write-Host "🔧 Fixing EC2 Deployment Issues..." -ForegroundColor Green

# Step 1: Stop all containers
Write-Host "🛑 Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.yml down

# Step 2: Remove volumes to reset database
Write-Host "🗑️  Cleaning up old volumes..." -ForegroundColor Yellow
docker volume rm rankupapi_sqlserver-data 2>$null

# Step 3: Copy production environment to docker folder
Write-Host "📋 Setting up production environment..." -ForegroundColor Yellow
Copy-Item .env.production docker/.env -Force

# Step 4: Build and start services
Write-Host "🚀 Building and starting services..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.yml up -d --build

# Step 5: Wait for SQL Server to be ready
Write-Host "⏳ Waiting for SQL Server to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Step 6: Copy database scripts to container
Write-Host "📁 Copying database scripts..." -ForegroundColor Yellow
docker cp database/RankUp_AdminDB_Script.sql rankup-sqlserver:/tmp/
docker cp database/RankUp_MasterDB_Script.sql rankup-sqlserver:/tmp/
docker cp database/RankUp_UserDB_Script.sql rankup-sqlserver:/tmp/

# Step 7: Run database scripts
Write-Host "🗄️  Setting up databases..." -ForegroundColor Yellow
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -C -i /tmp/RankUp_AdminDB_Script.sql
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -C -i /tmp/RankUp_MasterDB_Script.sql  
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -C -i /tmp/RankUp_UserDB_Script.sql

# Step 8: Check container status
Write-Host "📊 Checking service status..." -ForegroundColor Cyan
docker-compose -f docker/docker-compose.yml ps

# Step 9: Show logs for any failing services
Write-Host "📋 Checking logs..." -ForegroundColor Cyan
docker-compose -f docker/docker-compose.yml logs --tail=20

Write-Host "✅ Deployment fix completed!" -ForegroundColor Green
Write-Host "🌐 Gateway should be available at: http://localhost:5087" -ForegroundColor Green

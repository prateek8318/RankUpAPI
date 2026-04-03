#!/bin/bash
# RankUpAPI EC2 Production Deployment Script

echo "🌐 Starting RankUpAPI EC2 Deployment..."

# Step 1: Update System
echo "🔄 Updating system packages..."
sudo yum update -y || sudo apt-get update -y

# Step 2: Install Docker & Docker Compose
echo "🐳 Installing Docker..."
if command -v yum &> /dev/null; then
    sudo yum install -y docker
    sudo systemctl start docker
    sudo systemctl enable docker
    sudo usermod -a -G docker ec2-user
else
    sudo apt-get update
    sudo apt-get install -y docker.io docker-compose
    sudo systemctl start docker
    sudo systemctl enable docker
    sudo usermod -a -G docker ubuntu
fi

# Step 3: Clone Repository
echo "📥 Cloning repository..."
git clone https://github.com/prateek8318/RankUpAPI.git RankUpAPI
cd RankUpAPI

# Step 4: Deploy Database
echo "🗄️  Deploying database..."
docker compose -f docker/docker-compose.yml up -d sqlserver
sleep 30
docker cp database/RankUp_UserDB_Script.sql rankup-sqlserver:/tmp/database_script.sql
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -i /tmp/database_script.sql

# Step 5: Configure Environment
echo "⚙️  Configuring production environment..."
cp .env.production docker/.env
echo "🔧 Environment configured with production settings"

# Step 6: Deploy All Services
echo "🚀 Deploying all microservices..."
docker compose -f docker/docker-compose.yml up --build -d

# Step 7: Setup Monitoring
echo "📊 Setting up monitoring..."
docker compose -f docker/docker-compose.yml ps

# Step 8: Configure Security Groups (Manual Step)
echo "🔒 Configure EC2 Security Groups for ports:"
echo "   - 80 (HTTP)"
echo "   - 443 (HTTPS)" 
echo "   - 5087 (Gateway)"
echo "   - 22 (SSH)"

echo "✅ EC2 deployment complete!"
echo "🌐 Access your API at: http://your-ec2-ip:5087"

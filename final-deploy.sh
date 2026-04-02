#!/bin/bash
# FINAL DEPLOYMENT SCRIPT - Run this on EC2

echo "🚀 Starting RankUpAPI Production Deployment..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker not found. Installing..."
    sudo yum update -y
    sudo yum install -y docker
    sudo systemctl start docker
    sudo systemctl enable docker
    sudo usermod -a -G docker ec2-user
    echo "✅ Docker installed. Please logout and login again."
    exit 1
fi

# Check if Git is installed
if ! command -v git &> /dev/null; then
    echo "❌ Git not found. Installing..."
    sudo yum install -y git
fi

# Clone repository if not exists
if [ ! -d "RankUpAPI" ]; then
    echo "📥 Cloning repository..."
    git clone https://github.com/yourusername/RankUpAPI.git
fi

cd RankUpAPI

# Setup environment
if [ ! -f .env ]; then
    echo "⚙️ Setting up environment..."
    cp .env.example .env
    echo "🔧 EDIT .env FILE WITH PRODUCTION SETTINGS!"
    echo "   - Set strong database password"
    echo "   - Update connection strings"
    echo "   - Configure production settings"
    read -p "Press Enter after editing .env file..."
fi

# Deploy services
echo "🚀 Deploying microservices..."
docker compose -f docker/docker-compose.yml down
docker compose -f docker/docker-compose.yml up --build -d

# Wait for services
echo "⏳ Waiting for services to start..."
sleep 60

# Check status
echo "📊 Checking service status..."
docker compose -f docker/docker-compose.yml ps

# Test API
echo "🧪 Testing API..."
if curl -s http://localhost:5087/api/users > /dev/null; then
    echo "✅ API is working!"
else
    echo "❌ API not responding. Check logs:"
    docker compose -f docker/docker-compose.yml logs -f gateway
fi

echo "🌐 Your API is running at: http://$(curl -s ifconfig.me):5087"
echo "📝 View logs: docker compose -f docker/docker-compose.yml logs -f"
echo "🛑 Stop services: docker compose -f docker/docker-compose.yml down"

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
git clone <YOUR_REPO_URL> RankUpAPI
cd RankUpAPI

# Step 4: Configure Environment
echo "⚙️  Configuring production environment..."
cp .env.example .env
echo "🔧 Edit .env file with production settings:"
echo "   - Database connection strings"
echo "   - Production passwords"
echo "   - External service URLs"

# Step 5: Deploy Services
echo "🚀 Deploying microservices..."
docker compose -f docker/docker-compose.yml up --build -d

# Step 6: Setup Monitoring
echo "📊 Setting up monitoring..."
docker compose -f docker/docker-compose.yml ps

# Step 7: Configure Security Groups (Manual Step)
echo "🔒 Configure EC2 Security Groups for ports:"
echo "   - 80 (HTTP)"
echo "   - 443 (HTTPS)" 
echo "   - 5087 (Gateway)"
echo "   - 22 (SSH)"

echo "✅ EC2 deployment complete!"
echo "🌐 Access your API at: http://your-ec2-ip:5087"

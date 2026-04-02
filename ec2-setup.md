# EC2 Setup Commands

## 1. Connect to EC2 Instance
```bash
# Replace with your key file and IP
ssh -i your-key.pem ec2-user@your-ec2-ip

# For Ubuntu
ssh -i your-key.pem ubuntu@your-ec2-ip
```

## 2. Update System
```bash
# Amazon Linux 2
sudo yum update -y

# Ubuntu
sudo apt-get update -y
```

## 3. Install Docker
```bash
# Amazon Linux 2
sudo yum install -y docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -a -G docker ec2-user

# Ubuntu
sudo apt-get install -y docker.io docker-compose
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -a -G docker ubuntu
```

## 4. Install Git
```bash
# Amazon Linux 2
sudo yum install -y git

# Ubuntu
sudo apt-get install -y git
```

## 5. Clone Repository
```bash
git clone <YOUR_GITHUB_REPO_URL>
cd RankUpAPI
```

## 6. Setup Environment
```bash
cp .env.example .env
nano .env  # Edit with production settings
```

## 7. Deploy Application
```bash
chmod +x deploy-ec2.sh
./deploy-ec2.sh
```

## 8. Monitor Services
```bash
# Check status
docker compose -f docker/docker-compose.yml ps

# View logs
docker compose -f docker/docker-compose.yml logs -f

# Test API
curl http://localhost:5087/api/users
```

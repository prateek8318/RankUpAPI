[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet(
        "gateway",
        "userservice",
        "examservice",
        "adminservice",
        "masterservice",
        "questionservice",
        "quizservice",
        "subscriptionservice",
        "paymentservice",
        "homedashboardservice",
        "testservice"
    )]
    [string]$Service,

    [string]$AwsAccountId = "940831808542",

    [string]$Region = "ap-south-1",

    [string]$RepositoryName,

    [string]$ImageTag = "latest"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$serviceConfig = @{
    gateway              = @{ Dockerfile = "Services/GatewayAPI/Dockerfile"; ImageName = "gateway" }
    userservice          = @{ Dockerfile = "Services/UserService/Dockerfile"; ImageName = "userservice" }
    examservice          = @{ Dockerfile = "Services/ExamService/Dockerfile"; ImageName = "examservice" }
    adminservice         = @{ Dockerfile = "Services/AdminService/Dockerfile"; ImageName = "adminservice" }
    masterservice        = @{ Dockerfile = "Services/MasterService/Dockerfile"; ImageName = "masterservice" }
    questionservice      = @{ Dockerfile = "Services/QuestionService/Dockerfile"; ImageName = "questionservice" }
    quizservice          = @{ Dockerfile = "Services/QuizService/Dockerfile"; ImageName = "quizservice" }
    subscriptionservice  = @{ Dockerfile = "Services/SubscriptionService/Dockerfile"; ImageName = "subscriptionservice" }
    paymentservice       = @{ Dockerfile = "Services/PaymentService/Dockerfile"; ImageName = "paymentservice" }
    homedashboardservice = @{ Dockerfile = "Services/HomeDashboardService/Dockerfile"; ImageName = "homedashboardservice" }
    testservice          = @{ Dockerfile = "Services/TestService/Dockerfile"; ImageName = "testservice" }
}

$selectedService = $serviceConfig[$Service]
$dockerfilePath = Join-Path $repoRoot $selectedService.Dockerfile

if (-not (Test-Path $dockerfilePath)) {
    throw "Dockerfile not found: $dockerfilePath"
}

if (-not $RepositoryName) {
    $RepositoryName = $selectedService.ImageName
}

$localImage = "{0}:{1}" -f $selectedService.ImageName, $ImageTag
$registry = "{0}.dkr.ecr.{1}.amazonaws.com" -f $AwsAccountId, $Region
$remoteImage = "{0}/{1}:{2}" -f $registry, $RepositoryName, $ImageTag

foreach ($tool in @("docker", "aws")) {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        throw "Required command not found: $tool"
    }
}

Write-Host "Service      : $Service"
Write-Host "Dockerfile   : $($selectedService.Dockerfile)"
Write-Host "Local image  : $localImage"
Write-Host "Remote image : $remoteImage"

if ($PSCmdlet.ShouldProcess($localImage, "Build Docker image")) {
    & docker build -f $selectedService.Dockerfile -t $localImage .
    if ($LASTEXITCODE -ne 0) {
        throw "Docker build failed."
    }
}

if ($PSCmdlet.ShouldProcess($registry, "Login to Amazon ECR")) {
    $password = & aws ecr get-login-password --region $Region
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($password)) {
        throw "Failed to retrieve ECR login password."
    }

    $password | & docker login --username AWS --password-stdin $registry
    if ($LASTEXITCODE -ne 0) {
        throw "Docker login failed."
    }
}

if ($PSCmdlet.ShouldProcess($remoteImage, "Tag Docker image")) {
    & docker tag $localImage $remoteImage
    if ($LASTEXITCODE -ne 0) {
        throw "Docker tag failed."
    }
}

if ($PSCmdlet.ShouldProcess($remoteImage, "Push Docker image")) {
    & docker push $remoteImage
    if ($LASTEXITCODE -ne 0) {
        throw "Docker push failed."
    }
}

Write-Host "Push completed successfully."

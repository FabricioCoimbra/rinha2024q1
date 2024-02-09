@echo off
ECHO "======================================================"
ECHO "===========      Building Image         =============="
ECHO "======================================================"
docker build -t rinhabackend2024 .

ECHO "======================================================"
ECHO "===========        Push Image           =============="
ECHO "======================================================"
docker tag rinhabackend2024 fabriciocoimbra/rinhabackend2024:latest 
docker push fabriciocoimbra/rinhabackend2024:latest

ECHO "======================================================"
ECHO "===========      Restart cluster        =============="
ECHO "======================================================"
docker-compose down --volumes --remove-orphans --rmi all
timeout 5
docker-compose up -d

ECHO "======================================================"
ECHO "===========      Waiting cluster Up     =============="
ECHO "======================================================"

timeout 10

ECHO "======================================================"
ECHO "===========         Start Test          =============="
ECHO "======================================================"

ECHO "cd C:\Fontes\rinha-de-backend-2024-q1"

ECHO ".\executar-teste-local.ps1"

timeout /t -1
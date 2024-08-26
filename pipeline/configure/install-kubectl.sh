#!/bin/bash
# Install dependencies
apt-get update && apt-get install -y apt-transport-https curl

# Download and install kubectl
curl -LO "https://dl.k8s.io/release/$(curl -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
chmod +x ./kubectl
mv ./kubectl /usr/local/bin/kubectl

# Verify installation
kubectl version --client

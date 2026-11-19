# Module aws-vpc

Infrastructure réseau AWS pour SmartDiscount + Kubernetes multi-node.

## Composants créés

- 1 VPC (10.1.0.0/16)
- 2 Subnets (public + private)
- 1 Internet Gateway
- 1 NAT Gateway (+ Elastic IP)
- 2 Route Tables
- 2 Security Groups (K8s master + workers)

## Architecture

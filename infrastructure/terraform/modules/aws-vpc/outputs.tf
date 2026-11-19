# ─────────────────────────────────────────────────
# Outputs du module aws-vpc
# ─────────────────────────────────────────────────

output "vpc_id" {
  description = "ID du VPC"
  value       = aws_vpc.smartdiscount.id
}

output "vpc_cidr" {
  description = "CIDR block du VPC"
  value       = aws_vpc.smartdiscount.cidr_block
}

output "subnet_public_id" {
  description = "ID du subnet public"
  value       = aws_subnet.public.id
}

output "subnet_private_id" {
  description = "ID du subnet privé"
  value       = aws_subnet.private.id
}

output "subnet_public_cidr" {
  description = "CIDR du subnet public"
  value       = aws_subnet.public.cidr_block
}

output "subnet_private_cidr" {
  description = "CIDR du subnet privé"
  value       = aws_subnet.private.cidr_block
}

output "internet_gateway_id" {
  description = "ID de l'Internet Gateway"
  value       = aws_internet_gateway.igw.id
}

output "nat_gateway_id" {
  description = "ID du NAT Gateway"
  value       = aws_nat_gateway.nat.id
}

output "security_group_k8s_master_id" {
  description = "ID du Security Group K8s Master"
  value       = aws_security_group.k8s_master.id
}

output "security_group_k8s_workers_id" {
  description = "ID du Security Group K8s Workers"
  value       = aws_security_group.k8s_workers.id
}

output "availability_zone" {
  description = "Availability Zone utilisée"
  value       = aws_subnet.public.availability_zone
}

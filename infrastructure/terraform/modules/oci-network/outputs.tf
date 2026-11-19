# ─────────────────────────────────────────────────
# Outputs du module oci-network
# Ces valeurs sont utilisées par d'autres modules
# ─────────────────────────────────────────────────

output "vcn_id" {
  description = "OCID du VCN"
  value       = oci_core_vcn.smartdiscount.id
}

output "vcn_cidr" {
  description = "CIDR block du VCN"
  value       = var.vcn_cidr
}

output "subnet_public_id" {
  description = "OCID du subnet public"
  value       = oci_core_subnet.public.id
}

output "subnet_private_id" {
  description = "OCID du subnet privé (Kubernetes)"
  value       = oci_core_subnet.private.id
}

output "subnet_db_id" {
  description = "OCID du subnet database"
  value       = oci_core_subnet.db.id
}

output "internet_gateway_id" {
  description = "OCID de l'Internet Gateway"
  value       = oci_core_internet_gateway.igw.id
}

output "nat_gateway_id" {
  description = "OCID du NAT Gateway"
  value       = oci_core_nat_gateway.nat.id
}

output "service_gateway_id" {
  description = "OCID du Service Gateway"
  value       = oci_core_service_gateway.sgw.id
}
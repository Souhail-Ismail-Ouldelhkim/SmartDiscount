# ─────────────────────────────────────────────────
# Variables du module oci-network
# ─────────────────────────────────────────────────

variable "compartment_id" {
  description = "OCID du compartment où créer les ressources réseau"
  type        = string
}

variable "environment" {
  description = "Nom de l'environnement (dev, test, sandbox, preprod, prod)"
  type        = string
}

variable "vcn_cidr" {
  description = "Plage CIDR du VCN"
  type        = string
  default     = "10.0.0.0/16"
}

variable "subnet_public_cidr" {
  description = "Plage CIDR du subnet public (Load Balancer, Ingress)"
  type        = string
  default     = "10.0.1.0/24"
}

variable "subnet_private_cidr" {
  description = "Plage CIDR du subnet privé (Kubernetes workers)"
  type        = string
  default     = "10.0.2.0/24"
}

variable "subnet_db_cidr" {
  description = "Plage CIDR du subnet database (PostgreSQL)"
  type        = string
  default     = "10.0.3.0/24"
}

variable "tags" {
  description = "Tags à appliquer sur toutes les ressources"
  type        = map(string)
  default     = {}
}

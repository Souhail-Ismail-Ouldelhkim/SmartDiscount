# ─────────────────────────────────────────────────
# Variables du module aws-vpc
# ─────────────────────────────────────────────────

variable "environment" {
  description = "Nom de l'environnement (test, prod, etc.)"
  type        = string
}

variable "vpc_cidr" {
  description = "Plage CIDR du VPC AWS (différente du VCN OCI 10.0.0.0/16)"
  type        = string
  default     = "10.1.0.0/16"
}

variable "subnet_public_cidr" {
  description = "Plage CIDR du subnet public"
  type        = string
  default     = "10.1.1.0/24"
}

variable "subnet_private_cidr" {
  description = "Plage CIDR du subnet privé"
  type        = string
  default     = "10.1.2.0/24"
}

variable "availability_zone" {
  description = "Availability Zone AWS"
  type        = string
  default     = "eu-west-1a"
}

variable "tags" {
  description = "Tags à appliquer sur toutes les ressources"
  type        = map(string)
  default     = {}
}

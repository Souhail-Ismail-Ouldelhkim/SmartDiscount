# ─────────────────────────────────────────────────
# Variables du module oci-oke
# ─────────────────────────────────────────────────

variable "compartment_id" {
  description = "OCID du compartment où créer le cluster"
  type        = string
}

variable "environment" {
  description = "Nom de l'environnement (dev, test, sandbox, preprod, prod)"
  type        = string
}

variable "vcn_id" {
  description = "OCID du VCN (vient du module oci-network)"
  type        = string
}

variable "subnet_public_id" {
  description = "OCID du subnet public (pour API endpoint + Load Balancer)"
  type        = string
}

variable "subnet_private_id" {
  description = "OCID du subnet privé (pour les nodes Kubernetes)"
  type        = string
}

variable "kubernetes_version" {
  description = "Version de Kubernetes"
  type        = string
  default     = "v1.36.4"
}

variable "node_shape" {
  description = "Type de VM pour les nodes (ARM gratuit)"
  type        = string
  default     = "VM.Standard.A1.Flex"
}

variable "node_count" {
  description = "Nombre de nodes dans le node pool"
  type        = number
  default     = 2
}

variable "node_ocpus" {
  description = "Nombre d'OCPUs par node"
  type        = number
  default     = 1
}

variable "node_memory_gb" {
  description = "Mémoire RAM par node en GB"
  type        = number
  default     = 6
}

variable "availability_domain" {
  description = "Availability Domain (name retourné par oci iam availability-domain list)"
  type        = string
}

variable "tags" {
  description = "Tags à appliquer"
  type        = map(string)
  default     = {}
}
variable "node_image_id" {
  description = "OCID de l'image OKE pour les nodes"
  type        = string
}

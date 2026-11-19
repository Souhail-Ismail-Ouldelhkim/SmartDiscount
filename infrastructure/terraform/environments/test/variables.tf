# ─────────────────────────────────────────────────
# Variables pour l'environnement TEST
# ─────────────────────────────────────────────────

variable "tenancy_ocid" {
  description = "OCID de ta tenancy OCI"
  type        = string
}

variable "user_ocid" {
  description = "OCID de ton user OCI"
  type        = string
}

variable "fingerprint" {
  description = "Fingerprint de ta clé API OCI"
  type        = string
}

variable "private_key_path" {
  description = "Chemin vers ta clé privée OCI"
  type        = string
  default     = "~/.oci/oci_api_key.pem"
}

variable "region" {
  description = "Région OCI"
  type        = string
  default     = "af-casablanca-1"
}

variable "compartment_id" {
  description = "OCID du compartment où déployer"
  type        = string
}

variable "environment" {
  description = "Nom de l'environnement"
  type        = string
  default     = "test"
}

variable "availability_domain" {
  description = "Nom de l'availability domain OCI"
  type        = string
}

variable "node_image_id" {
  description = "OCID de l'image OKE pour les nodes Kubernetes"
  type        = string
}

variable "ssh_public_key" {
  description = "Clé SSH publique pour la VM K3s"
  type        = string
}

variable "vm_image_id" {
  description = "OCID de l'image Ubuntu 22.04 ARM"
  type        = string
}
variable "aws_region" {
  description = "Région AWS"
  type        = string
  default     = "eu-west-1"
}

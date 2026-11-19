# ─────────────────────────────────────────────────
# Variables du module oci-k3s
# ─────────────────────────────────────────────────

variable "compartment_id" {
  description = "OCID du compartment"
  type        = string
}

variable "environment" {
  description = "Nom de l'environnement (test, prod, etc.)"
  type        = string
}

variable "availability_domain" {
  description = "Nom de l'availability domain"
  type        = string
}

variable "subnet_id" {
  description = "OCID du subnet où déployer la VM (public pour accès kubectl)"
  type        = string
}

variable "ssh_public_key" {
  description = "Clé SSH publique pour se connecter à la VM"
  type        = string
}

variable "vm_shape" {
  description = "Shape de la VM (ARM gratuit par défaut)"
  type        = string
  default     = "VM.Standard.A1.Flex"
}

variable "vm_ocpus" {
  description = "Nombre d'OCPUs"
  type        = number
  default     = 4
}

variable "vm_memory_gb" {
  description = "Mémoire en GB"
  type        = number
  default     = 24
}

variable "vm_image_id" {
  description = "OCID de l'image Ubuntu 22.04 ARM pour la VM"
  type        = string
}

variable "tags" {
  description = "Tags à appliquer"
  type        = map(string)
  default     = {}
}

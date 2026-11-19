# ─────────────────────────────────────────────────
# Variables du module oci-ocir
# ─────────────────────────────────────────────────

variable "compartment_id" {
  description = "OCID du compartment où créer les repositories"
  type        = string
}

variable "environment" {
  description = "Nom de l'environnement (dev, test, sandbox, preprod, prod)"
  type        = string
}

variable "microservices" {
  description = "Liste des microservices SmartDiscount à héberger dans OCIR"
  type        = list(string)
  default = [
    "identity-api",
    "basket-api",
    "catalog-api",
    "ordering-api",
    "webhooks-api",
    "notification-api",
    "discount-api",
    "wishlist-api",
    "order-processor",
    "payment-processor",
    "mobile-bff",
    "webapp",
    "webhooksclient"
  ]
}

variable "tags" {
  description = "Tags à appliquer sur toutes les ressources"
  type        = map(string)
  default     = {}
}

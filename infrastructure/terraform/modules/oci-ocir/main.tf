# ─────────────────────────────────────────────────
# Module OCI Container Registry (OCIR)
# Crée 1 repository par microservice SmartDiscount
# ─────────────────────────────────────────────────

# ────────────────────────
# Création d'un repository Docker par microservice
# ────────────────────────
resource "oci_artifacts_container_repository" "microservices" {
  for_each = toset(var.microservices)

  compartment_id = var.compartment_id
  display_name   = "smartdiscount/${var.environment}/${each.value}"

  is_immutable = false   # true en prod → empêche d'écraser une image existante
  is_public    = false   # repository privé

  readme {
    content = "Repository Docker pour le microservice ${each.value} (${var.environment})"
    format  = "text/markdown"
  }

  freeform_tags = merge(var.tags, {
    Environment  = var.environment
    Microservice = each.value
    Project      = "SmartDiscount"
    ManagedBy    = "Terraform"
  })
}


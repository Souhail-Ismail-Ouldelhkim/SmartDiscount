# ─────────────────────────────────────────────────
# Outputs du module oci-ocir
# ─────────────────────────────────────────────────

output "repositories" {
  description = "Map des repositories créés (nom → OCID)"
  value = {
    for name, repo in oci_artifacts_container_repository.microservices :
    name => repo.id
  }
}

output "repository_urls" {
  description = "URLs des repositories (à utiliser pour docker push/pull)"
  value = {
    for name, repo in oci_artifacts_container_repository.microservices :
    name => repo.display_name
  }
}


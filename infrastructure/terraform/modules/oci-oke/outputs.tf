# ─────────────────────────────────────────────────
# Outputs du module oci-oke
# ─────────────────────────────────────────────────

output "cluster_id" {
  description = "OCID du cluster OKE"
  value       = oci_containerengine_cluster.smartdiscount.id
}

output "cluster_name" {
  description = "Nom du cluster"
  value       = oci_containerengine_cluster.smartdiscount.name
}

output "cluster_endpoint" {
  description = "Endpoint API Kubernetes"
  value       = oci_containerengine_cluster.smartdiscount.endpoints[0].public_endpoint
}

output "node_pool_id" {
  description = "OCID du node pool"
  value       = oci_containerengine_node_pool.main.id
}

output "kubeconfig_command" {
  description = "Commande pour récupérer le kubeconfig localement"
  value       = "oci ce cluster create-kubeconfig --cluster-id ${oci_containerengine_cluster.smartdiscount.id} --file ~/.kube/config --region $(oci-region) --token-version 2.0.0 --kube-endpoint PUBLIC_ENDPOINT"
}

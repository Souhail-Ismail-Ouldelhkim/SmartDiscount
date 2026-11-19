# ─────────────────────────────────────────────────
# Outputs du module oci-k3s
# ─────────────────────────────────────────────────

output "vm_id" {
  description = "OCID de la VM K3s"
  value       = oci_core_instance.k3s_server.id
}

output "vm_name" {
  description = "Nom de la VM"
  value       = oci_core_instance.k3s_server.display_name
}

output "public_ip" {
  description = "IP publique de la VM K3s (pour SSH et kubectl)"
  value       = oci_core_instance.k3s_server.public_ip
}

output "private_ip" {
  description = "IP privée de la VM K3s"
  value       = oci_core_instance.k3s_server.private_ip
}

output "ssh_command" {
  description = "Commande SSH pour se connecter"
  value       = "ssh ubuntu@${oci_core_instance.k3s_server.public_ip}"
}

output "kubectl_config_command" {
  description = "Commande pour récupérer le kubeconfig"
  value       = "scp ubuntu@${oci_core_instance.k3s_server.public_ip}:/home/ubuntu/.kube/config ~/.kube/config"
}

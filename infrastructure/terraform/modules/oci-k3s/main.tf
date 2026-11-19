# ─────────────────────────────────────────────────
# Module oci-k3s
# Crée 1 VM OCI ARM gratuite avec K3s installé
# ─────────────────────────────────────────────────

resource "oci_core_instance" "k3s_server" {
  compartment_id      = var.compartment_id
  availability_domain = var.availability_domain
  shape               = var.vm_shape

  display_name = "k3s-smartdiscount-${var.environment}"

  shape_config {
    ocpus         = var.vm_ocpus
    memory_in_gbs = var.vm_memory_gb
  }

  source_details {
    source_type = "image"
    source_id   = var.vm_image_id
  }

  create_vnic_details {
    subnet_id        = var.subnet_id
    assign_public_ip = true
    display_name     = "vnic-k3s-${var.environment}"
  }

  metadata = {
    ssh_authorized_keys = var.ssh_public_key
    user_data           = base64encode(file("${path.module}/cloud-init.yaml"))
  }

  freeform_tags = merge(var.tags, {
    Environment = var.environment
    Project     = "SmartDiscount"
    Component   = "K3s-Server"
    ManagedBy   = "Terraform"
  })
}

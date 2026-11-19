# ─────────────────────────────────────────────────
# Module OCI Kubernetes Engine (OKE)
# Cluster Kubernetes managé
# ─────────────────────────────────────────────────

# ────────────────────────
# 1. Récupérer la dernière image Oracle Linux pour les nodes
# ────────────────────────
data "oci_core_images" "oke_node_image" {
  compartment_id           = var.compartment_id
  operating_system         = "Oracle Linux"
  operating_system_version = "8"
  shape                    = var.node_shape
  sort_by                  = "TIMECREATED"
  sort_order               = "DESC"
}

# ────────────────────────
# 2. Le Cluster OKE
# ────────────────────────
resource "oci_containerengine_cluster" "smartdiscount" {
  compartment_id     = var.compartment_id
  name               = "oke-smartdiscount-${var.environment}"
  kubernetes_version = var.kubernetes_version
  vcn_id             = var.vcn_id

  # API endpoint accessible depuis internet (dans subnet public)
  endpoint_config {
    is_public_ip_enabled = true
    subnet_id            = var.subnet_public_id
  }

  options {
    service_lb_subnet_ids = [var.subnet_public_id]

    add_ons {
      is_kubernetes_dashboard_enabled = false
      is_tiller_enabled               = false
    }

    kubernetes_network_config {
      pods_cidr     = "10.244.0.0/16"
      services_cidr = "10.96.0.0/16"
    }
  }

  type = "BASIC_CLUSTER"

  freeform_tags = merge(var.tags, {
    Environment = var.environment
    Project     = "SmartDiscount"
    ManagedBy   = "Terraform"
  })
}

# ────────────────────────
# 3. Node Pool (les workers Kubernetes)
# ────────────────────────
resource "oci_containerengine_node_pool" "main" {
  compartment_id     = var.compartment_id
  cluster_id         = oci_containerengine_cluster.smartdiscount.id
  name               = "np-smartdiscount-${var.environment}"
  kubernetes_version = var.kubernetes_version
  node_shape         = var.node_shape

  # Taille des nodes (ARM Free Tier)
  node_shape_config {
    ocpus         = var.node_ocpus
    memory_in_gbs = var.node_memory_gb
  }

  # Image des nodes
  node_source_details {
    image_id    = "ocid1.image.oc1.af-casablanca-1.aaaaaaaasgghvwcqtj7ewud43ya6lxta5z3f7oelk44xmbq4rrroowh2jtba"
    source_type = "IMAGE"
  }

  # Configuration du placement
  node_config_details {
    size = var.node_count

    placement_configs {
      availability_domain = var.availability_domain
      subnet_id           = var.subnet_private_id
    }
  }

  freeform_tags = merge(var.tags, {
    Environment = var.environment
    Project     = "SmartDiscount"
    ManagedBy   = "Terraform"
  })
}

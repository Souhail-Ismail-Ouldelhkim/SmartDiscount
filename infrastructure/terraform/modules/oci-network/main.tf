# ─────────────────────────────────────────────────
# Module OCI Network — VCN + Subnets + Gateways
# ─────────────────────────────────────────────────

# ────────────────────────
# 1. Le VCN (réseau virtuel principal)
# ────────────────────────
resource "oci_core_vcn" "smartdiscount" {
  compartment_id = var.compartment_id
  display_name   = "vcn-smartdiscount-${var.environment}"
  cidr_blocks    = [var.vcn_cidr]
  dns_label      = "sdvcn${var.environment}"

  freeform_tags = merge(var.tags, {
    Environment = var.environment
    Project     = "SmartDiscount"
    ManagedBy   = "Terraform"
  })
}

# ────────────────────────
# 2. Internet Gateway (entrée/sortie internet pour subnet public)
# ────────────────────────
resource "oci_core_internet_gateway" "igw" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "igw-smartdiscount-${var.environment}"
  enabled        = true

  freeform_tags = var.tags
}

# ────────────────────────
# 3. NAT Gateway (sortie internet pour subnets privés)
# ────────────────────────
resource "oci_core_nat_gateway" "nat" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "nat-smartdiscount-${var.environment}"

  freeform_tags = var.tags
}

# ────────────────────────
# 4. Service Gateway (accès services OCI sans internet)
# ────────────────────────
data "oci_core_services" "all_oci_services" {
  filter {
    name   = "name"
    values = ["All .* Services In Oracle Services Network"]
    regex  = true
  }
}

resource "oci_core_service_gateway" "sgw" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "sgw-smartdiscount-${var.environment}"

  services {
    service_id = data.oci_core_services.all_oci_services.services[0]["id"]
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 5. Route Table — Subnet PUBLIC (route vers Internet Gateway)
# ────────────────────────
resource "oci_core_route_table" "public" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "rt-public-${var.environment}"

  route_rules {
    destination       = "0.0.0.0/0"
    destination_type  = "CIDR_BLOCK"
    network_entity_id = oci_core_internet_gateway.igw.id
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 6. Route Table — Subnet PRIVÉ (route vers NAT Gateway + Service Gateway)
# ────────────────────────
resource "oci_core_route_table" "private" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "rt-private-${var.environment}"

  # Vers internet via NAT (sortie seulement)
  route_rules {
    destination       = "0.0.0.0/0"
    destination_type  = "CIDR_BLOCK"
    network_entity_id = oci_core_nat_gateway.nat.id
  }

  # Vers les services OCI via Service Gateway
  route_rules {
    destination       = data.oci_core_services.all_oci_services.services[0]["cidr_block"]
    destination_type  = "SERVICE_CIDR_BLOCK"
    network_entity_id = oci_core_service_gateway.sgw.id
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 7. Route Table — Subnet DB (route vers Service Gateway uniquement)
# ────────────────────────
resource "oci_core_route_table" "db" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "rt-db-${var.environment}"

  route_rules {
    destination       = data.oci_core_services.all_oci_services.services[0]["cidr_block"]
    destination_type  = "SERVICE_CIDR_BLOCK"
    network_entity_id = oci_core_service_gateway.sgw.id
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 8. Security List PUBLIC (firewall — ports 80, 443 ouverts)
# ────────────────────────
resource "oci_core_security_list" "public" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "sl-public-${var.environment}"

  # Sortie — autoriser tout
  egress_security_rules {
    destination = "0.0.0.0/0"
    protocol    = "all"
  }

  # Entrée — HTTP (port 80)
  ingress_security_rules {
    protocol = "6" # TCP
    source   = "0.0.0.0/0"
    tcp_options {
      min = 80
      max = 80
    }
  }

  # Entrée — HTTPS (port 443)
  ingress_security_rules {
    protocol = "6" # TCP
    source   = "0.0.0.0/0"
    tcp_options {
      min = 443
      max = 443
    }
  }

  # Entrée — SSH (port 22) — restreindre à ton IP en prod
  ingress_security_rules {
    protocol = "6" # TCP
    source   = "0.0.0.0/0"
    tcp_options {
      min = 22
      max = 22
    }
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 9. Security List PRIVATE (firewall — accessible uniquement depuis le VCN)
# ────────────────────────
resource "oci_core_security_list" "private" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "sl-private-${var.environment}"

  # Sortie — autoriser tout
  egress_security_rules {
    destination = "0.0.0.0/0"
    protocol    = "all"
  }

  # Entrée — autoriser tout depuis le VCN (communication interne)
  ingress_security_rules {
    protocol = "all"
    source   = var.vcn_cidr
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 10. Security List DB (firewall — PostgreSQL port 5432)
# ────────────────────────
resource "oci_core_security_list" "db" {
  compartment_id = var.compartment_id
  vcn_id         = oci_core_vcn.smartdiscount.id
  display_name   = "sl-db-${var.environment}"

  # Sortie — autoriser tout
  egress_security_rules {
    destination = "0.0.0.0/0"
    protocol    = "all"
  }

  # Entrée — PostgreSQL (5432) uniquement depuis le subnet privé
  ingress_security_rules {
    protocol = "6" # TCP
    source   = var.subnet_private_cidr
    tcp_options {
      min = 5432
      max = 5432
    }
  }

  freeform_tags = var.tags
}

# ────────────────────────
# 11. Subnet PUBLIC (Load Balancer, Ingress)
# ────────────────────────
resource "oci_core_subnet" "public" {
  compartment_id             = var.compartment_id
  vcn_id                     = oci_core_vcn.smartdiscount.id
  cidr_block                 = var.subnet_public_cidr
  display_name               = "subnet-public-${var.environment}"
  dns_label                  = "pub${var.environment}"
  route_table_id             = oci_core_route_table.public.id
  security_list_ids          = [oci_core_security_list.public.id]
  prohibit_public_ip_on_vnic = false # IPs publiques autorisées

  freeform_tags = var.tags
}

# ────────────────────────
# 12. Subnet PRIVATE (Kubernetes workers, microservices)
# ────────────────────────
resource "oci_core_subnet" "private" {
  compartment_id             = var.compartment_id
  vcn_id                     = oci_core_vcn.smartdiscount.id
  cidr_block                 = var.subnet_private_cidr
  display_name               = "subnet-private-${var.environment}"
  dns_label                  = "priv${var.environment}"
  route_table_id             = oci_core_route_table.private.id
  security_list_ids          = [oci_core_security_list.private.id]
  prohibit_public_ip_on_vnic = true # Pas d'IP publique

  freeform_tags = var.tags
}

# ────────────────────────
# 13. Subnet DB (PostgreSQL managé)
# ────────────────────────
resource "oci_core_subnet" "db" {
  compartment_id             = var.compartment_id
  vcn_id                     = oci_core_vcn.smartdiscount.id
  cidr_block                 = var.subnet_db_cidr
  display_name               = "subnet-db-${var.environment}"
  dns_label                  = "db${var.environment}"
  route_table_id             = oci_core_route_table.db.id
  security_list_ids          = [oci_core_security_list.db.id]
  prohibit_public_ip_on_vnic = true # Pas d'IP publique

  freeform_tags = var.tags
}
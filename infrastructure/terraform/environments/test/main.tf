# ─────────────────────────────────────────────────
# Environnement TEST — SmartDiscount
# Déploiement OCI Casablanca + AWS Ireland
# ─────────────────────────────────────────────────

terraform {
  required_version = ">= 1.5.0"

  required_providers {
    oci = {
      source  = "oracle/oci"
      version = "~> 6.0"
    }
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

# ───────── Provider OCI ─────────
provider "oci" {
  tenancy_ocid     = var.tenancy_ocid
  user_ocid        = var.user_ocid
  fingerprint      = var.fingerprint
  private_key_path = var.private_key_path
  region           = var.region
}

# ───────── Provider AWS ─────────
provider "aws" {
  region = var.aws_region
}

# ───────── Module OCI Network ─────────
module "network" {
  source = "../../modules/oci-network"

  compartment_id = var.compartment_id
  environment    = var.environment

  tags = {
    Owner       = "souhail"
    Project     = "SmartDiscount"
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

# ───────── Module OCI OCIR (Container Registry) ─────────
module "ocir" {
  source = "../../modules/oci-ocir"

  compartment_id = var.compartment_id
  environment    = var.environment

  tags = {
    Owner       = "souhail"
    Project     = "SmartDiscount"
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

# ───────── Module AWS VPC ─────────
module "aws_vpc" {
  source = "../../modules/aws-vpc"

  environment = var.environment

  tags = {
    Owner       = "souhail"
    Project     = "SmartDiscount"
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

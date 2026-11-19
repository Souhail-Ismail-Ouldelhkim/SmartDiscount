# Module oci-oke

Crée un cluster Kubernetes managé (OKE) sur OCI.

## Composants créés

- 1 Cluster OKE (control plane managé par OCI)
- 1 Node Pool avec N nodes ARM (Free Tier compatible)
- Configuration réseau (pods CIDR, services CIDR)

## Usage

```hcl
module "oke" {
  source              = "../../modules/oci-oke"
  compartment_id      = var.compartment_id
  environment         = "test"
  vcn_id              = module.network.vcn_id
  subnet_public_id    = module.network.subnet_public_id
  subnet_private_id   = module.network.subnet_private_id
  availability_domain = "AbCd:AF-CASABLANCA-1-AD-1"
}
```


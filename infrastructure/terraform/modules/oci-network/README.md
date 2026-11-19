\# Module oci-network



Crée l'infrastructure réseau OCI pour SmartDiscount :



\- 1 VCN

\- 3 subnets (public, private, db)

\- 3 gateways (Internet, NAT, Service)

\- 3 route tables

\- 3 security lists



\## Utilisation



```hcl

module "network" {

&#x20; source         = "../../modules/oci-network"

&#x20; compartment\_id = var.compartment\_id

&#x20; environment    = "test"

&#x20; vcn\_cidr       = "10.0.0.0/16"

&#x20; tags = {

&#x20;   Owner = "souhail"

&#x20; }

}

```



\## Architecture



```

VCN (10.0.0.0/16)

├── Subnet PUBLIC   (10.0.1.0/24) → IGW

├── Subnet PRIVATE  (10.0.2.0/24) → NAT + SGW

└── Subnet DB       (10.0.3.0/24) → SGW only

```


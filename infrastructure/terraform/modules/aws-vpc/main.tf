# ─────────────────────────────────────────────────
# Module AWS VPC
# ─────────────────────────────────────────────────

resource "aws_vpc" "smartdiscount" {
  cidr_block           = var.vpc_cidr
  enable_dns_support   = true
  enable_dns_hostnames = true

  tags = merge(var.tags, {
    Name        = "vpc-smartdiscount-${var.environment}"
    Environment = var.environment
    Project     = "SmartDiscount"
    ManagedBy   = "Terraform"
  })
}

resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.smartdiscount.id

  tags = merge(var.tags, {
    Name = "igw-smartdiscount-${var.environment}"
  })
}

resource "aws_subnet" "public" {
  vpc_id                  = aws_vpc.smartdiscount.id
  cidr_block              = var.subnet_public_cidr
  availability_zone       = var.availability_zone
  map_public_ip_on_launch = true

  tags = merge(var.tags, {
    Name = "subnet-public-${var.environment}"
    Type = "public"
  })
}

resource "aws_subnet" "private" {
  vpc_id                  = aws_vpc.smartdiscount.id
  cidr_block              = var.subnet_private_cidr
  availability_zone       = var.availability_zone
  map_public_ip_on_launch = false

  tags = merge(var.tags, {
    Name = "subnet-private-${var.environment}"
    Type = "private"
  })
}

resource "aws_eip" "nat" {
  domain = "vpc"

  tags = merge(var.tags, {
    Name = "eip-nat-${var.environment}"
  })

  depends_on = [aws_internet_gateway.igw]
}

resource "aws_nat_gateway" "nat" {
  allocation_id = aws_eip.nat.id
  subnet_id     = aws_subnet.public.id

  tags = merge(var.tags, {
    Name = "nat-smartdiscount-${var.environment}"
  })

  depends_on = [aws_internet_gateway.igw]
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.smartdiscount.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }

  tags = merge(var.tags, {
    Name = "rt-public-${var.environment}"
  })
}

resource "aws_route_table" "private" {
  vpc_id = aws_vpc.smartdiscount.id

  route {
    cidr_block     = "0.0.0.0/0"
    nat_gateway_id = aws_nat_gateway.nat.id
  }

  tags = merge(var.tags, {
    Name = "rt-private-${var.environment}"
  })
}

resource "aws_route_table_association" "public" {
  subnet_id      = aws_subnet.public.id
  route_table_id = aws_route_table.public.id
}

resource "aws_route_table_association" "private" {
  subnet_id      = aws_subnet.private.id
  route_table_id = aws_route_table.private.id
}

# ────────────────────────
# Security Group K8s Master
# ────────────────────────
resource "aws_security_group" "k8s_master" {
  name        = "k8s-master-${var.environment}"
  description = "Security group for K8s master node"
  vpc_id      = aws_vpc.smartdiscount.id

  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "SSH"
  }

  ingress {
    from_port   = 6443
    to_port     = 6443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "Kubernetes API"
  }

  ingress {
    from_port   = 2379
    to_port     = 2380
    protocol    = "tcp"
    self        = true
    description = "etcd"
  }

  ingress {
    from_port   = 10250
    to_port     = 10250
    protocol    = "tcp"
    self        = true
    description = "Kubelet API"
  }

  ingress {
    from_port   = 10259
    to_port     = 10259
    protocol    = "tcp"
    self        = true
    description = "kube-scheduler"
  }

  ingress {
    from_port   = 10257
    to_port     = 10257
    protocol    = "tcp"
    self        = true
    description = "kube-controller-manager"
  }

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = [var.subnet_private_cidr]
    description = "All traffic from private subnet"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.tags, {
    Name = "k8s-master-${var.environment}"
  })
}

# ────────────────────────
# Security Group K8s Workers
# ────────────────────────
resource "aws_security_group" "k8s_workers" {
  name        = "k8s-workers-${var.environment}"
  description = "Security group for K8s worker nodes"
  vpc_id      = aws_vpc.smartdiscount.id

  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
    description = "SSH from VPC"
  }

  ingress {
    from_port   = 10250
    to_port     = 10250
    protocol    = "tcp"
    self        = true
    description = "Kubelet API"
  }

  ingress {
    from_port   = 30000
    to_port     = 32767
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "NodePort Services"
  }

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = [var.subnet_public_cidr]
    description = "All traffic from master subnet"
  }

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    self        = true
    description = "All inter-worker traffic"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.tags, {
    Name = "k8s-workers-${var.environment}"
  })
}

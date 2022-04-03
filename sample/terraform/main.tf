# k8s connection settings are stored in k8s_config variable in Terraform cloud
provider "kubernetes" {
  host = var.k8s_config.host
  token = var.k8s_config.token
  client_certificate = base64decode(var.k8s_config.client_certificate)
  client_key = base64decode(var.k8s_config.client_key)
  cluster_ca_certificate = base64decode(var.k8s_config.cluster_ca_certificate)
}

# store mysql master config (max_connections=512)
resource "kubernetes_config_map" "mysql_master_config" {
  metadata {
    name = "mysql-master-config"
    namespace = "theidserver"
  }

  data = {
    "my.cnf" = file("${path.module}/my-master.cnf")
  }
}

# store mysql secondary config (max_connections=512)
resource "kubernetes_config_map" "mysql_scondary_config" {
  metadata {
    name = "mysql-secondary-config"
    namespace = "theidserver"
  }

  data = {
    "my.cnf" = file("${path.module}/my-secondary.cnf")
  }
}

# k8s connection settings are stored in k8s_config variable in Terraform cloud
provider "helm" {
  kubernetes {
    host = var.k8s_config.host
    token = var.k8s_config.token
    client_certificate = base64decode(var.k8s_config.client_certificate)
    client_key = base64decode(var.k8s_config.client_key)
    cluster_ca_certificate = base64decode(var.k8s_config.cluster_ca_certificate)
  }
}

locals {
  # set node affinity to userpool nodes
  affinity = {
    nodeAffinity = {
      requiredDuringSchedulingIgnoredDuringExecution = {
        nodeSelectorTerms = [{
          matchExpressions = [{
            key = "agentpool"
            operator = "In"
            values = [
              "userpool"
            ]
          }]
        }]
      }
    }
  }
  # enable wave on config change
  deploymentAnnotations = {
      "wave.pusher.com/update-on-config-change" = "true"
  }
  host = "theidserver.com"
  tls_issuer_name = "letsencrypt"
  tls_issuer_kind = "ClusterIssuer"
  image = {
    repository = "aguacongas/theidserver.duende"
    pullPolicy = "Always"
    tag = "next"
  }
  # SendGrid settings are store in env_settings var in Terraform cloud
  env_settings = var.env_settings
  override_settings = {
    # set node affinity to userpool nodes
    affinity = local.affinity
    seq = {
      # set node affinity to userpool nodes
      affinity = local.affinity
    }
    mysql = {
      primary = {
        # set node affinity to userpool nodes
        affinity = local.affinity
        # user custom master config (max_connections=512)
        existingConfigmap = "mysql-master-config"
      }
      secondary = {
        # set node affinity to userpool nodes
        affinity = local.affinity
        # user custom secondary config (max_connections=512)
        existingConfigmap = "mysql-secondary-config"
      }
    }
    redis = {
      master = {
        # set node affinity to userpool nodes
        affinity = local.affinity
      }
      replica = {
        # set node affinity to userpool nodes
        affinity = local.affinity
      }    
    }
    # enable wave on config change
    deploymentAnnotations = local.deploymentAnnotations
    appSettings = {
      file = {
        # override serilog settings
        Serilog = {
          MinimumLevel = {
            ControlledBy = "$controlSwitch"
            Override = {
              "Microsoft.EntityFrameworkCore" = "Warning"
              System = "Warning"
            }
          }
        }
        # enable honeycomb
        OpenTelemetryOptions = {
          Trace = {
            ConsoleEnabled = false
            Honeycomb = var.Honeycomb
          }
        }
      }
    }
  }

  wait = false
}

module "theidserver" {
  source = "Aguafrommars/theidserver/helm"

  chart_version = "4.8.0"

  host = local.host
  tls_issuer_name = local.tls_issuer_name
  tls_issuer_kind = local.tls_issuer_kind
  image = local.image
  env_settings = local.env_settings
  override_settings = local.override_settings
  replica_count = 3

  wait = local.wait
}

resource "helm_release" "cert_manager" {
  name       = "cert-manager"
  repository = "https://charts.jetstack.io"
  chart      = "cert-manager"
  version    = "1.7.2"
  namespace  = "ingress-nginx"
  create_namespace = true
  
  wait = local.wait
}

resource "helm_release" "nginx_ingress" {
  name       = "nginx-ingress"
  repository = "https://helm.nginx.com/stable"
  chart      = "ingress-nginx"
  version    = "4.0.18"
  namespace  = "ingress-nginx"
  create_namespace = true
  
  wait = local.wait
}

resource "helm_release" "wave" {
  name       = "wave"
  repository = "https://wave-k8s.github.io/wave/"
  chart      = "wave"
  version    = "2.0.0"
  namespace  = "wave"
  create_namespace = true
  
  wait = local.wait
}

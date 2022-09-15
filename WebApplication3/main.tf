# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "registry.terraform.io/hashicorp/azurerm"
      version = "~> 3.0.2"
    }
    
  }
  backend "azurerm" {
        resource_group_name  = "NoteApp"
        storage_account_name = "notewebapp"
        container_name       = "tfstate"
        key                  = "terraform.tfstate"
    }

  required_version = ">= 1.1.0"
}

variable "imagebuild"{
  type = string

  description = "Latest Image Build"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "TerraformSpeechAPI"
  location = "Australia East"
}

resource "azurerm_container_group" "cg" {
  name                      = "speechapi"
  location                  = azurerm_resource_group.rg.location
  resource_group_name       = azurerm_resource_group.rg.name

  ip_address_type     = "Public"
  dns_name_label      = "kingjactfapi"
  os_type             = "Linux"

  container {
      name            = "speechapi"
      image           = "kingjac/speechapi:${var.imagebuild}"
        cpu             = "1"
        memory          = "1"

        ports {
            port        = 80
            protocol    = "TCP"
        }
  }
}
namespace CosmosHelpers.CosmosRegions
{
  public static class AzureRegionToCosmosTranslator
  {
    public static string? Translate(string? regionName)
    {
      switch (regionName?.ToLower())
      {
        case "centralus": return "Central US";
        case "eastus": return "East US";
        case "eastus2": return "East US 2";
        case "westus": return "West US";
        case "northcentralus": return "North Central US";
        case "southcentralus": return "South Central US";
        case "westcentralus": return "West Central US";
        case "westus2": return "West US 2";
        case "westus3": return "West US 3";
        case "canadacentral": return "Canada Central";
        case "canadaeast": return "Canada East";
        case "brazilsouth": return "Brazil South";
        case "northeurope": return "North Europe";
        case "westeurope": return "West Europe";
        case "easteurope": return "East Europe";
        case "uksouth": return "UK South";
        case "ukwest": return "UK West";
        case "francecentral": return "France Central";
        case "germanywestcentral": return "Germany West Central";
        case "norwayeast": return "Norway East";
        case "uaenorth": return "UAE North";
        case "southafricanorth": return "South Africa North";
        case "switzerlandnorth": return "Switzerland North";
        case "eastasia": return "East Asia";
        case "southeastasia": return "Southeast Asia";
        case "australiaeast": return "Australia East";
        case "australiasoutheast": return "Australia Southeast";
        case "centralindia": return "Central India";
        case "southindia": return "South India";
        case "westindia": return "West India";
        case "japaneast": return "Japan East";
        case "japanwest": return "Japan West";
        case "koreacentral": return "Korea Central";
        case "chinaeast": return "China East";
        case "chinanorth": return "China North";
        case "chinaeast2": return "China East 2";
        case "chinanorth2": return "China North 2";
        default: return regionName; // If it's not on the list, return the original
      }
    }
  }
}

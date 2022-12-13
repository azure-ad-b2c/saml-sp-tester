namespace SAMLTEST.Models;
public class AzureAdB2C
{
    public static string ConfigurationName => nameof(AzureAdB2C);
    public string Tenant { get; set; }
    public string HostName { get; set; }
    public string Policy { get; set; }
    public string Issuer { get; set; }
    public string DCInfo { get; set; }
}
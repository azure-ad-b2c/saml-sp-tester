using Microsoft.AspNetCore.Mvc.RazorPages;
using SAMLTEST.SAMLObjects;
using System;

namespace SAMLTEST.Pages
{
    /// <summary>
    /// This is the Metadata Page Model
    /// </summary>
    public class MetadataModel : PageModel
    {
        public string ServerName { get; private set; }
        public Boolean ShowView { get; private set; } = false;

        public void OnGet(string showpage="false")
        {
            ServerName = SAMLHelper.GetThisURL(this);
            if (showpage != "false")
            {
                ShowView = true;
            }
        }

    }
}
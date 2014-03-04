using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.DirectoryServices.ActiveDirectory;

namespace KeywordSubstitution
{
    public class WindowsDomain
    {
        public static string GetDomainName()
        {
            // NOTE: In a somewhat large network with a PDC, and the computer is not a member of a domain, GetCurrentDomain() may hang for seconds to minutes
            // As an attempted workaround, we'll get the legacy NetBIOS name to check if the computer is joined a domain, before calling GetCurrentDomain()
            // TODO: Check if this is correct and reliable

            string netbiosDomainName = GetNetBiosDomainName();
            string domain = null;
            if (!string.IsNullOrEmpty(netbiosDomainName))
            {
                try
                {
                    var cd = Domain.GetCurrentDomain();
                    domain = cd.ToString();
                }
                catch (ActiveDirectoryOperationException) { }
            }

            return domain;
        }

        //
        // Ref.: http://www.pinvoke.net/default.aspx/netapi32/netgetjoininformation.html
        // NOTE: The returned domain name is the legacy NetBIOS domain name
        //

        // Returns the domain name the computer is joined to, or "" if not joined.
        public static string GetNetBiosDomainName()
        {
            int result = 0;
            string domain = null;
            IntPtr pDomain = IntPtr.Zero;
            NetJoinStatus status = NetJoinStatus.NetSetupUnknownStatus;
            try
            {
                result = NetGetJoinInformation(null, out pDomain, out status);
                if (result == ErrorSuccess &&
                    status == NetJoinStatus.NetSetupDomainName)
                {
                    domain = Marshal.PtrToStringAuto(pDomain);
                }
            }
            finally
            {
                if (pDomain != IntPtr.Zero) NetApiBufferFree(pDomain);
            }
            if (domain == null) domain = "";
            return domain;
        }

        [DllImport("Netapi32.dll")]
        static extern int NetApiBufferFree(IntPtr Buffer);

        // Win32 Result Code Constant
        const int ErrorSuccess = 0;

        // NetGetJoinInformation() Enumeration
        enum NetJoinStatus
        {
            NetSetupUnknownStatus = 0,
            NetSetupUnjoined,
            NetSetupWorkgroupName,
            NetSetupDomainName
        } // NETSETUP_JOIN_STATUS

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int NetGetJoinInformation(
            string server,
            out IntPtr domain,
            out NetJoinStatus status);
    }
}

using System;
using NATUPNPLib;
using NETCONLib;
using NetFwTypeLib;

namespace RentBike.Common
{
    /// <summary>
    /// A class that contains methods to display the current information
    /// about the local computers Windows Firewall.
    /// </summary>
    public class WindowsFirewall
    {
        #region Constants

        private const string CLSID_FIREWALL_MANAGER = "{304CE942-6E39-40D8-943A-B913C40C9CD4}";
        private const NET_FW_PROFILE_TYPE_ NET_FW_PROFILE_DOMAIN = NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN;
        private const string LINE_HEADER = "--------------------------------------------------------------------------------";
        private const string SHORT_LINE_HEADER = "-----------------";

        #endregion

        #region Constructor

        public WindowsFirewall()
        {

        }

        #endregion

        #region Public Methods

        // Provides access to the firewall settings profile.
        public INetFwProfile GetFirewallProfile()
        {
            INetFwMgr oINetFwMgr = GetFirewallManager();
            return oINetFwMgr.LocalPolicy.CurrentProfile;
        }

        // Enable windows firewall.
        public void ActivateFirewall()
        {
            INetFwProfile fwProfile = GetFirewallProfile();
            fwProfile.FirewallEnabled = true;
        }

        // Disable windows firewall.
        public void DisableFirewall()
        {
            INetFwProfile fwProfile = GetFirewallProfile();
            fwProfile.FirewallEnabled = false;
        }

        // Firewall state || False = Disabled - True = Enabled.
        public bool FirewallEnabled()
        {
            INetFwProfile fwProfile = GetFirewallProfile();
            return fwProfile.FirewallEnabled;
        }

        /// <summary>
        /// Displays a comprehensive list of information regarding the Windows Firewall
        /// </summary>
        public void DisplayFirewallInformation()
        {
            INetFwMgr manager = GetFirewallManager();

            this.DisplayFirewallProfile(manager);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a firewall manager object
        /// </summary>
        /// <returns>INetFwMgr interface</returns>
        private static INetFwMgr GetFirewallManager()
        {
            Type objectType = Type.GetTypeFromCLSID(new Guid(CLSID_FIREWALL_MANAGER));

            return Activator.CreateInstance(objectType) as INetFwMgr;
        }

        /// <summary>
        /// Writes out various firewall configurations for the local firewall policy.
        /// </summary>
        /// <param name="manager">INetFwMgr object</param>
        private void DisplayFirewallProfile(INetFwMgr manager)
        {

            INetFwProfile profile = manager.LocalPolicy.CurrentProfile;

            /*
             * 
             * Profile Information
             *    
             */

            Logger.Log(WindowsFirewall.LINE_HEADER);
            Logger.Log("Windows Firewall Report\n");
            Logger.Log(string.Format(string.Format("\n\n{0}\n{1}", "Profile", WindowsFirewall.SHORT_LINE_HEADER)));
            Logger.Log(string.Format("Firewall Policy Type: {0}", this.GetPolicyType(profile)));
            Logger.Log(string.Format("Exceptions Not Allowed: {0}", profile.ExceptionsNotAllowed));
            Logger.Log(string.Format("Notifications Disabled: {0}", profile.NotificationsDisabled));
            Logger.Log(string.Format("Remote Administration Enabled: {0}", profile.RemoteAdminSettings.Enabled));

            /*
             *    
             * ICMP Settings
             * 
             */

            Logger.Log(string.Format("\n\n{0}\n{1}", "ICMP Settings", WindowsFirewall.SHORT_LINE_HEADER));
            Logger.Log(string.Format("Allow Inbound Echo Request: {0}", profile.IcmpSettings.AllowInboundEchoRequest));
            Logger.Log(string.Format("Allow Inbound Mask Request: {0}", profile.IcmpSettings.AllowInboundMaskRequest));
            Logger.Log(string.Format("Allow Inbound Router Request: {0}", profile.IcmpSettings.AllowInboundRouterRequest));
            Logger.Log(string.Format("Allow Inbound TimeStamp Request: {0}", profile.IcmpSettings.AllowInboundTimestampRequest));
            Logger.Log(string.Format("Allow Outbound Destination Unreachable: {0}", profile.IcmpSettings.AllowOutboundDestinationUnreachable));
            Logger.Log(string.Format("Allow Outbound Packet Too Big: {0}", profile.IcmpSettings.AllowOutboundPacketTooBig));
            Logger.Log(string.Format("Allow Outbout Parameter Problem: {0}", profile.IcmpSettings.AllowOutboundParameterProblem));
            Logger.Log(string.Format("Allow Outbound Source Quench: {0}", profile.IcmpSettings.AllowOutboundSourceQuench));
            Logger.Log(string.Format("Allow Outbound Time Exceeded: {0}", profile.IcmpSettings.AllowOutboundTimeExceeded));
            Logger.Log(string.Format("Allow Redirect: {0}", profile.IcmpSettings.AllowRedirect));

            /*
             *    
             * Port Information
             * 
             */

            Logger.Log(string.Format("\n\n{0}\n{1}", "Port Information", WindowsFirewall.SHORT_LINE_HEADER));
            Logger.Log(string.Format("Globally Opened Ports: {0}", profile.GloballyOpenPorts.Count));

            // Display detailed port information.
            foreach (INetFwOpenPort port in profile.GloballyOpenPorts)
            {
                Logger.Log(string.Format("\n\nPort Name: {0}", port.Name));
                Logger.Log(string.Format("{0, 20}{1}", "Port Number: ", port.Port));
                Logger.Log(string.Format("{0, 20}{1}", "Port Protocol: ", this.GetPortType(port)));
                Logger.Log(string.Format("{0, 20}{1}", "Port IP Version: ", this.GetIPVersion(port)));
                Logger.Log(string.Format("{0, 20}{1}", "Port Enabled: ", port.Enabled));
                Logger.Log(string.Format("{0, 20}{1}", "Remote Addresses: ", port.RemoteAddresses));
            }

            /*
             *    
             * Service Information
             * 
             */

            Logger.Log(string.Format("\n\n{0}\n{1}", "Services Information", WindowsFirewall.SHORT_LINE_HEADER));
            Logger.Log(string.Format("# of Services: {0}", profile.Services.Count));

            // Display detailed service information.
            foreach (INetFwService service in profile.Services)
            {
                Logger.Log(string.Format("\n\nService Name: {0}", service.Name));
                Logger.Log(string.Format("{0, 20}{1}", "Enabled: ", service.Enabled));
                Logger.Log(string.Format("{0, 20}{1}", "Scope: ", this.GetServiceScope(service)));

                // Obtain all the port information the service is utilizing.
                foreach (INetFwOpenPort port in service.GloballyOpenPorts)
                {
                    Logger.Log(string.Format("{0, 20}{1}", "Port Number: ", port.Port));
                    Logger.Log(string.Format("{0, 20}{1}", "Port Protocol: ", this.GetPortType(port)));
                    Logger.Log(string.Format("{0, 20}{1}", "Port IP Version: ", this.GetIPVersion(port)));
                    Logger.Log(string.Format("{0, 20}{1}", "Port Enabled: ", port.Enabled));
                    Logger.Log(string.Format("{0, 20}{1}", "Remote Addresses: ", port.RemoteAddresses));
                }
            }

            /*
             *    
             * Authorized Applications
             * 
             */

            Logger.Log(string.Format("\n\n{0}\n{1}", "Authorized Applications", WindowsFirewall.SHORT_LINE_HEADER));
            Logger.Log(string.Format("# of Authorized Applications: {0}", profile.AuthorizedApplications.Count));

            // Display detailed authorized application information.
            foreach (INetFwAuthorizedApplication application in profile.AuthorizedApplications)
            {
                Logger.Log(string.Format("\n\nApplication Name: {0}", application.Name));
                Logger.Log(string.Format("{0, 20}{1}", "Enabled: ", application.Enabled));
                Logger.Log(string.Format("{0, 20}{1}", "Remote Addresses: ", application.RemoteAddresses));
                Logger.Log(string.Format("{0, 20}{1}", "File Path: ", application.ProcessImageFileName));
            }

        }

        /// <summary>
        /// Returns a friendly string format of the policy type.
        /// </summary>
        /// <param name="profile">INetFwProfile object</param>
        /// <returns>string</returns>
        private string GetPolicyType(INetFwProfile profile)
        {
            string policyType = string.Empty;

            // Displays what type of policy the Windows Firewall is controlled by.
            switch (profile.Type)
            {
                case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN:
                    policyType = "Domain";
                    break;

                case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD:
                    policyType = "Standard";
                    break;

                case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_CURRENT:
                    policyType = "Current";
                    break;

                case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_TYPE_MAX:
                    policyType = "Max";
                    break;

            }

            return policyType;
        }

        /// <summary>
        /// Returns a friendly string format of the type of protocol.
        /// </summary>
        /// <param name="port">INetFwOpenPort port object</param>
        /// <returns>string</returns>
        private string GetPortType(INetFwOpenPort port)
        {
            string protocolType = string.Empty;

            switch (port.Protocol)
            {
                case NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP:
                    protocolType = "TCP";
                    break;

                case NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP:
                    protocolType = "UDP";

                    break;
            }

            return protocolType;
        }

        /// <summary>
        /// Returns a friendly string format of the IP version.
        /// </summary>
        /// <param name="port">INetFwOpenPort port object</param>
        /// <returns>string</returns>
        private string GetIPVersion(INetFwOpenPort port)
        {
            string ipVersion = string.Empty;

            switch (port.IpVersion)
            {
                case NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY:
                    ipVersion = "Any";
                    break;

                case NET_FW_IP_VERSION_.NET_FW_IP_VERSION_MAX:
                    ipVersion = "Max";
                    break;

                case NET_FW_IP_VERSION_.NET_FW_IP_VERSION_V4:
                    ipVersion = "IPV4";
                    break;

                case NET_FW_IP_VERSION_.NET_FW_IP_VERSION_V6:
                    ipVersion = "IPV6";
                    break;
            }

            return ipVersion;
        }

        /// <summary>
        /// Returns a friendly string format of the service scope.
        /// </summary>
        /// <param name="service">INetFwService object</param>
        /// <returns>string</returns>
        private string GetServiceScope(INetFwService service)
        {
            string serviceScope = string.Empty;

            switch (service.Scope)
            {
                case NET_FW_SCOPE_.NET_FW_SCOPE_ALL:
                    serviceScope = "All";
                    break;

                case NET_FW_SCOPE_.NET_FW_SCOPE_CUSTOM:
                    serviceScope = "Custom";
                    break;

                case NET_FW_SCOPE_.NET_FW_SCOPE_LOCAL_SUBNET:
                    serviceScope = "Local Subnet";
                    break;

                case NET_FW_SCOPE_.NET_FW_SCOPE_MAX:
                    serviceScope = "Max";
                    break;
            }

            return serviceScope;
        }

        #endregion
    }
}
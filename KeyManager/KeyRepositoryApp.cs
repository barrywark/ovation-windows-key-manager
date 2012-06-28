using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Physion.Ovation.KeyRepositoryService;

namespace KeyManager
{
    internal class KeyRepositoryApp
    {

        private ServiceHost _serviceHost = null;

        public void Start()
        {
            // start WCF host via self host
            //http://msdn.microsoft.com/en-us/library/ms730935

            if (_serviceHost != null)
            {
                _serviceHost.Close();
            }

            //Config via App.config
            _serviceHost = new ServiceHost(typeof (FileSystemKeyRepository));
            _serviceHost.Authorization.ServiceAuthorizationManager = new KeyRepositoryAuthorizationManager();

            try
            {
                _serviceHost.Open();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Unable to start WCF service: {0}", ce.Message);
                _serviceHost.Abort();
            }
        }

        public void Stop()
        {
            try
            {
                if (_serviceHost.State != CommunicationState.Closed)
                {
                    _serviceHost.Close();
                }
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Unable to stop WCF service: {0}", ce.Message);
                _serviceHost.Abort();
            }
        }
    }

    internal class KeyRepositoryAuthorizationManager : ServiceAuthorizationManager
    {

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if(!base.CheckAccessCore(operationContext))
                return false;

            Debug.Assert(operationContext.ServiceSecurityContext.WindowsIdentity != null, "operationContext.ServiceSecurityContext.WindowsIdentity != null");

            // Allow access when client is a member of the local Administers group
            var windowsPrincipal = new WindowsPrincipal(operationContext.ServiceSecurityContext.WindowsIdentity);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);

            //Claims-based version
            /*
            return operationContext.ServiceSecurityContext.AuthorizationContext.ClaimSets
                .Where(cs => cs.Issuer == ClaimSet.System)
                .Any(cs => cs.FindClaims(ClaimTypes.Sid, Rights.PossessProperty).Any(claim => _localAdminSid.Equals(claim.Resource)));
             */
        }
    }
}
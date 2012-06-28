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

    public class KeyRepositoryApp
    {

        public ServiceHost ServiceHost { get; private set; }

        //public for testing
        private ServiceAuthorizationManager AuthorizationManager { get; set; }

        private EventLog Log { get; set; }

        public KeyRepositoryApp()
            : this(new KeyRepositoryAuthorizationManager(), null)
        {
        }

        public KeyRepositoryApp(EventLog eventSource) : this(new KeyRepositoryAuthorizationManager(), eventSource)
        {
        }


        public KeyRepositoryApp(ServiceAuthorizationManager manager)
            : this(manager, null)
        {
        }

        public KeyRepositoryApp(ServiceAuthorizationManager manager, EventLog eventSource)
        {
            AuthorizationManager = manager;
            Log = eventSource;
        }

        public void Start()
        {
            // start WCF host via self host
            //http://msdn.microsoft.com/en-us/library/ms730935

            if (ServiceHost != null)
            {
                ServiceHost.Close();
            }

            //Config via App.config
            ServiceHost = new ServiceHost(typeof(FileSystemKeyRepository));

            Debug.Assert(AuthorizationManager != null, "AuthorizationManager != null");
            ServiceHost.Authorization.ServiceAuthorizationManager = AuthorizationManager;

            try
            {
                ServiceHost.Open();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Unable to start WCF service: {0}", ce.Message);
                if(Log != null)
                {
                    Log.WriteEntry(ce.Message);
                }
                ServiceHost.Abort();
            }
        }

        public void Stop()
        {
            try
            {
                if (ServiceHost.State != CommunicationState.Closed)
                {
                    ServiceHost.Close();
                }
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Unable to stop WCF service: {0}", ce.Message);
                if (Log != null)
                {
                    Log.WriteEntry(ce.Message);
                }
                ServiceHost.Abort();
            }
        }
    }

    internal class KeyRepositoryAuthorizationManager : ServiceAuthorizationManager
    {

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if (!base.CheckAccessCore(operationContext))
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
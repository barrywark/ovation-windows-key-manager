using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Physion.Ovation.KeyRepositoryService;

namespace KeyManager
{
    class KeyRepositoryApp
    {

        private ServiceHost _serviceHost = null;

        public void Start()
        {
            // start WCF host via self host
            //http://msdn.microsoft.com/en-us/library/ms730935
            
            if(_serviceHost != null)
            {
                _serviceHost.Close();
            }

            //Config via App.config
            _serviceHost = new ServiceHost(typeof(FileSystemKeyRepository));

            try
            {
                _serviceHost.Open();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
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
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                _serviceHost.Abort();
            }
        }
    }
}

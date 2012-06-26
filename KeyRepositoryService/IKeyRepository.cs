using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.ServiceModel;
using System.Text;

namespace Physion.Ovation.KeyRepositoryService
{
    [ServiceContract]
    public interface IKeyRepository
    {
        /// <summary>
        /// Writes a shared encryption key to the repository.
        /// </summary>
        /// <param name="institution">Licensed institution</param>
        /// <param name="group">Licensed group</param>
        /// <param name="product">Licensed product</param>
        /// <param name="key">Shared encryption key</param>
        [OperationContract]
        void WriteKey(string institution, string group, string product, string key);

    }

    /*
    // Use a data contract as illustrated in the sample below to add composite types to service operations
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    */
}

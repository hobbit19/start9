//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Start9.Host.Adapters
{
    
    public class IConfigurationContractToViewHostAdapter : Start9.Host.Views.IConfiguration
    {
        private Start9.Api.Contracts.IConfigurationContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        static IConfigurationContractToViewHostAdapter()
        {
        }
        public IConfigurationContractToViewHostAdapter(Start9.Api.Contracts.IConfigurationContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }
        public System.Collections.Generic.IList<Start9.Host.Views.IConfigurationEntry> Entries
        {
            get
            {
                return System.AddIn.Pipeline.CollectionAdapters.ToIList<Start9.Api.Contracts.IConfigurationEntryContract, Start9.Host.Views.IConfigurationEntry>(_contract.Entries, Start9.Host.Adapters.IConfigurationEntryHostAdapter.ContractToViewAdapter, Start9.Host.Adapters.IConfigurationEntryHostAdapter.ViewToContractAdapter);
            }
        }
        internal Start9.Api.Contracts.IConfigurationContract GetSourceContract()
        {
            return _contract;
        }
    }
}


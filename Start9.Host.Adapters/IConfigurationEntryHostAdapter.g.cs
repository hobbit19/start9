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
    
    public class IConfigurationEntryHostAdapter
    {
        internal static Start9.Host.Views.IConfigurationEntry ContractToViewAdapter(Start9.Api.Contracts.IConfigurationEntryContract contract)
        {
            if ((contract == null))
            {
                return null;
            }
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IConfigurationEntryViewToContractHostAdapter))))
            {
                return ((IConfigurationEntryViewToContractHostAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IConfigurationEntryContractToViewHostAdapter(contract);
            }
        }
        internal static Start9.Api.Contracts.IConfigurationEntryContract ViewToContractAdapter(Start9.Host.Views.IConfigurationEntry view)
        {
            if ((view == null))
            {
                return null;
            }
            if (view.GetType().Equals(typeof(IConfigurationEntryContractToViewHostAdapter)))
            {
                return ((IConfigurationEntryContractToViewHostAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IConfigurationEntryViewToContractHostAdapter(view);
            }
        }
    }
}


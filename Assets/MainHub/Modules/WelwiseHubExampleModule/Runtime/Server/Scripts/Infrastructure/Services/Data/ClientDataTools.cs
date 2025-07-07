using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services.Data
{
    public static class ClientDataTools
    {
        public static void TryMergingClientsData(this ClientData currentData, ClientData newData)
        {
            if (newData == null)
                return;
            
            var properties = typeof(ClientData).GetProperties();

            foreach (var property in properties)
            {
                var newDataValue = property.GetValue(newData);

                if (newDataValue != null)
                    property.SetValue(currentData, newDataValue);
            }
        }
    }
}
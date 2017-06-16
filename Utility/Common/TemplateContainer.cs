using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility.Enum;

namespace Utility.Core
{
    public class TemplateContainer : IDisposable
    {
        static TemplateContainer()
        {
            RegistAllSource();
        }

        private static void RegistAllSource()
        {
            Regist("C51F47AD-4B33-4596-9B00-BF5A38A67669", "DbContextExtensions.sem");
            Regist("A14218A1-2271-40E7-B9A5-68FC8D36667A", "DBContext.sem");
            Regist("3BA3BF42-42FD-4DD8-B6DB-53BE1A116AD7", "DBContext.slm");
            Regist("65888D89-A4C7-4DC0-87E2-71667ED0DE2E", "ContextUnit.sem");
            Regist("DA66A2D1-1F56-4DD6-8CF3-093E4842A3E3", "Map.slm");
            Regist("B1243F2F-EB98-4165-8F6C-0B93F2353A85", "Repository.slm");
            Regist("E9354F4B-CB6B-488F-8E0E-B4AD24C7C246", "Entity.slm");
            Regist("EA8D007E-65ED-4AAE-9E02-988650EEA81F", "Entities.sem");
            Regist("4C22DD8D-A0C5-4616-85C9-AB82CBE2A6E6", "IRepository.slm");
            Regist("E5543E7E-B17D-42A3-9AC2-6565F87E99D4", "Application.slm");
            Regist("68A6482E-C517-46BF-B908-6D735BFE0B4C", "IApplication.slm");
            Regist("B673DE88-23A2-4C33-BA1F-FE2FFDBD1DE0", "Data2Obj.slm");
            Regist("A6A5A696-D2BF-49C8-95F5-F53EE35BFEDC", "Profile.sem");
            Regist("5D5EFA32-0F42-4915-877E-E19D90FBA44C", "Profile.slm");
            Regist("2337856B-A581-4D3A-9E00-E1026E43040F", "AttachDataSignBehavior.sem");
            Regist("6B60795B-C2B9-433E-9E26-B5352053AC80", "Container.sem");
            Regist("FE7F5F13-D800-46AB-90E1-618C5DDBB329", "Container.slm");
            Regist("38FF4772-4BA3-4E44-94BB-012579ADC937", "UnityInstanceProvider.sem");
            Regist("DD8479F0-E3C2-44D6-A9A6-6E8252F43490", "UnityInstanceProviderServiceBehavior.sem");
            Regist("FAB56BAC-C9BC-4350-B362-5D40F2E5BED3", "WebConfig.sem");
            Regist("06924AE3-D5F5-4973-9D0E-CE28738DCBCA", "IService.sem");
            Regist("3A685CEE-1E8F-4373-B605-624BE2FB072D", "IService.slm");
            Regist("9F8CD6F5-7F51-48B7-9D04-0CAFE029A3EC", "Service.sem");
            Regist("E8071412-0147-4AD3-BBF7-489854B665CE", "Service.slm");
            Regist("A0517CF0-4B45-4D23-B11A-40434D247DC1", "Codebehind.sem");
           
        }

        private static Dictionary<string, object> _templateContainer = new Dictionary<string, object>();

        public static void Regist<T>(string guid, T source)
        {
            if (_templateContainer.ContainsKey(guid))
                _templateContainer[guid] = source;
            else
                _templateContainer.Add(guid, source);

        }

        public static T Resove<T>(string guid)
        {
            if (_templateContainer == null || !_templateContainer.ContainsKey(guid))
                return default(T);
            return (T)_templateContainer[guid];
        }


        public void Dispose()
        {
            _templateContainer = null;
        }
    }
}

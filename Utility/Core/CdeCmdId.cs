using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core
{
    public class CdeCmdId
    {
        static CdeCmdId()
        {
            SetBelongId(InfrastructureId.DbContextExtensions,PrjCmdId.Infrastructure);
            SetBelongId(InfrastructureId.Repository, PrjCmdId.Infrastructure);
            SetBelongId(InfrastructureId.Map, PrjCmdId.Infrastructure);
            SetBelongId(InfrastructureId.ContextUnit, PrjCmdId.Infrastructure);
            SetBelongId(InfrastructureId.DBContext.Fix, PrjCmdId.Infrastructure);
            SetBelongId(InfrastructureId.DBContext.UnFix, PrjCmdId.Infrastructure);
            SetBelongId(DomainEntityId.Entity, PrjCmdId.DomainEntity);
            SetBelongId(DomainEntityId.Entities, PrjCmdId.DomainEntity);
            SetBelongId(DomainContextId.IRepository, PrjCmdId.DomainContext);
            SetBelongId(ApplicationId.Application, PrjCmdId.Application);
            SetBelongId(IApplicationId.IApplication, PrjCmdId.IApplication);
            SetBelongId(Data2ObjectId.Data2Obj, PrjCmdId.Data2Object);
            SetBelongId(Data2ObjectId.Profile.Fix, PrjCmdId.Data2Object);
            SetBelongId(Data2ObjectId.Profile.UnFix,PrjCmdId.Data2Object);
            SetBelongId(ServiceId.AttachDataSignBehavior, PrjCmdId.Service);
            SetBelongId(ServiceId.Container.Fix, PrjCmdId.Service);
            SetBelongId(ServiceId.Container.UnFix, PrjCmdId.Service);
            SetBelongId(ServiceId.IService.Fix, PrjCmdId.Service);
            SetBelongId(ServiceId.IService.UnFix, PrjCmdId.Service);
            SetBelongId(ServiceId.Service.Fix, PrjCmdId.Service);
            SetBelongId(ServiceId.Service.UnFix, PrjCmdId.Service);
            SetBelongId(ServiceId.UnityInstanceProvider, PrjCmdId.Service);
            SetBelongId(ServiceId.UnityInstanceProviderServiceBehavior, PrjCmdId.Service);
            SetBelongId(ServiceId.WebConfig, PrjCmdId.Service);
            SetBelongId(ServiceId.CodeBehind, PrjCmdId.Service);

            SetFileName(InfrastructureId.Repository, "$Data2Obj$Repository.cs");
            SetFileName(InfrastructureId.Map, "$Entity$Map.cs");
            SetFileName(InfrastructureId.ContextUnit, "$ProjectName$ContextUnit.cs");
            SetFileName(InfrastructureId.DBContext.UnFix, "$ProjectName$Context.cs");
            SetFileName(Data2ObjectId.Profile.UnFix, "$ProjectName$Profile.cs");
            SetFileName(DomainEntityId.Entity, "$Entity$.cs");
            SetFileName(DomainContextId.IRepository, "I$Data2Obj$Repository.cs");
            SetFileName(ApplicationId.Application, "$Data2Obj$App.cs");
            SetFileName(IApplicationId.IApplication, "I$Data2Obj$App.cs");
            SetFileName(Data2ObjectId.Data2Obj, "$Data2Obj$.cs");
            SetFileName(ServiceId.IService.UnFix, "I$ProjectName$Service.cs");
            SetFileName(ServiceId.Service.UnFix, "$ProjectName$Service.svc.cs");
            SetFileName(ServiceId.Service.UnFix, "$ProjectName$Service.svc");
            SetFileName(ServiceId.WebConfig, "web.config");

            SetForlder(Data2ObjectId.Profile.UnFix, "Profile");
            SetForlder(InfrastructureId.DbContextExtensions, "Extention");
            SetForlder(InfrastructureId.Map, "Map");
            SetForlder(InfrastructureId.Repository, "Repository");
            SetForlder(ServiceId.UnityInstanceProviderServiceBehavior, "InstanceProviders");
            SetForlder(ServiceId.UnityInstanceProvider, "InstanceProviders");
            SetForlder(ServiceId.AttachDataSignBehavior, "InstanceProviders");
            SetForlder(ServiceId.Container.UnFix, "InstanceProviders");

        }

        private static Dictionary<string, string> _idContainer = new Dictionary<string, string>();
        private static Dictionary<string, string> _nameContainer = new Dictionary<string, string>();
        private static Dictionary<string, string> _forldContainer = new Dictionary<string, string>();

        private static void SetBelongId(string pid, string id)
        {
            if (!_idContainer.ContainsKey(pid))
                _idContainer.Add(pid, id);
            else
                _idContainer[pid] = id;
        }

        private static void SetFileName(string guid, string fileNameFormate)
        {
            if (!_nameContainer.ContainsKey(guid))
                _nameContainer.Add(guid, fileNameFormate);
            else
                _nameContainer[guid] = fileNameFormate;
        }

        private static void SetForlder(string guid, string forlderName)
        {
            if (!_forldContainer.ContainsKey(guid))
                _forldContainer.Add(guid, forlderName);
            else
                _forldContainer[guid] = forlderName;
        }

        public static string TempFileName(string guid)
        {
            if(!_nameContainer.ContainsKey(guid))
                return null;
            return _nameContainer[guid];
        }

        public static bool HasForlder(string guid, out string Forlder)
        {
            if (_forldContainer.ContainsKey(guid))
            {
                Forlder = _forldContainer[guid];
                return true;
            }
            Forlder = null;
            return false;
        }

        public static string BelongId(string guid)
        {
            if (!_idContainer.ContainsKey(guid))
                return null;
            return _idContainer[guid];
        }

        public static class InfrastructureId
        {
            public const string DbContextExtensions = "C51F47AD-4B33-4596-9B00-BF5A38A67669";

            public static class DBContext
            {
                public const string Fix = "A14218A1-2271-40E7-B9A5-68FC8D36667A";

                public const string UnFix = "3BA3BF42-42FD-4DD8-B6DB-53BE1A116AD7";
            }
            public const string ContextUnit = "65888D89-A4C7-4DC0-87E2-71667ED0DE2E";

            public const string Map = "DA66A2D1-1F56-4DD6-8CF3-093E4842A3E3";

            public const string Repository = "B1243F2F-EB98-4165-8F6C-0B93F2353A85";
        }

        public static class DomainEntityId
        {
            public const string Entity = "E9354F4B-CB6B-488F-8E0E-B4AD24C7C246";

            public const string Entities = "EA8D007E-65ED-4AAE-9E02-988650EEA81F";
        }

        public static class DomainContextId
        {
            public const string IRepository = "4C22DD8D-A0C5-4616-85C9-AB82CBE2A6E6";
        }

        public static class ApplicationId
        {
            public const string Application = "E5543E7E-B17D-42A3-9AC2-6565F87E99D4";
        }

        public static class IApplicationId
        {
            public const string IApplication = "68A6482E-C517-46BF-B908-6D735BFE0B4C";

        }

        public static class Data2ObjectId
        {
            public const string Data2Obj = "B673DE88-23A2-4C33-BA1F-FE2FFDBD1DE0";

            public static class Profile
            {
                public const string Fix = "A6A5A696-D2BF-49C8-95F5-F53EE35BFEDC";

                public const string UnFix = "5D5EFA32-0F42-4915-877E-E19D90FBA44C";
            }

        }

        public static class ServiceId
        {
            public const string AttachDataSignBehavior = "2337856B-A581-4D3A-9E00-E1026E43040F";

            public static class Container
            {
                public const string Fix = "6B60795B-C2B9-433E-9E26-B5352053AC80";

                public const string UnFix = "FE7F5F13-D800-46AB-90E1-618C5DDBB329";
            }

            public const string UnityInstanceProvider = "38FF4772-4BA3-4E44-94BB-012579ADC937";

            public const string UnityInstanceProviderServiceBehavior = "DD8479F0-E3C2-44D6-A9A6-6E8252F43490";

            public const string WebConfig = "FAB56BAC-C9BC-4350-B362-5D40F2E5BED3";

            public static class IService
            {
                public const string Fix = "06924AE3-D5F5-4973-9D0E-CE28738DCBCA";

                public const string UnFix = "3A685CEE-1E8F-4373-B605-624BE2FB072D";
            }

            public static class Service
            {
                public const string Fix = "9F8CD6F5-7F51-48B7-9D04-0CAFE029A3EC";

                public const string UnFix = "E8071412-0147-4AD3-BBF7-489854B665CE";
            }

            public const string CodeBehind = "A0517CF0-4B45-4D23-B11A-40434D247DC1";
        }
    }
}

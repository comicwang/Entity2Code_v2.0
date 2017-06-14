using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Utility.Properties;

namespace Utility
{
    public class ConfirmResource
    {
        public static void Copy()
        {
            if (!Directory.Exists(Resource.RootPath))
                Directory.CreateDirectory(Resource.RootPath);

            Confirm("Code.ini", Resource.CodeIni);
            Confirm("img.jpg", Resource.img);
            Confirm("AttachDataSignBehavior.sem", Resource.AttachDataSignBehavior);
            Confirm("Container.sem", Resource.Container);
            Confirm("ContextUnit.sem", Resource.ContextUnit);
            Confirm("DBContext.sem", Resource.DBContext);
            Confirm("DbContextExtensions.sem", Resource.DbContextExtensions);
            Confirm("Entities.sem", Resource.Entities);
            Confirm("IService.sem", Resource.IService);
            Confirm("Profile.sem", Resource.Profile);
            Confirm("Service.sem", Resource.Service);
            Confirm("UnityInstanceProvider.sem", Resource.UnityInstanceProvider);
            Confirm("UnityInstanceProviderServiceBehavior.sem", Resource.UnityInstanceProviderServiceBehavior);
            Confirm("WebConfig.sem", Resource.WebConfig);

            Confirm("Application.slm", Resource.Application);
            Confirm("Container.slm", Resource.Container1);
            Confirm("Data2Obj.slm", Resource.Data2Obj);
            Confirm("DBContext.slm", Resource.DBContext1);
            Confirm("Entity.slm", Resource.Entity);
            Confirm("IApplication.slm", Resource.IApplication);
            Confirm("IRepository.slm", Resource.IRepository);
            Confirm("IService.slm", Resource.IService1);
            Confirm("Map.slm", Resource.Map);
            Confirm("MethodApp.slm", Resource.MethodApp);
            Confirm("MethodIApp.slm", Resource.MethodIApp);
            Confirm("MethodIServer.slm", Resource.MethodIServer);
            Confirm("MethodServer.slm", Resource.MethodServer);
            Confirm("Profile.slm", Resource.Profile1);
            Confirm("Repository.slm", Resource.Repository);
            Confirm("Service.slm", Resource.Service1);

            Confirm("AutoMapper.dll", Resource.AutoMapper);
            Confirm("AutoMapper.Net4.dll", Resource.AutoMapper_Net4);
            Confirm("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll", Resource.iTelluro_Explorer_Domain_CodeFirst_Seedwork);
            Confirm("iTelluro.Explorer.InfoUtility.dll", Resource.iTelluro_Explorer_InfoUtility);
            Confirm("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll", Resource.iTelluro_Explorer_Infrastruct_CodeFirst_Seedwork);
            Confirm("iTelluro.Explorer.Infrastructure.CrossCutting.dll", Resource.iTelluro_Explorer_Infrastructure_CrossCutting);
            Confirm("iTelluro.Explorer.Infrastructure.CrossCutting.NetFramework.dll", Resource.iTelluro_Explorer_Infrastructure_CrossCutting_NetFramework);
            Confirm("iTelluro.SSO.Common.dll", Resource.iTelluro_SSO_Common);
            Confirm("iTelluro.SSO.dll", Resource.iTelluro_SSO);
            Confirm("iTelluro.SSO.WebServices.dll", Resource.iTelluro_SSO_WebServices);
            Confirm("iTelluro.SYS.Entity.dll", Resource.iTelluro_SYS_Entity);
            Confirm("iTelluro.Utility.dll", Resource.iTelluro_Utility);
            Confirm("log4net.dll", Resource.log4net);
            Confirm("Microsoft.Practices.Unity.dll", Resource.Microsoft_Practices_Unity);
            Confirm("EntityFramework.dll", Resource.EntityFramework);
            Confirm("iTelluro.Explorer.Application.CodeFirst.Seedwork.dll", Resource.iTelluro_Explorer_Application_CodeFirst_Seedwork);

        }


        private static void Confirm<T>(string sourceName, T sourceType)
        {
            string targetPath = Path.Combine(Properties.Resource.RootPath, sourceName);

            if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))

                return;

            if (sourceType.GetType() == typeof(string))
                FileOprateHelp.SaveTextFile(sourceType.ToString(), targetPath);

            else if (sourceType.GetType() == typeof(Bitmap))
            {
                Bitmap bitmap = sourceType as Bitmap;
                bitmap.Save(targetPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            else
            {
                byte[] buffer = sourceType as byte[];
                File.WriteAllBytes(targetPath, buffer);
            }
        }
    }
}

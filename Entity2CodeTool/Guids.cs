// Guids.cs
// MUST match guids.h
using System;

namespace Infoearth.Entity2CodeTool
{
    static class GuidList
    {
        public const string guidEntity2CodeToolPkgString = "e6183a32-ed9c-41e8-977c-38b9ee911151";
        public static readonly Guid guidEntity2CodeToolPkg = new Guid(guidEntity2CodeToolPkgString);

        public const string guidEntity2CodeToolCmdSetString = "af924f2f-1b02-4340-b5d1-8da8fe8e531a";
        public static readonly Guid guidEntity2CodeToolCmdSet = new Guid(guidEntity2CodeToolCmdSetString);

        public static readonly Guid guidEntity2CodeModelManager = new Guid("a7754210-46da-491a-80e3-6f9674689632");

        public static readonly Guid guidEntity2CodeAddMethod = new Guid("d7757210-46da-481a-80e3-6f9674189632");

        public static readonly Guid guidEntity2CodeMain = new Guid("0f7fb20e-a0cc-47a9-893a-7f55a85d37ff");

        public static readonly Guid guidEntity2CodeReferManager = new Guid("85078341-f4cd-4ac7-8331-d3d9931bb2a5");

        public static readonly Guid guidEntity2CodeToolGroup = new Guid("c5112e48-fe9b-445c-b2b3-2c2052153f03");

        public static readonly Guid guidEntity2CodeToolSetting = new Guid("6D8AC12E-59C4-4708-9947-840880B5402F");
    };
}
using System;

namespace KanarDrive.Common.Entities.Cloud
{
    public class UserDirectory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Owner { get; set; }
    }
}
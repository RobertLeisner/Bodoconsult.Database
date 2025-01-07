// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Database.Test.EntityBackup;
using System.Collections.Generic;
using System;
using System.IO;

namespace Bodoconsult.Database.Test.Helpers
{
    /// <summary>
    /// Helper class for unit tests
    /// </summary>
    internal static class TestHelper
    {
        public static string TestFolder => @"C:\temp\EntityBackup";

        /// <summary>
        /// Default ctor
        /// </summary>
        static TestHelper()
        {
            if (!Directory.Exists(TestFolder))
            {
                Directory.CreateDirectory(TestFolder);
            }
        }

        public static void GetDataForService(ICollection<DemoEntity> result, DateTime from, DateTime to)
        {
            var entity = new DemoEntity
            {
                Id = 1,
                Name = "Test1",
                Date = from
            };

            result.Add(entity);

            entity = new DemoEntity
            {
                Id = 2,
                Name = "Test2",
                Date = from.AddDays(1)
            };

            result.Add(entity);

            entity = new DemoEntity
            {
                Id = 3,
                Name = "Test3",
                Date = to.AddDays(1)
            };

            result.Add(entity);

            entity = new DemoEntity
            {
                Id = 4,
                Name = "Test4",
                Date = from
            };

            result.Add(entity);
        }

    }
}

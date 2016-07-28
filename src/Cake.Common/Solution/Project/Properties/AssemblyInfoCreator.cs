﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Common.Solution.Project.Properties
{
    /// <summary>
    /// The assembly info creator.
    /// </summary>
    public sealed class AssemblyInfoCreator
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly ICakeLog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoCreator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public AssemblyInfoCreator(IFileSystem fileSystem, ICakeEnvironment environment, ICakeLog log)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            _fileSystem = fileSystem;
            _environment = environment;
            _log = log;
        }

        /// <summary>
        /// Creates an assembly info file.
        /// </summary>
        /// <param name="outputPath">The output path.</param>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public void Create(FilePath outputPath, AssemblyInfoSettings settings)
        {
            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var data = new AssemblyInfoCreatorData(settings);

            outputPath = outputPath.MakeAbsolute(_environment);
            _log.Verbose("Creating assembly info file: {0}", outputPath);

            using (var stream = _fileSystem.GetFile(outputPath).OpenWrite())
            using (var writer = new StreamWriter(stream, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("//------------------------------------------------------------------------------");
                writer.WriteLine("// <auto-generated>");
                writer.WriteLine("//     This code was generated by Cake.");
                writer.WriteLine("// </auto-generated>");
                writer.WriteLine("//------------------------------------------------------------------------------");

                if (data.Namespaces.Count > 0)
                {
                    var namespaces = data.Namespaces.Select(n => string.Concat("using ", n, ";"));
                    foreach (var @namespace in namespaces)
                    {
                        writer.WriteLine(@namespace);
                    }
                    writer.WriteLine();
                }

                if (data.Attributes.Count > 0)
                {
                    foreach (var attribute in data.Attributes)
                    {
                        writer.WriteLine(string.Concat("[assembly: ", attribute.Key, "(", attribute.Value, ")]"));
                    }
                    writer.WriteLine();
                }

                if (data.InternalVisibleTo.Count > 0)
                {
                    foreach (var temp in data.InternalVisibleTo)
                    {
                        writer.WriteLine(string.Concat("[assembly: ", temp, "]"));
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
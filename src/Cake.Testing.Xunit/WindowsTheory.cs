// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Cake.Core.Polyfill;
using Xunit;

namespace Cake.Testing.Xunit
{
    public sealed class WindowsTheory : TheoryAttribute
    {
        private static readonly bool _isUnix;

        static WindowsTheory()
        {
            _isUnix = EnvironmentHelper.IsUnix();
        }

        // ReSharper disable once UnusedParameter.Local
        public WindowsTheory(string reason = null)
        {
            if (_isUnix)
            {
                Skip = reason ?? "Windows test.";
            }
        }
    }
}
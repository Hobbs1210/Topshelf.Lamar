using System;
using System.Collections.Generic;
using System.Text;

namespace Topshelf.Common.Tests
{
    public class SampleLamarService
    {
        private readonly ISampleDependency _dependency;

        public SampleLamarService(ISampleDependency dependency)
        {
            _dependency = dependency;
        }

        public bool Start()
        {
            _dependency.DoWork();
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}
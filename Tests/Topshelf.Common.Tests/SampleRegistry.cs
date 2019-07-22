using Lamar;
using Quartz;
using System;
using Moq;

namespace Topshelf.Common.Tests
{
    public class SampleRegistry : ServiceRegistry
    {
        public SampleRegistry()
        {
            For<ISampleDependency>().Use<SampleDependency>();
        }
    }
}
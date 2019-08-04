Topshelf.Lamar
=====================
[![Build status](https://ci.appveyor.com/api/projects/status/d67n332yx4x6xk9a?svg=true)](https://ci.appveyor.com/project/Hobbs1210/topshelf-lamar)
[![NuGet Version](https://img.shields.io/nuget/v/Topshelf.Lamar.svg)](https://www.nuget.org/packages/Topshelf.Lamar)
[![NuGet Downloads](http://img.shields.io/nuget/dt/Topshelf.Lamar.svg)](https://www.nuget.org/packages/Topshelf.Lamar/)

Topshelf.Lamar is a collection of packages that extend the [Topshelf Project](http://topshelf-project.com). These packages handle the boiler plate code necessary for several use cases that we have found very useful for quickly developing small self contained services on Windows.

These packages solve these problems by integrating the following technologies with Topshelf and providing extensions to quickly integrate them.

*	[Quartz.NET](http://quartznet.sourceforge.net/) - Scheduling
*	[Lamar](https://jasperfx.github.io/lamar/) - IoC Container

## Getting Started

The packages are available on [Nuget](http://nuget.org/) and can be used in any combination desired. The Lamar package is directly integrated with Topshelf, and also has accompanying packages for Quartz.Net projects to make it possible to initiate Quartz.NET IJob instances via the container.


### Topshelf.Ninject

To get the package: `Install-Package Topshelf.Lamar`

To use Lamar with your Topshelf service, all you need is :

	using Topshelf.Lamar;

	...

    class Program
    {
        static void Main()
        {
            HostFactory.Run(c =>
            {
                c.UseLamar( new Container());

                c.Service<SampleService>(s =>
                {
                    s.ConstructUsingLamar(); 
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
            });
        }
    }

### Topshelf.Quartz.Lamar

To get the package: `Install-Package Topshelf.Quartz.Lamar`

To use Lamar with your Topshelf service and Quartz, all you need is : 


	using Topshelf.Lamar;
	using Topshelf.Quartz.Lamar

	...

    class Program
    {
        static void Main()
        {
            HostFactory.Run(c =>
            {
                c.UseLamar( new Container());

                c.Service<SampleService>(s =>
                {
                    s.ConstructUsingLamar(); 
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());

					s.UseQuartzLamar();

					s.ScheduleQuartzJob(q =>
                        q.WithJob(() => JobBuilder.Create<SampleJob>().WithIdentity("SampleJob").Build())
                            .AddTrigger(() => TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithIntervalInSeconds(5).RepeatForever()).Build())
                        );
                });
            });
        }
    }

## References

- [Topshelf](http://topshelf-project.com)
- [Lamar](https://jasperfx.github.io/lamar/)
- [Quartz.NET](http://www.quartz-scheduler.net)

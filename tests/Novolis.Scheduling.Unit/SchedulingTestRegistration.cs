using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Novolis.Scheduling;

namespace Novolis.Scheduling.Unit;

internal static class SchedulingTestRegistration
{
    private static readonly Assembly SchedulingAssembly = typeof(ICronJob).Assembly;

    private static readonly Type DescriptorType =
        SchedulingAssembly.GetType("Novolis.Scheduling.Internals.CronJobDescriptor", throwOnError: true)!;

    private static readonly Type DescriptorInterface =
        SchedulingAssembly.GetType("Novolis.Scheduling.Internals.ICronJobDescriptor", throwOnError: true)!;

    private static readonly Type SchedulerType =
        SchedulingAssembly.GetType("Novolis.Scheduling.Internals.CronJobScheduler", throwOnError: true)!;

    private static readonly Type MaintainerType =
        SchedulingAssembly.GetType("Novolis.Scheduling.Internals.ScheduleMaintainer", throwOnError: true)!;

    public static void AddInvalidRunningDescriptor(IServiceCollection services, Type jobType)
    {
        EnsureSchedulerServices(services);
        services.AddSingleton(DescriptorInterface, CreateDescriptor(jobType, "not a cron", running: true));
    }

    public static void AddDescriptorWithoutKeyedJob(IServiceCollection services, Type jobType, string schedule)
    {
        EnsureSchedulerServices(services);
        services.AddSingleton(DescriptorInterface, CreateDescriptor(jobType, schedule, running: true));
    }

    public static object GetDescriptor(IServiceProvider services, Type jobType)
    {
        var descriptors = GetDescriptorInstances(services);
        var match = descriptors.FirstOrDefault(d => GetName(d) == jobType.FullName);
        if (match is null)
            throw new InvalidOperationException($"Descriptor for {jobType.FullName} was not found.");
        return match;
    }

    public static void SetSchedule(object descriptor, string schedule) =>
        DescriptorInterface.GetProperty("Schedule")!.SetValue(descriptor, schedule);

    public static void InvokeScheduleChanged(IServiceProvider services, object descriptor)
    {
        var maintainer = services.GetRequiredService<IScheduleMaintainer>();
        var maintainerType = maintainer.GetType();
        var changed = maintainerType.GetField("ScheduleChanged", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(maintainer) as Delegate
                      ?? maintainerType.GetProperty("ScheduleChanged")?.GetValue(maintainer) as Delegate;
        changed?.DynamicInvoke(descriptor);
    }

    private static IEnumerable<object> GetDescriptorInstances(IServiceProvider services)
    {
        var getServices = typeof(ServiceProviderServiceExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(ServiceProviderServiceExtensions.GetServices)
                        && m.IsGenericMethodDefinition
                        && m.GetParameters().Length == 1);

        var generic = getServices.MakeGenericMethod(DescriptorInterface);
        return ((System.Collections.IEnumerable)generic.Invoke(null, [services])!).Cast<object>();
    }

    private static void EnsureSchedulerServices(IServiceCollection services)
    {
        if (services.All(d => d.ServiceType != typeof(IHostedService) || d.ImplementationType != SchedulerType))
            services.Add(ServiceDescriptor.Singleton(typeof(IHostedService), SchedulerType));

        if (services.All(d => d.ServiceType != typeof(IScheduleMaintainer)))
            services.AddSingleton(typeof(IScheduleMaintainer), MaintainerType);
    }

    private static object CreateDescriptor(Type jobType, string schedule, bool running) =>
        Activator.CreateInstance(
            DescriptorType,
            jobType,
            schedule,
            running,
            TimeZoneInfo.Utc)!;

    private static string GetName(object descriptor) =>
        (string)DescriptorInterface.GetProperty("Name")!.GetValue(descriptor)!;
}

# Autofac: Dependency Injection in Bonyan Layer

The `Bonyan` library leverages **Autofac**, a powerful and flexible Inversion of Control (IoC) container, to facilitate Dependency Injection (DI) throughout the library. Autofac is used for its advanced features, particularly property injection, which is a key aspect of Bonyan's DI strategy.

## What is Autofac?

**Autofac** is an IoC container for .NET that helps with managing dependencies in an application. Instead of manually creating and managing objects and their dependencies, Autofac takes care of the instantiation, lifetime management, and dependency resolution, allowing developers to focus on business logic.

Autofac is especially powerful when working with complex systems where classes often have many dependencies. By using Autofac, these dependencies can be managed automatically, ensuring that the correct objects are injected at runtime without tightly coupling the components.

## Key Features of Autofac

1. **Flexible Registration**: Autofac allows for multiple ways to register components, including registering instances, types, or by scanning assemblies. This flexibility makes it easy to integrate into existing projects.

2. **Module System**: Autofac provides a `Module` feature, which allows grouping related registrations. This is particularly useful in large applications, where managing dependencies in a modular way is important for scalability.

3. **Property Injection**: Unlike many other DI frameworks, Autofac supports **property injection** out of the box, allowing you to inject dependencies into properties rather than just via constructor parameters. This is extremely useful in scenarios where constructor injection is not feasible, or where dependencies are optional.

4. **Lifetime Scoping**: Autofac offers a range of options for controlling the lifecycle of objects, including instance-per-dependency, singleton, and more. This level of control ensures that objects are managed in a memory-efficient way, which is critical in large applications.

5. **Integration with ASP.NET Core**: Autofac integrates seamlessly with ASP.NET Core, making it a good choice for modern web application development. Its tight integration with the existing ASP.NET Core DI infrastructure allows for a smooth transition for those who need more features beyond the built-in DI container.

## Why Autofac is Used in Bonyan Layer

The **Bonyan** package uses Autofac as its default DI framework because of the need for **property injection**. Property injection is essential in scenarios where you cannot inject dependencies through constructors or when you want to inject services dynamically after the object is constructed.

### Property Injection in Bonyan Layer

In `Bonyan.Layer.Domain`, many modules and services require property injection due to the nature of modularity and decoupled design. For example, if you need to configure dependencies dynamically based on specific runtime needs, property injection provides a simple way to handle that without overcomplicating the constructor.

The Bonyan library internally integrates Autofac with ASP.NET Core's default DI container, which means when you add services to the `IServiceCollection`, those services are automatically registered with Autofac. This approach allows you to use ASP.NET Core's standard registration methods while still benefiting from Autofac's advanced features like property injection.

In this example, services are added using `IServiceCollection`, and Autofac is used internally to support additional features like property injection automatically. This seamless integration allows you to enjoy Autofac's capabilities while maintaining familiarity with ASP.NET Core's DI syntax.

## Built-in Usage of Autofac in Bonyan

The Bonyan package has built-in support for Autofac to manage **Dependency Injection (DI)**. This means that if you use Bonyan, you can leverage Autofac for all your DI needs without any additional setup. Here are some features that are automatically available in Bonyan due to Autofac integration:

- **Module Registration**: All modules in Bonyan are registered using Autofac’s `Module` system, making it easy to manage dependencies modularly.
- **Automatic Scanning**: The package takes advantage of Autofac's scanning feature, which allows it to automatically find and register all types, services, and interfaces in the assembly.
- **Advanced DI Patterns**: Autofac supports advanced DI patterns such as decorator, mediator, and interceptors, all of which can be used to extend Bonyan’s functionality effortlessly.

## Conclusion

**Autofac** is a highly capable and flexible IoC container, making it an ideal choice for complex software systems like `Bonyan.Layer.Domain`. The primary reason for integrating Autofac within Bonyan is to utilize features like **property injection** effectively, allowing dependencies to be managed in a dynamic and decoupled manner.

If you are using Bonyan, the Autofac container is already set up, making it easy to use its powerful DI features for your project. By providing advanced functionality and an easy integration path, Autofac helps ensure that Bonyan remains scalable, maintainable, and flexible.

